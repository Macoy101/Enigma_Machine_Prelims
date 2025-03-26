using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Enigma_Machine_Prelims
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _control = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string _ring1 = "DMTWSILRUYQNKFEJCAZBPGXOHV";
        string _ring2 = "HQZGPJTMOBLNCIFDYAWVEUSRKX";
        string _ring3 = "UQNTLSZFMREHDPXKIBVYGJCWOA";
        string _reflector = "YRUHQSLDPXNGOKMIEBFZCWVJAT";

        int[] _keyOffset = { 0, 0, 0 }; 
        int[] _initOffset = { 0, 0, 0 }; 

        bool _rotor = false;

        Dictionary<char, char> _plugboard = new Dictionary<char, char>();
        private bool _plugboardSet = false; 

        public MainWindow()
        {
            InitializeComponent();

            SetDefaults(); 

            _rotor = false; 
            btnRotor.Content = "Rotor On"; 
            btnRotor.IsEnabled = false;
            DisplayRing(reflectorlbl, _reflector);
        }

        private void DisplayRing(Label displayLabel, string ring)
        {
            string temp = "";
            foreach (char r in ring)
                temp += r + "   "; // Add tab for spacing
            displayLabel.Content = temp;
        }

        private int IndexSearch(string ring, char letter)
        {
            int index = 0;
            for (int x = 0; x < ring.Length; x++)
            {
                if (ring[x] == letter)
                {
                    index = x;
                    break;
                }
            }
            return index;
        }

        // Added method for Virtual Keyboard Input
        private void KeyPress(char letter)
        {
            lblInput.Content = lblInput.Content.ToString() + letter;

            lblEncrpyt.Content = lblEncrpyt.Content.ToString() + Encrypt(letter);
            lblEncrpytMirror.Content = lblEncrpytMirror.Content.ToString() + Mirror(letter);

            if (_rotor)
            {
                Rotate(true);
                UpdateRotorDisplay();
            }
        }
        // This updates the Rotor Display
        private void UpdateRotorDisplay()
        {
            DisplayRing(lblRing1, _ring1);
            DisplayRing(lblRing2, _ring2);
            DisplayRing(lblRing3, _ring3);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Length == 1 && lblInput.Content.ToString().Length < 128)
            {
                if ((int)e.Key.ToString()[0] >= 65 && (int)e.Key.ToString()[0] <= 90)
                {
                    lblInput.Content += e.Key.ToString();
                    lblEncrpyt.Content += Encrypt(e.Key.ToString()[0]) + "";
                    lblEncrpytMirror.Content += Mirror(e.Key.ToString()[0]) + "";

                    if (_rotor)
                    {
                        Rotate(true);
                        DisplayRing(lblRing1, _ring1); 
                        DisplayRing(lblRing2, _ring2);
                        DisplayRing(lblRing3, _ring3);
                    }
                }
            }
            else if (e.Key == Key.Space)
            {
                lblInput.Content += " ";
                lblEncrpyt.Content += " ";
                lblEncrpytMirror.Content += " ";
            }
            else if (e.Key == Key.Back)
            {
                Rotate(false); 
                DisplayRing(lblRing1, _ring1);
                DisplayRing(lblRing2, _ring2);
                DisplayRing(lblRing3, _ring3);

                lblInput.Content = RemoveLastLetter(lblInput.Content.ToString()); 
                lblEncrpyt.Content = RemoveLastLetter(lblEncrpyt.Content.ToString());
                lblEncrpytMirror.Content = RemoveLastLetter(lblEncrpytMirror.Content.ToString());
            }
        }

        private char Encrypt(char letter)
        {
            char newChar = letter;

            if (_plugboard.ContainsKey(newChar))
                newChar = _plugboard[newChar];
            else if (_plugboard.ContainsValue(newChar))
                newChar = _plugboard.FirstOrDefault(x => x.Value == newChar).Key;

            newChar = _ring1[IndexSearch(_control, newChar)];
            newChar = _ring2[IndexSearch(_control, newChar)];
            newChar = _ring3[IndexSearch(_control, newChar)];

            newChar = _reflector[IndexSearch(_control, newChar)];

            newChar = _ring3[IndexSearch(_control, newChar)];
            newChar = _ring2[IndexSearch(_control, newChar)];
            newChar = _ring1[IndexSearch(_control, newChar)];

            if (_plugboard.ContainsKey(newChar))
                newChar = _plugboard[newChar];
            else if (_plugboard.ContainsValue(newChar))
                newChar = _plugboard.FirstOrDefault(x => x.Value == newChar).Key;

            return newChar;
        }

        private char Mirror(char letter)
        {
            char newChar = Encrypt(letter);

            newChar = _ring3[IndexSearch(_control, newChar)];
            newChar = _ring2[IndexSearch(_control, newChar)];
            newChar = _ring1[IndexSearch(_control, newChar)];

            return newChar;
        }

        private void SetDefaults()
        {
            _control = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            _ring1 = "DMTWSILRUYQNKFEJCAZBPGXOHV";
            _ring2 = "HQZGPJTMOBLNCIFDYAWVEUSRKX";
            _ring3 = "UQNTLSZFMREHDPXKIBVYGJCWOA";
            _keyOffset = new int[] { 0, 0, 0 };

            lblInput.Content = "";
            lblEncrpyt.Content = "";
            lblEncrpytMirror.Content = "";

            DisplayRing(lblControlRing, _control);
            DisplayRing(lblRing1, _ring1);
            DisplayRing(lblRing2, _ring2);
            DisplayRing(lblRing3, _ring3);
        }

        private void Rotate(bool forward)
        {
            if (forward)
            {
                _keyOffset[2]++;
                _ring1 = MoveValues(forward, _ring1);

                if (_keyOffset[2] / _control.Length >= 1)
                {
                    _keyOffset[2] = 0;
                    _keyOffset[1]++;
                    _ring2 = MoveValues(forward, _ring2);
                    if (_keyOffset[1] / _control.Length >= 1)
                    {
                        _keyOffset[1] = 0;
                        _keyOffset[0]++;
                        _ring3 = MoveValues(forward, _ring3);
                    }
                }
            }
            else
            {
                if (_keyOffset[2] > 0 || _keyOffset[1] > 0)
                {
                    _keyOffset[2]--;
                    _ring1 = MoveValues(forward, _ring1);
                    if (_keyOffset[2] < 0)
                    {
                        _keyOffset[2] = 25;
                        _keyOffset[1]--;
                        _ring2 = MoveValues(forward, _ring2);
                        if (_keyOffset[1] < 0)
                        {
                            _keyOffset[1] = 25;
                            _keyOffset[0]--;
                            _ring3 = MoveValues(forward, _ring3);
                            if (_keyOffset[0] < 0)
                                _keyOffset[0] = 25;
                        }
                    }
                }
            }

            DisplayOffset(); 
        }

        private string MoveValues(bool forward, string ring)
        {
            char movingValue = ' ';
            string newRing = "";

            if (forward)
            {
                movingValue = ring[0];
                for (int x = 1; x < ring.Length; x++)
                    newRing += ring[x];
                newRing += movingValue;
            }
            else
            {
                movingValue = ring[25];
                for (int x = 0; x < ring.Length - 1; x++)
                    newRing += ring[x];
                newRing = movingValue + newRing;
            }

            return newRing;
        }

        private void btnRotor_Click(object sender, RoutedEventArgs e)
        {
            SetDefaults();

            if (int.TryParse(txtBRing1Init.Text, out _initOffset[0]) &&
                int.TryParse(txtBRing2Init.Text, out _initOffset[1]) &&
                int.TryParse(txtBRing3Init.Text, out _initOffset[2]))
            {
                if (_initOffset[0] >= 0 && _initOffset[0] <= 25 &&
                    _initOffset[1] >= 0 && _initOffset[1] <= 25 &&
                    _initOffset[2] >= 0 && _initOffset[2] <= 25)
                {
                    txtBRing1Init.IsEnabled = false;
                    txtBRing2Init.IsEnabled = false;
                    txtBRing3Init.IsEnabled = false;

                    _rotor = true;
                    btnRotor.Content = "Settings Lock";

                    _ring1 = InitializeRotors(_initOffset[0], _ring1);
                    _ring2 = InitializeRotors(_initOffset[1], _ring2);
                    _ring3 = InitializeRotors(_initOffset[2], _ring3);

                    DisplayRing(lblRing1, _ring1);
                    DisplayRing(lblRing2, _ring2);
                    DisplayRing(lblRing3, _ring3);
                    DisplayOffset();
                }
            }
        }

        private string InitializeRotors(int initial, string ring)
        {
            string newRing = ring;
            for (int x = 0; x < initial; x++)
                newRing = MoveValues(true, newRing);
            return newRing;
        }

        private string RemoveLastLetter(string word)
        {
            string newWord = "";
            for (int x = 0; x < word.Length - 1; x++)
                newWord += word[x];
            return newWord;
        }


        private void txtBRing1Init_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBRing1Init.Text = "";
        }

        private void txtBRing2Init_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBRing2Init.Text = "";
        }

        private void txtBRing3Init_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBRing3Init.Text = "";
        }


        private void DisplayOffset()
        {
            txtBRing1Init.Text = _keyOffset[0] + "";
            txtBRing2Init.Text = _keyOffset[1] + "";
            txtBRing3Init.Text = _keyOffset[2] + "";
        }


        private void SetupPlugboard(string plugboardSettings)
        {
            _plugboard.Clear();
            bool hasError = false;

            string[] pairs = plugboardSettings.ToUpper().Split(' ');
            foreach (string pair in pairs)
            {
                if (pair.Length == 2)
                {
                    _plugboard[pair[0]] = pair[1];
                    _plugboard[pair[1]] = pair[0];
                }
                //Added Error Handling
                else if (pair.Length != 2 && pair.Length != 0)
                {
                    MessageBox.Show("Error! Please enter two consecutive letters.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    hasError = true;
                }
            }

            if (!hasError)
            {
                txtPlugboard.IsEnabled = false;
            }
        }

        //Added Error Handling
        private void btnSetPlugboard_Click(object sender, RoutedEventArgs e)
        {
            if (_plugboardSet)
            {
                MessageBox.Show("Plugboard is already set.");
                return;
            }

            SetupPlugboard(txtPlugboard.Text);
            if (!txtPlugboard.IsEnabled)
            {
                _plugboardSet = true;
                btnRotor.IsEnabled = true;
            }
        }

        private void txtPlugboard_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblInput.Content = "";
            lblEncrpyt.Content = "";
            lblEncrpytMirror.Content = "";
        }

        //All events for Virtual Keyboard
        private void qbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'Q';
            KeyPress(letter);
        }

        private void wbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'W';
            KeyPress(letter);
        }

        private void ebtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'E';
            KeyPress(letter);
        }

        private void rbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'R';
            KeyPress(letter);
        }

        private void tbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'T';
            KeyPress(letter);
        }

        private void zbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'Z';
            KeyPress(letter);
        }

        private void ubtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'U';
            KeyPress(letter);
        }   

        private void ibtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'I';
            KeyPress(letter);
        }

        private void obtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'O';
            KeyPress(letter);
        }

        private void abtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'A';
            KeyPress(letter);
        }

        private void sbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'S';
            KeyPress(letter);
        }

        private void dbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'D';
            KeyPress(letter);
        }

        private void fbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'F';
            KeyPress(letter);
        }

        private void gbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'G';
            KeyPress(letter);
        }

        private void hbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'H';
            KeyPress(letter);
        }

        private void jbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'J';
            KeyPress(letter);
        }

        private void kbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'K';
            KeyPress(letter);
        }

        private void pbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'P';
            KeyPress(letter);
        }

        private void ybtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'Y';
            KeyPress(letter);
        }

        private void xbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'X';
            KeyPress(letter);
        }

        private void cbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'C';
            KeyPress(letter);
        }

        private void vbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'V';
            KeyPress(letter);
        }

        private void bbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'B';
            KeyPress(letter);
        }

        private void nbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'N';
            KeyPress(letter);
        }

        private void mbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'M';
            KeyPress(letter);
        }

        private void lbtn_Click(object sender, RoutedEventArgs e)
        {
            char letter = 'L';
            KeyPress(letter);
        }
    }
}


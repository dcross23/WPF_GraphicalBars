using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphicalBars.src.utils
{
    class NumericTextBox : TextBox
    {
        public NumericTextBox()
        {
            this.PreviewTextInput += new TextCompositionEventHandler(NumericTextBox_PreviewTextInput);
            this.KeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
            this.LostFocus += new RoutedEventHandler(NumericTextBox_LostFocus);
            this.ContextMenu = null; //Remove default context menu for cut,copy,paste
        }

        public double DoubleValue
        {
            get {
                if (this.Text == "") return 0;
                else return Double.Parse(this.Text, ConfigConstants.cultureInfoUsed); 
            }
        }

        void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            NumberFormatInfo numberFormatInfo = ConfigConstants.cultureInfoUsed.NumberFormat;

            //Acordarse de poner esto en la documentación
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            string negativeSign = numberFormatInfo.NegativeSign;
            string caracter = e.Text;

            if (e.Text.Length > 0 &&!Char.IsDigit(e.Text[0]))
            {
                if (!caracter.Equals(decimalSeparator) && !caracter.Equals(negativeSign))
                {
                    if (!(caracter == "\b"))
                    {
                        //Is the caracter introduced is not a number, is not '.' or '-', and is not a backslash, the
                        // event spread is stopped.
                        e.Handled = true;
                    }
                }
            }
        }
   
        void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                AdjustInvalidTextToDoubleValues();
            }
        }

        void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AdjustInvalidTextToDoubleValues();
        }

        /**
         * If we enter a text like: '-', '.', '-.3', '2-3', ... (or combinations of these),
         *  the double parser will fail, so, to prevent this, it will give a default value
         *  of 0.
         */
        private void AdjustInvalidTextToDoubleValues()
        {
            //out means a returned value through that parameter
            //_   means we discard that returned value
            if (!double.TryParse(this.Text,NumberStyles.Any, ConfigConstants.cultureInfoUsed, out _)) 
                this.Text = "0";
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace VaroctoOCT
{
    public class AlphaNumTextBox : TextBox
    {
        public AlphaNumTextBox()
        {
            MaxLength = Properties.Settings.Default.TextBoxMaxLength;
        }
    }

    public class NumTextBox : TextBox
    {
        public NumTextBox()
        {
            MaxLength = Properties.Settings.Default.TextBoxMaxLength;
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (e.Text.Contains('0')
                || e.Text.Contains('1')
                || e.Text.Contains('2')
                || e.Text.Contains('3')
                || e.Text.Contains('4')
                || e.Text.Contains('5')
                || e.Text.Contains('6')
                || e.Text.Contains('7')
                || e.Text.Contains('8')
                || e.Text.Contains('9'))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }

            base.OnPreviewTextInput(e);
        }
    }

}

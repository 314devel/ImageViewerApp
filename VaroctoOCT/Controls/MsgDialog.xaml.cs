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
using System.Windows.Shapes;

namespace VaroctoOCT
{
    /// <summary>
    /// Interaction logic for MsgDialog.xaml
    /// </summary>
    public partial class MsgDialog : Window
    {
        public enum MsgDialogResult
        {
            None,
            OK,
            Yes,
            No
        }

        public enum MsgDialogType
        {
            OK,
            YesNo
        }

        public enum MsgDialogIcon
        {
            Question,
            Info,
            Warning
        }

        private MsgDialog(MsgDialogType type)
        {
            InitializeComponent();

            Type = type;
            Result = MsgDialogResult.None;
            Closing += MsgDialog_Closing;
        
        }

        private void MsgDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Check if the user pressed the X button on the title bar
            if (Result == MsgDialogResult.None)
            {
                if (Type == MsgDialogType.OK)
                    Result = MsgDialogResult.OK;
                else
                    Result = MsgDialogResult.No;
            }
        }

        public MsgDialogResult Result { get; private set; }

        private MsgDialogType Type { get; set; }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (sender as Button);

            if (button.Name == "OKButton")
            {
                Result = MsgDialogResult.OK;
            }
            else if (button.Name == "YesButton")
            {
                Result = MsgDialogResult.Yes;
            }
            else if (button.Name == "NoButton")
            {
                Result = MsgDialogResult.No;
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }

            this.Close();
        }

        #region Public Show Methods
        public static MsgDialogResult Show(string message, string dialogTitle, 
            MsgDialogType dialogType = MsgDialogType.OK, 
            MsgDialogIcon dialogIcon = MsgDialogIcon.Info)
        {
            MsgDialogResult result = MsgDialogResult.None;

            MsgDialog dialog = new MsgDialog(dialogType);
            dialog.Title = dialogTitle;
            dialog.MsgBlock.Text = message;

            switch (dialogType)
            {
                case MsgDialogType.OK:
                    dialog.OKButton.Visibility = Visibility.Visible;
                    dialog.YesButton.Visibility = dialog.NoButton.Visibility = Visibility.Hidden;
                break;

                case MsgDialogType.YesNo:
                    dialog.OKButton.Visibility = Visibility.Hidden;
                    dialog.YesButton.Visibility = dialog.NoButton.Visibility = Visibility.Visible;
                    break;
            }

            string iconPath = String.Empty;

            switch (dialogIcon)
            {
                case MsgDialogIcon.Info:
                    iconPath = "/ImageViewer;component/Icons/Information.png";
                    break;

                case MsgDialogIcon.Question:
                    iconPath = "/ImageViewer;component/Icons/Question.png";
                    break;

                case MsgDialogIcon.Warning:
                    iconPath = "/ImageViewer;component/Icons/Warning.png";
                    break;
                    
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(iconPath, UriKind.Relative);
            image.EndInit();
            dialog.DialogIcon.Source = image;

            dialog.Topmost = true;
            dialog.ShowDialog();
            result = dialog.Result;
            dialog = null;

            return result;
        }
        #endregion
    }
}

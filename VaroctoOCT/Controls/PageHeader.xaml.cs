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

namespace VaroctoOCT
{
    /// <summary>
    /// Interaction logic for PageHeader.xaml
    /// </summary>
    public partial class PageHeader : UserControl
    {
        /// <summary>
        /// Indicates whether the mouse is currently in the logout label's bounds
        /// </summary>
        bool _IsMouseIn = false;

        /// <summary>
        /// This delegate allows the parent to decide whether to show a confirmation
        /// dialog if the user wants to navigate backwards. This is meant to handle the
        /// case if data has been entered in a page.
        /// </summary>
        /// <returns></returns>
        public delegate bool ShowBackConfDialogCallback();
        public ShowBackConfDialogCallback BackConfDialogCallback
        {
            get; set;
        }


        public PageHeader()
        {
            InitializeComponent();

            Loaded += PageHeader_Loaded;
        }

        private void PageHeader_Loaded(object sender, RoutedEventArgs e)
        {
            if (User.Current != null)
            {
                SetBackButtonVisible();

                _LogoutLabel.Text = Application.Current.Resources["LogoutTitle"].ToString() +
                    " " + User.Current.FullName() + "...";
            }

        }

        /// <summary>
        /// Sets the back button's visibility
        /// </summary>
        private void SetBackButtonVisible()
        {
            if (PageStateMachine.Instance.CanGoBackFromCurrentPage)
            {
                _BackButton.Visibility = Visibility.Visible;
            }
            else
            {
                _BackButton.Visibility = Visibility.Collapsed;
            }
        }

        public string HeaderTitle
        {
            get
            {
                return _Title.Text;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _Title.Text = value;

            }
        }

        private void Logoutlabel_MouseEnter(object sender, MouseEventArgs e)
        {
            _IsMouseIn = true;
            Cursor = Cursors.Hand;
        }

        private void LogoutLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            _LogoutLabel.Foreground = Brushes.White;
            _IsMouseIn = false;
        }

        private void LogoutLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _LogoutLabel.Foreground = Brushes.Silver;

        }

        private void LogoutLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _LogoutLabel.Foreground = Brushes.White;

            if (_IsMouseIn)
            {
                // Show the logout dialog 
                MessageBoxResult result = MessageBox.Show(Application.Current.Resources["LogoutConfMsg"].ToString(),
                    Application.Current.Resources["LogoutTitle"].ToString(),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);    
                
                if (result == MessageBoxResult.Yes)
                {
                    User.LogoutCurrent();
                    PageStateMachine.Instance.ResetAndGoHome();
                }          
            }

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            bool goBack = true;

            if (BackConfDialogCallback != null)
            {
                // Have the control's parent check whether to show a dialog prompting the
                // user whether they would like to navigate back. This is typically done
                // if they have entered data.
                if (BackConfDialogCallback())
                {
                    if (App.ShowGoBackDialog(_Title.Text) == MessageBoxResult.No)
                    {
                        goBack = false;
                    }
                }

            }

            if (goBack)
                PageStateMachine.Instance.GotoPrevPage();
        }

    }
}

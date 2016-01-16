using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Security;
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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();

            // Clear the current user
            User.Current = null;

            // Give focus to the username textbox
            Focus();
            _UsernameTextBox.Focus();
            
        }
        
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            // Verify that the Current User is invalid
            System.Diagnostics.Debug.Assert(User.Current == null);

            // Disable the controls while the entered credentials are checked
            AreControlsEnabled = false;
            // Check if the specified user and password are valid
            Tuple<string, SecureString> t = new Tuple<string, SecureString>(_UsernameTextBox.Text, _PasswordBox.SecurePassword.Copy());
            DatabaseResult result = await Task.Run(() =>
            {
#if IGNORE_LOGIN
                User.Current = User.TestUser;
                return DatabaseResult.OK;
#else
                User user = null;
                DatabaseResult ur = Database.CheckUserCredentials(t.Item1, t.Item2, out user);
                User.Current = user;
                return ur;
#endif
            });

            // Re-enable the controls
            AreControlsEnabled = true;

            // Process the result of the login
            ProcessLoginResult(result);
        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetLoginButtonVisibility();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_PasswordBox != null)
            {
                _PasswordBox.SecurePassword.Dispose();
            }
        }

        private bool AreControlsEnabled
        {
            set
            {
                _LoginButton.IsEnabled = _UsernameTextBox.IsEnabled =
                    _PasswordBox.IsEnabled = value;
            }
        }

        private void SetLoginButtonVisibility()
        {
            _LoginButton.IsEnabled = (_UsernameTextBox.Text.Length > 0);
        }

        private void ProcessLoginResult(DatabaseResult result)
        {
            switch (result)
            {
                case DatabaseResult.InvalidUsername:
                case DatabaseResult.UserNotActive:
                case DatabaseResult.InvalidPassword:
                case DatabaseResult.ReadError:
                case DatabaseResult.CommunicationError:
                    // Verify that the Current User is invalid
                    System.Diagnostics.Debug.Assert(User.Current == null);

                    // Pass the result to the error handler to notify the user
                    App.HandleDatabaseError(result);

                    _PasswordBox.Clear();
                    break;

                case DatabaseResult.OK:
                    // Verify that the Current User is valid
                    System.Diagnostics.Debug.Assert(User.Current != null);

                    // Go to the next page
                    PageStateMachine.Instance.GotoPage(PageStateMachine.PageType.SelectPatient);
                    break;

                case DatabaseResult.None:
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

    }
}

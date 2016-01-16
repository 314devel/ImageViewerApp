using System;
using System.Windows;
using System.Windows.Data;
using uEye;

namespace VaroctoOCT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MessageBoxResult ShowGoBackDialog(string pageName)
        {
            MessageBoxResult result = MessageBox.Show((string)App.Current.FindResource("GoBackDialogMsg"),
                pageName,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result;
        }

        public static void HandleDatabaseError(DatabaseResult result)
        {
            switch (result)
            {
                case DatabaseResult.CommunicationError:
                    MessageBox.Show(Application.Current.Resources["DBConnetError"].ToString(),
                        Application.Current.Resources["DatabaseDlgTitle"].ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;

                case DatabaseResult.WriteError:
                    MessageBox.Show(Application.Current.Resources["DBWriteError"].ToString(),
                        Application.Current.Resources["DatabaseDlgTitle"].ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;

                case DatabaseResult.ReadError:
                    MessageBox.Show(Application.Current.Resources["DBReadrror"].ToString(),
                        Application.Current.Resources["DatabaseDlgTitle"].ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;

                case DatabaseResult.InvalidUsername:
                    // Verify that the Current User is invalid
                    System.Diagnostics.Debug.Assert(User.Current == null);

                    MessageBox.Show(Application.Current.Resources["InvalidUsernameMsg"].ToString(),
                        Application.Current.Resources["LoginButtonTitle"].ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;

                case DatabaseResult.UserNotActive:
                    // Verify that the Current User is invalid
                    System.Diagnostics.Debug.Assert(User.Current == null);

                    MessageBox.Show(Application.Current.Resources["UserNotActiveMsg"].ToString(),
                        Application.Current.Resources["LoginButtonTitle"].ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;

                case DatabaseResult.InvalidPassword:
                    // Verify that the Current User is invalid
                    System.Diagnostics.Debug.Assert(User.Current == null);

                    MessageBox.Show(Application.Current.Resources["InvalidPasswordMsg"].ToString(),
                        Application.Current.Resources["LoginButtonTitle"].ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;
            }
        }
    }

    [ValueConversion(typeof(string),typeof(string))]
    public class GenderConverter : IValueConverter

    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {
            if (string.Compare((string)value, PatientInfo.FEMALE_DB_VALUE, true) == 0)
            {
                return App.Current.Resources["FemaleTitle"].ToString();
            }
            else if (string.Compare((string)value, PatientInfo.MALE_DB_VALUE, true) == 0)
            {
                return App.Current.Resources["MaleTitle"].ToString();
            }
            else
            {
                throw new ArgumentException();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            throw new NotImplementedException();

        }

        #endregion

    }

}

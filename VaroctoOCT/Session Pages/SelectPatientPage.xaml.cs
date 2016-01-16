using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
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
    /// Interaction logic for SelectPatientPage.xaml
    /// </summary>
    public partial class SelectPatientPage : UserControl
    {
        public SelectPatientPage()
        {
            InitializeComponent();
            _PageHeader.HeaderTitle = Application.Current.Resources["PatientPageTitle"].ToString();

            BindDataGrid();
            SetDobColFormatting();
        }

        /// <summary>
        /// Reads the contents of the patients row from the DB and populates the Patient 
        /// Selection list. 
        /// </summary>
        public void BindDataGrid()
        {
            DBDataSetTableAdapters.patientsTableAdapter adapter = new DBDataSetTableAdapters.patientsTableAdapter();
            _PatientList.DataContext = null;
            _PatientList.DataContext = adapter.GetData();
        }

        /// <summary>
        /// Formats the date of birth column based on the current culture's date pattern
        /// </summary>
        private void SetDobColFormatting()
        {
            string colName = (string)App.Current.FindResource("DOBTitle");
            foreach (DataGridTextColumn column in _PatientList.Columns)
            {
                if ((string)column.Header == colName)
                {
                    column.Binding.StringFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                }
            }
        }

        private void HandlePatientSelection()
        {
            if (_PatientList.SelectedIndex >= 0)
            {
                InitSession();
                PageStateMachine.Instance.GotoPage(PageStateMachine.PageType.ViewImages);
            }
        }


        private void DataGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_PatientList.SelectedIndex >= 0)
            {
                HandlePatientSelection();
            }
        }

        
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            HandlePatientSelection();
        }

        private void PatientList_RowSelected(object sender, SelectionChangedEventArgs e)
        {
            if (_PatientList.SelectedIndex >= 0)
            {
                _NextButton.IsEnabled = true;
            }
        }

        private void InitSession()
        {
            System.Diagnostics.Debug.Assert(_PatientList.SelectedIndex >= 0);

            // Pull the selected row
            DBDataSet.patientsRow row = ((_PatientList.SelectedItem as DataRowView).Row as DBDataSet.patientsRow);

            if (row != null)
            {
                // Reset the session info object
                SessionInfo.Instance.ResetSession();

                // Configure the session info object with the row data
                SessionInfo.Instance.PatientInfo = new PatientInfo()
                {
                    PatientID = row.pPatientID,
                    LastName = row.lastName,
                    FirstName = row.firstName,
                    DateOfBirth = row.dob,
                    GenderDBValue = row.gender,
                    Guid = Guid.Parse(row.vartoctoGUID)
                };
            }

        }

    }

}

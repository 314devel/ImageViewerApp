using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaroctoOCT
{

    public enum Gender
    {
        Female = 0,
        Male = 1
    }

    public enum Eye
    {
        OS = 0,
        OD
    }

    public enum ScanType
    {
        None = 0
    }

    public struct PatientInfo
    {
        /// <summary>
        /// Defines the string that is saved to the databse when the configured patient
        ///is a female.
        /// </summary>
        public const string FEMALE_DB_VALUE = "F";
        /// <summary>
        /// Defines the string that is saved to the databse when the configured patient
        ///is a male.
        /// </summary>
        public const string MALE_DB_VALUE = "M";

        public string PatientID;
        public string FirstName;
        public string LastName;
        public DateTime DateOfBirth;
        public Gender Gender;
        public Guid Guid;

        /// <summary>
        /// Converts the value stored in Gender to its string represenation in the database.
        /// Returns the string representation of the value stored in Gender that can be used to 
        /// save into the database.
        /// </summary>
        public string GenderDBValue
        {
            get
            {
                if (Gender == Gender.Female)
                    return FEMALE_DB_VALUE;
                else
                    return MALE_DB_VALUE;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (string.Compare(value, FEMALE_DB_VALUE, true) == 0)
                {
                    Gender = Gender.Female;
                }
                else if (string.Compare(value, MALE_DB_VALUE, true) == 0)
                {
                    Gender = Gender.Male;
                }
                else
                {
                    throw new ArgumentException();
                }

            }
        }

        /// <summary>
        /// Returns the string value of Gender. This is used so that the string
        /// is pulled from the ApplicationStrings resource file.
        /// </summary>
        public string GenderString
        {
            get
            {
                if (Gender == Gender.Female)
                {
                    return App.Current.Resources["FemaleTitle"].ToString();
                }
                else
                {
                    return App.Current.Resources["MaleTitle"].ToString();
                }
            }
        }

    }

    class SessionInfo
    {
        public PatientInfo PatientInfo { get; set; }

        public Eye Eye { get; set; }

        public Int64 visitID { get; set; }

        public string VisitNotes { get; set; }

        //aaa Stores acquired scans
        public List<ScanData> Scans { get; set; }

        public static SessionInfo Instance = new SessionInfo();

        private SessionInfo() { }

        public void ResetSession()
        {
            PatientInfo = new PatientInfo();
            Eye = Eye.OS;
        }
    }

    public class ScanData
    {
        public ScanType ScanType { get; set; }
        public string Notes { get; set; }
        public System.Windows.Media.ImageSource Scan { get; set; }

    }
}

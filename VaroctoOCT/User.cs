using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaroctoOCT
{
    public class User
    {

        /// <summary>
        /// Gets or sets the currently logged in user.
        /// </summary>
        public static User Current { get; set; }
        /// <summary>
        /// Logs out the "Current" user.
        /// </summary>
        public static void LogoutCurrent()
        {
            Current = null;
        }

#if IGNORE_LOGIN
        /// <summary>
        /// Returns a test user type that can be used for testing the GUI. 
        /// </summary>
        public static User TestUser
        {
            get
            {
                return new User()
                {
                    UserId = 0,
                    UserName = "jsmith",
                    FirstName = "John",
                    LastName = "Smith",
                    Password = "",
                    IsActive = true,
                    Privileges = (UserPrivileges.Acquire | UserPrivileges.Administrator | UserPrivileges.Save| UserPrivileges.Technician)
                };
            }
        }
#endif

        public User()
        { }

        public User(DBDataSet.usersRow row)
        {
            if (row == null)
                throw new ArgumentNullException();

            UserId = row.uUserID;
            FirstName = row.firstName;
            LastName = row.lastName;
            UserName = row.userName;
            Password = row.password;
            IsActive = row.active;
            CreationDate = row.creationDate;
        }

        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName(bool isLastNameFirst = false)
        {
            string fullName;

            if (isLastNameFirst)
            {
                fullName = LastName + ", " + FirstName;
            }
            else
            {
                fullName = FirstName + " " + LastName;
            }

            return fullName;
        }

        /// <summary>
        /// Stores a MD5 hash of the password.
        /// </summary>
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public UserPrivileges Privileges { get; set; }

        /// <summary>
        /// Returns true if the user has Acquire rights.
        /// </summary>
        public bool HasAcquireRights
        {
            get
            {
                return (Privileges & UserPrivileges.Acquire) == UserPrivileges.Acquire;
            }
            set
            {
                if (value)
                    Privileges |= UserPrivileges.Acquire;
                else
                    Privileges &= ~UserPrivileges.Acquire;
            }
        }

        /// <summary>
        /// Returns true if the user has Save rights.
        /// </summary>
        public bool HasSaveRights
        {
            get
            {
                return (Privileges & UserPrivileges.Save) == UserPrivileges.Save;
            }
            set
            {
                if (value)
                    Privileges |= UserPrivileges.Save;
                else
                    Privileges &= ~UserPrivileges.Save;
            }
        }

        /// <summary>
        /// Returns true if the user has Technician rights.
        /// </summary>
        public bool HasTechRights
        {
            get
            {
                return (Privileges & UserPrivileges.Technician) == UserPrivileges.Technician;
            }
            set
            {
                if (value)
                    Privileges |= UserPrivileges.Technician;
                else
                    Privileges &= ~UserPrivileges.Technician;
            }
        }

        /// <summary>
        /// Returns true if the user has Administrator rights.
        /// </summary>
        public bool HasAdminRights
        {
            get
            {
                return (Privileges & UserPrivileges.Administrator) == UserPrivileges.Administrator;
            }
            set
            {
                if (value)
                    Privileges |= UserPrivileges.Administrator;
                else
                    Privileges &= ~UserPrivileges.Administrator;
            }
        }

        /// <summary>
        /// Converts the value of the specified securestring to a MD5 hash and saves
        /// it to the Password field.
        /// </summary>
        /// <param name="secureString"></param>
        public void SetPasswordMd5Hash(System.Security.SecureString secureString)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            Password = Utilities.GetMD5Hash(md5, secureString.ConvertToUnsecureString());
        }
    }

    [Flags]
    public enum UserPrivileges : UInt32
    {
        None = 0,
        Acquire = 1,
        Save = 2,
        Technician = 32,
        Administrator = 64
    }
}

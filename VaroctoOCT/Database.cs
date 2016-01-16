using System;
using System.Security;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;

namespace VaroctoOCT
{
    public enum DatabaseResult
    {
        None,
        OK,
        InvalidUsername,
        UserNotActive,
        InvalidPassword,
        CommunicationError,
        WriteError,
        ReadError
    }

    //TODO: Be able to add password to connection string
    class Database
    {
        #region Patient Table Routines
        /// <summary>
        /// Adds a new patient to the "patients" table in the database
        /// </summary>
        /// <param name="patientInfo"></param>
        /// <returns></returns>
        public static DatabaseResult AddNewPatient(PatientInfo patientInfo)
        {
            DatabaseResult result = DatabaseResult.None;
            DBDataSet.patientsDataTable patDataTable = new DBDataSet.patientsDataTable();

            if (patDataTable != null)
            {
                try
                {
                    // Commit the new row to the database
                    DBDataSetTableAdapters.patientsTableAdapter adapter = new DBDataSetTableAdapters.patientsTableAdapter();

                    int writeResult = adapter.Insert(patientInfo.PatientID,
                            patientInfo.LastName,
                            patientInfo.FirstName,
                            patientInfo.DateOfBirth,
                            patientInfo.GenderDBValue,
                            DateTime.Now,
                            Guid.NewGuid().ToString());

                    if (writeResult == 1)
                    {
                        result = DatabaseResult.OK;
                    }
                    else
                    {
                        result = DatabaseResult.WriteError;
                    }
                }
                catch
                {
                    result = DatabaseResult.CommunicationError;
                }
            }

            return result;
        }

        #endregion

        #region User Routines
        /// <summary>
        /// Checks the specified user credentials against the database. If the 
        /// user is found and the password is valid (DatabaseResult = OK), the 
        /// passed User object will contain the user's credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="foundUser"></param>
        /// <returns></returns>
        public static DatabaseResult CheckUserCredentials(string username, SecureString password, out User foundUser)
        {
            DatabaseResult result = DatabaseResult.None;
            foundUser = null;

            try
            {
                User user = null;
                result = FindUser(username, out user);

                if (result == DatabaseResult.OK)
                {
                    if (user != null)
                    {
                        // Check that the user is active
                        if (user.IsActive)
                        {
                            // Compare the entered password to the stored password  
                            using (MD5 md5 = MD5.Create())
                            {
                                if (Utilities.CompareMD5Hash(md5, password.ConvertToUnsecureString(), user.Password))
                                {
                                    foundUser = user;

                                    // Update the database with the login date/time for this user
                                    result = SetLoginDateTime(user.UserId);
                                }
                                else
                                {
                                    result = DatabaseResult.InvalidPassword;
                                }
                            }

                        }
                        else
                        {
                            result = DatabaseResult.UserNotActive;
                        }
                    }
                    else
                    {
                        result = DatabaseResult.InvalidUsername;
                    }
                }

            }
            catch
            {
                result = DatabaseResult.CommunicationError;
            }
            finally
            {
                // Dispose the password object
                password.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Searches the database for the specified username, regardless if the entry in the database is marked inactive. 
        /// A user entry is returned if the user is found. Otherwise the returned value is null.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static DatabaseResult FindUser(string username, out User foundUser)
        {
            foundUser = null;
            DatabaseResult result = DatabaseResult.None;

            try
            {
                DBDataSet.usersDataTable users = new DBDataSet.usersDataTable();

                if (users != null)
                {
                    DBDataSetTableAdapters.usersTableAdapter useradapter = new DBDataSetTableAdapters.usersTableAdapter();
                    DBDataSetTableAdapters.userrolesTableAdapter urolesadapt = new DBDataSetTableAdapters.userrolesTableAdapter();
                    DBDataSetTableAdapters.rolesTableAdapter rolesadapt = new DBDataSetTableAdapters.rolesTableAdapter();

                    users = useradapter.GetData();

                    // Find the specified user
                    var userProperties =
                                    // Go through the list of users
                                    from user in users.AsEnumerable()
                                        // Find all roles assigned to the user
                                    join userroles in urolesadapt.GetData() on user.uUserID equals userroles.uUserID
                                    join rolesMask in rolesadapt.GetData() on userroles.rRoleID equals rolesMask.rRoleID
                                    // Filter for the specified user and for roles that are marked active
                                    where (string.Compare((string)user["userName"], username, StringComparison.OrdinalIgnoreCase) == 0
                                        && userroles.active == true)
                                    select new
                                    {
                                        FirstName = user.firstName,
                                        LastName = user.lastName,
                                        UserId = user.uUserID,
                                        UserName = user.userName,
                                        Password = user.password,
                                        IsActive = user.active,
                                        CreationDate = user.creationDate,
                                        Permission = rolesMask.permissionsMask
                                    };

                    if (userProperties.Count() != 0)
                    {
                        int mask = 0;
                        // Get all the permissions that are active for the current user
                        // (there is a row in the database per permission)
                        foreach (var userProp in userProperties)
                        {
                            mask |= userProp.Permission;
                        }

                        // Initialize the user object
                        var user = userProperties.First();
                        foundUser = new User()
                        {
                            UserId = user.UserId,
                            UserName = user.UserName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Password = user.Password,
                            Privileges = (UserPrivileges)mask,
                            IsActive = user.IsActive,
                            CreationDate = user.CreationDate
                        };
                    }

                    result = DatabaseResult.OK;
                }

            }
            catch
            {
                result = DatabaseResult.CommunicationError;
            }

            return result;
        }

        /// <summary>
        /// Returns true if the specified username exists in the database. This method
        /// ignores whether the user account is active.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static DatabaseResult DoesUsernameExist(string username, out bool isUserFound)
        {
            User user = null;
            isUserFound = false;
            return FindUser(username, out user);
        }

        public static DatabaseResult GetUserPermissions(long userID, out UserPrivileges userPermissions)
        {
            DatabaseResult result = DatabaseResult.None;
            userPermissions = UserPrivileges.None;

            try
            {
                DBDataSetTableAdapters.userrolesTableAdapter urolesadapt = new DBDataSetTableAdapters.userrolesTableAdapter();
                DBDataSetTableAdapters.rolesTableAdapter rolesadapt = new DBDataSetTableAdapters.rolesTableAdapter();

                var permissions = from userroles in urolesadapt.GetData()
                                  join roles in rolesadapt.GetData() on userroles.rRoleID equals roles.rRoleID
                                  where userID == userroles.uUserID && userroles.active == true
                                  select new { Value = roles.permissionsMask };

                if (permissions != null)
                {
                    foreach (var permission in permissions)
                    {
                        userPermissions |= (UserPrivileges)permission.Value;
                    }
                }
            }
            finally
            {
                result = DatabaseResult.CommunicationError;
            }

            return result;
        }


        private static DatabaseResult SetLoginDateTime(long userId)
        {
            DatabaseResult result = DatabaseResult.None;

            DBDataSetTableAdapters.userrolesTableAdapter uradapter = new DBDataSetTableAdapters.userrolesTableAdapter();
            DBDataSet.userrolesDataTable urtable = uradapter.GetData();

            Dictionary<UserPrivileges, long> mapping = new Dictionary<UserPrivileges, long>();
            result = GetUserRoleMapping(out mapping);
            int insertResult = 0;

            DateTime loginTime = DateTime.Now;

            if (result == DatabaseResult.OK)
            {
                foreach (KeyValuePair<UserPrivileges, long> kvp in mapping)
                {
                    DBDataSet.userrolesRow rolesRow = urtable.FindByrRoleIDuUserID(kvp.Value, userId);

                    // Set the login time for this user
                    rolesRow.lastLogin = loginTime;
                }

                insertResult = uradapter.Update(urtable);

                if (insertResult == 0)
                {
                    result = DatabaseResult.WriteError;
                }
                else
                {
                    result = DatabaseResult.OK;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a new ID based on a 'Julian Date' (actually the same idea but different algorithm).
        /// This will work for up to about 1000 simultaneous users trying to get a new id at the same time.
        /// </summary>
        public static long NewUserId
        {
            get
            {
                DateTime d = DateTime.Now;
                //get the new id from the time
                long userId = Convert.ToInt64(d.ToOADate() * 10000);
                return userId;
            }
        }


        public static DatabaseResult AddNewUser(User user)
        {
            DatabaseResult result = DatabaseResult.None;

            try
            {
                DBDataSetTableAdapters.usersTableAdapter adapter = new DBDataSetTableAdapters.usersTableAdapter();
                DBDataSetTableAdapters.userrolesTableAdapter uradapter = new DBDataSetTableAdapters.userrolesTableAdapter();

                // Add the user entry to the database
                long userId = NewUserId;
                DateTime createDate = DateTime.Now;

                int insertResult = adapter.Insert(userId,
                    user.LastName,
                    user.FirstName,
                    user.UserName,
                    user.Password,
                    createDate,
                    (byte)1);

                // Add each user role to the userroles table
                if (insertResult == 1)
                {
                    insertResult = 1;

                    // Get a dictionary that maps user permissions to its respective row in the DB
                    Dictionary<UserPrivileges, long> mapping = new Dictionary<UserPrivileges, long>();
                    result = GetUserRoleMapping(out mapping);

                    if (result == DatabaseResult.OK)
                    {
                        foreach (KeyValuePair<UserPrivileges, long> kvp in mapping)
                        {
                            insertResult &= uradapter.Insert(kvp.Value,
                                userId,
                                createDate,
                                null,
                                Convert.ToByte(((user.Privileges & kvp.Key) == kvp.Key)));

                            if (insertResult != 1)
                            {
                                break;
                            }
                        }

                        if (insertResult != 1)
                        {
                            result = DatabaseResult.WriteError;
                        }
                        else
                        {
                            result = DatabaseResult.OK;
                        }
                    }

                }
                else
                {
                    result = DatabaseResult.WriteError;
                }
            }
            catch
            {
                result = DatabaseResult.CommunicationError;
            }

            return result;
        }

        /// <summary>
        /// Modifies an existing row in the users table. This method only updates the users table,
        /// the userroles table is not modified.
        /// </summary>
        /// <param name="row"></param>
        public static DatabaseResult UpdateUserRow(DBDataSet.usersRow row)
        {
            DatabaseResult result = DatabaseResult.None;

            try
            {
                DBDataSetTableAdapters.usersTableAdapter adapter = new DBDataSetTableAdapters.usersTableAdapter();

                int updateResult = adapter.Update(row);

                if (updateResult == 1)
                {
                    result = DatabaseResult.OK;
                }
                else
                {
                    result = DatabaseResult.WriteError;
                }
            }
            catch
            {
                result = DatabaseResult.CommunicationError;
            }

            return result;
        }

        public static DatabaseResult UpdateUserRow(User user)
        {
            DatabaseResult result = DatabaseResult.None;

            try
            {
                DBDataSetTableAdapters.usersTableAdapter adapter = new DBDataSetTableAdapters.usersTableAdapter();
                DBDataSetTableAdapters.userrolesTableAdapter uradapter = new DBDataSetTableAdapters.userrolesTableAdapter();

                DBDataSet.usersRow row = adapter.GetData().FindByuUserID(user.UserId);

                if (row != null)
                {
                    row.uUserID = user.UserId;
                    row.userName = user.UserName;
                    row.firstName = user.FirstName;
                    row.lastName = user.LastName;
                    row.password = user.Password;
                    row.active = user.IsActive;
                    row.creationDate = user.CreationDate;

                    int updateResult = adapter.Update(row);

                    if (updateResult == 1)
                    {
                        // Update the rows in the userroles table for this user

                        // Get a dictionary that maps user permissions to its respective row in the DB
                        Dictionary<UserPrivileges, long> mapping = new Dictionary<UserPrivileges, long>();
                        result = GetUserRoleMapping(out mapping);

                        int insertResult = 1;

                        if (result == DatabaseResult.OK)
                        {
                            // Get a table of the user roles so that there's one read and one 
                            // write operation to the database
                            DBDataSet.userrolesDataTable userRolesTable = uradapter.GetData();

                            foreach (KeyValuePair<UserPrivileges, long> kvp in mapping)
                            {
                                DBDataSet.userrolesRow rolesRow = userRolesTable.FindByrRoleIDuUserID(kvp.Value, user.UserId);

                                // Set whether this role is active for the user
                                rolesRow.active = (user.Privileges & kvp.Key) == kvp.Key;
                            }

                            insertResult = uradapter.Update(userRolesTable);

                            if (insertResult == 0)
                            {
                                result = DatabaseResult.WriteError;
                            }
                            else
                            {
                                result = DatabaseResult.OK;
                            }
                        }


                        result = DatabaseResult.OK;
                    }
                    else
                    {
                        result = DatabaseResult.WriteError;
                    }
                }
            }
            catch
            {
                result = DatabaseResult.CommunicationError;
            }

            return result;
        }

        /// <summary>
        /// Caches the results of calling GetUserRoleMapping.
        /// </summary>
        private static Dictionary<UserPrivileges, long> _RolesMap = null;
        /// <summary>
        /// Returns a mapping of each privilege type to its corresponding row in the database
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static DatabaseResult GetUserRoleMapping(out Dictionary<UserPrivileges, long> mapping)
        {
            DatabaseResult result = DatabaseResult.None;

            if (_RolesMap == null)
            {
                _RolesMap = new Dictionary<UserPrivileges, long>();

                try
                {
                    DBDataSetTableAdapters.rolesTableAdapter radapter = new DBDataSetTableAdapters.rolesTableAdapter();

                    foreach (var row in radapter.GetData())
                    {
                        _RolesMap.Add((UserPrivileges)row.permissionsMask, row.rRoleID);
                    }

                    result = DatabaseResult.OK;
                }
                catch
                {
                    result = DatabaseResult.CommunicationError;
                }

            }
            else
            {
                result = DatabaseResult.OK;
            }

            mapping = _RolesMap;

            return result;
        }

        #endregion

        #region visit
        public static Int64 AddNewVisit(Int64 lLocationID,
                                                string pPatientID,
                                                string dDeviceID,
                                                string physicianName,
                                                Int64 uUserID,
                                                string odRx,
                                                string osRx,
                                                float odLen,
                                                float osLen,
                                                float odSphere,
                                                float osSphere,
                                                float odAxis,
                                                float osAxis)
        {
            MySqlTransaction TR;
            MySqlConnection cn = new MySqlConnection();
            MySqlCommand cmd = new MySqlCommand();
            string sqlstring = @"INSERT INTO `varoctooct`.`visits` ( 
                                     `vVisitID`,`lLocationID`, `pPatientID`, `dDeviceID`, `physicianName`, 
                                    `uUserID`, `odRx`, `osRx`, `odLen`, `osLen`, `odSphere`, 
                                    `osSphere`, `odAxis`, `osAxis`) 
                                    VALUES 
                                    ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}')";
            DateTime d = DateTime.Now;
            long VisitID = -1;

            try
            {


                //todo format the strings

                DB_Open_Connection(cmd, cn);
                TR = cn.BeginTransaction();
                cmd.Connection = cn;
                cmd.Transaction = TR;

                //get the new visit id from the time
                VisitID = Convert.ToInt64(d.ToOADate() * 10000);


                string sql = string.Format(sqlstring, VisitID,
                                                     lLocationID,
                                                     pPatientID,
                                                     dDeviceID,
                                                     physicianName,
                                                     uUserID,
                                                     odRx,
                                                     osRx,
                                                     odLen,
                                                     osLen,
                                                     odSphere,
                                                     osSphere,
                                                     odAxis,
                                                     osAxis);
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                
                TR.Commit();
                cmd = null;

            }
            catch
            {
                VisitID = -1;
            }

            return (VisitID);

        }
        #endregion

        #region scanmetadata
        public static DatabaseResult AddScanmetadata(
                                                string scanGUID,
                                                Int64 vVisitID,
                                                int fixationTargetX,
                                                int fixationTargetY,
                                                int fastAxisLength,
                                                int slowAxisLength,
                                                int fastAxisOffset,
                                                int slowAxisOffset,
                                                byte rotate,
                                                byte reverse,
                                                int aScansPerBScan,
                                                int mRepeats,
                                                int bmRepeats,
                                                int xPixelOffset,
                                                int xFlybackPixels,
                                                string oDoS,
                                                string scanFilename,
                                                string fileMD5)
        {
            DatabaseResult result = DatabaseResult.None;

            DBDataSetTableAdapters.scanmetadataTableAdapter adapter = new DBDataSetTableAdapters.scanmetadataTableAdapter();
            int insertResult = adapter.Insert(
                                scanGUID,
                                vVisitID,
                                fixationTargetX,
                                fixationTargetY,
                                fastAxisLength,
                                slowAxisLength,
                                fastAxisOffset,
                                slowAxisOffset,
                                rotate,
                                reverse,
                                aScansPerBScan,
                                mRepeats,
                                bmRepeats,
                                xPixelOffset,
                                xFlybackPixels,
                                oDoS,
                                scanFilename,
                                fileMD5, DateTime.Now );

            // Add each user role to the userroles table
            if (insertResult != 1)
            {
                result = DatabaseResult.WriteError;
            }
            return result;
        }
        #endregion

        #region helper functions
        private static void DB_Open_Connection(MySqlCommand cmd, MySqlConnection cn)
        {

            if (cn.State == System.Data.ConnectionState.Closed)
            {
                cn.ConnectionString = Properties.Settings.Default.varoctooctConnectionString;
                cn.Open();
                cmd.Connection = cn;
            }
        }
        public static void DB_Close_Connection(MySqlConnection cn)
        {
            cn.Close();
        }
        #endregion
    }

}

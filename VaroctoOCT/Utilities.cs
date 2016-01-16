using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;

namespace VaroctoOCT
{
    static class Utilities
    {
        /// <summary>
        /// Converts the specified SecureString object to an unsecure string.
        /// </summary>
        /// <param name="securePassword"></param>
        /// <returns></returns>
        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        
        /// <summary>
        /// Compares the two specified secure strings by converting them to 
        /// unmanaged strings. Returns true if the strings are the same.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool DoSecureStringsMatch(SecureString s1, SecureString s2)
        {
            string str1 = s1.ConvertToUnsecureString(),
                str2 = s2.ConvertToUnsecureString();

            return (string.Compare(str1, str2, false) == 0);
        }

        /// <summary>
        /// Computes a hash for the specified string and returns the hash code.
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Computes the MD5 hash for the input string and compares it to the
        /// value of the comparison hash. 
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <param name="compareHash"></param>
        /// <returns>Returns true if the input string's computed hash is the same
        /// as the comparison hash.</returns>
        public static bool CompareMD5Hash(MD5 md5Hash, string input, string compareHash)
        {
            // Hash the input.
            string hashOfInput = GetMD5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, compareHash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [DllImport("gdi32")]
        static extern bool DeleteObject(IntPtr o);

        public static BitmapSource LoadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, System.Windows.Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }

    }
}

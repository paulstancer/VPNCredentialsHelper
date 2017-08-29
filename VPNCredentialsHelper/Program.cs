using System;
using System.Runtime.InteropServices;

namespace VPNCredentialsHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            VPNHelper.SetCredentials("ExpressVPN New Jersey", null, "username", "password");
        }

        #region RASCM

        /// <summary>
        /// Defines the flags indicating which members of a <see cref="RASCREDENTIALS"/> instance are valid.
        /// </summary>
        [Flags]
        public enum RASCM
        {
            /// <summary>
            /// No options are valid.
            /// </summary>
            None = 0x0,

            /// <summary>
            /// The user name member is valid.
            /// </summary>
            UserName = 0x1,

            /// <summary>
            /// The password member is valid.
            /// </summary>
            Password = 0x2,

            /// <summary>
            /// The domain name member is valid.
            /// </summary>
            Domain = 0x4,
#if (WINXP || WIN2K8 || WIN7 || WIN8)
            /// <summary>
            /// Indicates the credentials are the default credentials for an all-user connection.
            /// </summary>
            DefaultCredentials = 0x8,

            /// <summary>
            /// Indicates a pre-shared key should be retrieved.
            /// </summary>
            PreSharedKey = 0x10,

            /// <summary>
            /// Used to set the pre-shared key on the remote access server.
            /// </summary>
            ServerPreSharedKey = 0x20,

            /// <summary>
            /// Used to set the pre-shared key for a demand dial interface.
            /// </summary>
            DdmPreSharedKey = 0x40
#endif
        }

        #endregion

        /// <summary>
        /// Defines the maximum length of a username.
        /// </summary>
        public const int UNLEN = 256;

        /// <summary>
        /// Defines the maximum length of a password.
        /// </summary>
        public const int PWLEN = 256;

        /// <summary>
        /// Defines the maximum length of a domain name.
        /// </summary>
        public const int DNLEN = 15;

        /// <summary>
        /// Describes user credentials associated with a phone book entry.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        public struct RASCREDENTIALS
        {
            public int size;
            public RASCM options;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UNLEN + 1)]
            public string userName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PWLEN + 1)]
            public string password;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DNLEN + 1)]
            public string domain;
        }

        /// <summary>
        /// Sets the user credentials for a phone book entry.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of the entry whose credentials to set.</param>
        /// <param name="lpCredentials">Pointer to an <see cref="NativeMethods.RASCREDENTIALS"/> object containing user credentials.</param>
        /// <param name="fClearCredentials"><b>true</b> clears existing credentials by setting them to an empty string, otherwise <b>false</b>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasSetCredentials(
            string lpszPhonebook,
            string lpszEntryName,
            IntPtr lpCredentials,
            [MarshalAs(UnmanagedType.Bool)] bool fClearCredentials);

        /// <summary>
        /// The operation was successful.
        /// </summary>
        public const int SUCCESS = 0;

        /// <summary>
        /// The user did not have appropriate permissions to perform the requested action.
        /// </summary>
        public const int ERROR_ACCESS_DENIED = 5;

        /// <summary>
        /// Sets the user credentials for a phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path (including filename) of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of the entry whose credentials to set.</param>
        /// <param name="credentials">An <see cref="NativeMethods.RASCREDENTIALS"/> object containing user credentials.</param>
        /// <param name="clearCredentials"><b>true</b> clears existing credentials by setting them to an empty string, otherwise <b>false</b>.</param>
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBookPath"/> or <paramref name="entryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public static bool SetCredentials(string entryName, string domain, string username, string password, bool clearCredentials)
        {
            var credentials = new RASCREDENTIALS() { userName = username, password = password, domain = domain ?? string.Empty, options = RASCM.Domain | RASCM.UserName | RASCM.Password };

            int size = Marshal.SizeOf(typeof(RASCREDENTIALS));
            bool retval = false;

            IntPtr pCredentials = IntPtr.Zero;
            try
            {
                credentials.size = size;

                pCredentials = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(credentials, pCredentials, true);

                try
                {
                    int ret = RasSetCredentials(null, entryName, pCredentials, clearCredentials);

                    if (ret == SUCCESS)
                    {
                        retval = true;
                    }
                    else if (ret == ERROR_ACCESS_DENIED)
                    {
                        retval = false;
                    }
                    else
                    {
                        retval = false;
                    }
                }
                catch (EntryPointNotFoundException ex)
                {
                    throw;
                }
            }
            finally
            {
                if (pCredentials != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pCredentials);
                }
            }

            return retval;
        }
    }


}

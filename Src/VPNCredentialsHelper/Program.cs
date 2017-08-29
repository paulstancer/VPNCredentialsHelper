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
    }
}

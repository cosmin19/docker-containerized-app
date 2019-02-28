using System;
using System.Net;
using System.Net.Sockets;

namespace PokerBuddyApp.Core
{
    public partial class Utilities
    {
        /// <summary>
        /// Verifica conexiunea la internet
        /// </summary>
        /// <returns></returns>
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica conexiunea la server
        /// </summary>
        /// <returns></returns>
        //public static bool IsServerConnected()
        //{
        //    using (var l_oConnection = new SqlConnection(DbSingleton.Instance.Database.Connection.ConnectionString))
        //    {
        //        try
        //        {
        //            l_oConnection.Open();
        //            return true;
        //        }
        //        catch (SqlException)
        //        {
        //            return false;
        //        }
        //    }
        //}

        /// <summary>
        /// Returneaza adresa IP locala
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}

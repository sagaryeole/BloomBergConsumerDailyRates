using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;


namespace BloomBergConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);

            //Set localization
            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");

            DataSet DS = new DataSet();
            DataTable DT = new DataTable();
           
            BusinessCollect BsColl = new BusinessCollect();
            try
            {
                DS = BsColl.GetCurrencyToFetch();
                DT = DS.Tables[0];

                //call using channel factory
                BsColl.GetCurrencyRates(DT);

                //call using client
                BsColl.GetCurrencyRates1(DT);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool AllwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }
    }
}

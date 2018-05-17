using System;
using System.Data;
using BloomBergConsumer.BloomB;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace BloomBergConsumer
{
    public class BusinessCollect
    {
        string certificatePath;
        string endpointurl = "https://dlws-uat.bloomberg.com/dlps";
        public BusinessCollect()
        {
            //certificatePath = "\\\\global.scd.scania.com\\app\\Infomate\\EROS\\DataLicence\\Bloomberg.cer";
            certificatePath = "C:\\Scania Projects\\Bloomberg.cer";
        }

        #region DB Operations
        public DataSet GetCurrencyToFetch()
        {
            DataSet ds = new DataSet();
            string sqlstmt = "EROS_GetCurrencyToFetch";
            DataAccess da = new DataAccess();

            try
            {
                ds = da.CallStoredProcedure(sqlstmt);
            }
            catch (Exception ex)
            {
            }
            finally
            {


            }
            return ds;
        }
        public DataSet GetCurrencyTypeToFetch()
        {
            DataSet ds = new DataSet();
            string sqlstmt = "EROS_GetCurrencyTypeToFetch";
            DataAccess da = new DataAccess();

            try
            {
                ds = da.CallStoredProcedure(sqlstmt);
            }
            catch (Exception ex)
            {
            }
            finally
            {


            }
            return ds;
        }
        public string[] CurrencyTypeToFetch()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRow dr;
            int noOfCurType;

            ds = GetCurrencyTypeToFetch();
            dt = ds.Tables[0];
            noOfCurType = dt.Rows.Count;

            string[] curTypes = new string[noOfCurType + 1];

            //Add Quote factor
            curTypes[0] = "QUOTE_FACTOR";

            for (int i = 1; i < noOfCurType + 1; i++)
            {
                dr = dt.Rows[i - 1];
                curTypes[i] = dr.ItemArray[0].ToString();
            }

            return curTypes;

            // string [] = new string[24];


        }
        #endregion
        
        public bool GetCurrencyRates(DataTable exchangeRates)
        {
            int noOfRates;
            DataRow dr;
            //var binding = new BasicHttpsBinding("PerSecurityHttpsBinding");
            var binding = new BasicHttpBinding("PerSecurityHttpBinding");
            //var binding = new WSHttpBinding("PerSecurityWSBinding2");
            var endpoint = new EndpointAddress(new Uri(endpointurl));
            //var channelFactory = new ChannelFactory<PerSecurityWSChannel>(binding, endpoint);
            var channelFactory = new ChannelFactory<PerSecurityWS>(binding, endpoint);

            channelFactory.Credentials.
                ClientCertificate.Certificate = new X509Certificate2(certificatePath);

          

            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            channelFactory.Open();

            //PerSecurityWSChannel ps = channelFactory.CreateChannel();
            PerSecurityWS ps = channelFactory.CreateChannel();


            //Setting request header information	
            GetDataHeaders getDataHeaders = new GetDataHeaders
            {
                secmaster = true,
                secmasterSpecified = true,
                closingvalues = true,
                closingvaluesSpecified = true
            };

            // Console.WriteLine("test 1 ");
            noOfRates = exchangeRates.Rows.Count;
            if (noOfRates < 1)
            {
                return true;
            }

            SubmitGetDataRequest sbmtGtDtreq = new SubmitGetDataRequest
            {
                headers = getDataHeaders,
                fields = CurrencyTypeToFetch(),

                //Old version
                instruments = new Instrument[noOfRates] // { ticker1};
            };
            //sbmtGtDtreq.instruments = new Instruments();
            //sbmtGtDtreq.instruments.instrument = new Instrument[noOfRates];

            for (int i = 0; i < noOfRates; i++)
            {
                dr = exchangeRates.Rows[i];

                //Old version
                sbmtGtDtreq.instruments[i] = new Instrument
                {
                    id = dr.ItemArray[0].ToString(),
                    yellowkey = MarketSector.Curncy,
                    yellowkeySpecified = true
                };

            } // end for 

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                submitGetDataRequestRequest sbmtGtDtreq0 = new submitGetDataRequestRequest(sbmtGtDtreq);
                SubmitGetDataResponse sbmtGtDtresp = ps.submitGetDataRequest(sbmtGtDtreq0).submitGetDataResponse;
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.Message);
                return false;
            }
            finally
            {
                ((PerSecurityWSChannel)ps).Close();
                channelFactory.Close();
            }
            return true;
        }

        public bool GetCurrencyRates1(DataTable exchangeRates)
        {
            DataRow dr;
            int noOfRates;

            PerSecurityWSClient ps = new PerSecurityWSClient("PerSecurityHttpPort");
            //PerSecurityWSClient ps = new PerSecurityWSClient("PerSecurityHttpsPort");
            // PerSecurityWSClient ps = new PerSecurityWSClient("PerSecurityWSPort");

            //X509Certificate2 clientCert = new X509Certificate2(certificatePath);

            X509Certificate2 clientCert = GetBloomBergCertificate();

            ps.ClientCredentials.ClientCertificate.Certificate = clientCert;

            


            //Setting request header information	
            GetDataHeaders getDataHeaders = new GetDataHeaders
            {
                secmaster = true,
                secmasterSpecified = true,
                closingvalues = true,
                closingvaluesSpecified = true
            };

            // Console.WriteLine("test 1 ");
            noOfRates = exchangeRates.Rows.Count;
            if (noOfRates < 1)
            {
                return true;
            }

            SubmitGetDataRequest sbmtGtDtreq = new SubmitGetDataRequest
            {
                headers = getDataHeaders,
                fields = CurrencyTypeToFetch(),

                //Old version
                instruments = new Instrument[noOfRates] // { ticker1};
            };
            //sbmtGtDtreq.instruments = new Instruments();
            //sbmtGtDtreq.instruments.instrument = new Instrument[noOfRates];

            for (int i = 0; i < noOfRates; i++)
            {
                dr = exchangeRates.Rows[i];

                //Old version
                sbmtGtDtreq.instruments[i] = new Instrument
                {
                    id = dr.ItemArray[0].ToString(),
                    yellowkey = MarketSector.Curncy,
                    yellowkeySpecified = true
                };
                //sbmtGtDtreq.instruments.instrument[i] = new Instrument();
                //sbmtGtDtreq.instruments.instrument[i].id = dr.ItemArray[0].ToString();
                //sbmtGtDtreq.instruments.instrument[i].yellowkey = MarketSector.Curncy;
                //sbmtGtDtreq.instruments.instrument[i].yellowkeySpecified = true;

            } // end for 

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                SubmitGetDataResponse sbmtGtDtresp = new SubmitGetDataResponse();

                sbmtGtDtresp = ps.submitGetDataRequest(sbmtGtDtreq);
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.Message);
                return false;
            }
            return true;

        }

        private X509Certificate2 GetBloomBergCertificate()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySerialNumber, "4C00000520E07101AD1185A2A4000000000520", false);
            store.Close();
            return certs[0];
        }
    }
}

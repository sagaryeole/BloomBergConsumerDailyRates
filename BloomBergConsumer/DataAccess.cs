using System;
using System.Data;
using System.Data.SqlClient;

namespace BloomBergConsumer
{
    internal class DataAccess
    {
        private string conString;

        public DataAccess()
        {
            conString = "SERVER=SESOCXXXXXX;DATABASE=EROSXXXX;User ID =EROSUser;Password=XXXXX";
        }

        public DataSet CallStoredProcedure(string storedproc)
        {
            DataSet DS = new DataSet();
            SqlConnection connection = new SqlConnection(conString);
            SqlCommand command = new SqlCommand();
            SqlDataAdapter DA = new SqlDataAdapter(command);

            int temp;
            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = storedproc;
                command.Connection = connection;

                connection.Open();
                temp = DA.Fill(DS);
            }
            catch (Exception ex)
            {
                // write to eventlog


            }
            finally
            {
                command.Dispose();
                connection.Close();
                connection.Dispose();
                DS.Dispose();
            }

            return DS;


        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Configuration;

namespace BatchPhotoConvert
{
    public class ItemDescriptionInfoDA
    {
        public static string connectionString
        {
            get
            {
                var str = ConfigurationManager.ConnectionStrings["IMConn"].ConnectionString;
                if (str.Contains(";"))
                {
                    return str;
                }
                else
                {
                    return RijndaelManagedEncryptor.Decrypt(str);
                }
            }
        }
        public static string providerName
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["IMConn"].ProviderName;
            }
        }

        public static DataTable GetItemDescriptionInfo(string cmdText)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(cmdText, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds.Tables[0];
            }
        }
    }
}

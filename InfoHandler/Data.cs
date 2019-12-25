using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InfoHandler
{
    public static class Data
    {
        public static string ConnectionString = "Data Source=c:\\NewsWeb.db";

        public static int ExcuteSql(string sql) {
            SqlConnection mycon = new SqlConnection(ConnectionString);
            mycon.Open();
            SqlCommand cmd = new SqlCommand(sql, mycon);
            int num = cmd.ExecuteNonQuery();
            mycon.Close();
            return num;
        }
    }
}

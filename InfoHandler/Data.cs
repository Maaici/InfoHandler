using System;
using System.Data;
using System.Data.SQLite;

namespace InfoHandler
{
    public static class Data
    {
        public static string ConnectionString = "Data Source=E:\\2_MyCodes\\InfoHandler\\InfoHandler\\NewsWeb.db";

        public static int ExcuteSql(string sql) {
            SQLiteConnection mycon = null;
            try
            {
                mycon = new SQLiteConnection(ConnectionString);
                mycon.Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, mycon);
                int num = cmd.ExecuteNonQuery();
                mycon.Close();
                return num;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excute(" + sql + ")Err:" + ex.Message);
                return -1;
            }
            finally
            {
                mycon.Close();
            }
        }

        public static DataTable SqlTable(string sql)
        {
            SQLiteConnection mycon = null ;
            try
            {
                mycon = new SQLiteConnection(ConnectionString); 
                mycon.Open();
                SQLiteCommand sqlcmd = new SQLiteCommand(sql, mycon);//sql语句
                sqlcmd.CommandTimeout = 120;
                SQLiteDataReader reader = sqlcmd.ExecuteReader();
                DataTable dt = new DataTable();
                if (reader != null)
                {
                    dt.Load(reader, LoadOption.PreserveChanges, null);
                }
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SqlReader(" + sql + ")Err:" + ex.Message );
                return null;
            }
            finally
            {
                mycon.Close();
            }
        }
    }
}

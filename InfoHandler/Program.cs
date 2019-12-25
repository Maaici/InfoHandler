using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InfoHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            //GetGaoxiao();
            GetZhuanye();
            Console.ReadLine();
        }

        private static void GetGaoxiao()
        {
            Console.WriteLine("开始读取高校数据。。。。。。");
            //一共 2834 条数据   每页 20 条  要 142 页
            for (int i = 1; i <= 142; i++)
            {
                string url = "https://api.eol.cn/gkcx/api/?page=" + i + "&request_type=1&size=20&sort=view_total&uri=apigkcx/api/school/hotlists";
                string res = HttpHelper.HttpGet(url);
                var data = JsonConvert.DeserializeObject<dynamic>(res).data.item;

                foreach (var item in data)
                {
                    var school_id = item.school_id.ToString();
                    var name = item.name.ToString();
                    var city_name = item.city_name.ToString();
                    var province_name = item.province_name.ToString();
                    var address = item.address.ToString();
                    var type_name = item.type_name.ToString();
                    var nature_name = item.nature_name.ToString();
                    string sql = $@" insert into dt_universities ( school_id, name ,city_name,province_name,address,type_name,nature_name) 
                            values ({school_id},'{name}','{city_name}','{province_name}','{address}','{type_name}','{nature_name}') ";
                    Data.ExcuteSql(sql);
                }
                Console.WriteLine($"第 {i} 页处理完成 -》 -》 -》");
            }
            Console.WriteLine("++++++++++高校数据读取完成+++++++++++++");
            Console.WriteLine();
        }

        private static void GetZhuanye()
        {
            Console.WriteLine("开始读取高校专业数据。。。。。。");
            DataTable data = null;
            do
            {
                //sqlite语法不太一样
                data = Data.SqlTable(" select school_id from dt_universities where ifnull(state,0) = 0 limit 10 ");
                //data = Data.SqlTable(" select top 10 school_id from dt_universities where isnull(state,0) = 0 ");
                foreach (DataRow dr in data.Rows) {
                    var json = HttpHelper.HttpGet($"https://static-data.eol.cn/www/school/{dr["school_id"].ToString()}/pc_special.json");
                    var ret = JsonConvert.DeserializeObject<dynamic>(json).special_detail;
                    Type dtype = ret.GetType();
                    foreach (var item in dtype.GetProperties()) {
                        if (item.GetValue(ret).ToString() == "")
                            continue;
                        foreach (var ii in item.GetValue(ret))
                        {
                            var special_id = ii.special_id.ToString();
                            var school_id = ii.school_id.ToString();
                            var special_name = ii.special_name.ToString();
                            var type_name = ii.type_name.ToString();
                            string sql = $@"insert into dt_zhuanye ( special_id, school_id ,special_name,type_name) 
                            values({ special_id},{school_id},'{special_name}','{type_name}' ) ";
                            Data.ExcuteSql(sql);
                        }
                    }
                }
            } while (data.Rows.Count > 0);
            
            
            Console.WriteLine("+++++++++Done++++++++++++++");
        }
    }
}

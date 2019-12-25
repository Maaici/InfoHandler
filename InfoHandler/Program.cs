using System;
using System.Collections.Generic;
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
            Console.WriteLine("开始读取数据。。。。。。");
            
            for (int i = 1; i <= 142; i++) {
                string url = "https://api.eol.cn/gkcx/api/?page="+i+"&request_type=1&size=20&sort=view_total&uri=apigkcx/api/school/hotlists";
                string res = HttpHelper.HttpGet(url);
                var data = JsonConvert.DeserializeObject<dynamic>(res).data.item;
                
                foreach (var item in data) {
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
                Console.WriteLine("处理第一页 -》 -》 -》");
            }
            Console.WriteLine("");

            Console.ReadLine();
        }
    }
}

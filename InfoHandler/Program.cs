using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

namespace InfoHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("+++++++++Begain+++++++++++");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("开始读取高校数据。。。。。。");
            ProgressBar progressBar1 = new ProgressBar(Console.CursorLeft, Console.CursorTop, 100, ProgressBarType.Multicolor);
            GetGaoxiao(progressBar1.Dispaly);

            Console.WriteLine("开始读取高校专业数据。。。。。。");
            ProgressBar progressBar2 = new ProgressBar(Console.CursorLeft, Console.CursorTop, 100, ProgressBarType.Multicolor);
            GetZhuanye(progressBar2.Dispaly);
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("++++++++++Done++++++++++++");
            Console.WriteLine("请按任意键退出。。。");
            Console.ReadKey();
        }

        private static void GetGaoxiao(Func<int, int> dispalyProgress = null)
        {
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
                    string sql = $@" insert into dt_gaoxiao ( school_id, name ,city_name,province_name,address,type_name,nature_name,state) 
                            values ({school_id},'{name}','{city_name}','{province_name}','{address}','{type_name}','{nature_name}',0) ";
                    Data.ExcuteSql(sql);
                } 
                double ss = ( i*1.0 / 142) * 100;
                dispalyProgress?.Invoke(Convert.ToInt32(ss));
            }

            Console.WriteLine();
            Console.WriteLine("++++++++++高校数据读取完成+++++++++++++");
            Console.WriteLine();
        }

        private static void GetZhuanye(Func<int, int> dispalyProgress = null)
        {
            DataTable data = null;
            int total = 0;
            do
            {
                List<string> school_ids = new List<string>();
                //sqlite语法不太一样
                data = Data.SqlTable(" select school_id from dt_gaoxiao where ifnull(state,0) = 0 limit 10 ");
                //data = Data.SqlTable(" select top 10 school_id from dt_universities where isnull(state,0) = 0 ");
                foreach (DataRow dr in data.Rows) {
                    school_ids.Add(dr["school_id"].ToString());
                    var json = HttpHelper.HttpGet($"https://static-data.eol.cn/www/school/{dr["school_id"].ToString()}/pc_special.json");
                    json = json.Replace("\"1\":", "\"ben\":").Replace("\"2\":", "\"zhuan\":").Replace("\"3\":", "\"qita\":");
                    var ret = JsonConvert.DeserializeObject<dynamic>(json).special_detail;
                    List<dynamic> datas = new List<dynamic>();
                    datas.Add(ret.ben);
                    datas.Add(ret.zhuan);
                    //datas.Add(ret.ben);
                    foreach (var item in datas) {
                        if (item == null || item.ToString() == "")
                            continue;
                        foreach (var ii in item)
                        {
                            var special_id = ii.special_id.ToString();
                            var school_id = ii.school_id.ToString();
                            var special_name = ii.special_name.ToString();
                            var type_name = ii.type_name.ToString();
                            string sql = $@" insert into dt_gaoxiaozhuanye ( special_id, school_id ,special_name,type_name) 
                            values({ special_id},{school_id},'{special_name}','{type_name}' ) ";
                            Data.ExcuteSql(sql);
                        }
                    }
                    total++;
                }
                //设置完成标记
                Data.ExcuteSql($" update dt_gaoxiao set state = 1 where school_id in ( '{string.Join("','", school_ids)}' ) ");
                dispalyProgress?.Invoke(Convert.ToInt32((total*1.0 / 2834) * 100));
                Console.WriteLine();
                Console.WriteLine("++++++++++专业数据读取完成+++++++++++++");
            } while (data.Rows.Count == 10);
        }

        }
}

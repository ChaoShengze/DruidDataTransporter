using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DruidDataTransporter.Req
{
    public class BaseDruidQuery
    {
        /**
        * 用以查询的SQL语句
        */
        public string query = null;
        /**
         * 返回类型，注意多条数据返回必须指定 “object” 以确保返回对象最外层为 Object 而非 Array；单条数据查询必须指定 “objectLines”
         */
        public string resultFormat = ResultFormat.OBJECT_LINES;
        /**
         * 参数数组
         */
        public string[] parameters = null;
        /**
         * 时区设置对象
         */
        public DruidTimeZoneConfig context = new DruidTimeZoneConfig();

        public static class ResultFormat
        {
            public static string OBJECT_LINES = "objectLines";
            public static string OBJECT = "object";
        }
    }
}

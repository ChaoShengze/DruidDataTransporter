using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DruidDataTransporter
{
    public class Configuration
    {
        /// <summary>
        /// 当前Block数，用于记录当前已获取数据的位置；每次下载完毕一个Block时会自动进行维护
        /// </summary>
        public int CurrBlock = 0;
        /// <summary>
        /// 每个Block储存的数据的行数
        /// </summary>
        public int BlockSize { get; set; } = 10000;
        /// <summary>
        /// 传输文件的存储路径
        /// </summary>
        public string SavePath { get; set; } = string.Empty;
        /// <summary>
        /// 访问的Url（包括类似 /druid/v2/ 和 http:// 的部分）
        /// </summary>
        public string DruidUrl { get; set; } = @"http://localhost:8082/druid/v2/";
        /// <summary>
        /// Druid的访问用户名，如未设置请留空
        /// </summary>
        public string DruidUsr { get; set; } = string.Empty;
        /// <summary>
        /// Druid的访问密码，如未设置请留空
        /// </summary>
        public string DruidPwd {  get; set; } = string.Empty;
        /// <summary>
        /// Druid的查询SQL语句，不包括LIMIT限制的部分
        /// </summary>
        public string DruidQuery { get; set; } = string.Empty;
        /// <summary>
        /// Druid的总数据量的查询语句
        /// </summary>
        public string DruidCountQuery { get; set; } = string.Empty;
        /// <summary>
        /// 查询的超时设置，数据量较大时需要设置较大值保证查询能够完成，单位：分钟
        /// </summary>
        public int QueryTimeout { get; set; } = 10;
    }
}

using DruidDataTransporter.Req;
using DruidDataTransporter.Rsp;
using LogLib;
using Newtonsoft.Json;
using RestSharp;
using System.Text;
using System.Text.Json.Nodes;

namespace DruidDataTransporter
{
    internal class Program
    {
        private static Logger Log = Logger.GetLogger("DDT");

        #region 配置相关 / Configuration
        private const string ConfigPath = "Configuration.json";
        private static Configuration? _Config = null;
        public static Configuration Config
        {
            get
            {
                try
                {
                    if (_Config == null)
                    {
                        if (!File.Exists(ConfigPath))
                        {
                            _Config = new Configuration();
                            File.WriteAllText(ConfigPath,
                                JsonConvert.SerializeObject(_Config, Formatting.Indented));
                        }
                        else
                        {
                            _Config = JsonConvert.DeserializeObject<Configuration>
                                (File.ReadAllText(ConfigPath)) ?? new Configuration();
                        }
                    }

                    return _Config;
                }
                catch (Exception ex)
                {
                    Log.Error("Config Getter", ex.ToString());
                    return new Configuration();
                }
            }

            set
            {
                _Config = value;
            }
        }
        #endregion

        public static void Main(string[] args)
        {
            try
            {
                var blockNum = GetBlockNum();
                var currNum = Config.CurrBlock;
                while (currNum < blockNum)
                {
                    try
                    {
                        //下载Block
                        Log.Info("Main While", $"开始下载第[{currNum + 1}/{blockNum}]个文件。");

                        if (DownloadBlock(currNum))
                        {
                            Log.Info("Main While", $"下载第[{currNum + 1}/{blockNum}]个文件完成。");
                            //完成任务，更新Block计数
                            Config.CurrBlock = ++currNum;
                            UpdateConfigFile();
                        }
                        else
                        {
                            Log.Info("Main While", $"下载第[{currNum + 1}/{blockNum}]个文件失败，重试。");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Main While", ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Main", ex.ToString());
            }
        }

        /// <summary>
        /// 创建一个Http请求并读取响应内容
        /// </summary>
        public static string Post(BaseDruidQuery postData)
        {
            using (RestClient client = new(new RestClientOptions()
            {
                BaseUrl = new Uri(Config.DruidUrl),
                MaxTimeout = Config.QueryTimeout * 60 * 1000
            }))
            {
                var json = JsonConvert.SerializeObject(postData);
                var request = new RestRequest()
                {
                    Method = Method.Post
                };

                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                request.Timeout = Config.QueryTimeout * 60 * 1000;
                request.AddJsonBody(json);

                var rsp = client.Execute(request);
                if (rsp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return rsp.Content ?? string.Empty;
                }
                else
                {
                    Log.Error("Post Rsp", $"{rsp.StatusCode}——{rsp.Content}");
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 更新配置文件至硬盘
        /// </summary>
        private static void UpdateConfigFile()
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Config, Formatting.Indented));
        }

        /// <summary>
        /// 根据当前的Block数获取下载下一个Block的请求对象
        /// </summary>
        /// <param name="currBlockNum">当前的Block数</param>
        /// <returns></returns>
        private static DownloadReq GetQueryString(int currBlockNum)
        {
            var query = $"{Config.DruidQuery} LIMIT {Config.BlockSize} OFFSET {Config.BlockSize * currBlockNum}";
            var req = new DownloadReq()
            {
                query = query
            };
            return req;
        }

        /// <summary>
        /// 下载下一个Block
        /// </summary>
        /// <param name="currBlockNum">当前的Block数</param>
        /// <returns></returns>
        private static bool DownloadBlock(int currBlockNum)
        {
            try
            {
                var req = GetQueryString(currBlockNum);
                var fileName = $"{Config.SavePath}.{currBlockNum:000000}";
                var rspContent = Post(req);
                if (!string.IsNullOrEmpty(rspContent))
                {
                    var folder = Path.GetDirectoryName(fileName);
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    File.WriteAllText(fileName, rspContent, Encoding.UTF8);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("DownloadBlock", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 计算文件数
        /// </summary>
        /// <returns></returns>
        private static int GetBlockNum()
        {
            var countRsp = JsonConvert.DeserializeObject<CountRsp>(Post(new BaseDruidQuery()
            {
                query = Config.DruidCountQuery,
                resultFormat = BaseDruidQuery.ResultFormat.OBJECT_LINES
            })) ?? new CountRsp();
            var total = countRsp.Count;
            int blockNum = (int)Math.Ceiling(total/(Config.BlockSize * 1.0));

            Log.Info("Main", $"数据条目共计[{total}]条，按照配置的[{Config.BlockSize}/个文件]需要创建[{blockNum}]个文件。");

            return blockNum;
        }
    }
}
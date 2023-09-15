using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DruidDataTransporter.Req
{
    public class DownloadReq : BaseDruidQuery
    {
        public DownloadReq() 
        {
            resultFormat = ResultFormat.OBJECT;
        }
    }
}

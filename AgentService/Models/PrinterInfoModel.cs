using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentService.Models
{
    public class PrinterInfoModel
    {
        public string Ip { get; set; }
        public string Community { get; set; }

        public List<OidModel> OidList { get; set; } = new List<OidModel>();
    }
}

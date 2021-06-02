using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentService.Models
{
    public class AgentInfo
    {
        public string AgentName { get; set; }

        public List<OidModel> Oid { get; set; } = new List<OidModel>();
        //public string Response{ get; set; }
    }
}

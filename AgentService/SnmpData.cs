using System;
using System.Collections.Generic;
using System.Linq;
using SnmpSharpNet;
using System.Text;
using System.Threading.Tasks;
using AgentService.Models;

namespace AgentService
{
    public class SnmpData 
    {
        //public string snmpGet(string ipAddress, string snmpCommunity, List<string> OidList)
        public PrinterInfoModel snmpGet(PrinterInfoModel printerInfo)
        {

            foreach (var oid in printerInfo.OidList)
            {
                try
                {
                    Dictionary<Oid, AsnType> response = SNMPGetRequest(printerInfo.Ip, printerInfo.Community, oid.OidNumber);
                    oid.Response = response.ElementAt(0).Value.ToString();
                }
                catch (Exception)
                {
                    oid.Response = "Error";
                } 
            }

            return printerInfo;
        }
        private Dictionary<Oid, AsnType> SNMPGetRequest(string ipAddress, string snmpCommunity, string oid)
        {
            string[] oidArray = new string[]
            {
                oid
            };

            SimpleSnmp snmp = new SimpleSnmp(ipAddress, snmpCommunity);
            Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver1, oidArray);

            return result;
        }
    }
}

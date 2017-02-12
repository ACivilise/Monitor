using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    class Tools
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Tools));

        public static string Traceroute(string ipAddressOrHostName)
        {
            try
            {
                IPAddress ipAddress = Dns.GetHostEntry(ipAddressOrHostName).AddressList[0];
                StringBuilder traceResults = new StringBuilder();
                using (Ping pingSender = new Ping())
                {
                    PingOptions pingOptions = new PingOptions();
                    Stopwatch stopWatch = new Stopwatch();
                    byte[] bytes = new byte[32];
                    pingOptions.DontFragment = true;
                    pingOptions.Ttl = 1;
                    int maxHops = 30;
                    traceResults.AppendLine(
                        string.Format(
                            "Tracing route to {0} over a maximum of {1} hops:",
                            ipAddress,
                            maxHops));
                    traceResults.AppendLine();
                    for (int i = 1; i < maxHops + 1; i++)
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        PingReply pingReply = pingSender.Send(
                            ipAddress,
                            5000,
                            new byte[32], pingOptions);
                        stopWatch.Stop();
                        IPHostEntry entry = Dns.GetHostEntry(pingReply.Address);
                        traceResults.AppendLine(
                            string.Format("{0}\t{1} ms\t{2} Url : {3}",
                            i,
                            stopWatch.ElapsedMilliseconds,
                            pingReply.Address, entry.HostName.ToString()));
                        if (pingReply.Status == IPStatus.Success)

                        {
                            traceResults.AppendLine();
                            traceResults.AppendLine("Trace complete."); break;
                        }
                        pingOptions.Ttl++;
                    }
                }
                return " IP : " + ipAddressOrHostName +" Result : " + traceResults.ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return " IP : " + ipAddressOrHostName + " Result : " +  ex.Message;
            }
        }


        public static string GetUserCountryByIp(string ip)
        {
            var ipInfo = new IpInfo();
            try
            {
                if (ip.Contains(':'))
                {
                    ip = ip.Split(':')[0];
                }
                using (var webClient = new WebClient())
                {
                    var info = webClient.DownloadString("http://ipinfo.io/" + ip);
                    ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                    var myRI1 = new RegionInfo(ipInfo.Country);
                    ipInfo.Country = myRI1.EnglishName;
                }
            }
            catch (Exception e)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }
    }
}

using log4net;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Principal;
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


        public static IpInfo GetUserCountryByIp(string ip)
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

            return ipInfo;
        }

        public static IpInfo GetCytiCountryByIpFromDBB(string ip)
        {
            var ipInfo = new IpInfo();
            try
            {
                if (ip.Contains(':'))
                {
                    ip = ip.Split(':')[0];
                }
                if (File.Exists(@"GeoLite2-City.mmdb"))
                {
                    // This creates the DatabaseReader object, which should be reused across
                    // lookups.
                    using (var reader = new DatabaseReader(@"GeoLite2-City.mmdb"))
                    {
                        // Replace "City" with the appropriate method for your database, e.g.,
                        // "Country".
                        var city = reader.City(ip);
                        if (city != null)
                        {
                            ipInfo.Country = city.Country.Name;
                            ipInfo.City = city.City.Name;
                            ipInfo.Postal = city.Postal.Code;
                            ipInfo.Ip = ip;
                            ipInfo.Region = city.Continent.Name;
                            ipInfo.Loc = city.Location.Latitude.ToString().Replace(',', '.') + ',' + city.Location.Longitude.ToString().Replace(',', '.');
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return ipInfo;
        }


        

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }    
    }
}

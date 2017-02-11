using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    public class Entry : INotifyPropertyChanged
    {

        private IPAddress m_SourceAddress;
        public IPAddress SourceAddress
        {
            get { return m_SourceAddress; }
            set
            {
                m_SourceAddress = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("SourceAddress"));
            }
        }

        private IPAddress m_DestinationAddress;
        public IPAddress DestinationAddress
        {
            get { return m_DestinationAddress; }
            set
            {
                m_DestinationAddress = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("DestinationAddress"));
            }
        }

        private string m_ContrySource;
        public string ContrySource
        {
            get { return m_ContrySource; }
            set
            {
                m_ContrySource = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("ContrySource"));
            }
        }

        private string m_ContryDestination;
        public string ContryDestination
        {
            get { return m_ContryDestination; }
            set
            {
                m_ContryDestination = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("ContryDestination"));
            }
        }

        private int m_NbofPackets;
        public int NbofPackets
        {
            get { return m_NbofPackets; }
            set
            {
                m_NbofPackets = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("ContryDestination"));
            }
        }

        public Entry(Packet pc)
        {
            m_SourceAddress = pc.SourceAddress;
            m_DestinationAddress = pc.DestinationAddress;
            m_NbofPackets = 1;
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

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

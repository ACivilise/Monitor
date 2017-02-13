using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.ViewModel
{
    public class EntriesListViewModel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EntriesListViewModel));

        public Sniffer Sniffer;

        public event SearchLocationEventHandler SearchLocationEvent;
        public delegate void SearchLocationEventHandler(IpInfo ipInfo);

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public EntriesListViewModel()
        {
            _ListOfEntries = new ObservableCollection<Entry>();
        }

        private ObservableCollection<Entry> _ListOfEntries;
        public ObservableCollection<Entry> ListOfEntries
        {
            get { return _ListOfEntries; }
            set
            {
                _ListOfEntries = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("ListOfEntries"));
            }
        }
        
        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public IpInfo Localize(Entry selectedEntry)
        {
            IpInfo ipInfo = null;
                 
            if (Sniffer.Local_IP.Equals(selectedEntry.SourceAddress))
            {
                ipInfo = Tools.GetUserCountryByIp(selectedEntry.DestinationAddress.ToString());
            }
            else if (Sniffer.Local_IP.Equals(selectedEntry.DestinationAddress))
            {
                ipInfo = Tools.GetUserCountryByIp(selectedEntry.SourceAddress.ToString());
            }

            SearchLocationEvent(ipInfo);

            return ipInfo;

        }

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

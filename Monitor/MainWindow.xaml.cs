using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using log4net;
using log4net.Config;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Collections.Specialized;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Packet> _ListOfEntries;
        public ObservableCollection<Packet> ListOfEntries
        {
            get { return _ListOfEntries; }
            set
            {
                _ListOfEntries = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("ListOfEntries"));
            }
        }

        // On définit une variable logger static qui référence l'instance du logger nommé Program
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private delegate void UpdatePacketList(Packet p);
        int m_PacketsSize;

        public MainWindow()
        {
            try
            {                
                InitializeComponent();
            }
            catch(Exception e)
            {
                log.Error(e);
            }
         
        }

        private void _ListOfEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {            
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { this.EntriesList.ItemsSource = _ListOfEntries; }));
                
            }
        }

        private void OnNewPacket(Sniffer pm, Packet p)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { _ListOfEntries.Add(p); }));

                
                m_PacketsSize += p.TotalLength;
            }
            catch (Exception e)
            {
                throw e;
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ListOfEntries = new ObservableCollection<Packet>();
                _ListOfEntries.CollectionChanged += _ListOfEntries_CollectionChanged;
                var theMonitor = new Sniffer();
                theMonitor.NewPacket += OnNewPacket;
            }
            catch (Exception ex)
            {
                log.Error(e);
            }
        }
    }

    public class IpToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                if(value is IPAddress)
                {
                    return value.ToString();
                }
                return value.ToString();
            }

            return null;

        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

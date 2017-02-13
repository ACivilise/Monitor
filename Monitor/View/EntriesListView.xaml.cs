using log4net;
using Microsoft.Practices.ServiceLocation;
using Monitor.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace Monitor.View
{
    /// <summary>
    /// Interaction logic for EntriesListView.xaml
    /// </summary>
    public partial class EntriesListView : UserControl
    {        
        private static readonly ILog log = LogManager.GetLogger(typeof(EntriesListView));

        private EntriesListViewModel Model;

        public EntriesListView()
        {
            InitializeComponent();

            Model = ServiceLocator.Current.GetInstance<EntriesListViewModel>();
        }
        
        private void _ListOfEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { this.EntriesList.ItemsSource = Model.ListOfEntries; }));

            }
        }

        private void OnNewPacket(Sniffer pm, Entry newEntry)
        {
            try
            {

                if (Model.ListOfEntries.Any(x => x.SourceAddress.ToString() == newEntry.SourceAddress.ToString() && x.DestinationAddress.ToString() == newEntry.DestinationAddress.ToString()))
                {
                    var entry = Model.ListOfEntries.First(x => x.SourceAddress.ToString() == newEntry.SourceAddress.ToString() && x.DestinationAddress.ToString() == newEntry.DestinationAddress.ToString());
                    entry.NbofPackets++;
                    entry.TotalExchanged = entry.TotalExchanged + (newEntry.LastPacketLenght / 1024f) / 1024f; ;
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        try
                        {
                            this.EntriesList.ItemsSource = Model.ListOfEntries;
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                    }));

                }
                else
                {

                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        try
                        {
                            newEntry.TotalExchanged = (newEntry.LastPacketLenght / 1024f) / 1024f;
                            Model.ListOfEntries.Add(newEntry);
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                    }));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.ListOfEntries = new ObservableCollection<Entry>();
                Model.ListOfEntries.CollectionChanged += _ListOfEntries_CollectionChanged;
                Model.Sniffer = new Sniffer();
                Model.Sniffer.NewPacket += OnNewPacket;
            }
            catch (Exception ex)
            {
                log.Error(e);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                if ((sender as ListBoxItem).Content is Entry)
                {
                    Entry entry = ((ListBoxItem)sender).Content as Entry;
                    string result = Tools.Traceroute(entry.SourceAddress.ToString());
                    string result2 = Tools.Traceroute(entry.DestinationAddress.ToString());
                    MessageBox.Show("Trace root result : " + result + " Trace root result2 : " + result2);

                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null && EntriesList.SelectedItem != null && EntriesList.SelectedItem is Entry)
            {
                Entry selectedEntry = EntriesList.SelectedItem as Entry;
                if("Host Source" == mi.Header.ToString())
                {
                    Clipboard.SetText(selectedEntry.SourceURL);
                }
                else if ("IP Source" == mi.Header.ToString())
                {
                    Clipboard.SetText(selectedEntry.SourceAddress.ToString());
                }
                else if ("Host Destination" == mi.Header.ToString())
                {
                    Clipboard.SetText(selectedEntry.DestinationURL);
                }
                else if ("IP Destination" == mi.Header.ToString())
                {
                    Clipboard.SetText(selectedEntry.DestinationAddress.ToString());
                }
            }  
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (EntriesList.SelectedItem != null && EntriesList.SelectedItem is Entry)
            {
                Entry selectedEntry = EntriesList.SelectedItem as Entry;
                IpInfo ipInfo = Model.Localize(selectedEntry);

                MessageBox.Show("Country : " + ipInfo.Country + "\tCity : " + ipInfo.City);

            }
        }
    }
}

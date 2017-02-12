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

        // On définit une variable logger static qui référence l'instance du logger nommé Program
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

        private void OnNewPacket(Sniffer pm, Packet p)
        {
            try
            {

                if (Model.ListOfEntries.Any(x => x.SourceAddress.ToString() == p.SourceAddress.ToString() && x.DestinationAddress.ToString() == p.DestinationAddress.ToString()))
                {
                    var entry = Model.ListOfEntries.First(x => x.SourceAddress.ToString() == p.SourceAddress.ToString() && x.DestinationAddress.ToString() == p.DestinationAddress.ToString());
                    entry.NbofPackets++;
                    Application.Current.Dispatcher.Invoke(new Action(() => { this.EntriesList.ItemsSource = Model.ListOfEntries; }));

                }
                else
                {
                    var newEntry = new Entry(p);
                    Application.Current.Dispatcher.Invoke(new Action(() => { Model.ListOfEntries.Add(newEntry); }));
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
                var theMonitor = new Sniffer();
                theMonitor.NewPacket += OnNewPacket;
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
                    Console.WriteLine(result);
                }
            }
        }

    }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using log4net;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Collections.Specialized;
using GalaSoft.MvvmLight.Command;
using Monitor.ViewModel;
using Microsoft.Practices.ServiceLocation;
using log4net.Config;
using System.Text;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));


        private MainViewModel Model;

        public MainWindow()
        {
            try
            {
                if (Tools.IsAdministrator())
                {
                    // BasicConfigurator replaced with XmlConfigurator.
                    XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));

                    // Set up a simple configuration that logs on the console.
                    BasicConfigurator.Configure();

                    InitializeComponent();

                    TheEntriesListView.Model.SearchLocationEvent += OnLocalise;

                    Closing += (s, e) => ViewModelLocator.Cleanup();
                }
                else
                {
                    MessageBox.Show("You have to have administrator privilege to run this app...");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        private void OnLocalise(IpInfo ipInfo)
        {
            try
            {
                StringBuilder query = new StringBuilder("https://www.google.com/maps?ll=");
                query.Append(ipInfo.Loc);

            
                //StringBuilder query = new StringBuilder("http://maps.google.com/?q=");
                //query.Append(ipInfo.City).Append("+");
                //query.Append(ipInfo.Country).Append("+");
                //query.Append(ipInfo.Region).Append("+");
                //query.Append(ipInfo.Postal).Append("+");
                //query.Append(ipInfo.Loc);

                TheWebBrowser.Adress.Text = query.ToString();
                TheWebBrowser.UpdateAdress();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }    
}

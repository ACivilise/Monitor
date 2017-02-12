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
                // BasicConfigurator replaced with XmlConfigurator.
                XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));

                // Set up a simple configuration that logs on the console.
                BasicConfigurator.Configure();

                InitializeComponent();
                Closing += (s, e) => ViewModelLocator.Cleanup();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }
    }    
}

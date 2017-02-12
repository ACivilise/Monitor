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

namespace Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // On définit une variable logger static qui référence l'instance du logger nommé Program
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        private MainViewModel Model;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Closing += (s, e) => ViewModelLocator.Cleanup();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            browser.Address = Adress.Text;
        }

    }    
}

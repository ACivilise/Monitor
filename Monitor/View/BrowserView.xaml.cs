using log4net;
using Microsoft.Practices.ServiceLocation;
using Monitor.ViewModel;
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

namespace Monitor.View
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class BrowserView : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BrowserView));

        private BrowserViewModel Model;

        public BrowserView()
        {
            InitializeComponent();
            Model = ServiceLocator.Current.GetInstance<BrowserViewModel>();
            Model.UpdateAdress += UpdateAdress;
        }

        private void UpdateAdress()
        {
            try
            {
                browser.Load(Adress.Text);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateAdress();
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}

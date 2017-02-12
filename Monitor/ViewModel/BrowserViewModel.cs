using CefSharp.Wpf;
using GalaSoft.MvvmLight.Command;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Monitor.ViewModel
{
    public class BrowserViewModel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BrowserViewModel));
        
        public event ChangeAdressEventHandler UpdateAdress;

        public delegate void ChangeAdressEventHandler();


        public BrowserViewModel()
        {
            CreateChangeAdressCommand();
        }    

        public ICommand ChangeAdressCommand
        {
            get;
            internal set;
        }

        private static bool CanExecuteChangeAdressCommand()
        {
            return true;
        }

        private void CreateChangeAdressCommand()
        {
            ChangeAdressCommand = new RelayCommand(ChangeAdressExecute, CanExecuteChangeAdressCommand);
        }

        public void ChangeAdressExecute()
        {
            UpdateAdress();
        }
    }
}

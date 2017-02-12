using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Monitor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
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

        public static void ChangeAdressExecute()
        {
            MessageBox.Show("The New command was invoked");
        }
    }
}
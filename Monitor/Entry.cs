using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    class Entry : INotifyPropertyChanged
    {




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
    }
}

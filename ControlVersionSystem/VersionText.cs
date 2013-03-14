using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WpfApplication1
{
    public class VersionText : INotifyPropertyChanged
    {
        string h_VersName;
        string h_VersText;
        public string VersName
        {
            get { return h_VersName; }
            set { h_VersName = value; OnPropertyChanged("VersName"); }
        }
        public string VersText
        {
            get { return h_VersText; }
            set { h_VersText = value; OnPropertyChanged("VersText"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string s)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(s));
            }
        }
    }
}

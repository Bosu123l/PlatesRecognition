using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PlatesRecognition
{
    public class OnPropertyChangeImp : INotifyPropertyChanged
    {


        public void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            var tempHandler = PropertyChanged;
            if (tempHandler != null)
            {
                tempHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }
}

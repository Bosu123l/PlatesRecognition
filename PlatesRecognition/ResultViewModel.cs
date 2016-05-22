using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatesRecognition
{
    public class ResultViewModel : OnPropertyChangeImp
    {

        public string PlateCharacters
        {
            get
            {
                return _plateCharacters;
            }
            set
            {
                if (_plateCharacters != value)
                {
                    OnPropertyChanged(nameof(PlateCharacters));
                    _plateCharacters = value;

                }
            }
        }

        public TimeSpan TimeSpan
        {
            get
            {
                return _timeSpan;
            }
            set
            {
                if (_timeSpan != value)
                {
                    OnPropertyChanged(nameof(TimeSpan));
                    _timeSpan = value;

                }
            }
        }

        private string _plateCharacters;
        private TimeSpan _timeSpan;

        public ResultViewModel(string plateCharacters, TimeSpan timespan)
        {
            if (string.IsNullOrEmpty(plateCharacters)) throw new ArgumentNullException(nameof(plateCharacters));
            if (timespan == null) throw new ArgumentNullException(nameof(timespan));


            PlateCharacters = plateCharacters;
            TimeSpan = timespan;

        }
    }
}

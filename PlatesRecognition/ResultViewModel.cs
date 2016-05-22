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

        public float Confidence
        {
            get
            {
                return _confidence;
            }
            set
            {
                if (_confidence != value)
                {
                    OnPropertyChanged(nameof(Confidence));
                    _confidence = value;
                }
            }
        }
        private string _plateCharacters;
        private TimeSpan _timeSpan;
        private float _confidence;

        public ResultViewModel(string plateCharacters, TimeSpan timespan, float confidence)
        {
            if (string.IsNullOrEmpty(plateCharacters)) throw new ArgumentNullException(nameof(plateCharacters));
            if (timespan == null) throw new ArgumentNullException(nameof(timespan));


            PlateCharacters = plateCharacters;
            TimeSpan = timespan;
            Confidence = confidence;
        }
    }
}

using System.ComponentModel;

namespace SpotbaseSharp.Model
{
    public class SelectableSpot : Spot, INotifyPropertyChanged
    {
        private bool _selected;

        public SelectableSpot(Spot baseSpot)
        {
            City = baseSpot.City;
            Description = baseSpot.Description;
            CreatedAt = baseSpot.CreatedAt;
            Lng = baseSpot.Lng;
            Lat = baseSpot.Lat;
            Type = baseSpot.Type;
            SmallFile = baseSpot.SmallFile;
            LargeFile = baseSpot.LargeFile;
            Name = baseSpot.Name;
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                RaisePropertyChanged("Selected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
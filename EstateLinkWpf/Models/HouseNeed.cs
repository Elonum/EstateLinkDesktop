using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace EstateLinkWpf.Models
{
    public class HouseNeed : INotifyPropertyChanged
    {
        private int _minRoomCount;
        private int _maxRoomCount;
        private int _minFloor;
        private int _maxFloor;
        private Need _need;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Key]
        [ForeignKey("Need")]
        public int NeedID { get; set; }

        public int MinRoomCount
        {
            get => _minRoomCount;
            set
            {
                if (_minRoomCount != value)
                {
                    _minRoomCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxRoomCount
        {
            get => _maxRoomCount;
            set
            {
                if (_maxRoomCount != value)
                {
                    _maxRoomCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MinFloor
        {
            get => _minFloor;
            set
            {
                if (_minFloor != value)
                {
                    _minFloor = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxFloor
        {
            get => _maxFloor;
            set
            {
                if (_maxFloor != value)
                {
                    _maxFloor = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual Need Need
        {
            get => _need;
            set
            {
                if (_need != value)
                {
                    _need = value;
                    OnPropertyChanged();
                }
            }
        }
    }
} 
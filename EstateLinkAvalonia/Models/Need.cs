using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace EstateLinkAvalonia.Models
{
    public class Need : INotifyPropertyChanged
    {
        private Client? _client;
        private Realtor? _realtor;
        private Property? _property;
        private int _minPrice;
        private int _maxPrice;
        private bool _isNew;
        private double _minArea;
        private double _maxArea;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Key]
        public int NeedID { get; set; }

        public int ClientID { get; set; }
        [ForeignKey("ClientID")]
        public virtual Client? Client
        {
            get => _client;
            set
            {
                if (_client != value)
                {
                    _client = value;
                    OnPropertyChanged();
                }
            }
        }

        public int RealtorID { get; set; }
        [ForeignKey("RealtorID")]
        public virtual Realtor? Realtor
        {
            get => _realtor;
            set
            {
                if (_realtor != value)
                {
                    _realtor = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PropertyID { get; set; }
        [ForeignKey("PropertyID")]
        public virtual Property? Property
        {
            get => _property;
            set
            {
                if (_property != value)
                {
                    _property = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MinPrice
        {
            get => _minPrice;
            set
            {
                if (_minPrice != value)
                {
                    _minPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxPrice
        {
            get => _maxPrice;
            set
            {
                if (_maxPrice != value)
                {
                    _maxPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MinArea
        {
            get => _minArea;
            set
            {
                if (_minArea != value)
                {
                    _minArea = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxArea
        {
            get => _maxArea;
            set
            {
                if (_maxArea != value)
                {
                    _maxArea = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual LandNeed? LandNeed { get; set; }
        public virtual HouseNeed? HouseNeed { get; set; }
        public virtual ApartmentNeed? ApartmentNeed { get; set; }

        [NotMapped]
        public bool IsNew
        {
            get => _isNew;
            set
            {
                if (_isNew != value)
                {
                    _isNew = value;
                    OnPropertyChanged();
                }
            }
        }
    }
} 
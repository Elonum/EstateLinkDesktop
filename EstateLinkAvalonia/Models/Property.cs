using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EstateLinkAvalonia.Models
{
    [Table("Property")]
    public class Property : INotifyPropertyChanged
    {
        private PropertyType? _propertyType;
        private string? _city;
        private string? _street;
        private string? _house;
        private string? _apartment;
        private double? _latitude;
        private double? _longitude;
        private int? _floor;
        private int? _roomCount;
        private double? _area;
        private bool _isNew;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        [Key]
        [Column("PropertyID")]
        public int PropertyID { get; set; }

        [Column("City")]
        public string? City 
        { 
            get => _city;
            set
            {
                if (_city != value)
                {
                    _city = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("Street")]
        public string? Street 
        { 
            get => _street;
            set
            {
                if (_street != value)
                {
                    _street = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("House")]
        public string? House 
        { 
            get => _house;
            set
            {
                if (_house != value)
                {
                    _house = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("Apartment")]
        public string? Apartment 
        { 
            get => _apartment;
            set
            {
                if (_apartment != value)
                {
                    _apartment = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("Latitude")]
        public double? Latitude 
        { 
            get => _latitude;
            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("Longitude")]
        public double? Longitude 
        { 
            get => _longitude;
            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("Floor")]
        public int? Floor 
        { 
            get => _floor;
            set
            {
                if (_floor != value)
                {
                    _floor = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("RoomCount")]
        public int? RoomCount 
        { 
            get => _roomCount;
            set
            {
                if (_roomCount != value)
                {
                    _roomCount = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("Area")]
        public double? Area 
        { 
            get => _area;
            set
            {
                if (_area != value)
                {
                    _area = value;
                    OnPropertyChanged();
                }
            }
        }

        [Column("PropertyTypeID")]
        public int PropertyTypeID { get; set; }

        [ForeignKey("PropertyTypeID")]
        public PropertyType? PropertyType 
        { 
            get => _propertyType;
            set
            {
                if (_propertyType != value)
                {
                    _propertyType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsApartment));
                    OnPropertyChanged(nameof(IsHouse));
                    OnPropertyChanged(nameof(IsLand));
                    OnPropertyChanged(nameof(ShowFloor));
                    OnPropertyChanged(nameof(ShowRoomCount));
                    OnPropertyChanged(nameof(ShowApartment));
                }
            }
        }

        [NotMapped]
        public bool IsApartment => PropertyType?.TypeName == "Квартира";

        [NotMapped]
        public bool IsHouse => PropertyType?.TypeName == "Дом";

        [NotMapped]
        public bool IsLand => PropertyType?.TypeName == "Земля";

        [NotMapped]
        public bool ShowFloor => IsApartment;

        [NotMapped]
        public bool ShowRoomCount => IsApartment || IsHouse;

        [NotMapped]
        public bool ShowApartment => IsApartment;

        public override string ToString()
        {
            var address = new[] { City, Street, House, Apartment }
                .Where(x => !string.IsNullOrWhiteSpace(x));
            return string.Join(", ", address);
        }
    }

    [Table("PropertyType")]
    public class PropertyType
    {
        [Key]
        [Column("PropertyTypeID")]
        public int PropertyTypeID { get; set; }

        [Required]
        [Column("TypeName")]
        public string TypeName { get; set; } = string.Empty;

        public ICollection<Property> Properties { get; set; } = new List<Property>();

        public override string ToString()
        {
            return TypeName;
        }
    }
}

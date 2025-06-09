using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace EstateLinkWpf.Models
{
    [Table("Offer")]
    public class Offer : INotifyPropertyChanged
    {
        private Client _client;
        private Realtor _realtor;
        private Property _property;
        private int _price;
        private bool _isNew;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Key]
        [Column("OfferID")]
        public int OfferID { get; set; }

        [Required]
        [Column("ClientID")]
        public int ClientID { get; set; }

        [ForeignKey("ClientID")]
        public Client Client
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

        [Required]
        [Column("RealtorID")]
        public int RealtorID { get; set; }

        [ForeignKey("RealtorID")]
        public Realtor Realtor
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

        [Required]
        [Column("PropertyID")]
        public int PropertyID { get; set; }

        [ForeignKey("PropertyID")]
        public Property Property
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

        [Required]
        [Column("Price")]
        public int Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                }
            }
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

        public override string ToString()
        {
            return $"{Property}, {Price:C}";
        }
    }
}

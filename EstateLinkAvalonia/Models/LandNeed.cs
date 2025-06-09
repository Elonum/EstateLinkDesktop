using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace EstateLinkAvalonia.Models
{
    public class LandNeed : INotifyPropertyChanged
    {
        private Need? _need;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Key]
        [ForeignKey("Need")]
        public int NeedID { get; set; }

        public virtual Need? Need
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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EstateLinkWpf.Models;
using System.Globalization;

namespace EstateLinkWpf.Views
{
    public partial class RealtorEditView : Window
    {
        public Realtor Realtor { get; }
        private readonly bool _isNew;

        public RealtorEditView(Realtor realtor = null)
        {
            InitializeComponent();
            _isNew = realtor == null;

            if (_isNew)
            {
                Realtor = new Realtor();
                Title = "Добавление риэлтора";
                Header.Text = "Добавление риэлтора";
                SaveButton.Content = "Добавить";
            }
            else
            {
                Realtor = realtor;
                Title = "Редактирование риэлтора";
                Header.Text = "Редактирование риэлтора";
                SaveButton.Content = "Сохранить";
            }

            DataContext = Realtor;
        }

        private void ShowError(TextBox textBox, TextBlock errorBlock, string error)
        {
            textBox.BorderBrush = (Brush)FindResource("red_0");
            errorBlock.Text = error;
            errorBlock.Visibility = Visibility.Visible;
            textBox.Tag = null;
        }

        private void ClearError(TextBox textBox, TextBlock errorBlock)
        {
            textBox.BorderBrush = (Brush)FindResource("gray_3");
            errorBlock.Visibility = Visibility.Collapsed;
            textBox.Tag = "Valid";
        }

        private string FormatName(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(value.ToLower());
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            FirstNameTextBox.Text = FormatName(FirstNameTextBox.Text);
            LastNameTextBox.Text = FormatName(LastNameTextBox.Text);
            PatronymicTextBox.Text = FormatName(PatronymicTextBox.Text);

            if (string.IsNullOrEmpty(FirstNameTextBox.Text))
            {
                ShowError(FirstNameTextBox, FirstNameError, "Имя обязательно для заполнения");
                isValid = false;
            }
            else if (FirstNameTextBox.Text.Contains(" "))
            {
                ShowError(FirstNameTextBox, FirstNameError, "Имя не должно содержать пробелов");
                isValid = false;
            }
            else if (FirstNameTextBox.Text.Length < 2 || FirstNameTextBox.Text.Length > 50)
            {
                ShowError(FirstNameTextBox, FirstNameError, "Имя должно быть от 2 до 50 символов");
                isValid = false;
            }
            else if (Regex.IsMatch(FirstNameTextBox.Text, @"\d"))
            {
                ShowError(FirstNameTextBox, FirstNameError, "Имя не должно содержать цифр");
                isValid = false;
            }
            else
            {
                ClearError(FirstNameTextBox, FirstNameError);
            }

            if (string.IsNullOrEmpty(LastNameTextBox.Text))
            {
                ShowError(LastNameTextBox, LastNameError, "Фамилия обязательна для заполнения");
                isValid = false;
            }
            else if (LastNameTextBox.Text.Contains(" "))
            {
                ShowError(LastNameTextBox, LastNameError, "Фамилия не должна содержать пробелов");
                isValid = false;
            }
            else if (LastNameTextBox.Text.Length < 2 || LastNameTextBox.Text.Length > 50)
            {
                ShowError(LastNameTextBox, LastNameError, "Фамилия должна быть от 2 до 50 символов");
                isValid = false;
            }
            else if (Regex.IsMatch(LastNameTextBox.Text, @"\d"))
            {
                ShowError(LastNameTextBox, LastNameError, "Фамилия не должна содержать цифр");
                isValid = false;
            }
            else
            {
                ClearError(LastNameTextBox, LastNameError);
            }

            if (string.IsNullOrEmpty(PatronymicTextBox.Text))
            {
                ShowError(PatronymicTextBox, PatronymicError, "Отчество обязательно для заполнения");
                isValid = false;
            }
            else if (PatronymicTextBox.Text.Contains(" "))
            {
                ShowError(PatronymicTextBox, PatronymicError, "Отчество не должно содержать пробелов");
                isValid = false;
            }
            else if (PatronymicTextBox.Text.Length < 2 || PatronymicTextBox.Text.Length > 50)
            {
                ShowError(PatronymicTextBox, PatronymicError, "Отчество должно быть от 2 до 50 символов");
                isValid = false;
            }
            else if (Regex.IsMatch(PatronymicTextBox.Text, @"\d"))
            {
                ShowError(PatronymicTextBox, PatronymicError, "Отчество не должно содержать цифр");
                isValid = false;
            }
            else
            {
                ClearError(PatronymicTextBox, PatronymicError);
            }

            if (!string.IsNullOrEmpty(CommissionShareTextBox.Text))
            {
                if (CommissionShareTextBox.Text.Contains(" "))
                {
                    ShowError(CommissionShareTextBox, CommissionShareError, "Доля от комиссии не должна содержать пробелов");
                    isValid = false;
                }
                else if (!decimal.TryParse(CommissionShareTextBox.Text, out decimal share) || share < 0 || share > 100)
                {
                    ShowError(CommissionShareTextBox, CommissionShareError, "Доля от комиссии должна быть от 0 до 100");
                    isValid = false;
                }
                else
                {
                    ClearError(CommissionShareTextBox, CommissionShareError);
                }
            }
            else
            {
                ClearError(CommissionShareTextBox, CommissionShareError);
                Realtor.CommissionShare = 0;
            }

            if (isValid)
            {
                DialogResult = true;
                Close();
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

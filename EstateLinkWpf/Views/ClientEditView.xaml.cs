using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EstateLinkWpf.Models;
using System.Globalization;

namespace EstateLinkWpf.Views
{
    public partial class ClientEditView : Window
    {
        public Client Client { get; }
        private readonly bool _isNew;

        public ClientEditView(Client client = null)
        {
            InitializeComponent();
            _isNew = client == null;

            if (_isNew)
            {
                Client = new Client();
                Title = "Добавление клиента";
                Header.Text = "Добавление клиента";
                SaveButton.Content = "Добавить";
            }
            else
            {
                Client = client;
                Title = "Редактирование клиента";
                Header.Text = "Редактирование клиента";
                SaveButton.Content = "Сохранить";
            }

            DataContext = Client;
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
            if (!string.IsNullOrEmpty(PatronymicTextBox.Text))
            {
                PatronymicTextBox.Text = FormatName(PatronymicTextBox.Text);
            }

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

            if (!string.IsNullOrEmpty(PatronymicTextBox.Text))
            {
                if (PatronymicTextBox.Text.Contains(" "))
                {
                    ShowError(PatronymicTextBox, PatronymicError, "Отчество не должно содержать пробелов");
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
            }

            if (string.IsNullOrEmpty(PhoneTextBox.Text))
            {
                ShowError(PhoneTextBox, PhoneError, "Телефон обязателен для заполнения");
                isValid = false;
            }
            else if (PhoneTextBox.Text.Contains(" "))
            {
                ShowError(PhoneTextBox, PhoneError, "Телефон не должен содержать пробелов");
                isValid = false;
            }
            else if (!Regex.IsMatch(PhoneTextBox.Text, @"^\+?[1-9]\d{10,11}$"))
            {
                ShowError(PhoneTextBox, PhoneError, "Введите корректный номер телефона");
                isValid = false;
            }
            else
            {
                ClearError(PhoneTextBox, PhoneError);
            }

            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                ShowError(EmailTextBox, EmailError, "Email обязателен для заполнения");
                isValid = false;
            }
            else if (EmailTextBox.Text.Contains(" "))
            {
                ShowError(EmailTextBox, EmailError, "Email не должен содержать пробелов");
                isValid = false;
            }
            else if (!Regex.IsMatch(EmailTextBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowError(EmailTextBox, EmailError, "Введите корректный email адрес");
                isValid = false;
            }
            else
            {
                ClearError(EmailTextBox, EmailError);
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

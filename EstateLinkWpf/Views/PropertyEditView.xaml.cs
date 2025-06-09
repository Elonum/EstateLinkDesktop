using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.Entity;
using System.Linq;
using EstateLinkWpf.Data;
using EstateLinkWpf.Models;
using System.Globalization;

namespace EstateLinkWpf.Views
{
    public partial class PropertyEditView : Window
    {
        private readonly EstateLinkContext _db;
        private readonly Property _property;
        private readonly bool _isNew;

        public PropertyEditView(EstateLinkContext db, Property property = null)
        {
            InitializeComponent();
            _db = db;
            _isNew = property == null;

            if (_isNew)
            {
                _property = new Property { IsNew = true };
                Title = "Добавление объекта недвижимости";
                Header.Text = "Добавление объекта недвижимости";
                SaveButton.Content = "Добавить";
            }
            else
            {
                _property = property;
                _property.IsNew = false;
                Title = "Редактирование объекта недвижимости";
                Header.Text = "Редактирование объекта недвижимости";
                SaveButton.Content = "Сохранить";
                PropertyTypeComboBox.IsEnabled = false;
            }

            DataContext = _property;
            LoadPropertyTypes();
            PropertyTypeComboBox.SelectionChanged += OnPropertyTypeChanged;
        }

        private string FormatName(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(value.ToLower());
        }

        private void ShowError(FrameworkElement element, TextBlock errorBlock, string error)
        {
            if (element is TextBox textBox)
            {
                textBox.BorderBrush = (Brush)FindResource("red_0");
                textBox.Tag = null;
            }
            else if (element is ComboBox comboBox)
            {
                comboBox.BorderBrush = (Brush)FindResource("red_0");
                comboBox.Tag = null;
            }
            
            errorBlock.Text = error;
            errorBlock.Visibility = Visibility.Visible;
        }

        private void ClearError(FrameworkElement element, TextBlock errorBlock)
        {
            if (element is TextBox textBox)
            {
                textBox.BorderBrush = (Brush)FindResource("gray_3");
                textBox.Tag = "Valid";
            }
            else if (element is ComboBox comboBox)
            {
                comboBox.BorderBrush = (Brush)FindResource("gray_3");
                comboBox.Tag = "Valid";
            }
            
            errorBlock.Visibility = Visibility.Collapsed;
        }

        private bool ValidateTextInput(TextBox textBox, TextBlock errorBlock, string fieldName, bool allowNumbers = false, bool required = true)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (required)
                {
                    ShowError(textBox, errorBlock, $"{fieldName} обязателен для заполнения");
                    return false;
                }
                return true;
            }

            if (textBox.Text.Contains(" "))
            {
                ShowError(textBox, errorBlock, $"{fieldName} не должен содержать пробелов");
                return false;
            }

            if (!allowNumbers && Regex.IsMatch(textBox.Text, @"\d"))
            {
                ShowError(textBox, errorBlock, $"{fieldName} не должен содержать цифр");
                return false;
            }

            ClearError(textBox, errorBlock);
            return true;
        }

        private bool ValidateNumericInput(TextBox textBox, TextBlock errorBlock, string fieldName, bool required = true)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (required)
                {
                    ShowError(textBox, errorBlock, $"{fieldName} обязателен для заполнения");
                    return false;
                }
                return true;
            }

            if (textBox.Text.Contains(" "))
            {
                ShowError(textBox, errorBlock, $"{fieldName} не должен содержать пробелов");
                return false;
            }

            if (Regex.IsMatch(textBox.Text, @"[^0-9]"))
            {
                ShowError(textBox, errorBlock, $"{fieldName} должен содержать только цифры");
                return false;
            }

            ClearError(textBox, errorBlock);
            return true;
        }

        private bool ValidateCoordinate(TextBox textBox, TextBlock errorBlock, string fieldName, double minValue, double maxValue, out double? value)
        {
            value = null;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                if (textBox.Text.Contains(" "))
                {
                    ShowError(textBox, errorBlock, $"{fieldName} не должна содержать пробелов");
                    return false;
                }
                else if (!double.TryParse(textBox.Text, out double coord) || coord < minValue || coord > maxValue)
                {
                    ShowError(textBox, errorBlock, $"{fieldName} должна быть от {minValue} до {maxValue}");
                    return false;
                }
                else
                {
                    value = coord;
                    ClearError(textBox, errorBlock);
                    return true;
                }
            }
            ClearError(textBox, errorBlock);
            return true;
        }

        private bool ValidatePositiveInteger(TextBox textBox, TextBlock errorBlock, string fieldName, out int? value, bool required = true)
        {
            value = null;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                if (required)
                {
                    ShowError(textBox, errorBlock, $"{fieldName} обязателен для заполнения");
                    return false;
                }
                return true;
            }

            if (textBox.Text.Contains(" "))
            {
                ShowError(textBox, errorBlock, $"{fieldName} не должен содержать пробелов");
                return false;
            }

            if (!int.TryParse(textBox.Text, out int number) || number <= 0)
            {
                ShowError(textBox, errorBlock, $"{fieldName} должен быть положительным числом");
                return false;
            }

            value = number;
            ClearError(textBox, errorBlock);
            return true;
        }

        private bool ValidatePositiveDouble(TextBox textBox, TextBlock errorBlock, string fieldName, out double? value, bool required = true)
        {
            value = null;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                if (required)
                {
                    ShowError(textBox, errorBlock, $"{fieldName} обязателен для заполнения");
                    return false;
                }
                return true;
            }

            if (textBox.Text.Contains(" "))
            {
                ShowError(textBox, errorBlock, $"{fieldName} не должен содержать пробелов");
                return false;
            }

            if (!double.TryParse(textBox.Text, out double number) || number <= 0)
            {
                ShowError(textBox, errorBlock, $"{fieldName} должен быть положительным числом");
                return false;
            }

            value = number;
            ClearError(textBox, errorBlock);
            return true;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            CityTextBox.Text = FormatName(CityTextBox.Text);
            StreetTextBox.Text = FormatName(StreetTextBox.Text);

            isValid &= ValidateTextInput(CityTextBox, CityError, "Город");
            isValid &= ValidateTextInput(StreetTextBox, StreetError, "Улица");

            isValid &= ValidateNumericInput(HouseTextBox, HouseError, "Номер дома");

            if (_property.IsApartment)
            {
                isValid &= ValidateNumericInput(ApartmentTextBox, ApartmentError, "Номер квартиры");
            }

            double? latitude, longitude;
            isValid &= ValidateCoordinate(LatitudeTextBox, LatitudeError, "Широта", -90, 90, out latitude);
            isValid &= ValidateCoordinate(LongitudeTextBox, LongitudeError, "Долгота", -180, 180, out longitude);
            _property.Latitude = latitude;
            _property.Longitude = longitude;

            if (_property.IsApartment)
            {
                int? floor;
                isValid &= ValidatePositiveInteger(FloorTextBox, FloorError, "Этаж", out floor);
                _property.Floor = floor;
            }
            else
            {
                _property.Floor = null;
                ClearError(FloorTextBox, FloorError);
            }

            if (_property.IsApartment || _property.IsHouse)
            {
                int? roomCount;
                isValid &= ValidatePositiveInteger(RoomCountTextBox, RoomCountError, "Количество комнат", out roomCount);
                _property.RoomCount = roomCount;
            }
            else
            {
                _property.RoomCount = null;
                ClearError(RoomCountTextBox, RoomCountError);
            }

            double? area;
            isValid &= ValidatePositiveDouble(AreaTextBox, AreaError, "Площадь", out area);
            _property.Area = area ?? 0;

            if (_property.PropertyType == null)
            {
                ShowError(PropertyTypeComboBox, PropertyTypeError, "Тип недвижимости обязателен для заполнения");
                isValid = false;
            }
            else
            {
                ClearError(PropertyTypeComboBox, PropertyTypeError);
            }

            if (isValid)
            {
                try
                {
                    if (_isNew)
                    {
                        _db.Properties.Add(_property);
                        _db.SaveChanges();
                    }
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void LoadPropertyTypes()
        {
            PropertyTypeComboBox.ItemsSource = _db.PropertyTypes.ToList();
        }

        private void OnPropertyTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_property != null && PropertyTypeComboBox.SelectedItem is PropertyType selectedType)
            {
                _property.PropertyType = selectedType;
            }
        }
    }
}

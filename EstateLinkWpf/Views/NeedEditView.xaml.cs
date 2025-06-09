using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.Entity;
using EstateLinkWpf.Data;
using EstateLinkWpf.Models;
using System.Windows.Input;

namespace EstateLinkWpf.Views
{
    public partial class NeedEditView : Window
    {
        private readonly EstateLinkContext _db;
        private readonly Need _need;
        private readonly bool _isNew;

        public NeedEditView(EstateLinkContext db, Need need = null)
        {
            InitializeComponent();
            _db = db;
            _isNew = need == null;

            if (_isNew)
            {
                _need = new Need { IsNew = true };
                Title = "Добавление потребности";
                Header.Text = "Добавление потребности";
                SaveButton.Content = "Добавить";
            }
            else
            {
                _need = need;
                _need.IsNew = false;
                Title = "Редактирование потребности";
                Header.Text = "Редактирование потребности";
                SaveButton.Content = "Сохранить";

                _db.Entry(_need)
                    .Reference(n => n.Property)
                    .Load();
                _db.Entry(_need.Property)
                    .Reference(p => p.PropertyType)
                    .Load();
            }

            DataContext = _need;
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            try
            {
                PropertyTypeComboBox.ItemsSource = _db.PropertyTypes
                    .OrderBy(pt => pt.TypeName)
                    .ToList();

                RealtorComboBox.ItemsSource = _db.Realtors
                    .OrderBy(r => r.LastName)
                    .ToList();

                ClientComboBox.ItemsSource = _db.Clients
                    .OrderBy(c => c.LastName)
                    .ToList();

                if (!_isNew && _need.Property?.PropertyType != null)
                {
                    var propertyType = PropertyTypeComboBox.Items.Cast<PropertyType>()
                        .FirstOrDefault(pt => pt.PropertyTypeID == _need.Property.PropertyTypeID);
                    
                    if (propertyType != null)
                    {
                        PropertyTypeComboBox.SelectedItem = propertyType;
                        UpdateFieldsVisibility();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFieldsVisibility()
        {
            if (PropertyTypeComboBox.SelectedItem is PropertyType selectedType)
            {
                UpdateFieldsVisibility(selectedType);
            }
        }

        private void UpdateFieldsVisibility(PropertyType selectedType)
        {
            if (selectedType == null) return;

            var properties = _db.Properties
                .Where(p => p.PropertyTypeID == selectedType.PropertyTypeID)
                .ToList();

            PropertyComboBox.ItemsSource = properties
                .OrderBy(p => p.City)
                .ThenBy(p => p.Street)
                .ThenBy(p => p.House)
                .ThenBy(p => p.Apartment);

            switch (selectedType.TypeName)
            {
                case "Квартира":
                    ApartmentFields.Visibility = Visibility.Visible;
                    HouseFields.Visibility = Visibility.Collapsed;
                    LandFields.Visibility = Visibility.Collapsed;
                    break;
                case "Дом":
                    ApartmentFields.Visibility = Visibility.Collapsed;
                    HouseFields.Visibility = Visibility.Visible;
                    LandFields.Visibility = Visibility.Collapsed;
                    break;
                case "Земельный участок":
                    ApartmentFields.Visibility = Visibility.Collapsed;
                    HouseFields.Visibility = Visibility.Collapsed;
                    LandFields.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void PropertyTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PropertyTypeComboBox.SelectedItem is PropertyType selectedType)
            {
                UpdateFieldsVisibility(selectedType);
            }
        }

        private void PropertyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PropertyComboBox.SelectedItem != null)
            {
                PropertyError.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowError(Control element, TextBlock errorBlock, string error)
        {
            if (element is TextBox textBox)
            {
                textBox.Tag = null;
                element.BorderBrush = (Brush)FindResource("red_0");
            }
            errorBlock.Text = error;
            errorBlock.Visibility = Visibility.Visible;
        }

        private void ClearError(Control element, TextBlock errorBlock)
        {
            if (element is TextBox textBox)
            {
                element.BorderBrush = (Brush)FindResource("gray_3");
                textBox.Tag = "Valid";
            }
            errorBlock.Visibility = Visibility.Collapsed;
        }

        private bool ValidatePositiveInteger(TextBox textBox, TextBlock errorBlock, string fieldName, out int? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                ShowError(textBox, errorBlock, $"{fieldName} обязательна для заполнения");
                return false;
            }

            if (!int.TryParse(textBox.Text, out int value) || value <= 0)
            {
                ShowError(textBox, errorBlock, $"{fieldName} должна быть положительным числом");
                return false;
            }

            result = value;
            ClearError(textBox, errorBlock);
            return true;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            MinPriceTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            MaxPriceTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            MinAreaTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            MaxAreaTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            if (PropertyTypeComboBox.SelectedItem is PropertyType selectedType)
            {
                switch (selectedType.TypeName)
                {
                    case "Квартира":
                        ApartmentMinRoomsTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        ApartmentMaxRoomsTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        ApartmentMinFloorTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        ApartmentMaxFloorTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        break;
                    case "Дом":
                        HouseMinRoomsTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        HouseMaxRoomsTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        HouseMinFloorsTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        HouseMaxFloorsTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        break;
                }
            }

            if (ClientComboBox.SelectedItem == null)
            {
                ShowError(ClientComboBox, ClientError, "Выберите клиента");
                return;
            }

            if (RealtorComboBox.SelectedItem == null)
            {
                ShowError(RealtorComboBox, RealtorError, "Выберите риэлтора");
                return;
            }

            if (PropertyTypeComboBox.SelectedItem == null)
            {
                ShowError(PropertyTypeComboBox, PropertyTypeError, "Выберите тип недвижимости");
                return;
            }

            if (PropertyComboBox.SelectedItem == null)
            {
                ShowError(PropertyComboBox, PropertyError, "Выберите объект недвижимости");
                return;
            }

            if (!ValidatePositiveInteger(MinPriceTextBox, MinPriceError, "Минимальная цена", out int? minPrice) ||
                !ValidatePositiveInteger(MaxPriceTextBox, MaxPriceError, "Максимальная цена", out int? maxPrice) ||
                !ValidatePositiveInteger(MinAreaTextBox, MinAreaError, "Минимальная площадь", out int? minArea) ||
                !ValidatePositiveInteger(MaxAreaTextBox, MaxAreaError, "Максимальная площадь", out int? maxArea))
            {
                return;
            }

            if (maxPrice < minPrice)
            {
                ShowError(MaxPriceTextBox, MaxPriceError, "Максимальная цена не может быть меньше минимальной");
                return;
            }

            if (maxArea < minArea)
            {
                ShowError(MaxAreaTextBox, MaxAreaError, "Максимальная площадь не может быть меньше минимальной");
                return;
            }

            if (PropertyTypeComboBox.SelectedItem is PropertyType selectedPropertyType)
            {
                int? apartmentMinRooms = null, apartmentMaxRooms = null;
                int? apartmentMinFloor = null, apartmentMaxFloor = null;
                int? houseMinRooms = null, houseMaxRooms = null;
                int? houseMinFloors = null, houseMaxFloors = null;

                switch (selectedPropertyType.TypeName)
                {
                    case "Квартира":
                        if (!ValidatePositiveInteger(ApartmentMinRoomsTextBox, ApartmentMinRoomsError, "Минимальное количество комнат", out apartmentMinRooms) ||
                            !ValidatePositiveInteger(ApartmentMaxRoomsTextBox, ApartmentMaxRoomsError, "Максимальное количество комнат", out apartmentMaxRooms) ||
                            !ValidatePositiveInteger(ApartmentMinFloorTextBox, ApartmentMinFloorError, "Минимальный этаж", out apartmentMinFloor) ||
                            !ValidatePositiveInteger(ApartmentMaxFloorTextBox, ApartmentMaxFloorError, "Максимальный этаж", out apartmentMaxFloor))
                        {
                            return;
                        }

                        if (apartmentMaxRooms < apartmentMinRooms)
                        {
                            ShowError(ApartmentMaxRoomsTextBox, ApartmentMaxRoomsError, "Максимальное количество комнат не может быть меньше минимального");
                            return;
                        }

                        if (apartmentMaxFloor < apartmentMinFloor)
                        {
                            ShowError(ApartmentMaxFloorTextBox, ApartmentMaxFloorError, "Максимальный этаж не может быть меньше минимального");
                            return;
                        }
                        break;

                    case "Дом":
                        if (!ValidatePositiveInteger(HouseMinRoomsTextBox, HouseMinRoomsError, "Минимальное количество комнат", out houseMinRooms) ||
                            !ValidatePositiveInteger(HouseMaxRoomsTextBox, HouseMaxRoomsError, "Максимальное количество комнат", out houseMaxRooms) ||
                            !ValidatePositiveInteger(HouseMinFloorsTextBox, HouseMinFloorsError, "Минимальная этажность", out houseMinFloors) ||
                            !ValidatePositiveInteger(HouseMaxFloorsTextBox, HouseMaxFloorsError, "Максимальная этажность", out houseMaxFloors))
                        {
                            return;
                        }

                        if (houseMaxRooms < houseMinRooms)
                        {
                            ShowError(HouseMaxRoomsTextBox, HouseMaxRoomsError, "Максимальное количество комнат не может быть меньше минимального");
                            return;
                        }

                        if (houseMaxFloors < houseMinFloors)
                        {
                            ShowError(HouseMaxFloorsTextBox, HouseMaxFloorsError, "Максимальная этажность не может быть меньше минимальной");
                            return;
                        }
                        break;
                }

                try
                {
                    _need.MinPrice = minPrice ?? 0;
                    _need.MaxPrice = maxPrice ?? 0;
                    _need.MinArea = minArea ?? 0;
                    _need.MaxArea = maxArea ?? 0;

                    switch (selectedPropertyType.TypeName)
                    {
                        case "Квартира":
                            _need.ApartmentNeed = new ApartmentNeed
                            {
                                MinRoomCount = apartmentMinRooms ?? 0,
                                MaxRoomCount = apartmentMaxRooms ?? 0,
                                MinFloor = apartmentMinFloor ?? 0,
                                MaxFloor = apartmentMaxFloor ?? 0
                            };
                            break;

                        case "Дом":
                            _need.HouseNeed = new HouseNeed
                            {
                                MinRoomCount = houseMinRooms ?? 0,
                                MaxRoomCount = houseMaxRooms ?? 0,
                                MinFloor = houseMinFloors ?? 0,
                                MaxFloor = houseMaxFloors ?? 0
                            };
                            break;
                    }

                    if (_isNew)
                    {
                        _db.Needs.Add(_need);
                    }
                    else
                    {
                        _db.Entry(_need).State = EntityState.Modified;
                    }
                    _db.SaveChanges();

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
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void PriceTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!int.TryParse(text, out _))
                {
                    e.CancelCommand();
                }
            }
            else
                e.CancelCommand();
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using EstateLinkWpf.Data;
using EstateLinkWpf.Models;

namespace EstateLinkWpf.Views
{
    public partial class PropertyView : Window
    {
        private readonly EstateLinkContext _db;
        private IQueryable<Property> _propertiesQuery;

        public PropertyView()
        {
            InitializeComponent();
            try
            {
                _db = new EstateLinkContext();
                LoadPropertyTypes();
                LoadProperties();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPropertyTypes()
        {
            try
            {
                var types = _db.PropertyTypes.ToList();
                types.Insert(0, new PropertyType { PropertyTypeID = 0, TypeName = "Все" });
                TypeFilterComboBox.ItemsSource = types;
                TypeFilterComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов недвижимости: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProperties()
        {
            try
            {
                if (_db == null)
                {
                    MessageBox.Show("База данных не инициализирована.", 
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _propertiesQuery = _db.Properties.Include(p => p.PropertyType);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (_propertiesQuery == null)
                {
                    PropertiesGrid.ItemsSource = new List<Property>();
                    return;
                }

                var query = _propertiesQuery;

                if (TypeFilterComboBox.SelectedIndex > 0)
                {
                    var selectedType = (PropertyType)TypeFilterComboBox.SelectedItem;
                    query = query.Where(p => p.PropertyTypeID == selectedType.PropertyTypeID);
                }

                if (!string.IsNullOrWhiteSpace(CityFilterTextBox.Text))
                {
                    string cityFilter = CityFilterTextBox.Text.ToLower();
                    query = query.Where(p => p.City.ToLower().Contains(cityFilter));
                }

                if (!string.IsNullOrWhiteSpace(StreetFilterTextBox.Text))
                {
                    string streetFilter = StreetFilterTextBox.Text.ToLower();
                    query = query.Where(p => p.Street.ToLower().Contains(streetFilter));
                }

                var result = query.Include(p => p.PropertyType).ToList();
                PropertiesGrid.ItemsSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при применении фильтров: {ex.Message}", 
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                PropertiesGrid.ItemsSource = new List<Property>();
            }
        }

        private void OnTypeFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnCityFilterChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnStreetFilterChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            var addWindow = new PropertyEditView(_db);
            if (addWindow.ShowDialog() == true)
            {
                LoadProperties();
            }
        }

        private Property CreatePropertyCopy(Property source)
        {
            return new Property
            {
                PropertyID = source.PropertyID,
                PropertyTypeID = source.PropertyTypeID,
                PropertyType = source.PropertyType,
                City = source.City,
                Street = source.Street,
                House = source.House,
                Apartment = source.Apartment,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                Floor = source.Floor,
                RoomCount = source.RoomCount,
                Area = source.Area
            };
        }

        private void UpdatePropertyFromCopy(Property target, Property source)
        {
            target.PropertyTypeID = source.PropertyTypeID;
            target.PropertyType = source.PropertyType;
            target.City = source.City;
            target.Street = source.Street;
            target.House = source.House;
            target.Apartment = source.Apartment;
            target.Latitude = source.Latitude;
            target.Longitude = source.Longitude;
            target.Floor = source.Floor;
            target.RoomCount = source.RoomCount;
            target.Area = source.Area;
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (PropertiesGrid.SelectedItem is Property property)
            {
                var temp = CreatePropertyCopy(property);
                var editWindow = new PropertyEditView(_db, temp);
                if (editWindow.ShowDialog() == true)
                {
                    var dbProperty = _db.Properties.Include(p => p.PropertyType).FirstOrDefault(p => p.PropertyID == temp.PropertyID);
                    if (dbProperty != null)
                    {
                        UpdatePropertyFromCopy(dbProperty, temp);
                        _db.SaveChanges();
                        LoadProperties();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите объект недвижимости для редактирования.",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (PropertiesGrid.SelectedItem is Property property)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этот объект недвижимости?",
                                           "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (_db.Offers.Any(o => o.PropertyID == property.PropertyID))
                        {
                            MessageBox.Show("Нельзя удалить объект недвижимости, связанный с предложениями.",
                                          "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (_db.Needs.Any(n => n.PropertyID == property.PropertyID))
                        {
                            MessageBox.Show("Нельзя удалить объект недвижимости, связанный с потребностями.",
                                          "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        _db.Properties.Remove(property);
                        _db.SaveChanges();
                        LoadProperties();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите объект недвижимости для удаления.",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

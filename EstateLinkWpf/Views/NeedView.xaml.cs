using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using EstateLinkWpf.Data;
using EstateLinkWpf.Models;

namespace EstateLinkWpf.Views
{
    public partial class NeedView : Window
    {
        private readonly EstateLinkContext _db;
        private IQueryable<Need> _needsQuery;
        private Need _selectedNeed;

        public NeedView()
        {
            InitializeComponent();
            try
            {
                _db = new EstateLinkContext();
                LoadFilters();
                LoadNeeds();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFilters()
        {
            try
            {
                if (_db == null)
                {
                    MessageBox.Show("База данных не инициализирована.",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Загружаем клиентов
                var clients = _db.Clients.OrderBy(c => c.LastName).ToList();
                clients.Insert(0, new Client { Id = 0, LastName = "Все" });
                ClientFilter.ItemsSource = clients;
                ClientFilter.SelectedIndex = 0;

                // Загружаем риэлторов
                var realtors = _db.Realtors.OrderBy(r => r.LastName).ToList();
                realtors.Insert(0, new Realtor { Id = 0, LastName = "Все" });
                RealtorFilter.ItemsSource = realtors;
                RealtorFilter.SelectedIndex = 0;

                // Загружаем типы недвижимости
                var propertyTypes = _db.PropertyTypes.OrderBy(pt => pt.TypeName).ToList();
                propertyTypes.Insert(0, new PropertyType { PropertyTypeID = 0, TypeName = "Все" });
                PropertyTypeFilter.ItemsSource = propertyTypes;
                PropertyTypeFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фильтров: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadNeeds()
        {
            try
            {
                if (_db == null)
                {
                    MessageBox.Show("База данных не инициализирована.",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _needsQuery = _db.Needs
                    .Include(n => n.Client)
                    .Include(n => n.Realtor)
                    .Include(n => n.Property)
                    .Include(n => n.Property.PropertyType)
                    .Include(n => n.ApartmentNeed)
                    .Include(n => n.HouseNeed);

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
                if (_needsQuery == null)
                {
                    NeedsGrid.ItemsSource = Enumerable.Empty<Need>();
                    return;
                }

                var query = _needsQuery;

                if (ClientFilter.SelectedIndex > 0)
                {
                    var selectedClient = (Client)ClientFilter.SelectedItem;
                    query = query.Where(n => n.ClientID == selectedClient.Id);
                }

                if (RealtorFilter.SelectedIndex > 0)
                {
                    var selectedRealtor = (Realtor)RealtorFilter.SelectedItem;
                    query = query.Where(n => n.RealtorID == selectedRealtor.Id);
                }

                if (PropertyTypeFilter.SelectedIndex > 0)
                {
                    var selectedType = (PropertyType)PropertyTypeFilter.SelectedItem;
                    query = query.Where(n => n.Property.PropertyTypeID == selectedType.PropertyTypeID);
                }

                NeedsGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при применении фильтров: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                NeedsGrid.ItemsSource = Enumerable.Empty<Need>();
            }
        }

        private void OnFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedNeed = NeedsGrid.SelectedItem as Need;
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            var editWindow = new NeedEditView(_db);
            if (editWindow.ShowDialog() == true)
            {
                LoadNeeds();
            }
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (_selectedNeed == null)
            {
                MessageBox.Show("Выберите потребность для редактирования.", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var editWindow = new NeedEditView(_db, _selectedNeed);
            if (editWindow.ShowDialog() == true)
            {
                LoadNeeds();
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selectedNeed == null)
            {
                MessageBox.Show("Выберите потребность для удаления",
                              "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранную потребность?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Needs.Remove(_selectedNeed);
                    _db.SaveChanges();
                    LoadNeeds();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            _db.Dispose();
            this.Close();
        }
    }
} 
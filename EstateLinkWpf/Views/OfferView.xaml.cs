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
    public partial class OfferView : Window
    {
        private readonly EstateLinkContext _db;

        public OfferView()
        {
            InitializeComponent();
            try
            {
                _db = new EstateLinkContext();
                LoadOffers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOffers()
        {
            try
            {
                var offers = _db.Offers
                    .Include(o => o.Realtor)
                    .Include(o => o.Client)
                    .Include(o => o.Property.PropertyType)
                    .ToList();

                OffersGrid.ItemsSource = offers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке предложений: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                OffersGrid.ItemsSource = new List<Offer>();
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            var editWindow = new OfferEditView(_db);
            if (editWindow.ShowDialog() == true)
            {
                LoadOffers();
            }
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (OffersGrid.SelectedItem is Offer offer)
            {
                var editWindow = new OfferEditView(_db, offer);
                if (editWindow.ShowDialog() == true)
                {
                    LoadOffers();
                }
            }
            else
            {
                MessageBox.Show("Выберите предложение для редактирования.",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (OffersGrid.SelectedItem is Offer offer)
            {
                var result = MessageBox.Show(
                    "Вы действительно хотите удалить это предложение?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.Offers.Remove(offer);
                        _db.SaveChanges();
                        LoadOffers();
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
                MessageBox.Show("Выберите предложение для удаления.",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

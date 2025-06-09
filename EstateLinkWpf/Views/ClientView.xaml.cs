using System;
using System.Linq;
using System.Windows;
using EstateLinkWpf.Data;
using EstateLinkWpf.Models;

namespace EstateLinkWpf.Views
{
    public partial class ClientView : Window
    {
        private readonly EstateLinkContext _db = new EstateLinkContext();

        public ClientView()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            ClientsGrid.ItemsSource = _db.Clients.ToList();
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            var editWindow = new ClientEditView();
            if (editWindow.ShowDialog() == true)
            {
                var client = editWindow.Client;
                _db.Clients.Add(client);
                _db.SaveChanges();
                LoadClients();
            }
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client client)
            {
                var temp = new Client
                {
                    Id = client.Id,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Patronymic = client.Patronymic,
                    Phone = client.Phone,
                    Email = client.Email
                };
                var editWindow = new ClientEditView(temp);
                if (editWindow.ShowDialog() == true)
                {
                    var dbClient = _db.Clients.Find(temp.Id);
                    dbClient.FirstName = temp.FirstName;
                    dbClient.LastName = temp.LastName;
                    dbClient.Patronymic = temp.Patronymic;
                    dbClient.Phone = temp.Phone;
                    dbClient.Email = temp.Email;
                    _db.SaveChanges();
                    LoadClients();
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client client
                && MessageBox.Show("Удалить клиента?", "Подтверждение",
                       MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    if (_db.Offers.Any(o => o.ClientID == client.Id))
                    {
                        MessageBox.Show("Нельзя удалить клиента, связанного с предложениями.",
                                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (_db.Needs.Any(n => n.ClientID == client.Id))
                    {
                        MessageBox.Show("Нельзя удалить клиента, связанного с потребностями.",
                                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _db.Clients.Remove(client);
                    _db.SaveChanges();
                    LoadClients();
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
            this.Close();
        }
    }
}

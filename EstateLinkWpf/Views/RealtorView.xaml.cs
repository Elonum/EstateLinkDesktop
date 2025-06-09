using System;
using System.Linq;
using System.Windows;
using EstateLinkWpf.Data;
using EstateLinkWpf.Models;

namespace EstateLinkWpf.Views
{
    public partial class RealtorView : Window
    {
        private readonly EstateLinkContext _db = new EstateLinkContext();

        public RealtorView()
        {
            InitializeComponent();
            LoadRealtors();
        }

        private void LoadRealtors()
        {
            RealtorsGrid.ItemsSource = _db.Realtors.ToList();
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            var win = new RealtorEditView();
            if (win.ShowDialog() == true)
            {
                _db.Realtors.Add(win.Realtor);
                _db.SaveChanges();
                LoadRealtors();
            }
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (RealtorsGrid.SelectedItem is Realtor r)
            {
                var temp = new Realtor
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Patronymic = r.Patronymic,
                    CommissionShare = r.CommissionShare
                };
                var win = new RealtorEditView(temp);
                if (win.ShowDialog() == true)
                {
                    var dbR = _db.Realtors.Find(temp.Id);
                    dbR.FirstName = temp.FirstName;
                    dbR.LastName = temp.LastName;
                    dbR.Patronymic = temp.Patronymic;
                    dbR.CommissionShare = temp.CommissionShare;
                    _db.SaveChanges();
                    LoadRealtors();
                }
            }
            else
            {
                MessageBox.Show("Выберите риэлтора для редактирования.", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (RealtorsGrid.SelectedItem is Realtor r
                && MessageBox.Show("Удалить риэлтора?", "Подтверждение",
                       MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    if (_db.Offers.Any(o => o.RealtorID == r.Id))
                    {
                        MessageBox.Show("Нельзя удалить риэлтора, связанного с предложениями.",
                                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (_db.Needs.Any(n => n.RealtorID == r.Id))
                    {
                        MessageBox.Show("Нельзя удалить риэлтора, связанного с потребностями.",
                                      "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _db.Realtors.Remove(r);
                    _db.SaveChanges();
                    LoadRealtors();
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

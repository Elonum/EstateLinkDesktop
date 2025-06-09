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
    public partial class OfferEditView : Window
    {
        private readonly EstateLinkContext _db;
        private readonly Offer _offer;
        private readonly bool _isNew;

        public OfferEditView(EstateLinkContext db, Offer offer = null)
        {
            InitializeComponent();
            _db = db;
            _isNew = offer == null;

            if (_isNew)
            {
                _offer = new Offer { IsNew = true };
                Title = "Добавление предложения";
                Header.Text = "Добавление предложения";
                SaveButton.Content = "Добавить";
            }
            else
            {
                _offer = offer;
                _offer.IsNew = false;
                Title = "Редактирование предложения";
                Header.Text = "Редактирование предложения";
                SaveButton.Content = "Сохранить";
            }

            DataContext = _offer;
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            try
            {
                RealtorComboBox.ItemsSource = _db.Realtors
                    .OrderBy(r => r.LastName)
                    .ToList();

                ClientComboBox.ItemsSource = _db.Clients
                    .OrderBy(c => c.LastName)
                    .ToList();

                PropertyComboBox.ItemsSource = _db.Properties
                    .Include(p => p.PropertyType)
                    .OrderBy(p => p.City)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowError(Control element, TextBlock errorBlock, string error)
        {
            if (element is TextBox)
            {
                element.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            errorBlock.Text = error;
            errorBlock.Visibility = Visibility.Visible;
        }

        private void ClearError(Control element, TextBlock errorBlock)
        {
            if (element is TextBox)
            {
                element.BorderBrush = (Brush)FindResource("gray_3");
            }
            errorBlock.Visibility = Visibility.Collapsed;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if (RealtorComboBox.SelectedItem == null)
            {
                RealtorError.Text = "Выберите риэлтора";
                RealtorError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                RealtorError.Visibility = Visibility.Collapsed;
            }

            if (ClientComboBox.SelectedItem == null)
            {
                ClientError.Text = "Выберите клиента";
                ClientError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                ClientError.Visibility = Visibility.Collapsed;
            }

            if (PropertyComboBox.SelectedItem == null)
            {
                PropertyError.Text = "Выберите объект недвижимости";
                PropertyError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                PropertyError.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                ShowError(PriceTextBox, PriceError, "Цена обязательна для заполнения");
                isValid = false;
            }
            else if (!int.TryParse(PriceTextBox.Text, out int price) || price <= 0)
            {
                ShowError(PriceTextBox, PriceError, "Цена должна быть положительным целым числом");
                isValid = false;
            }
            else
            {
                ClearError(PriceTextBox, PriceError);
            }

            if (isValid)
            {
                try
                {
                    if (_isNew)
                    {
                        _db.Offers.Add(_offer);
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
            Close();
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text[0]);
        }

        private void PriceTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsDigit))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}

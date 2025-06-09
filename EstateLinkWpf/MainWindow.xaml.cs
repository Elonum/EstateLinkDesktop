using System.Windows;
using EstateLinkWpf.Views;

namespace EstateLinkWpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new ClientView();
            wnd.Owner = this;
            this.Hide();
            wnd.ShowDialog();
            this.Show();
        }

        private void RealtorsButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new RealtorView();
            wnd.Owner = this;
            this.Hide();
            wnd.ShowDialog();
            this.Show();
        }

        private void PropertiesButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new PropertyView();
            wnd.Owner = this;
            this.Hide();
            wnd.ShowDialog();
            this.Show();
        }

        private void OffersButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new OfferView();
            wnd.Owner = this;
            this.Hide();
            wnd.ShowDialog();
            this.Show();
        }

        private void NeedsButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new NeedView();
            wnd.Owner = this;
            this.Hide();
            wnd.ShowDialog();
            this.Show();
        }
    }
}

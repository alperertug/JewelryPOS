using System.Windows;

namespace JewelryPOS.App.Views
{
    public partial class PasswordDialog : Window
    {
        public string EnteredPassword { get; private set; }

        public PasswordDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            EnteredPassword = PasswordBox.Password;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
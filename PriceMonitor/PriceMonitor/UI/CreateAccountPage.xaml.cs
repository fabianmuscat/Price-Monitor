using System.Data.Entity;
using System.Windows;
using PriceMonitor.EntityFramework;

namespace PriceMonitor.UI
{
    /// <summary>
    /// Interaction logic for CreateAccountPage.xaml
    /// </summary>
    public partial class CreateAccountPage : Window
    {
        public CreateAccountPage()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string email = EmailBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
                MessageBox.Show("These fields cannot be left empty:\nUsername\nPassword\nConfirm Password", "",
                    MessageBoxButton.OK);
            else
            {
                if (!confirmPassword.Equals(password))
                    MessageBox.Show("Passwords do not match!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    UserAccount newUser = new UserAccount();
                    if (string.IsNullOrEmpty(email))
                    {
                        MessageBoxResult result = MessageBox.Show(
                            "By not inputting your email, you will not be able to receive emails about products that you are monitoring!\nDo you want to continue?",
                            "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            if (newUser.CreateAccount(username, confirmPassword))
                            {
                                AccountCreatedSign.Visibility = Visibility.Visible;
                                AccountNotCreatedSign.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                AccountNotCreatedSign.Visibility = Visibility.Visible;
                                AccountCreatedSign.Visibility = Visibility.Collapsed;
                                MessageBox.Show("An Account with that username already exists", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                    }
                    else
                    {
                        newUser.CreateAccount(username, confirmPassword, email);
                        MessageBox.Show("Account Created Successfully");
                    }
                }
            }

            UsernameBox.Clear();
            PasswordBox.Clear();
            ConfirmPasswordBox.Clear();
            EmailBox.Clear();
        }

        private void Arrow_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            Close();
        }
    }
}
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PriceMonitor.EntityFramework;

namespace PriceMonitor.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        public User LoggedUser;

        public LoginPage()
        {
            LoggedUser = new object() as User;
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                MessageBox.Show("Fields cannot be left empty", "",
                    MessageBoxButton.OK);
            else
            {
                UserAccount userLogin = new UserAccount();

                if (userLogin.Login(username, password))
                {
                    LoggedUser = UserAccount.LoggedUser;
                    DatabaseManager.Log(ActivityType.Login, UserAccount.LoggedUser);

                    StartupPage startupPage = new StartupPage();
                    startupPage.Show();
                    Close();
                }
                else MessageBox.Show("Incorrect Details", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            UsernameBox.Clear();
            PasswordBox.Clear();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            CreateAccountPage createAccountPage = new CreateAccountPage();
            createAccountPage.Show();
            Close();
        }

        private void PasswordBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); // clicking the login button programmatically
        }
    }
}

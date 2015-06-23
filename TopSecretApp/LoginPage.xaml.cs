using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TopSecretApp
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Prevent pasting into Password box.
            this.pbPassword.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, (o, ev) => ev.Handled = true));
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            pbPassword.SecurePassword.MakeReadOnly();
            if (verifyPasswordInDatabase(pbPassword.SecurePassword))
            {
                NavigationService nav = NavigationService.GetNavigationService(this);
                nav.Navigate(new Uri("HomePage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private bool verifyPasswordInDatabase(SecureString password)
        {
            string hashedPassword = HashSecureString(password);
            password.Dispose(); // We call this as soon as possible.

            // ...Send hashed password to db and verify here...

            // Done verifying, login is valid.
            return true;
        }

        /// <summary>
        /// Read the unmanaged memory and hash the SecureString value and return the hashed string.
        /// </summary>
        private static string HashSecureString(SecureString password)
        {
            IntPtr unmanagedBytes = Marshal.SecureStringToGlobalAllocAnsi(password);
            try
            {
                byte[] hashedBytes = new byte[password.Length];
                unsafe
                {
                    byte* byteArray = (byte*)unmanagedBytes.ToPointer();

                    for (int i = 0; i < hashedBytes.Length; ++i)
                    {
                        // Hash using OR operator
                        hashedBytes[i] = (byte)(byteArray[i] | 170);
                    }
                }

                return Convert.ToBase64String(hashedBytes);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocAnsi(unmanagedBytes);
            }
        }
    }
}

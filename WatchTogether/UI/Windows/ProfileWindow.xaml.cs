using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WatchTogether.Chatting;
using WatchTogether.Settings;

namespace WatchTogether.UI.Windows
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public string UserIconData { get; private set; }
        public string UserName { get; private set; }

        public ProfileWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the UserName from the settings or generate a random one if not present
            userNameTb.Text = ChatHelper.GetUserName();
            // Load the UserIconBrush from the settings or generate a random one if not present
            userIconBorder.Background = ChatHelper.GetUserIconBrush(out string userIconData);
            UserIconData = userIconData;

            MakeUserNameTextBoxReadOnly();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            UserName = userNameTb.Text;
            DialogResult = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void MakeUserNameTextBoxEditable()
        {
            userNameTb.BorderThickness = new Thickness(1);
            userNameTb.IsReadOnly = false;
        }

        private void MakeUserNameTextBoxReadOnly()
        {
            userNameTb.BorderThickness = new Thickness(0);
            userNameTb.IsReadOnly = true;
            userNameTb.Select(0, 0);

            if (string.IsNullOrEmpty(userNameTb.Text))
            {
                userNameTb.Text = ChatHelper.GetRandomUserName();
            }
        }

        private void UserNameTb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MakeUserNameTextBoxEditable();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MakeUserNameTextBoxReadOnly();
        }

        private void UserNameTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) MakeUserNameTextBoxReadOnly();
        }

        private void UserIconBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Initialize the openFileDialog object
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    var fileName = openFileDialog.FileName;
                    var image = UserIconHelper.LoadImageFromFileWithResize(fileName);
                    var userIconBrush = UserIconHelper.GetImageBrushFromImage(image);
                    var userIconData = UserIconHelper.ConvertImageToString(image);

                    userIconBorder.Background = userIconBrush;
                    UserIconData = userIconData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}, {ex.StackTrace}");
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
        }
    }
}

using System.Windows;
using WatchTogether.Chatting;
using WatchTogether.Settings;

namespace WatchTogether.UI.Windows
{
    /// <summary>
    /// Interaction logic for JoinServerWindow.xaml
    /// </summary>
    public partial class JoinServerWindow : Window
    {
        private int serverPort;
        private string serverIp;

        private const string DefaultAddress = "127.0.0.1:8910";
        private const string JoinButtonValidationErrorsToolTip = "The fields above must have a valid values in order to join a server!";

        public JoinServerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the last valid host address from settings or default address if the first one is missing
            addressTb.Text = SettingsModelManager.CurrentSettings.LastHostServerAddress ?? DefaultAddress;
            // Validate the loaded host address and assign the DefaultAddress if validation fails
            addressTb.Text = ValidateAddress() ? addressTb.Text : DefaultAddress;

            // Update the UI input fields
            ValidateAddress();
            ValidatePassword();

            // Assign events
            addressTb.TextChanged += AddressTb_TextChanged;
            passwordTb.PasswordChanged += PasswordTb_PasswordChanged;
        }

        /// <summary>
        /// Port number getter
        /// </summary>
        public int Port
        {
            get { return serverPort; }
        }

        /// <summary>
        /// Ip address getter
        /// </summary>
        public string Ip
        {
            get { return serverIp; }
        }

        /// <summary>
        /// Password getter
        /// </summary>
        public string Password
        {
            get { return passwordTb.Password; }
        }

        /// <summary>
        /// The Click event handler of the Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// The Click event handler of the Join button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JoinBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateAddress() == true
                && ValidatePassword() == true)
            {
                // We can successfully close the dialog window only if all fields are valid
                DialogResult = true;

                // Our host address is valid so save to the settings
                var settings = SettingsModelManager.CurrentSettings;
                settings.LastJoinServerAddress = addressTb.Text;
                SettingsModelManager.CurrentSettings = settings;
            }
        }

        /// <summary>
        /// The PreviewTextInput event handler of the "Server address" field. Allows user to enter only numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressTb_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var isInt = int.TryParse(e.Text, out int _);
            var isDot = e.Text == ".";
            var isColon = e.Text == ":";
            e.Handled = !(isInt || isDot || isColon);
        }

        /// <summary>
        /// The TextChanged event handler of the "Server address" field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressTb_TextChanged(object sender, RoutedEventArgs e)
        {
            if (ValidateAddress() == false)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// The TextChanged event handler of the "Server password" field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordTb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
        }

        /// <summary>
        /// Validates a "Server address" field value. Changes the ToolTip and Background properties 
        /// relative to the validation result
        /// </summary>
        /// <returns>True if the entered port is valid, otherwise False</returns>
        private bool ValidateAddress()
        {
            return ChatHelper.ValidateAddress(addressTb, ref serverPort,
                ref serverIp, (value) => SetJoinBtnIsEnabled(value));
        }

        /// <summary>
        /// Validates a "Server password" field value. Changes the ToolTip and Background properties relative to
        /// the validation result
        /// </summary>
        /// <returns>True if the entered password is valid, otherwise False</returns>
        private bool ValidatePassword()
        {
            return ChatHelper.ValidatePassword(passwordTb, (value) => SetJoinBtnIsEnabled(value));
        }

        /// <summary>
        /// Enables and disables the "Join" button depending on the fields validation result
        /// </summary>
        /// <param name="enabled">The value to be assined to IsEnabled property</param>
        private void SetJoinBtnIsEnabled(bool enabled)
        {
            var fieldValueValidBrush = ChatHelper.GetFieldValueValidBrush();

            if (enabled == true
                && addressTb.Background == fieldValueValidBrush
                && passwordTb.Background == fieldValueValidBrush)
            {
                joinBtn.IsEnabled = true;
                joinBtn.ToolTip = string.Empty;
            }
            else
            {
                joinBtn.IsEnabled = false;
                joinBtn.ToolTip = JoinButtonValidationErrorsToolTip;
            }
        }
    }
}

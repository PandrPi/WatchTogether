using System.Windows;
using WatchTogether.Chatting;
using WatchTogether.Settings;

namespace WatchTogether.UI.Windows
{
    /// <summary>
    /// Interaction logic for HostServerWindow.xaml
    /// </summary>
    public partial class HostServerWindow : Window
    {
        private int serverPort;
        private string serverIp;

        private const string DefaultAddress = "127.0.0.1:8910";
        private const string HostButtonValidationErrorsToolTip = "The fields above must have a valid values in order to host a server!";

        /// <summary>
        /// Initializes window
        /// </summary>
        public HostServerWindow()
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
            ValidateConfirmationPassword();

            // Assign events
            addressTb.TextChanged += AddressTb_TextChanged;
            passwordTb.PasswordChanged += PasswordTb_PasswordChanged;
            passwordConfirmationTb.PasswordChanged += PasswordConfirmationTb_PasswordChanged;
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
        /// Confirmation password getter
        /// </summary>
        public string ConfirmationPassword
        {
            get { return passwordConfirmationTb.Password; }
        }

        /// <summary>
        /// Validates a "Server address" field value. Changes the ToolTip and Background properties 
        /// relative to the validation result
        /// </summary>
        /// <returns>True if the entered port is valid, otherwise False</returns>
        private bool ValidateAddress()
        {
            return ChatHelper.ValidateAddress(addressTb, ref serverPort,
                ref serverIp, (value) => SetHostBtnIsEnabled(value));
        }

        /// <summary>
        /// Validates a "Server password" field value. Changes the ToolTip and Background properties relative to
        /// the validation result
        /// </summary>
        /// <returns>True if the entered password is valid, otherwise False</returns>
        private bool ValidatePassword()
        {
            return ChatHelper.ValidatePassword(passwordTb, (value) => SetHostBtnIsEnabled(value));
        }

        /// <summary>
        /// Validates a "Confirm password" field value. Changes the ToolTip and Background properties relative to
        /// the validation result
        /// </summary>
        /// <returns>True if the entered confirmation password is valid and matches the password,
        /// otherwise False</returns>
        private bool ValidateConfirmationPassword()
        {
            return ChatHelper.ValidateConfirmationPassword(passwordTb,
                passwordConfirmationTb, (value) => SetHostBtnIsEnabled(value));
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
        /// The Click event handler of the Host button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateAddress() == true
                && ValidatePassword() == true
                && ValidateConfirmationPassword() == true)
            {
                // We can successfully close the dialog window only if all fields are valid
                DialogResult = true;

                // Our host address is valid so save to the settings
                var settings = SettingsModelManager.CurrentSettings;
                settings.LastHostServerAddress = addressTb.Text;
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
        /// The TextChanged event handler of the "Confirm password" field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordConfirmationTb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateConfirmationPassword();
        }

        /// <summary>
        /// Enables and disables the "Host" button depending on the fields validation result
        /// </summary>
        /// <param name="enabled">The value to be assined to IsEnabled property</param>
        private void SetHostBtnIsEnabled(bool enabled)
        {
            var fieldValueValidBrush = ChatHelper.GetFieldValueValidBrush();

            if (enabled == true
                && addressTb.Background == fieldValueValidBrush
                && passwordTb.Background == fieldValueValidBrush
                && passwordConfirmationTb.Background == fieldValueValidBrush)
            {
                hostBtn.IsEnabled = true;
                hostBtn.ToolTip = string.Empty;
            }
            else
            {
                hostBtn.IsEnabled = false;
                hostBtn.ToolTip = HostButtonValidationErrorsToolTip;
            }
        }
    }
}

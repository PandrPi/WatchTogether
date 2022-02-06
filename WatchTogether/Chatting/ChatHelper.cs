using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WatchTogether.Settings;
using WatchTogether.UI.Windows;

namespace WatchTogether.Chatting
{
    public class ChatHelper
    {
        public enum SocketAddresParseResult
        {
            Success = 1,
            InvalidIP = 2,
            InvalidPort = 4,
            NoColon = 8,
        }

        public static bool ShouldSearchForBrushResources { get; set; }
        private static SolidColorBrush _fieldValueInvalidBrush = null;
        private static SolidColorBrush _fieldValueValidBrush = null;

        private const string FieldValueInvalidBrushKey = "TextBox_InvalidValue_Background";
        private const string FieldValueValidBrushKey = "TextBox_Background";

        private const string NoColon = "There is no ':' character in the address field!";
        private const string InvalidServerIp = "IP must be in 'x.x.x.x' format. Each x ranging from 0 to 255";
        private const string InvalidServerPort = "Port number must be in (1025, 65536) (inclusive) range!";
        private const string PasswordFieldEmpty = "Password field value cannot be empty!";
        private const string ConfirmPasswordFieldEmpty = "Confirm password field value cannot be empty!";
        private const string PasswordsAreNotEqual = "Password and Confirm password fields values are not equal!";

        private static readonly string[] InvalidSockedAddresToolTips =
        {
            InvalidServerIp, InvalidServerPort, NoColon
        };

        /// <summary>
        /// Gets the brush that matches the brush object for the field with an invalid value.
        /// </summary>
        /// <returns></returns>
        public static SolidColorBrush GetFieldValueInvalidBrush()
        {
            if (_fieldValueInvalidBrush is null == true || ShouldSearchForBrushResources == true)
            {
                var window = MainWindow.Instance;
                _fieldValueInvalidBrush = (SolidColorBrush)(window.FindResource(FieldValueInvalidBrushKey)
                    as Freezable).GetAsFrozen();
                return _fieldValueInvalidBrush;
            }
            else
            {
                return _fieldValueInvalidBrush;
            }
        }

        /// <summary>
        /// Gets the brush that matches the brush object for the field with an invalid value.
        /// </summary>
        /// <returns></returns>
        public static SolidColorBrush GetFieldValueValidBrush()
        {
            if (_fieldValueValidBrush is null == true || ShouldSearchForBrushResources == true)
            {
                var window = MainWindow.Instance;
                _fieldValueValidBrush = (SolidColorBrush)(window.FindResource(FieldValueValidBrushKey)
                    as Freezable).GetAsFrozen();
                return _fieldValueValidBrush;
            }
            else
            {
                return _fieldValueValidBrush;
            }
        }

        /// <summary>
        /// Converts the specified socket address string to IpAddress object and port 32-bit integer number. A return
        /// value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="socketAddress">The socket address string to convert</param>
        /// <param name="ip">The out IPAddress object</param>
        /// <param name="port">The out port integer</param>
        /// <returns>True if conversion succeeded and False otherwise</returns>
        public static SocketAddresParseResult TryParseSocketAddress(string socketAddress, out IPAddress ip, out int port)
        {
            const char socketSeparator = ':';

            SocketAddresParseResult parseResult = SocketAddresParseResult.Success;

            if (socketAddress.Contains(socketSeparator) == false)
            {
                ip = null;
                port = 0;
                return SocketAddresParseResult.NoColon;
            }

            var addressArray = socketAddress.Split(socketSeparator);
            string ipStr = addressArray[0];
            string portStr = addressArray[1];

            bool ipParseResult = IPAddress.TryParse(ipStr, out ip);
            bool portParseResult = int.TryParse(portStr, out port) && (port >= 1025 && port <= 65536);

            parseResult |= ipParseResult ? 0 : SocketAddresParseResult.InvalidIP;
            parseResult |= portParseResult ? 0 : SocketAddresParseResult.InvalidPort;

            return parseResult;
        }

        /// <summary>
        /// Validates a "Server address" field value. Changes the ToolTip and Background properties 
        /// relative to the validation result
        /// </summary>
        /// <returns>True if the entered port is valid, otherwise False</returns>
        public static bool ValidateAddress(
            TextBox addressTb,
            ref int serverPort,
            ref string serverIp,
            Action<bool> callback)
        {
            var socketAddress = addressTb.Text;
            var parseResult = TryParseSocketAddress(socketAddress, out var ip, out int port);

            if (parseResult == SocketAddresParseResult.Success)
            {
                addressTb.Background = GetFieldValueValidBrush();
                addressTb.ToolTip = string.Empty;

                serverPort = port;
                serverIp = ip.ToString();

                callback(true);
                return true;
            }
            else
            {
                int toolTipIndex = (int)Math.Log((int)parseResult, 2) - 1;

                addressTb.Background = GetFieldValueInvalidBrush();
                addressTb.ToolTip = InvalidSockedAddresToolTips[toolTipIndex];

                callback(false);
                return false;
            }
        }

        /// <summary>
        /// Validates a "Server password" field value. Changes the ToolTip and Background properties relative to
        /// the validation result
        /// </summary>
        /// <returns>True if the entered password is valid, otherwise False</returns>
        public static bool ValidatePassword(
            PasswordBox passwordTb,
            Action<bool> callback)
        {
            if (string.IsNullOrEmpty(passwordTb.Password) == true)
            {
                // Field is empty
                passwordTb.ToolTip = PasswordFieldEmpty;
                passwordTb.Background = GetFieldValueInvalidBrush();
                callback(false);
                return false;
            }
            else
            {
                // Field is not empty
                passwordTb.ToolTip = string.Empty;
                passwordTb.Background = GetFieldValueValidBrush();
                callback(true);
                return true;
            }
        }

        /// <summary>
        /// Validates a "Confirm password" field value. Changes the ToolTip and Background properties relative to
        /// the validation result
        /// </summary>
        /// <returns>True if the entered confirmation password is valid and matches the password,
        /// otherwise False</returns>
        public static bool ValidateConfirmationPassword(
            PasswordBox passwordTb,
            PasswordBox passwordConfirmationTb,
            Action<bool> callback)
        {
            if (string.IsNullOrEmpty(passwordConfirmationTb.Password) == true)
            {
                // Field is empty
                passwordConfirmationTb.ToolTip = ConfirmPasswordFieldEmpty;
                passwordConfirmationTb.Background = GetFieldValueInvalidBrush();
                callback(false);
                return false;
            }
            else if (passwordTb.Password != passwordConfirmationTb.Password)
            {
                // Password field and confirmation password field values are not equal
                passwordConfirmationTb.ToolTip = PasswordsAreNotEqual;
                passwordConfirmationTb.Background = GetFieldValueInvalidBrush();
                callback(false);
                return false;
            }
            else
            {
                // Field is not empty
                passwordConfirmationTb.ToolTip = string.Empty;
                passwordConfirmationTb.Background = GetFieldValueValidBrush();
                callback(true);
                return true;
            }
        }

        /// <summary>
        /// Generates a random UserName string
        /// </summary>
        public static string GetRandomUserName()
        {
            var positiveHashCode = Guid.NewGuid().GetHashCode() & 0x7FFFFFFF;
            return "User" + positiveHashCode.ToString();
        }

        /// <summary>
        /// Tries to load the UserName from the settings. If there is no saved UserName it
        /// generates a random one.
        /// </summary>
        public static string GetUserName()
        {
            var settings = SettingsModelManager.CurrentSettings;
            string userName = settings.UserName;

            return string.IsNullOrEmpty(userName) == false ? userName : GetRandomUserName();
        }

        /// <summary>
        /// Tries to load the UserIconBrush from the settings. If there is no saved UserIconBrush it
        /// generates a random one.
        /// </summary>
        /// <param name="userIconData"></param>
        /// <returns></returns>
        public static Brush GetUserIconBrush(out string userIconData)
        {
            var settings = SettingsModelManager.CurrentSettings;
            userIconData = settings.UserIconData;

            try
            {
                var userBrush = UserIconHelper.GetUserIconBrushFromString(userIconData);
                return userBrush;
            }
            catch
            {
                var colorBrush = UserIconColorBrushManager.GetRandomColorBrush();
                userIconData = colorBrush.Color.ToString();
                return colorBrush;
            }
        }
    }
}

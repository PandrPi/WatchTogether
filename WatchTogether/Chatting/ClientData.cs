using Newtonsoft.Json;
using System.Windows.Media;

namespace WatchTogether.Chatting
{
    class ClientData
    {
        /// <summary>
        /// The ID of the user client
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// The UserName of the client
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The Brush object of the user icon
        /// </summary>
        [JsonIgnore]
        public Brush UserIconBrush
        {
            get { return _userBrush; }
        }

        /// <summary>
        /// The string object that represents an actual data of the user icon from which an actual
        /// Brush object can be created
        /// </summary>
        public string UserIconData
        {
            get
            {
                return _userIconData;
            }
            set
            {
                _userIconData = value;
                _userBrush = UserIconHelper.GetUserIconBrushFromString(_userIconData);
            }
        }

        private string _userIconData;
        private Brush _userBrush;

        public ClientData(int userID, string userName, string userIconData)
        {
            UserID = userID;
            UserName = userName;
            UserIconData = userIconData;
        }

        public static ClientData FromString(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return JsonConvert.DeserializeObject<ClientData>(data);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

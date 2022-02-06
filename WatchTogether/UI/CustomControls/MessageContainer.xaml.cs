using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WatchTogether.UI.CustomControls
{
	/// <summary>
	/// Interaction logic for MessageContainer.xaml
	/// </summary>
	public partial class MessageContainer : UserControl
	{
        #region DependencyProperties

        private static readonly Type OwnerType = typeof(MessageContainer);

		public static readonly DependencyProperty MessageTextProperty = 
			DependencyProperty.Register("MessageText", typeof(string), OwnerType, new UIPropertyMetadata(string.Empty));
		
		public static readonly DependencyProperty SenderUserNameProperty = 
			DependencyProperty.Register("SenderUserName", typeof(string), OwnerType, new UIPropertyMetadata(string.Empty));
		
		public static readonly DependencyProperty SenderIconBrushProperty = 
			DependencyProperty.Register("SenderIconBrush", typeof(Brush), OwnerType, new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
		
		public static readonly DependencyProperty ShortSenderUserNameProperty = 
			DependencyProperty.Register("ShortSenderUserName", typeof(string), OwnerType, new UIPropertyMetadata(string.Empty));
		
		public static readonly DependencyProperty ReceivingTimeProperty = 
			DependencyProperty.Register("ReceivingTime", typeof(string), OwnerType, new UIPropertyMetadata(string.Empty));

		public string MessageText
		{
			get => (string)GetValue(MessageTextProperty);
			set => SetValue(MessageTextProperty, value);
		}

		public string SenderUserName
		{
			get => (string)GetValue(SenderUserNameProperty);
			set => SetValue(SenderUserNameProperty, value);
		}

		public Brush SenderIconBrush
		{
			get => (Brush)GetValue(SenderIconBrushProperty);
			set => SetValue(SenderIconBrushProperty, value);
		}

		public string ShortSenderUserName
		{
			get => (string)GetValue(ShortSenderUserNameProperty);
			set => SetValue(ShortSenderUserNameProperty, value);
		}

		public string ReceivingTime
		{
			get => (string)GetValue(ReceivingTimeProperty);
			set => SetValue(ReceivingTimeProperty, value);
		}

        #endregion

        public MessageContainer()
		{
			InitializeComponent();
		}
	}
}

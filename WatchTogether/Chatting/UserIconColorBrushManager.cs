using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WatchTogether.Chatting
{
    /// <summary>
    /// Stores a set of SolidColorBrush instances and provides a random item
    /// from that set at the request of the user
    /// </summary>
    class UserIconColorBrushManager
    {
        /// <summary>
        /// Stores a SolidColorBrush instances
        /// </summary>
        private static readonly List<SolidColorBrush> Brushes = new List<SolidColorBrush>
        {
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF4F670E"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFCEC028"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD03522"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFEE9EAC"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF96B4A2"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF80BBAA"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD9A485"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF2C9EA8"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFDA7F3C"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF5E8A3C"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFCF1317"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFB65D2F"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFC06C51"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF416B84"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFC15D66"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF356D85"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF408099"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF5A8CB3"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFDC661B"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF4C4F80"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF555AA7"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF7261A2"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FFBE71D0"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF594FD0"),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8150CA"),
        };

        /// <summary>
        /// Shuffles the brushes list and freezes each element of this list 
        /// </summary>
        static UserIconColorBrushManager()
        {
            var shuffled = Brushes.OrderBy(a => Guid.NewGuid().GetHashCode()).ToList();
            Brushes.Clear();
            Brushes.AddRange(shuffled);

            for (int i = 0; i < Brushes.Count; i++)
            {
                var brush = Brushes[i];
                if (brush.CanFreeze) brush.Freeze();
            }
        }

        /// <summary>
        /// Returns a random SolidColorBrush instance from the internal list
        /// </summary>
        public static SolidColorBrush GetRandomColorBrush()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            return Brushes[random.Next(0, Brushes.Count - 1)];
        }
    }
}

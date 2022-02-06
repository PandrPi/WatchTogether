using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WatchTogether.Chatting
{
    internal class UserIconHelper
    {
        public static int ImageWidth { get; private set; } = 64;
        private static readonly Regex HexColorRegex = new Regex("^#(?:[0-9a-fA-F]{3,4}){1,2}$", RegexOptions.Compiled);

        /// <summary>
        /// Loads a BitmapImage object from the specified file path
        /// </summary>
        /// <param name="fileName">The path to the image file</param>
        /// <returns>The freezed and resized BitmapImage object</returns>
        public static BitmapImage LoadImageFromFileWithResize(string fileName)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fileName);
            image.DecodePixelWidth = ImageWidth;
            image.EndInit();
            if (image.CanFreeze) image.Freeze();

            return image;
        }

        /// <summary>
        /// Creates a new ImageBrush object from the specified BitmapImage object
        /// </summary>
        /// <param name="image">The image to create the brush from</param>
        /// <returns>A new freezed ImageBrush object</returns>
        public static ImageBrush GetImageBrushFromImage(BitmapImage image)
        {
            var brush = new ImageBrush(image);
            brush.Stretch = Stretch.UniformToFill;
            if (brush.CanFreeze) brush.Freeze();

            return brush;
        }

        /// <summary>
        /// Converts the specified BitmapImage object to a new byte array
        /// </summary>
        /// <param name="image">The image to convert to byte array</param>
        /// <returns>The byte array that contains all data about image</returns>
        public static byte[] ConvertImageToByteArray(BitmapImage image)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        /// <summary>
        /// Converts the specified byte array to a decoded string object
        /// </summary>
        /// <param name="imageData">The array of bytes of image data to convert to a string</param>
        /// <returns>A decoded string</returns>
        public static string ConvertImageByteArrayToString(byte[] imageData)
        {
            return Convert.ToBase64String(imageData);
        }

        /// <summary>
        /// Converts the specified string object to a byte array
        /// </summary>
        /// <param name="imageData">The image data string to convert to byte array</param>
        /// <returns>A encoded byte array</returns>
        public static byte[] ConvertStringToImageByteArray(string imageData)
        {
            return Convert.FromBase64String(imageData);
        }

        /// <summary>
        /// Converts the specified BitmapImage object to a new string object
        /// </summary>
        /// <param name="image">The image to convert to string</param>
        /// <returns>A new string object which contains all data about the image</returns>
        public static string ConvertImageToString(BitmapImage image)
        {
            var imageData = ConvertImageToByteArray(image);
            return ConvertImageByteArrayToString(imageData);
        }

        /// <summary>
        /// Converts the specified string object to a new BitmapImage object
        /// </summary>
        /// <param name="imageData">The image string to convert to BitmapImage object</param>
        /// <returns>A new BitmapImage object</returns>
        public static BitmapImage ConvertStringToImage(string imageData)
        {
            var imageDataArray = ConvertStringToImageByteArray(imageData);

            using (var ms = new MemoryStream(imageDataArray))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                if (image.CanFreeze) image.Freeze();

                return image;
            }
        }

        /// <summary>
        /// Converts the specified string object to the Brush object
        /// </summary>
        /// <param name="iconData">The user icon data to get the Brush object from</param>
        /// <returns>A new Brush object that represents a user icon brush</returns>
        public static Brush GetUserIconBrushFromString(string iconData)
        {
            if (HexColorRegex.IsMatch(iconData) == true)
            {
                // The brushData is a Hex color string so just create a new SolidColorBrush from it
                var color = (Color)ColorConverter.ConvertFromString(iconData);
                return new SolidColorBrush(color).GetAsFrozen() as SolidColorBrush;
            }
            else
            {
                // the brushData is a base64 string object that contains data of the user icon image
                var iconImage = ConvertStringToImage(iconData);
                return GetImageBrushFromImage(iconImage);
            }
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ExifLib;

namespace SpotbaseSharp.Util
{
    public static class ImageUtility
    {
        private static readonly string BasePath = App.AppDirectory;

        public static byte[] LoadImage(string fileName)
        {
            byte[] image;

            Image tmpImage = ScaleImage(Image.FromFile(fileName), 800, 600);

            using (var memStream = new MemoryStream())
            {
                tmpImage.Save(memStream, ImageFormat.Jpeg);
                image = memStream.ToArray();
            }

            return image;
        }

        public static byte[] LoadImage(string fileName, int width, int height)
        {
            byte[] image;

            Image tmpImage = ScaleImage(Image.FromFile(fileName), width, height);

            using (var memStream = new MemoryStream())
            {
                tmpImage.Save(memStream, ImageFormat.Jpeg);
                image = memStream.ToArray();
            }

            return image;
        }

        public static byte[] LoadImageOriginal(string fileName)
        {
            byte[] image;

            Image tmpImage = Image.FromFile(fileName);

            using (var memStream = new MemoryStream())
            {
                tmpImage.Save(memStream, ImageFormat.Jpeg);
                image = memStream.ToArray();
            }

            return image;
        }

        public static void SaveSmallImage(string fileName, string smallName)
        {
            Image tmpImage = ScaleImage(Image.FromFile(fileName), 800, 600);
            tmpImage.Save(smallName);
        }

        public static byte[] LoadSmallImage(Guid smallFile)
        {
            byte[] image;

            string smallFilePath = Path.Combine(BasePath, "SmallImages");
            string smallFilename = Path.Combine(smallFilePath, smallFile + ".jpeg");

            Image tmpImage = Image.FromFile(smallFilename);
            using (var memStream = new MemoryStream())
            {
                tmpImage.Save(memStream, ImageFormat.Jpeg);
                image = memStream.ToArray();
            }

            return image;
        }

        public static double[] GetLatLongFromImage(string imagePath, out DateTime createDate)
        {
            var reader = new ExifReader(imagePath);

            //get create date
            reader.GetTagValue(ExifTags.DateTimeOriginal, out createDate);

            // EXIF lat/long tags stored as [Degree, Minute, Second]
            double[] latitudeComponents;
            double[] longitudeComponents;

            string latitudeRef; // "N" or "S" ("S" will be negative latitude)
            string longitudeRef; // "E" or "W" ("W" will be a negative longitude)

            if (reader.GetTagValue(ExifTags.GPSLatitude, out latitudeComponents)
                && reader.GetTagValue(ExifTags.GPSLongitude, out longitudeComponents)
                && reader.GetTagValue(ExifTags.GPSLatitudeRef, out latitudeRef)
                && reader.GetTagValue(ExifTags.GPSLongitudeRef, out longitudeRef))
            {
                double latitude = ConvertDegreeAngleToDouble(latitudeComponents[0], latitudeComponents[1],
                    latitudeComponents[2], latitudeRef);
                double longitude = ConvertDegreeAngleToDouble(longitudeComponents[0], longitudeComponents[1],
                    longitudeComponents[2], longitudeRef);

                return new[] {latitude, longitude};
            }

            return null;
        }

        public static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds,
            string latLongRef)
        {
            double result = ConvertDegreeAngleToDouble(degrees, minutes, seconds);

            if (latLongRef == "S" || latLongRef == "W")
            {
                result *= -1;
            }

            return result;
        }

        public static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds)
        {
            return degrees + (minutes/60) + (seconds/3600);
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioX = (double) maxWidth/image.Width;
            double ratioY = (double) maxHeight/image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int) (image.Width*ratio);
            var newHeight = (int) (image.Height*ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
    }
}
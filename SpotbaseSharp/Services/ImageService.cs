using System;
using System.Diagnostics;
using System.IO;
using SpotbaseSharp.Util;

namespace SpotbaseSharp.Services
{
    public class ImageService : IImageService
    {
        private const string SmallImageFoldername = "SmallImages";
        private const string LargeImageFoldername = "LargeImages";
        private const string JpegExtension = ".jpeg";
        private readonly string _basePath = App.AppDirectory;

        public double[] GetLatLongFromImage(string filename, out DateTime createDate)
        {
            return ImageUtility.GetLatLongFromImage(filename, out createDate);
        }

        public byte[] LoadImage(string filename)
        {
            return ImageUtility.LoadImage(filename);
        }

        public Guid CopyLargeFile(string filename)
        {
            string largeFilePath = Path.Combine(_basePath, LargeImageFoldername);

            if (!Directory.Exists(largeFilePath))
            {
                Directory.CreateDirectory(largeFilePath);
            }

            Guid largeFile = Guid.NewGuid();
            string largeFilename = Path.Combine(largeFilePath, largeFile + ".jpeg");
            File.Copy(filename, largeFilename);

            return largeFile;
        }

        public Guid CopySmallFile(string filename)
        {
            string smallFilePath = Path.Combine(_basePath, SmallImageFoldername);

            if (!Directory.Exists(smallFilePath))
            {
                Directory.CreateDirectory(smallFilePath);
            }

            Guid smallFile = Guid.NewGuid();
            string smallFilename = Path.Combine(smallFilePath, smallFile + JpegExtension);

            ImageUtility.SaveSmallImage(filename, smallFilename);

            return smallFile;
        }

        public void DeleteLargeFile(Guid largeFile)
        {
            string largeFilename = GetLargeFilenPath(largeFile);

            File.Delete(largeFilename);
        }

        public void OpenLargeFile(Guid largeFile)
        {
            string largeFilename = GetLargeFilenPath(largeFile);

            Process.Start(largeFilename);
        }

        public byte[] LoadLarge(Guid largeFile)
        {
            byte[] fileContent = null;

            if (largeFile != Guid.Empty)
            {
                string largeFilename = GetLargeFilenPath(largeFile);
                fileContent = ImageUtility.LoadImageOriginal(largeFilename);
            }

            return fileContent;
        }

        public byte[] LoadSmall(Guid smallFile)
        {
            byte[] fileContent;

            string smallFilename = GetSmallFilenPath(smallFile);
            fileContent = ImageUtility.LoadImage(smallFilename);

            return fileContent;
        }

        public Guid CopySmallFile(byte[] smallFileContent)
        {
            string smallFilePath = Path.Combine(_basePath, SmallImageFoldername);

            if (!Directory.Exists(smallFilePath))
            {
                Directory.CreateDirectory(smallFilePath);
            }

            Guid smallFile = Guid.NewGuid();
            string smallFilename = Path.Combine(smallFilePath, smallFile + JpegExtension);

            File.WriteAllBytes(smallFilename, smallFileContent);

            return smallFile;
        }

        public Guid CopyLargeFile(byte[] largeFileContent)
        {
            string largeFilePath = Path.Combine(_basePath, LargeImageFoldername);

            if (!Directory.Exists(largeFilePath))
            {
                Directory.CreateDirectory(largeFilePath);
            }

            Guid largeFile = Guid.NewGuid();
            string largeFilename = Path.Combine(largeFilePath, largeFile + JpegExtension);

            File.WriteAllBytes(largeFilename, largeFileContent);

            return largeFile;
        }

        public void DeleteSmallFile(Guid smallFile)
        {
            string smallFilename = GetSmallFilenPath(smallFile);

            File.Delete(smallFilename);
        }

        private string GetLargeFilenPath(Guid largeFile)
        {
            string largeFilePath = Path.Combine(_basePath, LargeImageFoldername);
            string largeFilename = Path.Combine(largeFilePath, largeFile + JpegExtension);

            return largeFilename;
        }

        private string GetSmallFilenPath(Guid smallFile)
        {
            string smallFilePath = Path.Combine(_basePath, SmallImageFoldername);
            string smallFilename = Path.Combine(smallFilePath, smallFile + JpegExtension);

            return smallFilename;
        }
    }
}
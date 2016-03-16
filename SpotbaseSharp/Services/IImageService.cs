using System;

namespace SpotbaseSharp.Services
{
    public interface IImageService
    {
        double[] GetLatLongFromImage(string filename, out DateTime createDate);

        byte[] LoadImage(string filename);

        Guid CopyLargeFile(string filename);

        Guid CopySmallFile(string filename);

        void DeleteLargeFile(Guid largeFile);

        void OpenLargeFile(Guid largeFile);

        byte[] LoadLarge(Guid largeFile);

        byte[] LoadSmall(Guid smallFile);

        Guid CopySmallFile(byte[] smallFile);

        Guid CopyLargeFile(byte[] largeFile);

        void DeleteSmallFile(Guid smallFile);
    }
}
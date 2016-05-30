namespace SpotbaseSharp
{
    public static class SpotbaseConstants
    {
        public const string LocationUrlTemplate =
            "https://maps.googleapis.com/maps/api/staticmap?size=500x400&maptype=roadmap&markers=color:blue%7C{0}, {1}&key=";
        public const string RouteUrlTemplate = "https://www.google.com/maps/dir/Current+Location/{0}, {1}";

        public const string SmallImageFoldername = "SmallImages";
        public const string LargeImageFoldername = "LargeImages";
        public const string JpegExtension = ".jpeg";

        public const string SpotsJsonFilename = "spots.json";
        public const string LastUpdateJsonFilename = "lastUpdate.json";
    }
}
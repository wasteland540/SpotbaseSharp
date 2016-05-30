namespace SpotbaseSharp.Model
{
    public class JsonSpot
    {
        // ReSharper disable once InconsistentNaming
        public string city { get; set; }
        // ReSharper disable once InconsistentNaming
        public string description { get; set; }
        // ReSharper disable once InconsistentNaming
        public short type { get; set; }
        // ReSharper disable once InconsistentNaming
        public string name { get; set; }
        // ReSharper disable once InconsistentNaming
        public string googleLink { get; set; }
        // ReSharper disable once InconsistentNaming
        public byte[] image { get; set; }
        // ReSharper disable once InconsistentNaming
        public double lng { get; set; }
        // ReSharper disable once InconsistentNaming
        public double lat { get; set; }
    }
}
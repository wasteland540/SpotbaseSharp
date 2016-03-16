using System;

namespace SpotbaseSharp.Model.Serialization
{
    [Serializable]
    public class SerializedSpot
    {
        public string City { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public SpotType Type { get; set; }
        public byte[] SmallFile { get; set; }
        public byte[] LargeFile { get; set; }
        public string Name { get; set; }
    }
}
using System;
using SpotbaseSharp.DataAccessLayer;

namespace SpotbaseSharp.Model
{
    public class Spot : IDbObject
    {
        public Spot()
        {
            Type = SpotType.NotSet;
        }

        public string City { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public SpotType Type { get; set; }
        public Guid SmallFile { get; set; }
        public Guid LargeFile { get; set; }
        public string Name { get; set; }
    }
}
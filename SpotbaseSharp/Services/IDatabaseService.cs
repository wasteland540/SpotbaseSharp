using System.Collections.Generic;
using SpotbaseSharp.Model;

namespace SpotbaseSharp.Services
{
    public interface IDatabaseService
    {
        List<Spot> GetSpots();

        void SaveChanges(Spot selectedSpot);

        List<string> GetCitiesDistinct();

        void AddSpot(Spot spot);

        void RemoveSpot(Spot spot);

        List<Spot> GetSpotsByName(string name);

        List<Spot> GetSpotsByCity(string city);

        List<Spot> GetSpotsByType(List<SpotType> types);
    }
}
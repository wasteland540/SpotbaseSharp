using System.Collections.Generic;
using System.Linq;
using SpotbaseSharp.DataAccessLayer;
using SpotbaseSharp.Model;

namespace SpotbaseSharp.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDataAccessLayer _database;

        public DatabaseService(IDataAccessLayer database)
        {
            _database = database;
        }

        public List<Spot> GetSpots()
        {
            return _database.GetAll<Spot>().OrderBy(s => s.CreatedAt).Reverse().ToList();
        }

        public void SaveChanges(Spot selectedSpot)
        {
            _database.Update(selectedSpot);
        }

        public List<string> GetCitiesDistinct()
        {
            return _database.GetCitiesDistinct();
        }

        public void AddSpot(Spot spot)
        {
            _database.Insert(spot);
        }

        public void RemoveSpot(Spot spot)
        {
            _database.Delete(spot);
        }

        public List<Spot> GetSpotsByName(string name)
        {
            return _database.GetByName<Spot>(name);
        }

        public List<Spot> GetSpotsByCity(string city)
        {
            return _database.GetByCity(city);
        }

        public List<Spot> GetSpotsByType(List<SpotType> types)
        {
            return _database.GetByType(types.ToArray());
        }
    }
}
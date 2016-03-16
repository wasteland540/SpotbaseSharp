using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDatabase;
using NDatabase.Api;
using NDatabase.Api.Query;
using SpotbaseSharp.Model;

namespace SpotbaseSharp.DataAccessLayer.NDatabase
{
    public class NDatabaseConnector : IDataAccessLayer
    {
        private const string DatabaseName = "spotbaseSharp.db";
        private readonly IOdb _odb;

        public NDatabaseConnector()
        {
            _odb = OdbFactory.Open(DatabaseName);
        }

        public NDatabaseConnector(string directory)
        {
            string fileName = Path.Combine(directory, DatabaseName);

            _odb = OdbFactory.Open(fileName);
        }

        public void Insert<T>(T value) where T : class, IDbObject
        {
            _odb.Store(value);
        }

        public void Delete<T>(T value) where T : class, IDbObject
        {
            _odb.Delete(value);
        }

        public void Update<T>(T value) where T : class, IDbObject
        {
            _odb.Store(value);
        }

        public List<T> GetAll<T>() where T : class, IDbObject
        {
            var resultList = new List<T>();

            IQuery query = _odb.Query<T>();
            resultList.AddRange(query.Execute<T>());

            return resultList;
        }

        public T GetSingleByName<T>(string name) where T : class, IDbObject
        {
            var resultList = new List<T>();

            IQuery query = _odb.Query<T>();
            resultList.AddRange(query.Execute<T>().Where(record => record.Name == name));

            return resultList.FirstOrDefault();
        }

        public List<T> GetByName<T>(string name) where T : class, IDbObject
        {
            var resultList = new List<T>();

            IQuery query = _odb.Query<T>();
            resultList.AddRange(
                query.Execute<T>()
                    .Where(record => !string.IsNullOrEmpty(record.Name))
                    .Where(record => record.Name.ToLower().Contains(name.ToLower())));

            return resultList;
        }

        public List<Spot> GetByCity(string city)
        {
            var resultList = new List<Spot>();

            IQuery query = _odb.Query<Spot>();
            resultList.AddRange(
                query.Execute<Spot>()
                    .Where(record => !string.IsNullOrEmpty(record.City))
                    .Where(record => record.City.ToLower().Contains(city.ToLower())));

            return resultList;
        }

        public List<string> GetCitiesDistinct()
        {
            var resultList = new List<string>();

            IQuery query = _odb.Query<Spot>();
            IEnumerable<string> cities =
                query.Execute<Spot>()
                    .Select(record => record.City)
                    .Where(city => !string.IsNullOrEmpty(city))
                    .Distinct();
            resultList.AddRange(cities);

            return resultList;
        }

        public List<Spot> GetByType(SpotType[] types)
        {
            var resultList = new List<Spot>();

            IQuery query = _odb.Query<Spot>();
            resultList.AddRange(query.Execute<Spot>().Where(record => types.Contains(record.Type)));

            return resultList;
        }

        public void Close()
        {
            _odb.Close();
        }
    }
}
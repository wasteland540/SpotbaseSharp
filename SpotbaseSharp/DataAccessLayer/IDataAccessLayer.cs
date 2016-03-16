using System.Collections.Generic;
using SpotbaseSharp.Model;

namespace SpotbaseSharp.DataAccessLayer
{
    public interface IDataAccessLayer
    {
        void Insert<T>(T value) where T : class, IDbObject;

        void Delete<T>(T value) where T : class, IDbObject;

        void Update<T>(T value) where T : class, IDbObject;

        List<T> GetAll<T>() where T : class, IDbObject;

        T GetSingleByName<T>(string name) where T : class, IDbObject;

        List<T> GetByName<T>(string name) where T : class, IDbObject;

        List<Spot> GetByCity(string city);

        List<string> GetCitiesDistinct();

        List<Spot> GetByType(SpotType[] types);

        void Close();
    }
}
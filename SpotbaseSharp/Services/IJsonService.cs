using SpotbaseSharp.Model;

namespace SpotbaseSharp.Services
{
    public interface IJsonService
    {
        string ToJson(Spot[] spots);
    }
}
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using SpotbaseSharp.Model;

namespace SpotbaseSharp.Services
{
    public class JsonService : IJsonService
    {
        private readonly IImageService _imageService;

        public JsonService(IImageService imageService)
        {
            _imageService = imageService;
        }

        public string ToJson(Spot[] spots)
        {
            string jsonSpots = string.Empty;

            if (spots != null && spots.Length > 0)
            {
                List<JsonSpot> jsonSpotList = (from spot in spots
                    let mobileImage = _imageService.CreateMobileImage(spot.SmallFile)
                    select new JsonSpot
                    {
                        city = spot.City,
                        description = spot.Description,
                        name = spot.Name,
                        googleLink =
                            string.Format(SpotbaseConstants.RouteUrlTemplate,
                                spot.Lat.ToString(CultureInfo.InvariantCulture).Replace(",", "."),
                                spot.Lng.ToString(CultureInfo.InvariantCulture).Replace(",", ".")),
                        type = (short) spot.Type,
                        lat = spot.Lat,
                        lng = spot.Lng,
                        image = mobileImage
                    }).ToList();

                jsonSpots = JsonConvert.SerializeObject(jsonSpotList);
            }

            return jsonSpots;
        }
    }
}
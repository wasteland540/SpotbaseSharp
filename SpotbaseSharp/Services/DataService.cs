using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SpotbaseSharp.Model;
using SpotbaseSharp.Model.Serialization;

namespace SpotbaseSharp.Services
{
    public class DataService : IDataService
    {
        public enum FileType
        {
            Large,
            Small,
        }

        private readonly IDatabaseService _databaseService;
        private readonly IImageService _imageService;

        public DataService(IImageService imageService, IDatabaseService databaseService)
        {
            _imageService = imageService;
            _databaseService = databaseService;
        }

        public bool Import(string path, bool copyLargeFiles)
        {
            bool imported = false;

            try
            {
                IFormatter formatter = new BinaryFormatter();

                SerializedSpot[] spotList;

                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    spotList = formatter.Deserialize(stream) as SerializedSpot[];
                    stream.Close();
                }

                if (spotList != null)
                {
                    foreach (SerializedSpot serializedSpot in spotList)
                    {
                        Guid smallFile = _imageService.CopySmallFile(serializedSpot.SmallFile);

                        Guid largeFile = Guid.Empty;
                        if (copyLargeFiles && serializedSpot.LargeFile != null)
                        {
                            largeFile = _imageService.CopyLargeFile(serializedSpot.LargeFile);
                        }

                        var spot = new Spot
                        {
                            Name = serializedSpot.Name,
                            City = serializedSpot.City,
                            Description = serializedSpot.Description,
                            Type = serializedSpot.Type,
                            CreatedAt = DateTime.Now,
                            Lat = serializedSpot.Lat,
                            Lng = serializedSpot.Lng,
                            SmallFile = smallFile,
                            LargeFile = largeFile,
                        };

                        _databaseService.AddSpot(spot);
                    }

                    imported = true;
                }
            }
            catch (Exception)
            {
            }

            return imported;
        }

        public bool Export(string path, List<SelectableSpot> spots, out string errorMsg)
        {
            bool exported = false;

            try
            {
                List<SerializedSpot> exportList = spots.Select(spot => new SerializedSpot
                {
                    City = spot.City,
                    CreatedAt = spot.CreatedAt,
                    Description = spot.Description,
                    LargeFile = _imageService.LoadLarge(spot.LargeFile),
                    Lat = spot.Lat,
                    Lng = spot.Lng,
                    Name = spot.Name,
                    SmallFile = _imageService.LoadSmall(spot.SmallFile),
                    Type = spot.Type
                }).ToList();

                IFormatter formatter = new BinaryFormatter();

                using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, exportList.ToArray());
                    stream.Close();
                }

                exported = true;
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }

            errorMsg = null;

            return exported;
        }
    }
}
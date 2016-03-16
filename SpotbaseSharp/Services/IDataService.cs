using System.Collections.Generic;
using SpotbaseSharp.Model;

namespace SpotbaseSharp.Services
{
    public interface IDataService
    {
        bool Import(string path, bool copyLargeFiles);

        bool Export(string path, List<SelectableSpot> spots, out string errorMsg);
    }
}
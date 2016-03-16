using System.ComponentModel;

namespace SpotbaseSharp.Model
{
    public enum SpotType
    {
        [Description("")] NotSet,
        [Description("Curb")] Curb,
        [Description("Ledge")] Ledge,
        [Description("Rail")] Rail,
        [Description("Gap")] Gap,
        [Description("Park")] Park,
        [Description("Creative")] Creative,
    }
}
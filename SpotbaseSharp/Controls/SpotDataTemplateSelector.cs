using System.Windows;
using System.Windows.Controls;
using SpotbaseSharp.Model;

namespace SpotbaseSharp.Controls
{
    public class SpotDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SpotTemplate { get; set; }
        public DataTemplate ExportSpotTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var selectableSpot = item as SelectableSpot;

            if (selectableSpot != null)
            {
                return ExportSpotTemplate;
            }

            var spot = item as Spot;

            if (spot != null)
            {
                return SpotTemplate;
            }

            return null;
        }
    }
}
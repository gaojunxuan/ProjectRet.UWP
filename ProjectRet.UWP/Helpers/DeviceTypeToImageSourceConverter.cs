using ProjectRet.UWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace ProjectRet.UWP.Helpers
{
    public class DeviceTypeToImageSourceConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DeviceType)
            {
                var type = (DeviceType)value;
                switch(type)
                {
                    case DeviceType.Desktop: return new BitmapImage(new Uri("ms-appx:///Assets/desktop.png"));
                    case DeviceType.Laptop: return new BitmapImage(new Uri("ms-appx:///Assets/laptop.png"));
                    case DeviceType.Phone: return new BitmapImage(new Uri("ms-appx:///Assets/phone.png"));
                    case DeviceType.Tablet:return new BitmapImage(new Uri("ms-appx:///Assets/tablet.png"));
                    default: return new BitmapImage(new Uri("ms-appx:///Assets/desktop.png"));
                }
            }
            else
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/desktop.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

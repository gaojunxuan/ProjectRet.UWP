using ProjectRet.UWP.Helpers;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.RemoteSystems;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ProjectRet.UWP.Models
{
    public class DeviceDetails
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string UniqueId { get; set; }
        public string Credential { get; set; }
        public DeviceType Type { get; set; }
        [Ignore]
        public string StatusMessage { get; set; }
        [Ignore]
        public Brush StatusColor
        {
            get
            {
                if(StatusMessage== "Available".GetLocalized())
                {
                    return new SolidColorBrush(Color.FromArgb(255, 16, 124, 16));
                }
                else if(StatusMessage=="Unavailable".GetLocalized())
                {
                    return new SolidColorBrush(Color.FromArgb(255, 168, 0, 0));
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(255, 255, 185,0));
                }
            }
        }
        [Ignore]
        public bool Configured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Credential);
            }
        }
        [Ignore]
        public Visibility ShowCommandButton
        {
            get { return Configured ? Visibility.Visible : Visibility.Collapsed; }
        }
        [Ignore]
        public Visibility ShowConfigureButton
        {
            get { return !Configured ? Visibility.Visible : Visibility.Collapsed; }
        }
        [Ignore]
        public DeviceDetails Details
        {
            get
            {
                return this;
            }
        }
        [Ignore]
        public RemoteSystem RemoteSys { get; set; }
        [Ignore]
        public bool IsEnabled
        {
            get
            {
                if (StatusMessage == "Unavailable".GetLocalized() || Type==DeviceType.Phone)
                    return false;
                return true;
            }
        }
    }
}

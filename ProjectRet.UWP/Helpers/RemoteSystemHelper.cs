using ProjectRet.UWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.System.RemoteSystems;

namespace ProjectRet.UWP.Helpers
{
    public static class RemoteSystemHelper
    {
        public static DeviceType ConvertToDeviceType(string kind)
        {
            switch(kind)
            {
                case "Desktop": return DeviceType.Desktop; 
                case "Laptop": return DeviceType.Laptop;
                case "Phone": return DeviceType.Phone;
                case "Xbox": return DeviceType.Xbox;
                case "Tablet":return DeviceType.Tablet;
                default:return DeviceType.Unknown;
            }
        }
        public static string GetStatusMessage(RemoteSystemStatus status,string uniqueId)
        {
            if(status==RemoteSystemStatus.Available)
            {
                if (DatabaseHelper.Exists(uniqueId))
                    return "Available".GetLocalized();
                else
                    return "ConfigurationRequired".GetLocalized();       
            }
            else
            {
                return "Unavailable".GetLocalized();
            }
        }
        public static async Task<RemoteLaunchUriStatus> ExecuteCommand(RemoteSystem remoteSystem,Command comm)
        {
            if (remoteSystem != null)
            {
                return await RemoteLauncher.LaunchUriAsync(new RemoteSystemConnectionRequest(remoteSystem), new Uri(comm.ToString()));
            }
            else return RemoteLaunchUriStatus.RemoteSystemUnavailable;
        }
    }
}

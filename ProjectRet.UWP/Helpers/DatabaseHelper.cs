using ProjectRet.UWP.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ProjectRet.UWP.Helpers
{
    public static class DatabaseHelper
    {
        public static SQLiteConnection _conn = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "data.db"));
        public static bool Exists(string uniqueId)
        {
            _conn.CreateTable<DeviceDetails>();
            var res=_conn.Query<DeviceDetails>($"SELECT * FROM DeviceDetails WHERE UniqueId = '{uniqueId}'");
            return res.Count != 0;
        }
        public static string GetKey(string uniqueId)
        {
            _conn.CreateTable<DeviceDetails>();
            var res = _conn.Query<DeviceDetails>($"SELECT * FROM DeviceDetails WHERE UniqueId = '{uniqueId}'");
            if(res!=null&&res.Count!=0)
            {
                return res.First().Credential;
            }
            return "";
        }
        public static void Add(DeviceDetails item)
        {
            _conn.CreateTable<DeviceDetails>();
            _conn.Insert(item);
        }
        public static void Delete(string uniqueId)
        {
            var item = _conn.Query<DeviceDetails>($"SELECT * FROM DeviceDetails WHERE UniqueId = '{uniqueId}'");
            if (item != null && item.Count != 0)
            {
                _conn.Delete(item.First());
            }
        }
    }
}

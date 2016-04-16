using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace SAT.Util {

    /// <summary>
    /// USBデバイスの一覧を取得するためのクラス
    /// </summary>
    public class USBDeviceUtil {
        /// <summary>
        /// USBデバイス
        /// </summary>
        public class DeviceInfo {
            internal DeviceInfo(string deviceID, string pnpDeviceID, string description) {
                this.DeviceID = deviceID;
                this.PnpDeviceID = pnpDeviceID;
                this.Description = description;
            }
            /// <summary>
            /// デバイスID
            /// </summary>
            public string DeviceID { get; private set; }
            /// <summary>
            /// PnPデバイスID
            /// </summary>
            public string PnpDeviceID { get; private set; }
            /// <summary>
            /// 説明
            /// </summary>
            public string Description { get; private set; }
        }
        /// <summary>
        /// USBデバイスが接続されているかどうかを返す
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsDeviceConnected(string name) {
            return GetDevice(name) != null;
        }
        /// <summary>
        /// USBデバイスを返す
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DeviceInfo GetDevice(string name) {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub Where "
                + "DeviceID LIKE '" + name 
                + "' OR PNPDeviceID LIKE '" + name 
                + "' OR Description LIKE '" + name + "'")) {
                collection = searcher.Get();
            }
            if (collection.Count > 0) {
                foreach (var device in collection) {
                    return new DeviceInfo(
                        (string)device.GetPropertyValue("DeviceID"),
                        (string)device.GetPropertyValue("PNPDeviceID"),
                        (string)device.GetPropertyValue("Description")
                    );
                }
            }
            return null;
        }

        /// <summary>
        /// USBデバイス一覧を返す
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DeviceInfo> GetDevices() {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub")) {
                collection = searcher.Get();
            }

            foreach (var device in collection) {
                devices.Add(new DeviceInfo(
                    (string)device.GetPropertyValue("DeviceID"),
                    (string)device.GetPropertyValue("PNPDeviceID"),
                    (string)device.GetPropertyValue("Description")
                ));
            }

            collection.Dispose();
            return devices;
        }
    }
}

using System.Collections.Generic;
using DirectShowLib;

namespace SistemaMultimedia.Services.Camera
{
    public class DeviceService
    {
        public string[] GetVideoDevices()
        {
            var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            var list = new List<string>();
            foreach (var d in devices)
            {
                list.Add(d.Name);
            }
            return list.ToArray();
        }

        public string[] GetAudioDevices()
        {
            var devices = DsDevice.GetDevicesOfCat(FilterCategory.AudioInputDevice);
            var list = new List<string>();
            foreach (var d in devices)
            {
                list.Add(d.Name);
            }
            return list.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using PavoStudio.ExApi;
using OpenHardwareMonitor.Hardware;
using OpenHardwareMonitor.Hardware.Nic;
using System.Net.NetworkInformation;

namespace OpenHardwareMonitor
{
    class ExApiClient
    {
        public static void Start(string[] args)
        {
            int port = 10086;
            if (args != null && args.Length > 0 && int.TryParse(args[0], out port))
                ExClient.Instance.Start(port);
            else
                ExClient.Instance.Start();
        }

        public static void Stop()
        {
            ExClient.Instance.Stop();
        }

        public static void SyncToLive2DViewerEX(Computer computer)
        {
            if (!ExClient.Instance.IsAlive)
                return;

            ComputerEntity entity = new ComputerEntity();

            List<ComputerEntity.Hardware> list = new List<ComputerEntity.Hardware>();
            List<ComputerEntity.Sensor> sensorList = new List<ComputerEntity.Sensor>();
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                sensorList.Clear();
                IHardware hardware = computer.Hardware[i];

                ComputerEntity.Hardware hw = null;
                switch (hardware.HardwareType)
                {
                    case HardwareType.CPU:
                        hw = CreateHardware(hardware, ComputerEntity.HardwareType.CPU);
                        ISensor sensor = FindSensor(hardware, SensorType.Load, "CPU Total");
                        if (sensor != null)
                            sensorList.Add(CreateSensor(sensor, ComputerEntity.SensorType.Load));
                        sensor = FindSensor(hardware, SensorType.Temperature, null);
                        if (sensor != null)
                            sensorList.Add(CreateSensor(sensor, ComputerEntity.SensorType.Temperature));
                        hw.sensors = sensorList.ToArray();
                        break;
                    case HardwareType.RAM:
                        hw = CreateHardware(hardware, ComputerEntity.HardwareType.RAM);
                        sensor = FindSensor(hardware, SensorType.Load, "Memory");
                        if (sensor != null)
                            sensorList.Add(CreateSensor(sensor, ComputerEntity.SensorType.Load));
                        hw.sensors = sensorList.ToArray();
                        break;
                    case HardwareType.GpuAti:
                    case HardwareType.GpuNvidia:
                        hw = CreateHardware(hardware, ComputerEntity.HardwareType.GPU);

                        hw.subType = hardware.HardwareType == HardwareType.GpuAti ? ComputerEntity.GpuType.Ati : ComputerEntity.GpuType.Nivida;
                        sensor = FindSensor(hardware, SensorType.Load, "GPU Core");
                        if (sensor != null)
                            sensorList.Add(CreateSensor(sensor, ComputerEntity.SensorType.Load));
                        sensor = FindSensor(hardware, SensorType.Temperature, null);
                        if (sensor != null)
                            sensorList.Add(CreateSensor(sensor, ComputerEntity.SensorType.Temperature));
                        hw.sensors = sensorList.ToArray();
                        break;
                    case HardwareType.HDD:
                        hw = CreateHardware(hardware, ComputerEntity.HardwareType.HDD);
                        sensor = FindSensor(hardware, SensorType.Load, null);
                        if (sensor != null)
                            sensorList.Add(CreateSensor(sensor, ComputerEntity.SensorType.Load));
                        hw.sensors = sensorList.ToArray();
                        break;
                    case HardwareType.NIC:
                        if (!NicEx.IsAvailable(hardware))
                            break;

                        hw = CreateHardware(hardware, ComputerEntity.HardwareType.NIC);
                        NetworkInterfaceType nit = NicEx.GetNetworkInterfaceType(hardware);
                        hw.subType = nit == NetworkInterfaceType.Wireless80211 ? ComputerEntity.NicType.Wireless : ComputerEntity.NicType.Ethernet;

                        sensor = FindSensor(hardware, SensorType.InternetSpeed, "Upload Speed");
                        if (sensor != null)
                            sensorList.Add(CreateSensor(sensor, ComputerEntity.SensorType.NetUpSpeed));
                        sensor = FindSensor(hardware, SensorType.InternetSpeed, "Download Speed");
                        if (sensor != null)
                            sensorList.Add(CreateSensor(sensor, ComputerEntity.SensorType.NetDownSpeed));
                        hw.sensors = sensorList.ToArray();
                        break;
                }
                if (hw != null)
                    list.Add(hw);
            }

            entity.hardware = list.ToArray();
            RemoteMessage.Send(Msg.SyncResourceMonitor, entity);
        }

        static ComputerEntity.Hardware CreateHardware(IHardware hardware, int type)
        {
            ComputerEntity.Hardware h = new ComputerEntity.Hardware();
            h.type = type;
            h.name = hardware.Name;
            return h;
        }

        static ISensor FindSensor(IHardware hardware, SensorType type, string name) {
            for (int i = 0; i < hardware.Sensors.Length; i++)
            {
                ISensor sensor = hardware.Sensors[i];
                if (sensor.SensorType != type
                    || (name != null && sensor.Name != name)
                    || !sensor.Value.HasValue)
                    continue;

                return sensor;
            }

            return null;
        }

        static ComputerEntity.Sensor CreateSensor(ISensor sensor, int type)
        {
            ComputerEntity.Sensor s = new ComputerEntity.Sensor();
            s.name = sensor.Name;
            s.type = type;
            s.value = sensor.Value.Value;

            return s;
        }

        static bool IsMatch(string s, string[] filters) {
            if (filters == null)
                return true;

            foreach (string f in filters) {
                if (f == s)
                    return true;
            }

            return false;
        }
    }
}

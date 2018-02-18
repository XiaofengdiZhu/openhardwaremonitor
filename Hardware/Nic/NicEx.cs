using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;

namespace OpenHardwareMonitor.Hardware.Nic
{
    public class NicEx
    {
        public static bool IsAvailable(IHardware hardware)
        {
            NetworkInterface ni = (hardware as Nic).GetInterface();
            return ni != null
                && ni.OperationalStatus == OperationalStatus.Up
                && ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback;
        }

        public static NetworkInterfaceType GetNetworkInterfaceType(IHardware hardware)
        {
            return (hardware as Nic).GetInterface().NetworkInterfaceType;
        }
    }
}

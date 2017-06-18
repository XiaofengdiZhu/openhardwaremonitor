/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2012 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace OpenHardwareMonitor.Hardware.Nic
{
    internal class Nic : Hardware
    {
        private Sensor connectionSpeed;
        private Sensor dataUploaded;
        private Sensor dataDownloaded;
        private Sensor uploadSpeed;
        private Sensor downloadSpeed;
        private Sensor networkUtilization;
        private NetworkInterface nic;

        private long bytesUploaded;
        private long bytesDownloaded;

        public Nic(string name, ISettings settings, NetworkInterface NIC, int index)
          : base(name, new Identifier("NIC",index.ToString(CultureInfo.InvariantCulture)), settings)
        {
            nic = NIC;
            connectionSpeed = new Sensor("Connection Speed", 0, SensorType.InternetSpeed, this,
              settings);
            ActivateSensor(connectionSpeed);
            dataUploaded = new Sensor("Data Uploaded", 2, SensorType.Data, this,
              settings);
            ActivateSensor(dataUploaded);
            dataDownloaded = new Sensor("Data Downloaded", 3, SensorType.Data, this,
              settings);
            ActivateSensor(dataDownloaded);
            uploadSpeed = new Sensor("Upload Speed", 4, SensorType.InternetSpeed, this,
              settings);
            ActivateSensor(uploadSpeed);
            downloadSpeed = new Sensor("Download Speed", 5, SensorType.InternetSpeed, this,
              settings);
            ActivateSensor(downloadSpeed);
            networkUtilization = new Sensor("Network Utilization", 1, SensorType.Load, this,
              settings);
            ActivateSensor(networkUtilization);
            bytesUploaded = nic.GetIPv4Statistics().BytesSent;
            bytesDownloaded = nic.GetIPv4Statistics().BytesReceived;
            time = DateTime.Now;
        }

        public override HardwareType HardwareType
        {
            get
            {
                return HardwareType.NIC;
            }
        }
        private DateTime time;
        public override void Update()
        {
            DateTime newTime = DateTime.Now;
            float dt = (float)(newTime - time).TotalSeconds;
            time = newTime;
            IPv4InterfaceStatistics interfaceStats = nic.GetIPv4Statistics();
            connectionSpeed.Value = nic.Speed;
            uploadSpeed.Value = (float)(interfaceStats.BytesSent - bytesUploaded) / dt;
            downloadSpeed.Value = (float)(interfaceStats.BytesReceived - bytesDownloaded) / dt;
            networkUtilization.Value = Math.Max((float)uploadSpeed.Value, (float)downloadSpeed.Value)/ connectionSpeed.Value*800;
            bytesUploaded = interfaceStats.BytesSent;
            bytesDownloaded = interfaceStats.BytesReceived;
            dataUploaded.Value = ((float)bytesUploaded / 1073741824);
            dataDownloaded.Value = ((float)bytesDownloaded / 1073741824);
        }
    }
}

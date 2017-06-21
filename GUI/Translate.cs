using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace OpenHardwareMonitor.GUI
{
    public class Translate
    {
        public static string toChinese(ISensor sensor)
        {
            string result = sensor.Name;
            switch (sensor.SensorType)
            {
                case SensorType.Clock:
                    switch (sensor.Name)
                    {
                        case "Bus Speed": result = "总线频率"; break;
                        case "GPU Core": result = "GPU核心频率"; break;
                        case "GPU Memory": result = "显存频率"; break;
                        case "GPU Shader": result = "GPU着色器频率"; break;
                        default:
                            {
                                if (sensor.Name.StartsWith("CPU Core"))
                                {
                                    result = "CPU#" + Regex.Match(sensor.Name, @"\d+$") + " 频率";
                                }
                            }
                            break;
                    }
                    break;
                case SensorType.Temperature:
                    switch (sensor.Name)
                    {
                        case "CPU Core": result = "CPU核心温度"; break;
                        case "CPU Package": result = "CPU封装温度"; break;
                        case "GPU Core": result = "GPU核心温度"; break;
                        case "Temperature": result = "温度"; break;
                        default:
                            {
                                if (sensor.Name.StartsWith("CPU Core"))
                                {
                                    result = "CPU#" + Regex.Match(sensor.Name, @"\d+$") + " 温度";
                                }
                                if (sensor.Name.StartsWith("Temperature"))
                                {
                                    result = "温度#" + Regex.Match(sensor.Name, @"\d+$");
                                }
                                if (sensor.Name.StartsWith("Digital Sensor"))
                                {
                                    result = "数字温度传感器 " + Regex.Match(sensor.Name, @"\d+$");
                                }
                                if (sensor.Name.StartsWith("Analog Sensor"))
                                {
                                    result = "传感器集线器 " + Regex.Match(sensor.Name, @"\d+$");
                                }
                            }
                            break;
                    }
                    break;
                case SensorType.Load:
                    switch (sensor.Name) {
                        case "CPU Total": result = "CPU总使用率"; break;
                        case "Memory": result = "内存使用率"; break;
                        case "GPU Core": result = "GPU核心负载"; break;
                        case "GPU Memory Controller": result = "显存控制器负载"; break;
                        case "GPU Video Engine": result = "视频引擎负载"; break;
                        case "GPU Memory": result = "显存使用率"; break;
                        case "Used Space": result = "已用空间"; break;
                        case "Network Utilization": result = "网络利用率"; break;
                        default:
                            {
                                if (sensor.Name.StartsWith("CPU Core"))
                                {
                                    result = "CPU#" + Regex.Match(sensor.Name, @"\d+$") + " 使用率";
                                }
                            }
                            break;
                    }
                    break;
                case SensorType.Data:
                    switch (sensor.Name)
                    {
                        case "Used Memory": result = "已用内存"; break;
                        case "Available Memory": result = "可用内存"; break;
                        case "Total Bytes Written": result = "总写入量"; break;
                        case "Data Uploaded": result = "已上传"; break;
                        case "Data Downloaded": result = "已下载"; break;
                        case "Total Data Downloaded in Static": result = "历史总下载"; break;
                        case "Total Data Uploaded in Static": result = "历史总上传"; break;
                        case "Total Data Flowed in Static": result = "历史总流量"; break;
                    }
                    break;
                case SensorType.SmallData:
                    switch (sensor.Name)
                    {
                        case "GPU Memory Free": result = "可用显存"; break;
                        case "GPU Memory Used": result = "已用显存"; break;
                        case "GPU Memory Total": result = "总显存"; break;
                    }
                    break;
                case SensorType.Fan:
                    switch (sensor.Name)
                    {
                        case "GPU": result = "显卡风扇转速"; break;
                        default:
                            {
                                if (sensor.Name.StartsWith("Fan"))
                                {
                                    result = "风扇#" + Regex.Match(sensor.Name, @"\d+$") + " 转速";
                                }
                            }
                            break;
                    }
                    break;
                case SensorType.Control:
                    switch (sensor.Name)
                    {
                        case "GPU Fan": result = "显卡风扇转速(%)"; break;
                        default:
                            {
                                if (sensor.Name.StartsWith("Fan Control"))
                                {
                                    result = "风扇#" + Regex.Match(sensor.Name, @"\d+$") + " 转速(%)";
                                }
                                if (sensor.Name.StartsWith("Fan Chanel"))
                                {
                                    result = "风扇通道 " + Regex.Match(sensor.Name, @"\d+$");
                                }
                            }
                            break;
                    }
                    break;
                case SensorType.Power:
                    switch (sensor.Name)
                    {
                        case "CPU Package": result = "CPU总功率"; break;
                        case "CPU Cores": result = "CPU核心功率"; break;
                        case "CPU Graphics": result = "核显功率"; break;
                        case "CPU DRAM": result = "内存功率"; break;
                    }
                    break;
                case SensorType.Voltage:
                    switch (sensor.Name)
                    {
                        case "CPU VCore": result = "CPU电压"; break;
                        case "AVCC": result = "3.3V交流电压"; break;
                        case "3VCC": result = "3.3V主电压"; break;
                        case "3VSB": result = "3.3V待机电压"; break;
                        case "VBAT": result = "电池电压"; break;
                        case "VTT": result = "内存控制器电压"; break;
                        default:
                            {
                                if (sensor.Name.StartsWith("Voltage"))
                                {
                                    result = "电压#" + Regex.Match(sensor.Name, @"\d+$");
                                }
                            }
                            break;
                    }
                    break;
                case SensorType.Factor:
                    switch (sensor.Name)
                    {
                        case "Write Amplification": result = "写入放大"; break;
                    }
                    break;
                case SensorType.Flow:
                    switch (sensor.Name)
                    {
                        default:
                            {
                                if (sensor.Name.StartsWith("Flowmeter"))
                                {
                                    result = "流量计 " + Regex.Match(sensor.Name, @"\d+$");
                                }
                            }
                            break;
                    }
                    break;
                case SensorType.InternetSpeed:
                    switch (sensor.Name)
                    {
                        case "Connection Speed": result = "连接速度"; break;
                        case "Upload Speed": result = "上传速度"; break;
                        case "Download Speed": result = "下载速度"; break;
                    }
                    break;
            }
            return result;
        }
        public static string toChinese(String str)
        {
            String result = str;
            switch (str)
            {
                case "Voltages": result = "电压"; break;
                case "Voltage": result = "电压"; break;
                case "Temperature": result = "温度"; break;
                case "Temperatures": result = "温度";break;
                case "Fan": result = "风扇"; break;
                case "Fans": result = "风扇"; break;
                case "Control": result = "风扇控制器"; break;
                case "Controls": result = "风扇控制器"; break;
                case "Clock": result = "时钟"; break;
                case "Clocks": result = "时钟"; break;
                case "Load": result = "负载"; break;
                case "Power": result = "功耗"; break;
                case "Powers": result = "功耗"; break;
                case "Data": result = "数据"; break;
                case "Generic Hard Disk": result = "硬盘"; break;
                case "Generic Memory": result = "内存"; break;
                case "Flow": result = "流速"; break;
                case "Factor": result = "倍数"; break;
                case "Internet Speed": result = "网速"; break;
            }
            return result;
        }
    }
}

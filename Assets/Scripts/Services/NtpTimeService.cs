using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace GUIGUI17F
{
    public class NtpTimeService
    {
        private const string LogTag = "NtpTimeService";

        private static readonly string[] NtpServers =
        {
            "ntp.aliyun.com",
            "ntp1.aliyun.com",
            "ntp2.aliyun.com",
            "ntp3.aliyun.com",
            "ntp4.aliyun.com",
            "ntp5.aliyun.com",
            "ntp6.aliyun.com",
            "ntp7.aliyun.com"
        };

        public bool HasNtpTime { get; private set; }
        public DateTime UtcNow => HasNtpTime ? _ntpDateTime.AddSeconds(Time.realtimeSinceStartup - _dataReceivedTime) : DateTime.UtcNow;
        public DateTime Now => UtcNow.ToLocalTime();

        private DateTime _ntpDateTime;
        private float _dataReceivedTime;
        private int _timeoutMs;

        public async void Refresh(int timeoutMs)
        {
            _timeoutMs = timeoutMs;
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (await Task.Run(RefreshNtpTime))
                {
                    _dataReceivedTime = Time.realtimeSinceStartup;
                    HasNtpTime = true;
                }
            }
            else
            {
                Debug.unityLogger.LogError(LogTag, "internet isn't reachable, cancel NTP time update");
            }
        }

        /// <summary>
        /// force update the NTP time to the given DateTime, only use this for debug
        /// </summary>
        public void OverrideNtpTime(DateTime ntpDateTime)
        {
            _ntpDateTime = ntpDateTime;
            _dataReceivedTime = Time.realtimeSinceStartup;
        }

        private bool RefreshNtpTime()
        {
            byte[] ntpData = new byte[48];
            //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)
            ntpData[0] = 0x1B;
            bool success = false;
            for (int i = 0; i < NtpServers.Length; i++)
            {
                IPAddress[] addresses = Dns.GetHostEntry(NtpServers[i]).AddressList;
                for (int j = 0; j < addresses.Length; j++)
                {
                    //The UDP port number assigned to NTP is 123
                    IPEndPoint ipEndPoint = new IPEndPoint(addresses[j], 123);
                    try
                    {
                        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                        {
                            socket.Connect(ipEndPoint);
                            socket.ReceiveTimeout = _timeoutMs;
                            socket.Send(ntpData);
                            socket.Receive(ntpData);
                            socket.Close();
                        }
                        success = true;
                        break;
                    }
                    catch (Exception e)
                    {
                        Debug.unityLogger.LogError(LogTag, e.Message);
                    }
                }
                if (success)
                {
                    break;
                }
            }
            if (success)
            {
                ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
                ulong fractionPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];
                ulong milliseconds = intPart * 1000 + fractionPart * 1000 / 0x100000000L;
                _ntpDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)milliseconds);
                Debug.unityLogger.Log(LogTag, $"received NTP time: {_ntpDateTime:yyyy-MM-dd HH:mm:ss}");
            }
            else
            {
                Debug.unityLogger.LogError(LogTag, "fetch NTP time failed");
            }
            return success;
        }
    }
}
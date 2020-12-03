using System;
using System.Collections.Generic;
using SharpOSC;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace Muse { 

    public class OSCMonitor
    {
        private OSCReceiver _Receiver;

        public readonly int port;
        public readonly HandleMessage<MuseMessage> MuseHandle;
        public static float Time = 0.1f;


        // 构造函数
        public OSCMonitor(int port, HandleMessage<MuseMessage> MuseHandle)
        {
            _Receiver = new OSCReceiver();
            this.port = port;
            this.MuseHandle = MuseHandle;
        }

        // 从port端口接收OSCPacket，得到MuseMessage后使用MuseHandle处理
        public void ReceiveOSC(){

            _Receiver.StartOSCListener(this.port, this.OscPacketHandle);
        }

        public void ResetOSC()
        {
            _Receiver.ResetOSC();
        }

        // UDPListener调用OscPacketHandle处理OscPacket
        private async void OscPacketHandle(OscPacket packet)
        {
            //OSCBundle为OSCPacket的子类
            OscBundle bundle = (OscBundle)packet;

            //得到Packet的时间戳
            DateTime dt = SharpOSC.Utils.TimetagToDateTime(bundle.Timetag);

            foreach (OscMessage message in bundle.Messages)
            {
                MuseMessage StandardMessage = new MuseMessage(dt, message.Address, message.Arguments,MuseMessage.ReceiveType.MuseMonitor);
                //Debug.Log(StandardMessage.ToString());

                //Debug.Log(message.Address);

                // 此处另开线程处理得到的MuseMessage
                await Task.Run(
                    () => Callback(StandardMessage)
                    );
            }

        }

        // 得到MuseMessage后调用
        private void Callback(MuseMessage StandardMessage)
        {
            MuseHandle?.Invoke(StandardMessage);
        }


        // 获取本地IP
        public static List<string> GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> ips = new List<string>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ips.Add(ip.ToString());
                }
            }
            return ips;
        }
    }

}

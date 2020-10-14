using System;
using System.Collections.Generic;
using SharpOSC;
using System.Net;
using System.Net.Sockets;

namespace Muse { 

    public partial class OSCMonitor
    {
        private UDPListener listener;   //UDP监听器
        public Boolean stream = false; //当前是否正在传输
        public readonly int port = 5000;    //监听本地端口

         // 得到标准Muse数据后调用
        private Action<MuseMessage> _Callback;


        // 构造函数
        public OSCMonitor(int UDPPort, Action<MuseMessage> callback)
        {
            this.port = UDPPort;
            this._Callback = callback;
        }

        // 开始监听端口的UDP报文
        public void StartUDPLstener()
        {
            OSCMonitor_Load(new object(), new EventArgs());
        }

        // 监听端口
        private void OSCMonitor_Load(object sender, EventArgs e)
        {
            HandleOscPacket callback = (OscPacket packet ) => OscPacketHandle(packet);

            //传入 端口号port、回调函数callback，当UDP监听到数据时，将调用callback处理packet
            listener = new UDPListener(port, callback);
        }

        // UDPListener调用OscPacketHandle处理OscPacket
        private void OscPacketHandle(OscPacket packet)
        {
            //OSCBundle为OSCPacket的子类
            OscBundle bundle = (OscBundle)packet;

            //得到Packet的时间戳
            DateTime dt = SharpOSC.Utils.TimetagToDateTime(bundle.Timetag);


            foreach (OscMessage message in bundle.Messages)
            {
                // 得到标准格式的Muse数据
                MuseMessage StandardMessage = new MuseMessage(dt, message.Address, message.Arguments);
                //Console.WriteLine(StandardMessage.ToString());

                // 此处调用委托传出Muse数据
                _Callback?.Invoke(StandardMessage);
            }

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


        void OnApplicationQuit()
        {
            listener.Close();
        }

        void OnDestroy()
        {
            listener.Close();
        }
        public void Dispose()
        {
            listener.Close();
        }
    }

}

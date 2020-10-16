using SharpOSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
    public class OSCReceiver
    {
        // UDP监听器
        private UDPListener _Listener;


        /// <summary>
        /// 监听端口的OSCPaket
        /// </summary>
        /// <param name="port"></param>
        /// <param name="PacketHandle"></param>
        public void StartOSCListener(int port, HandleOscPacket PacketHandle)
        {
            ResetOSC();
            // 开始监听
            _Listener = new UDPListener(port, PacketHandle);
        }


        /// <summary>
        /// 泛型函数，使用HandleMessage类委托，处理T类型的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Message"></param>
        /// <param name="MuseHandle"></param>
        public void AnalyseMessage<T>(T Message, HandleMessage<T> MuseHandle)
        {
            MuseHandle(Message);
        }


        /// <summary>
        /// 释放UDP资源，防止占用端口
        /// </summary>
        public void ResetOSC()
        {
            // 释放连接
            if (_Listener != null)
            {
                _Listener.Dispose();
            }

        }
    }
}

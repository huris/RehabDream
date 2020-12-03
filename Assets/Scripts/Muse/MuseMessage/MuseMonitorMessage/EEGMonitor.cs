using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
    // Muse Monitor 发送的5通道原始脑电信号
    public class EEGMonitor : MuseMessage
    {

        // 当前原始脑电信号的值
        private readonly int _Channel = 5;

        // Muse Monitor 的数据
        public readonly List<float> RawEEG;



        public EEGMonitor(DateTime TimeStamp, MuseDataType DataType, List<object> Data, ReceiveType Receive) : base(TimeStamp, DataType, Data, Receive)
        {
            RawEEG = new List<float>();
            foreach (object num in Data)
            {
                RawEEG.Add((float)num);
            }
        }

        public EEGMonitor(DateTime TimeStamp, string StringDataType, List<object> Data, ReceiveType Receive) : base(TimeStamp, StringDataType, Data, Receive)
        {
            RawEEG = new List<float>();
            foreach (object num in Data)
            {
                RawEEG.Add((float)num);
            }
        }

        // 由父类构造子类
        public EEGMonitor(MuseMessage muse) : base(muse.TimeStamp, muse.DataType, muse.Data, muse.Receive)
        {
            RawEEG = new List<float>();
            foreach (object num in Data)
            {
                RawEEG.Add((float)num);
            }
        }
    }
}

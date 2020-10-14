using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
    // 5通道原始脑电信号
    public class EEGMessage : MuseMessage
    {

        // 当前原始脑电信号的值
        private readonly int _Channel = 5;
        public readonly List<float> RawEEG;


        public EEGMessage(DateTime TimeStamp, MuseDataType DataType, List<object> Data) : base(TimeStamp, DataType, Data)
        {
            RawEEG = new List<float>();
            foreach (object num in Data)
            {
                RawEEG.Add((float)num);
            }
        }

        public EEGMessage(DateTime TimeStamp, string StringDataType, List<object> Data) : base(TimeStamp, StringDataType, Data)
        {
            RawEEG = new List<float>();
            foreach (object num in Data)
            {
                RawEEG.Add((float)num);
            }
        }

        // 由父类构造子类
        public EEGMessage(MuseMessage muse) : base(muse.TimeStamp, muse.DataType, muse.Data)
        {
            RawEEG = new List<float>();
            foreach (object num in this.Data)
            {
                RawEEG.Add((float)num);
            }
        }
    }
}

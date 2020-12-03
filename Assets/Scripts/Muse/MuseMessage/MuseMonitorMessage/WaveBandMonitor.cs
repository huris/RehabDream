using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
    // 单一波段数据
    public class WaveBandMonitor : MuseMessage
    {
        // 当波段的值
        public float WaveData;

        public WaveBandMonitor():base()
        {
            WaveData = default(float);
        }

        public WaveBandMonitor(DateTime TimeStamp, MuseDataType DataType, List<object> Data, ReceiveType Receive) : base(TimeStamp, DataType, Data, Receive)
        {
            WaveData = (float)(Data[0]);
        }

        public WaveBandMonitor(DateTime TimeStamp, string StringDataType, List<object> Data, ReceiveType Receive) : base(TimeStamp, StringDataType, Data, Receive)
        {
            WaveData = (float)(Data[0]);
        }
        public WaveBandMonitor(MuseMessage muse) : base(muse.TimeStamp, muse.DataType, muse.Data, muse.Receive)
        {
            WaveData = (float)(this.Data[0]);

        }

    }


}

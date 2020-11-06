using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
    // 单一波段数据
    public class WaveBandMessage : MuseMessage
    {
        // 当波段的值
        public float WaveData;

        public WaveBandMessage():base()
        {
            WaveData = (float)(0);
        }

        public WaveBandMessage(DateTime TimeStamp, MuseDataType DataType, List<object> Data) : base(TimeStamp, DataType, Data)
        {
            WaveData = (float)(Data[0]);
        }

        public WaveBandMessage(DateTime TimeStamp, string StringDataType, List<object> Data) : base(TimeStamp, StringDataType, Data)
        {
            WaveData = (float)(Data[0]);
        }
        public WaveBandMessage(MuseMessage muse) : base(muse.TimeStamp, muse.DataType, muse.Data)
        {
            WaveData = (float)(this.Data[0]);

        }




    }


}

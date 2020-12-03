using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
    // 从Muse Direct 收到的单一波段数据
    public class WaveBandDirect : MuseMessage
    {
        // 当波段的值
        public double WaveData;

        public WaveBandDirect() : base()
        {
            WaveData = default(double);
        }

        public WaveBandDirect(DateTime TimeStamp, MuseDataType DataType, List<object> Data, ReceiveType Receive) : base(TimeStamp, DataType, Data, Receive)
        {
            int i = 0;
            double sum = 0;
            foreach (object num in Data)
            {
                sum += (double)(num);
                i++;
            }

            if (i == 0)
            {
                WaveData = 0;
            }
            else
            {
                WaveData = sum / i;
            }
        }

        public WaveBandDirect(DateTime TimeStamp, string StringDataType, List<object> Data, ReceiveType Receive) : base(TimeStamp, StringDataType, Data, Receive)
        {
            int i = 0;
            double sum = 0;
            foreach (object num in Data)
            {
                sum += (double)(num);
                i++;
            }

            if (i == 0)
            {
                WaveData = 0;
            }
            else
            {
                WaveData = sum / i;
            }
        }
        public WaveBandDirect(MuseMessage muse) : base(muse.TimeStamp, muse.DataType, muse.Data, muse.Receive)
        {
            int i = 0;
            double sum = 0;
            foreach (object num in muse.Data)
            {
                sum += (double)(num);
                i++;
            }

            if (i == 0)
            {
                WaveData = 0;
            }
            else
            {
                WaveData = sum / i;
            }

        }

    }


}

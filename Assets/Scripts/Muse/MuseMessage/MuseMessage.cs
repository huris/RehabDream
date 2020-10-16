using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse
{
    // 单条Muse头环传输的信息
    public class MuseMessage
    {
        // 信息的时间戳
        public readonly DateTime TimeStamp ;
        // 信息的类型
        public readonly MuseDataType DataType;
        // 信息的数值
        public readonly List<object> Data;

        // 构造函数
        public MuseMessage()
        {
            TimeStamp = DateTime.Now;
            DataType = MuseDataType.DefaultType;
            Data = new List<object>();
        }

        // 构造函数
        public MuseMessage(DateTime TimeStamp, MuseDataType DataType, List<object> Data)
        {
            this.TimeStamp = TimeStamp;
            this.DataType = DataType;
            this.Data = Data;
        }

        // 构造函数
        public MuseMessage(DateTime TimeStamp, string StringDataType, List<object> Data)
        {
            this.TimeStamp = TimeStamp;
            this.DataType = String2DataType(StringDataType);
            this.Data = Data;
        }

        // Muse信息的类型
        public enum MuseDataType
        {
            // 五种波段
            alpha_absolute,
            beta_absolute,
            gamma_absolute,
            theta_absolute,
            delta_absolute,
            // 脑电原始信号
            eeg,
            // 陀螺仪
            gyro,
            // 加速度
            acc,
            //
            horseshoe,
            //
            touching_forehead,
            //
            batt,
            // 是否眨眼
            blink,
            // 
            jaw_clench,

            DefaultType,
        }

        // 是否为5种脑波
        public bool IsWaveBand()
        {
            return (this.DataType == MuseDataType.alpha_absolute) ||
                    (this.DataType == MuseDataType.beta_absolute) ||
                    (this.DataType == MuseDataType.gamma_absolute) ||
                    (this.DataType == MuseDataType.theta_absolute) ||
                    (this.DataType == MuseDataType.delta_absolute);
        }


        //将信息的类型由字符串转为枚举类型
        public static MuseDataType String2DataType(string RawName)
        {
            string[] temp = RawName.Split('/');

            string name = temp.Last();

            switch (name) {
                case "eeg":
                    return MuseDataType.eeg;
                    break;
                case "gyro":
                    return MuseDataType.gyro;
                    break;
                case "acc":
                    return MuseDataType.acc;
                    break;
                case "alpha_absolute":
                    return MuseDataType.alpha_absolute;
                    break;
                case "beta_absolute":
                    return MuseDataType.beta_absolute;
                    break;
                case "delta_absolute":
                    return MuseDataType.delta_absolute;
                    break;
                case "theta_absolute":
                    return MuseDataType.theta_absolute;
                    break;
                case "gamma_absolute":
                    return MuseDataType.gamma_absolute;
                    break;
                case "horseshoe":
                    return MuseDataType.horseshoe;
                    break;
                case "touching_forehead":
                    return MuseDataType.touching_forehead;
                    break;
                case "batt":
                    return MuseDataType.batt;
                    break;
                case "blink":
                    return MuseDataType.blink;
                    break;
                case "jaw_clench":
                    return MuseDataType.jaw_clench;
                    break;
                default:
                    Console.WriteLine("No Such Muse Type!");
                    return MuseDataType.DefaultType;
            }
        }

        // 将枚举转为字符串
        public static string DataType2String(MuseDataType Datatype)
        {
            return Enum.GetName(typeof(MuseDataType), Datatype);
        }

        //覆写ToString方法
        public override string ToString()
        {
            // 展示数据
            String text;
            text = this.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff tt") + " UTC";

            text += "," + DataType2String(this.DataType);  //即Packet包含的信息类型：/muse/eeg 或 //muse/gyro 或 /muse/acc 等

            if (this.Data.Count > 0)    //得到数据
            {
                text += "," + Data.First().ToString();
            }
            else
            {
                //不可能运行至此
            }

            if (this.Data.Count > 1)
            {
                foreach (object obj in this.Data.Skip(1))
                {
                    text += "," + obj.ToString();
                }
            }

            return text;

        }



    }
}

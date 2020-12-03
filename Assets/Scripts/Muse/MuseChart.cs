using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muse;
using XCharts;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;

// 存储单个波段的折线图信息
class SingleWaveBandChart
{
    // 该波段类型
    MuseMessage.MuseDataType Type = MuseMessage.MuseDataType.DefaultType;

    // 当前波段的值
    float NowValue = 0f;

    // 该波段对应的折线
    Serie LineSerie = null;

    // 更新折线图的间隔（太小会导致Unity崩溃）
    // UpdateInterval = 10 表示 每收到10个数据点，则更新最后1个
    int UpdateInterval = 4;

    private int _Count = 0;

    //构造函数
    public SingleWaveBandChart(MuseMessage.MuseDataType Type, Serie LineSerie, float NowValue=0, int UpdateInterval=10)
    {
        this.Type = Type;
        this.NowValue = NowValue;
        this.LineSerie = LineSerie;
        this.UpdateInterval = UpdateInterval;
    }

    // 更新波段折线图
    public void UpdateSerie(float Value)
    {
        if (_Count != UpdateInterval)
        {
            _Count++;
            return;
        }
        else
        {
            LineSerie.AddYData(Value); // 由Bels 转为 deciBels（分贝）
            //Debug.Log("Update " + MuseMessage.DataType2String(Type));
            _Count = 0;
        }
    }
}


public class MuseChart : MonoBehaviour {
	
    // 单个折线图最多同时展示多少数据
    public int maxCacheDataNumber = 50;
    public GameObject LineChart;
    //public float FatigueUpdateTime = 0.1f;

    public CoordinateChart WaveBandChart;
    //public Text Fatigue;
    public OSCMonitor oscM;
    public OSCDirect oscD;

    // 数据来源
    public static MuseMessage.ReceiveType receive = MuseMessage.ReceiveType.MuseMonitor;

    private Dictionary<MuseMessage.MuseDataType, SingleWaveBandChart> _WaveGraph;
    private string _FileName;
    private Dictionary<MuseMessage.MuseDataType, WaveBandMonitor> _BandValues;
    private float TimeCout = 0f;
    private bool IsTransmit = false;

    // 初始化折线图
    private void InitWaveBandChart()
    {
        _WaveGraph = new Dictionary<MuseMessage.MuseDataType, SingleWaveBandChart>();

        WaveBandChart.RemoveData();

        _WaveGraph.Add(
            MuseMessage.MuseDataType.alpha_absolute,
            new SingleWaveBandChart(MuseMessage.MuseDataType.alpha_absolute, WaveBandChart.AddSerie(SerieType.Line, "Alpha"))
            );

        _WaveGraph.Add(
            MuseMessage.MuseDataType.beta_absolute, 
            new SingleWaveBandChart(MuseMessage.MuseDataType.beta_absolute, WaveBandChart.AddSerie(SerieType.Line, "Beta"))
            );

        _WaveGraph.Add
            (MuseMessage.MuseDataType.gamma_absolute, 
            new SingleWaveBandChart(MuseMessage.MuseDataType.gamma_absolute, WaveBandChart.AddSerie(SerieType.Line, "Gamma"))
            );

        _WaveGraph.Add(
            MuseMessage.MuseDataType.delta_absolute, 
            new SingleWaveBandChart(MuseMessage.MuseDataType.delta_absolute, WaveBandChart.AddSerie(SerieType.Line, "Delta"))
            );

        _WaveGraph.Add(
            MuseMessage.MuseDataType.theta_absolute, 
            new SingleWaveBandChart(MuseMessage.MuseDataType.theta_absolute, WaveBandChart.AddSerie(SerieType.Line, "Theta"))
            );


        WaveBandChart.legend.show = true;
        WaveBandChart.SetMaxCache(maxCacheDataNumber);

    }



    void Awake()
    {
        InitWaveBandChart();

        _BandValues = new Dictionary<MuseMessage.MuseDataType, WaveBandMonitor> {
            { MuseMessage.MuseDataType.alpha_absolute, new WaveBandMonitor()},
            {MuseMessage.MuseDataType.beta_absolute, new WaveBandMonitor()},
            {MuseMessage.MuseDataType.gamma_absolute,new WaveBandMonitor()},
            {MuseMessage.MuseDataType.delta_absolute,new WaveBandMonitor()},
            {MuseMessage.MuseDataType.theta_absolute,new WaveBandMonitor()}
        };

        string temp;
        if(PatientDataManager.instance == null)
        {
            temp = "PatientName";
        }
        else
        {
            temp = PatientDataManager.instance.PatientName;
        }

        _FileName = "Data/" + temp + ".csv";


        // 接收UDP
        if(receive == MuseMessage.ReceiveType.MuseMonitor)
        {
            oscM = new OSCMonitor(5000, UpdateWaveBandChartMonitor);
            oscM.ReceiveOSC();
        }
        else
        {
            oscD = new OSCDirect(5000, UpdateWaveBandChartDirect);
            oscD.ReceiveOSC();
        }


    }

    void Update()
    {

        TimeCout += Time.deltaTime;
        //if (TimeCout > FatigueUpdateTime)
        //{
        //    TimeCout = 0f;

        //    // 疲劳值
        //    if ((_BandValues[MuseMessage.MuseDataType.beta_absolute].WaveData - 0) > float.Epsilon)
        //    {
        //        // (Alpha + Thelta) / Belta
        //        Fatigue.text = "Fatigue: " + (
        //            (_BandValues[MuseMessage.MuseDataType.alpha_absolute].WaveData +
        //            _BandValues[MuseMessage.MuseDataType.theta_absolute].WaveData) /
        //            _BandValues[MuseMessage.MuseDataType.beta_absolute].WaveData).ToString(".#2");
        //    }
        //}

        if (LineChart.activeSelf == false && IsTransmit == true)
        {
            LineChart.SetActive(true);
        }

    }



    public async void UpdateWaveBandChartMonitor(MuseMessage StandardMessage)
    {
        IsTransmit = true;
        if (StandardMessage.IsWaveBand())
        {

            if (receive == MuseMessage.ReceiveType.MuseMonitor)
            {
                // 由父类构造子类
                WaveBandMonitor WaveBand = new WaveBandMonitor(StandardMessage);
                // 更新对应波段的折线图
                _WaveGraph[WaveBand.DataType].UpdateSerie(WaveBand.WaveData);

                //记录波段值
                _BandValues[WaveBand.DataType].WaveData = WaveBand.WaveData;
            }
            else
            {
                Debug.LogError("No such Muse data!");
            }
        }
    

        await Task.Run(
                    () => WriteCSVString(StandardMessage)
                    );


    }


    public async void UpdateWaveBandChartDirect(MuseMessage StandardMessage)
    {
        IsTransmit = true;
        if (StandardMessage.IsWaveBand())
        {

            // 数据来自Muse Monitor
            if (receive == MuseMessage.ReceiveType.MuseDirect)
            {

                // 由父类构造子类
                WaveBandDirect WaveBand = new WaveBandDirect(StandardMessage);

                Debug.Log(WaveBand.DataType + WaveBand.WaveData.ToString());

                // 更新对应波段的折线图
                _WaveGraph[WaveBand.DataType].UpdateSerie((float)WaveBand.WaveData);
                //记录波段值
                _BandValues[WaveBand.DataType].WaveData = (float)WaveBand.WaveData;
            }// 数据来自Muse Direct
           
            else
            {
                Debug.LogError("No such Muse data!");
            }
        }
        await Task.Run(
                    () => WriteCSVString(StandardMessage)
                    );


    }

    public void WriteCSVString(MuseMessage StandardMessage)
    {
        //print(GameData.current_user_id.ToString());
        CSVUtil.WriteCSVString("Data/" + GameData.current_user_id.ToString() + ".csv", true, StandardMessage.ToString());
    }


    void OnApplicationQuit()
    {
        Reset();
    }

    public void Reset()
    {
        oscD?.ResetOSC();
        oscM?.ResetOSC();
    }


}

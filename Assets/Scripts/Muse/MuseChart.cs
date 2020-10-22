using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muse;
using XCharts;
using System;
using System.Threading.Tasks;

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

    public CoordinateChart WaveBandChart;
    public OSCMonitor osc;

    private Dictionary<MuseMessage.MuseDataType, SingleWaveBandChart> _WaveBands;
    private string _FileName;




    private float updateTime;
    private DateTime timeNow;

    // 初始化折线图
    private void InitWaveBandChart()
    {
        _WaveBands = new Dictionary<MuseMessage.MuseDataType, SingleWaveBandChart>();

        WaveBandChart.RemoveData();

        _WaveBands.Add(
            MuseMessage.MuseDataType.alpha_absolute,
            new SingleWaveBandChart(MuseMessage.MuseDataType.alpha_absolute, WaveBandChart.AddSerie(SerieType.Line, "Alpha"))
            );

        _WaveBands.Add(
            MuseMessage.MuseDataType.beta_absolute, 
            new SingleWaveBandChart(MuseMessage.MuseDataType.beta_absolute, WaveBandChart.AddSerie(SerieType.Line, "Beta"))
            );

        _WaveBands.Add
            (MuseMessage.MuseDataType.gamma_absolute, 
            new SingleWaveBandChart(MuseMessage.MuseDataType.gamma_absolute, WaveBandChart.AddSerie(SerieType.Line, "Gamma"))
            );

        _WaveBands.Add(
            MuseMessage.MuseDataType.delta_absolute, 
            new SingleWaveBandChart(MuseMessage.MuseDataType.delta_absolute, WaveBandChart.AddSerie(SerieType.Line, "Delta"))
            );

        _WaveBands.Add(
            MuseMessage.MuseDataType.theta_absolute, 
            new SingleWaveBandChart(MuseMessage.MuseDataType.theta_absolute, WaveBandChart.AddSerie(SerieType.Line, "Theta"))
            );
        

        WaveBandChart.legend.show = true;
        WaveBandChart.legend.itemWidth = 15f;
        WaveBandChart.SetMaxCache(maxCacheDataNumber);

    }



    void Awake()
    {
        InitWaveBandChart();
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
        osc = new OSCMonitor(5000, UpdateWaveBandChart);
        osc.ReceiveOSC();

    }

    void Update()
    {

       
    }



    public async void UpdateWaveBandChart(MuseMessage StandardMessage)
    {

        if (StandardMessage.IsWaveBand())
        {
            // 由父类构造子类
            WaveBandMessage WaveBand = new WaveBandMessage(StandardMessage);

            // 更新对应波段的折线图
            _WaveBands[WaveBand.DataType].UpdateSerie(WaveBand.WaveData);
        }
    

        await Task.Run(
                    () => WriteCSVString(StandardMessage)
                    );

    }

    public void WriteCSVString(MuseMessage StandardMessage)
    {
        CSVUtil.WriteCSVString(_FileName, true, StandardMessage.ToString());
    }


    void OnApplicationQuit()
    {
        osc.ResetOSC();
    }


}

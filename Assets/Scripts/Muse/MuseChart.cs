using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muse;
using XCharts;
using System;

public class MuseChart : MonoBehaviour {
	

    public int maxCacheDataNumber = 50;

    public CoordinateChart WaveBandChart;
    public OSCMonitor osc;


    private Dictionary<MuseMessage.MuseDataType, bool> IsUpdate = 
        new Dictionary<MuseMessage.MuseDataType, bool> {
            {MuseMessage.MuseDataType.alpha_absolute, false},
            { MuseMessage.MuseDataType.beta_absolute,false},
            { MuseMessage.MuseDataType.gamma_absolute,false},
            { MuseMessage.MuseDataType.theta_absolute,false},
            { MuseMessage.MuseDataType.delta_absolute,false},
        };

    private Dictionary<MuseMessage.MuseDataType, float> WaveBandValue =
        new Dictionary<MuseMessage.MuseDataType, float> {
            {MuseMessage.MuseDataType.alpha_absolute, 0f},
            { MuseMessage.MuseDataType.beta_absolute,0f},
            { MuseMessage.MuseDataType.gamma_absolute,0f},
            { MuseMessage.MuseDataType.theta_absolute,0f},
            { MuseMessage.MuseDataType.delta_absolute,0f},
        };


    private float updateTime;
    private DateTime timeNow;

    private void InitWaveBandChart()
    {
        WaveBandChart.RemoveData();
        WaveBandChart.AddSerie(SerieType.Line, "Alpha");
        WaveBandChart.AddSerie(SerieType.Line, "Beta");
        WaveBandChart.AddSerie(SerieType.Line, "Delta");
        WaveBandChart.AddSerie(SerieType.Line, "Theta");
        WaveBandChart.AddSerie(SerieType.Line, "Gamma");
        WaveBandChart.legend.show = true;
        WaveBandChart.legend.itemWidth = 15f;
        WaveBandChart.SetMaxCache(maxCacheDataNumber);
    }

    private bool AllUpdate()
    {
        if (IsUpdate.ContainsValue(false))
        {
            return false;
        }
        else
        {
            return true;
        }
    }



    void Awake()
    {
        InitWaveBandChart();
        osc = new OSCMonitor(5000, UpdateWaveBandChart);
        osc.StartUDPLstener();
    }

    void Update()
    {

        //updateTime += Time.deltaTime;


        //if (updateTime > 0.1f)
        //{
        //    updateTime = 0;
            
        //    float value = UnityEngine.Random.Range(60, 150);
        //    WaveBandChart.AddData(0, value+11);
        //    WaveBandChart.AddData(1, value+22);
        //    WaveBandChart.AddData(2, value+99);
        //    WaveBandChart.AddData(3, value+88);
        //    WaveBandChart.AddData(4, value+33);



        //    WaveBandChart.RefreshChart();
        //}
    }



    public void UpdateWaveBandChart(MuseMessage StandardMessage)
    {
        // 由父类构造子类
        WaveBandMessage WaveBand = new WaveBandMessage(StandardMessage);



        WaveBandChart.AddData((int)WaveBand.DataType, WaveBand.WaveData);

    }

    private void UpdateWaveBandChart()
    {
        WaveBandChart.AddData(0, WaveBandValue[MuseMessage.MuseDataType.alpha_absolute]);
        WaveBandChart.AddData(1, WaveBandValue[MuseMessage.MuseDataType.beta_absolute]);
        WaveBandChart.AddData(2, WaveBandValue[MuseMessage.MuseDataType.gamma_absolute]);
        WaveBandChart.AddData(3, WaveBandValue[MuseMessage.MuseDataType.theta_absolute]);
        WaveBandChart.AddData(4, WaveBandValue[MuseMessage.MuseDataType.delta_absolute]);

        IsUpdate[MuseMessage.MuseDataType.alpha_absolute] = false;
        IsUpdate[MuseMessage.MuseDataType.beta_absolute] = false;
        IsUpdate[MuseMessage.MuseDataType.gamma_absolute] = false;
        IsUpdate[MuseMessage.MuseDataType.theta_absolute] = false;
        IsUpdate[MuseMessage.MuseDataType.delta_absolute] = false;
    }

}

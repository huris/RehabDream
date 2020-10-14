using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Muse;
using XCharts;
using System;

public class MuseChart : MonoBehaviour {
	

    public int maxCacheDataNumber = 100;
    public float initDataTime = 2;

    private CoordinateChart WaveBandChart;
    private float updateTime;
    private float initTime;
    private int initCount;
    private int count;
    private bool isInited;
    private DateTime timeNow;

    void Awake()
    {
        // 获取并清空全部数据
        WaveBandChart = gameObject.GetComponentInChildren<CoordinateChart>();
        //WaveBandChart.RemoveData();

        // 五条折线图表示五个波段
        var AlphaSerie = WaveBandChart.AddSerie(SerieType.Line);
        var BetaSerie = WaveBandChart.AddSerie(SerieType.Line);
        var DeltaSerie = WaveBandChart.AddSerie(SerieType.Line);
        var ThetaSerie = WaveBandChart.AddSerie(SerieType.Line);
        var GammaSerie = WaveBandChart.AddSerie(SerieType.Line);




        AlphaSerie.symbol.show = true;
        BetaSerie.symbol.show = true;
        DeltaSerie.symbol.show = true;
        ThetaSerie.symbol.show = true;
        GammaSerie.symbol.show = true;




        // 最多同时显示多少数据
        WaveBandChart.xAxises[0].maxCache = maxCacheDataNumber;
        timeNow = DateTime.Now;
        timeNow = timeNow.AddSeconds(-maxCacheDataNumber);
    }

    void Update()
    {

        updateTime += Time.deltaTime;
        if (updateTime >= 1)
        {
            updateTime = 0;
            count++;
            string category = DateTime.Now.ToString("hh:mm:ss");
            float value = UnityEngine.Random.Range(60, 150);
            WaveBandChart.AddXAxisData(category);
            
            WaveBandChart.AddData(0, value+11);
            WaveBandChart.AddData(1, value+22);
            WaveBandChart.AddData(2, value+99);
            WaveBandChart.AddData(3, value+88);
            WaveBandChart.AddData(4, value+33);



            WaveBandChart.RefreshChart();
        }
    }



    public void UpdateWaveBandChart(MuseMessage StandardMessage)
    {
        // 由父类构造子类
        WaveBandMessage WaveBand = new WaveBandMessage(StandardMessage);
    }
}

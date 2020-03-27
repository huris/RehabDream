/******************************************/
/*                                        */
/*     Copyright (c) 2018 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XCharts
{
    public partial class CoordinateChart
    {
        protected float m_BarLastOffset = 0;

        protected void DrawYBarSerie(VertexHelper vh, Serie serie, int colorIndex, ref List<float> seriesHig)
        {
            if (!IsActive(serie.name)) return;
            if (serie.animation.HasFadeOut()) return;
            var xAxis = m_XAxises[serie.axisIndex];
            var yAxis = m_YAxises[serie.axisIndex];
            if (!yAxis.show) yAxis = m_YAxises[(serie.axisIndex + 1) % m_YAxises.Count];

            var showData = serie.GetDataList(m_DataZoom);
            float categoryWidth = yAxis.GetDataWidth(coordinateHeight, showData.Count, m_DataZoom);
            float barGap = GetBarGap();
            float totalBarWidth = GetBarTotalWidth(categoryWidth, barGap);
            float barWidth = serie.GetBarWidth(categoryWidth);
            float offset = (categoryWidth - totalBarWidth) / 2;
            float barGapWidth = barWidth + barWidth * barGap;
            float space = serie.barGap == -1 ? offset : offset + m_BarLastOffset;


            int maxCount = serie.maxShow > 0 ?
                (serie.maxShow > showData.Count ? showData.Count : serie.maxShow)
                : showData.Count;
            if (seriesHig.Count < serie.minShow)
            {
                for (int i = 0; i < serie.minShow; i++)
                {
                    seriesHig.Add(0);
                }
            }
            var isPercentStack = m_Series.IsPercentStack(serie.stack, SerieType.Bar);
            bool dataChanging = false;
            float dataChangeDuration = serie.animation.GetUpdateAnimationDuration();
            float xMinValue = xAxis.GetCurrMinValue(dataChangeDuration);
            float xMaxValue = xAxis.GetCurrMaxValue(dataChangeDuration);

            for (int i = serie.minShow; i < maxCount; i++)
            {
                if (i >= seriesHig.Count)
                {
                    seriesHig.Add(0);
                }
                var serieData = showData[i];
                if (serie.IsIgnoreValue(serieData.GetData(1)))
                {
                    serie.dataPoints.Add(Vector3.zero);
                    continue;
                }
                var highlight = (m_Tooltip.show && m_Tooltip.IsSelected(i))
                    || serie.data[i].highlighted
                    || serie.highlighted;
                var itemStyle = SerieHelper.GetItemStyle(serie, serieData, highlight);
                var borderWidth = itemStyle.runtimeBorderWidth;
                serieData.canShowLabel = true;
                float value = showData[i].GetCurrData(1, dataChangeDuration);
                if (showData[i].IsDataChanged()) dataChanging = true;
                float pX = seriesHig[i] + coordinateX + xAxis.runtimeZeroXOffset + yAxis.axisLine.width;
                float pY = coordinateY + +i * categoryWidth;
                if (!yAxis.boundaryGap) pY -= categoryWidth / 2;

                var barHig = 0f;
                var valueTotal = 0f;
                if (isPercentStack)
                {
                    valueTotal = GetSameStackTotalValue(serie.stack, i);
                    barHig = valueTotal != 0 ? (value / valueTotal * coordinateWidth) : 0;
                    seriesHig[i] += barHig;
                }
                else
                {
                    valueTotal = xMaxValue - xMinValue;
                    if (valueTotal != 0)
                        barHig = (xMinValue > 0 ? value - xMinValue : value)
                            / valueTotal * coordinateWidth;
                    seriesHig[i] += barHig;
                }

                float currHig = CheckAnimation(serie, i, barHig);

                Vector3 plt = new Vector3(pX + borderWidth, pY + space + barWidth - borderWidth);
                Vector3 prt = new Vector3(pX + currHig - borderWidth, pY + space + barWidth - borderWidth);
                Vector3 prb = new Vector3(pX + currHig - borderWidth, pY + space + borderWidth);
                Vector3 plb = new Vector3(pX + borderWidth, pY + space + borderWidth);
                Vector3 top = new Vector3(pX + currHig - borderWidth, pY + space + barWidth / 2);
                plt = ClampInCoordinate(plt);
                prt = ClampInCoordinate(prt);
                prb = ClampInCoordinate(prb);
                plb = ClampInCoordinate(plb);
                top = ClampInCoordinate(top);
                serie.dataPoints.Add(top);
                if (serie.show)
                {
                    switch (serie.barType)
                    {
                        case BarType.Normal:
                            DrawNormalBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, true);
                            break;
                        case BarType.Zebra:
                            DrawZebraBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, true);
                            break;
                        case BarType.Capsule:
                            DrawCapsuleBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, true);
                            break;
                    }
                }
            }
            if (!m_Series.IsStack(serie.stack, SerieType.Bar))
            {
                m_BarLastOffset += barGapWidth;
            }
            if (dataChanging)
            {
                RefreshChart();
            }
        }

        private float CheckAnimation(Serie serie, int dataIndex, float barHig)
        {
            float currHig = serie.animation.CheckBarProgress(dataIndex, barHig);
            if (!serie.animation.IsFinish())
            {
                RefreshChart();
                m_IsPlayingAnimation = true;
            }
            return currHig;
        }

        protected void DrawXBarSerie(VertexHelper vh, Serie serie, int colorIndex, ref List<float> seriesHig)
        {
            if (!IsActive(serie.name)) return;
            if (serie.animation.HasFadeOut()) return;
            var showData = serie.GetDataList(m_DataZoom);
            var yAxis = m_YAxises[serie.axisIndex];
            var xAxis = m_XAxises[serie.axisIndex];
            if (!xAxis.show) xAxis = m_XAxises[(serie.axisIndex + 1) % m_XAxises.Count];

            float categoryWidth = xAxis.GetDataWidth(coordinateWidth, showData.Count, m_DataZoom);
            float barGap = GetBarGap();
            float totalBarWidth = GetBarTotalWidth(categoryWidth, barGap);
            float barWidth = serie.GetBarWidth(categoryWidth);
            float offset = (categoryWidth - totalBarWidth) / 2;
            float barGapWidth = barWidth + barWidth * barGap;
            float space = serie.barGap == -1 ? offset : offset + m_BarLastOffset;
            int maxCount = serie.maxShow > 0 ?
                (serie.maxShow > showData.Count ? showData.Count : serie.maxShow)
                : showData.Count;

            if (seriesHig.Count < serie.minShow)
            {
                for (int i = 0; i < serie.minShow; i++)
                {
                    seriesHig.Add(0);
                }
            }

            var isPercentStack = m_Series.IsPercentStack(serie.stack, SerieType.Bar);
            bool dataChanging = false;
            float dataChangeDuration = serie.animation.GetUpdateAnimationDuration();
            float yMinValue = yAxis.GetCurrMinValue(dataChangeDuration);
            float yMaxValue = yAxis.GetCurrMaxValue(dataChangeDuration);
            for (int i = serie.minShow; i < maxCount; i++)
            {
                if (i >= seriesHig.Count)
                {
                    seriesHig.Add(0);
                }
                var serieData = showData[i];
                if (serie.IsIgnoreValue(serieData.GetData(1)))
                {
                    serie.dataPoints.Add(Vector3.zero);
                    continue;
                }
                var highlight = (m_Tooltip.show && m_Tooltip.IsSelected(i))
                    || serie.data[i].highlighted
                    || serie.highlighted;
                var itemStyle = SerieHelper.GetItemStyle(serie, serieData, highlight);
                var borderWidth = itemStyle.runtimeBorderWidth;
                float value = serieData.GetCurrData(1, dataChangeDuration);
                if (serieData.IsDataChanged()) dataChanging = true;
                float pX = coordinateX + i * categoryWidth;
                float zeroY = coordinateY + yAxis.runtimeZeroYOffset;
                if (!xAxis.boundaryGap) pX -= categoryWidth / 2;
                float pY = seriesHig[i] + zeroY + xAxis.axisLine.width;

                var barHig = 0f;
                var valueTotal = 0f;
                if (isPercentStack)
                {
                    valueTotal = GetSameStackTotalValue(serie.stack, i);
                    barHig = valueTotal != 0 ? (value / valueTotal * coordinateHeight) : 0;
                    seriesHig[i] += barHig;
                }
                else
                {
                    valueTotal = yMaxValue - yMinValue;
                    if (valueTotal != 0)
                        barHig = (yMinValue > 0 ? value - yMinValue : value)
                            / valueTotal * coordinateHeight;
                    seriesHig[i] += barHig;
                }
                float currHig = CheckAnimation(serie, i, barHig);
                Vector3 plb = new Vector3(pX + space + borderWidth, pY + borderWidth);
                Vector3 plt = new Vector3(pX + space + borderWidth, pY + currHig - borderWidth);
                Vector3 prt = new Vector3(pX + space + barWidth - borderWidth, pY + currHig - borderWidth);
                Vector3 prb = new Vector3(pX + space + barWidth - borderWidth, pY + borderWidth);
                Vector3 top = new Vector3(pX + space + barWidth / 2, pY + currHig - borderWidth);
                plb = ClampInCoordinate(plb);
                plt = ClampInCoordinate(plt);
                prt = ClampInCoordinate(prt);
                prb = ClampInCoordinate(prb);
                top = ClampInCoordinate(top);
                serie.dataPoints.Add(top);
                if (serie.show)
                {
                    switch (serie.barType)
                    {
                        case BarType.Normal:
                            DrawNormalBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, false);
                            break;
                        case BarType.Zebra:
                            DrawZebraBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                                pX, pY, plb, plt, prt, prb, false);
                            break;
                        case BarType.Capsule:
                            DrawCapsuleBar(vh, serie, serieData, itemStyle, colorIndex, highlight, space, barWidth,
                               pX, pY, plb, plt, prt, prb, false);
                            break;
                    }
                }
            }
            if (dataChanging)
            {
                RefreshChart();
            }
            if (!m_Series.IsStack(serie.stack, SerieType.Bar))
            {
                m_BarLastOffset += barGapWidth;
            }
        }

        private void DrawNormalBar(VertexHelper vh, Serie serie, SerieData serieData, ItemStyle itemStyle, int colorIndex,
            bool highlight, float space, float barWidth, float pX, float pY, Vector3 plb, Vector3 plt, Vector3 prt,
            Vector3 prb, bool isYAxis)
        {
            Color areaColor = SerieHelper.GetItemColor(serie, serieData, m_ThemeInfo, colorIndex, highlight);
            Color areaToColor = SerieHelper.GetItemToColor(serie, serieData, m_ThemeInfo, colorIndex, highlight);
            DrawBarBackground(vh, serie, serieData, itemStyle, colorIndex, highlight, pX, pY, space, barWidth, isYAxis);
            var borderWidth = itemStyle.runtimeBorderWidth;
            if (isYAxis)
            {
                CheckClipAndDrawPolygon(vh, plb, plt, prt, prb, areaColor, areaToColor, serie.clip);
                if (borderWidth > 0)
                {
                    var borderColor = itemStyle.borderColor;
                    var itemWidth = Mathf.Abs(prb.x - plt.x);
                    var itemHeight = Mathf.Abs(prt.y - plb.y);
                    var center = new Vector3((plt.x + prb.x) / 2, (prt.y + plb.y) / 2);
                    ChartDrawer.DrawBorder(vh, center, itemWidth, itemHeight, borderWidth, borderColor);
                }
            }
            else
            {
                CheckClipAndDrawPolygon(vh, ref prb, ref plb, ref plt, ref prt, areaColor, areaToColor, serie.clip);
                if (borderWidth > 0)
                {
                    var borderColor = itemStyle.borderColor;
                    var itemWidth = Mathf.Abs(prt.x - plb.x);
                    var itemHeight = Mathf.Abs(plt.y - prb.y);
                    var center = new Vector3((plb.x + prt.x) / 2, (plt.y + prb.y) / 2);
                    ChartDrawer.DrawBorder(vh, center, itemWidth, itemHeight, borderWidth, borderColor);
                }
            }
        }

        private void DrawZebraBar(VertexHelper vh, Serie serie, SerieData serieData, ItemStyle itemStyle, int colorIndex,
            bool highlight, float space, float barWidth, float pX, float pY, Vector3 plb, Vector3 plt, Vector3 prt,
            Vector3 prb, bool isYAxis)
        {
            Color areaColor = SerieHelper.GetItemColor(serie, serieData, m_ThemeInfo, colorIndex, highlight);
            DrawBarBackground(vh, serie, serieData, itemStyle, colorIndex, highlight, pX, pY, space, barWidth, isYAxis);
            if (isYAxis)
            {
                plt = (plb + plt) / 2;
                prt = (prt + prb) / 2;
                CheckClipAndDrawZebraLine(vh, plt, prt, barWidth / 2, serie.barZebraWidth, serie.barZebraGap,
                    areaColor, serie.clip);
            }
            else
            {
                plb = (prb + plb) / 2;
                plt = (plt + prt) / 2;
                CheckClipAndDrawZebraLine(vh, plb, plt, barWidth / 2, serie.barZebraWidth, serie.barZebraGap,
                    areaColor, serie.clip);
            }
        }

        private void DrawCapsuleBar(VertexHelper vh, Serie serie, SerieData serieData, ItemStyle itemStyle, int colorIndex,
            bool highlight, float space, float barWidth, float pX, float pY, Vector3 plb, Vector3 plt, Vector3 prt,
            Vector3 prb, bool isYAxis)
        {
            Color areaColor = SerieHelper.GetItemColor(serie, serieData, m_ThemeInfo, colorIndex, highlight);
            Color areaToColor = SerieHelper.GetItemToColor(serie, serieData, m_ThemeInfo, colorIndex, highlight);
            DrawBarBackground(vh, serie, serieData, itemStyle, colorIndex, highlight, pX, pY, space, barWidth, isYAxis);
            var borderWidth = itemStyle.runtimeBorderWidth;
            var radius = barWidth / 2 - borderWidth;
            if (isYAxis)
            {
                var diff = Vector3.right * radius;
                var pcl = (plt + plb) / 2 + diff;
                var pcr = (prt + prb) / 2 - diff;
                if (pcr.x > pcl.x)
                {
                    CheckClipAndDrawPolygon(vh, plb + diff, plt + diff, prt - diff, prb - diff, areaColor, areaToColor, serie.clip);
                    ChartDrawer.DrawSector(vh, pcl, radius, areaColor, 180, 360);
                    ChartDrawer.DrawSector(vh, pcr, radius, areaToColor, 0, 180);
                }
            }
            else
            {
                var diff = Vector3.up * radius;
                var pct = (plt + prt) / 2 - diff;
                var pcb = (plb + prb) / 2 + diff;
                if (pct.y > pcb.y)
                {
                    CheckClipAndDrawPolygon(vh, prb + diff, plb + diff, plt - diff, prt - diff, areaColor, areaToColor, serie.clip);
                    ChartDrawer.DrawSector(vh, pct, radius, areaToColor, 270, 450);
                    ChartDrawer.DrawSector(vh, pcb, radius, areaColor, 90, 270);
                }
            }
        }

        private void DrawBarBackground(VertexHelper vh, Serie serie, SerieData serieData, ItemStyle itemStyle, int colorIndex,
             bool highlight, float pX, float pY, float space, float barWidth, bool isYAxis)
        {
            Color color = SerieHelper.GetItemBackgroundColor(serie, serieData, m_ThemeInfo, colorIndex, highlight, false);
            if (color == Color.clear) return;
            if (isYAxis)
            {
                var axis = m_YAxises[serie.axisIndex];
                var axisWidth = axis.axisLine.width;
                Vector3 plt = new Vector3(coordinateX + axisWidth, pY + space + barWidth);
                Vector3 prt = new Vector3(coordinateX + axisWidth + coordinateWidth, pY + space + barWidth);
                Vector3 prb = new Vector3(coordinateX + axisWidth + coordinateWidth, pY + space);
                Vector3 plb = new Vector3(coordinateX + axisWidth, pY + space);
                if (serie.barType == BarType.Capsule)
                {
                    var radius = barWidth / 2;
                    var diff = Vector3.right * radius;
                    var pcl = (plt + plb) / 2 + diff;
                    var pcr = (prt + prb) / 2 - diff;
                    CheckClipAndDrawPolygon(vh, plb + diff, plt + diff, prt - diff, prb - diff, color, color, serie.clip);
                    ChartDrawer.DrawSector(vh, pcl, radius, color, 180, 360);
                    ChartDrawer.DrawSector(vh, pcr, radius, color, 0, 180);
                    if (itemStyle.NeedShowBorder())
                    {
                        var borderWidth = itemStyle.borderWidth;
                        var borderColor = itemStyle.borderColor;
                        var smoothness = m_Settings.cicleSmoothness;
                        var inRadius = radius - borderWidth;
                        var outRadius = radius;
                        var p1 = plb + diff + Vector3.up * borderWidth / 2;
                        var p2 = prb - diff + Vector3.up * borderWidth / 2;
                        var p3 = plt + diff - Vector3.up * borderWidth / 2;
                        var p4 = prt - diff - Vector3.up * borderWidth / 2;
                        ChartDrawer.DrawLine(vh, p1, p2, borderWidth / 2, borderColor);
                        ChartDrawer.DrawLine(vh, p3, p4, borderWidth / 2, borderColor);
                        ChartDrawer.DrawDoughnut(vh, pcl, inRadius, outRadius, borderColor, Color.clear, smoothness, 180, 360);
                        ChartDrawer.DrawDoughnut(vh, pcr, inRadius, outRadius, borderColor, Color.clear, smoothness, 0, 180);
                    }
                }
                else
                {
                    CheckClipAndDrawPolygon(vh, ref plb, ref plt, ref prt, ref prb, color, color, serie.clip);
                }
            }
            else
            {
                var axis = m_XAxises[serie.axisIndex];
                var axisWidth = axis.axisLine.width;
                Vector3 plb = new Vector3(pX + space, coordinateY + axisWidth);
                Vector3 plt = new Vector3(pX + space, coordinateY + coordinateHeight + axisWidth);
                Vector3 prt = new Vector3(pX + space + barWidth, coordinateY + coordinateHeight + axisWidth);
                Vector3 prb = new Vector3(pX + space + barWidth, coordinateY + axisWidth);
                if (serie.barType == BarType.Capsule)
                {
                    var radius = barWidth / 2;
                    var diff = Vector3.up * radius;
                    var pct = (plt + prt) / 2 - diff;
                    var pcb = (plb + prb) / 2 + diff;
                    CheckClipAndDrawPolygon(vh, prb + diff, plb + diff, plt - diff, prt - diff, color, color, serie.clip);
                    ChartDrawer.DrawSector(vh, pct, radius, color, 270, 450);
                    ChartDrawer.DrawSector(vh, pcb, radius, color, 90, 270);
                    if (itemStyle.NeedShowBorder())
                    {
                        var borderWidth = itemStyle.borderWidth;
                        var borderColor = itemStyle.borderColor;
                        var smoothness = m_Settings.cicleSmoothness;
                        var inRadius = radius - borderWidth;
                        var outRadius = radius;
                        var p1 = plb + diff + Vector3.right * borderWidth / 2;
                        var p2 = plt - diff + Vector3.right * borderWidth / 2;
                        var p3 = prb + diff - Vector3.right * borderWidth / 2;
                        var p4 = prt - diff - Vector3.right * borderWidth / 2;
                        ChartDrawer.DrawLine(vh, p1, p2, borderWidth / 2, borderColor);
                        ChartDrawer.DrawLine(vh, p3, p4, borderWidth / 2, borderColor);
                        ChartDrawer.DrawDoughnut(vh, pct, inRadius, outRadius, borderColor, Color.clear, smoothness, 270, 450);
                        ChartDrawer.DrawDoughnut(vh, pcb, inRadius, outRadius, borderColor, Color.clear, smoothness, 90, 270);
                    }
                }
                else
                {
                    CheckClipAndDrawPolygon(vh, ref prb, ref plb, ref plt, ref prt, color, color, serie.clip);
                }
            }
        }

        private float GetBarGap()
        {
            float gap = 0.3f;
            for (int i = 0; i < m_Series.Count; i++)
            {
                var serie = m_Series.list[i];
                if (serie.type == SerieType.Bar)
                {
                    if (serie.barGap != 0)
                    {
                        gap = serie.barGap;
                    }
                }
            }
            return gap;
        }

        private float GetSameStackTotalValue(string stack, int dataIndex)
        {
            if (string.IsNullOrEmpty(stack)) return 0;
            float total = 0;
            foreach (var serie in m_Series.list)
            {
                if (serie.type == SerieType.Bar)
                {
                    if (stack.Equals(serie.stack))
                    {
                        total += serie.data[dataIndex].data[1];
                    }
                }
            }
            return total;
        }


        private HashSet<string> barStackSet = new HashSet<string>();
        private float GetBarTotalWidth(float categoryWidth, float gap)
        {
            float total = 0;
            float lastGap = 0;
            barStackSet.Clear();
            for (int i = 0; i < m_Series.Count; i++)
            {
                var serie = m_Series.list[i];
                if (serie.type == SerieType.Bar && serie.show)
                {
                    if (!string.IsNullOrEmpty(serie.stack))
                    {
                        if (barStackSet.Contains(serie.stack)) continue;
                        barStackSet.Add(serie.stack);
                    }
                    var width = GetStackBarWidth(categoryWidth, serie);
                    if (gap == -1)
                    {
                        if (width > total) total = width;
                    }
                    else
                    {
                        lastGap = width * gap;
                        total += width;
                        total += lastGap;
                    }
                }
            }
            if (total > 0 && gap != -1) total -= lastGap;
            return total;
        }

        private float GetStackBarWidth(float categoryWidth, Serie now)
        {
            if (string.IsNullOrEmpty(now.stack)) return now.GetBarWidth(categoryWidth);
            float barWidth = 0;
            for (int i = 0; i < m_Series.Count; i++)
            {
                var serie = m_Series.list[i];
                if (serie.type == SerieType.Bar && serie.show && now.stack.Equals(serie.stack))
                {
                    if (serie.barWidth > barWidth) barWidth = serie.barWidth;
                }
            }
            if (barWidth > 1) return barWidth;
            else return barWidth * categoryWidth;
        }
    }
}
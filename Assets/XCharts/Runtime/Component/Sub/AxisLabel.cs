/******************************************/
/*                                        */
/*     Copyright (c) 2018 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace XCharts
{
    /// <summary>
    /// Settings related to axis label.
    /// 坐标轴刻度标签的相关设置。
    /// </summary>
    [Serializable]
    public class AxisLabel : SubComponent
    {
        [SerializeField] private bool m_Show = true;
        [SerializeField] private string m_Formatter;
        [SerializeField] private int m_Interval = 0;
        [SerializeField] private bool m_Inside = false;
        [SerializeField] private float m_Rotate;
        [SerializeField] private float m_Margin;
        [SerializeField] private Color m_Color;
        [SerializeField] private int m_FontSize;
        [SerializeField] private FontStyle m_FontStyle;
        [SerializeField] private bool m_ForceENotation = false;
        [SerializeField] private TextLimit m_TextLimit = new TextLimit();

        /// <summary>
        /// Set this to false to prevent the axis label from appearing.
        /// 是否显示刻度标签。
        /// </summary>
        public bool show { get { return m_Show; } set { m_Show = value; } }
        /// <summary>
        /// The display interval of the axis label.
        /// 坐标轴刻度标签的显示间隔，在类目轴中有效。0表示显示所有标签，1表示隔一个隔显示一个标签，以此类推。
        /// </summary>
        public int interval { get { return m_Interval; } set { m_Interval = value; } }
        /// <summary>
        /// Set this to true so the axis labels face the inside direction.
        /// 刻度标签是否朝内，默认朝外。
        /// </summary>
        public bool inside { get { return m_Inside; } set { m_Inside = value; } }
        /// <summary>
        /// Rotation degree of axis label, which is especially useful when there is no enough space for category axis.
        /// 刻度标签旋转的角度，在类目轴的类目标签显示不下的时候可以通过旋转防止标签之间重叠。
        /// </summary>
        public float rotate { get { return m_Rotate; } set { m_Rotate = value; } }
        /// <summary>
        /// The margin between the axis label and the axis line.
        /// 刻度标签与轴线之间的距离。
        /// </summary>
        public float margin { get { return m_Margin; } set { m_Margin = value; } }
        /// <summary>
        /// the color of axis label text. 
        /// 刻度标签文字的颜色，默认取Theme的axisTextColor。
        /// </summary>
        public Color color { get { return m_Color; } set { m_Color = value; } }
        /// <summary>
        /// font size.
        /// 文字的字体大小。
        /// </summary>
        public int fontSize { get { return m_FontSize; } set { m_FontSize = value; } }
        /// <summary>
        /// font style.
        /// 文字字体的风格。
        /// </summary>
        public FontStyle fontStyle { get { return m_FontStyle; } set { m_FontStyle = value; } }
        /// <summary>
        /// 图例内容字符串模版格式器。支持用 \n 换行。
        /// 模板变量为图例名称 {value}，支持{value:f0}，{value:f1}，{value:f2}
        /// </summary>
        public string formatter { get { return m_Formatter; } set { m_Formatter = value; } }
        /// <summary>
        /// 是否强制使用科学计数法格式化显示数值。默认为false，当小数精度大于3时才采用科学计数法。
        /// </summary>
        public bool forceENotation { get { return m_ForceENotation; } set { m_ForceENotation = value; } }
        /// <summary>
        /// 文本限制。
        /// </summary>
        public TextLimit textLimit { get { return m_TextLimit; } }

        public static AxisLabel defaultAxisLabel
        {
            get
            {
                return new AxisLabel()
                {
                    m_Show = true,
                    m_Interval = 0,
                    m_Inside = false,
                    m_Rotate = 0,
                    m_Margin = 8,
                    m_Color = Color.clear,
                    m_FontSize = 18,
                    m_FontStyle = FontStyle.Normal
                };
            }
        }
        public void Copy(AxisLabel other)
        {
            m_Show = other.show;
            m_Interval = other.interval;
            m_Inside = other.inside;
            m_Rotate = other.rotate;
            m_Margin = other.margin;
            m_Color = other.color;
            m_FontSize = other.fontSize;
            m_FontStyle = other.fontStyle;
            m_Formatter = other.formatter;
            m_TextLimit.Copy(other.textLimit);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (AxisLabel)obj;
            return m_Show == other.show &&
                m_Interval.Equals(other.interval) &&
                m_Inside == other.inside &&
                m_Rotate == other.rotate &&
                m_Margin == other.margin &&
                m_Color == other.color &&
                m_FontSize == other.fontSize &&
                m_FontStyle == other.fontStyle &&
                m_ForceENotation == other.forceENotation &&
                ChartHelper.IsValueEqualsString(m_Formatter, other.formatter) &&
                m_TextLimit.Equals(other.textLimit);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void SetRelatedText(Text txt, float labelWidth)
        {
            m_TextLimit.SetRelatedText(txt, labelWidth);
        }

        public string GetFormatterContent(string category)
        {
            if (string.IsNullOrEmpty(category)) return category;
            if (string.IsNullOrEmpty(m_Formatter))
            {
                return m_TextLimit.GetLimitContent(category);
            }
            else
            {
                var content = m_Formatter.Replace("{value}", category);
                content = content.Replace("\\n", "\n");
                content = content.Replace("<br/>", "\n");
                return m_TextLimit.GetLimitContent(content);
            }
        }

        public string GetFormatterContent(float value, float minValue, float maxValue, bool isLog = false)
        {
            if (string.IsNullOrEmpty(m_Formatter))
            {
                if (isLog)
                {
                    if (value - (int)value == 0)
                        return ChartCached.IntToStr((int)value);
                    else
                        return ChartCached.FloatToStr(value);
                }
                if (minValue >= -1 && minValue <= 1 && maxValue >= -1 && maxValue <= 1)
                {
                    int minAcc = ChartHelper.GetFloatAccuracy(minValue);
                    int maxAcc = ChartHelper.GetFloatAccuracy(maxValue);
                    int curAcc = ChartHelper.GetFloatAccuracy(value);
                    int acc = Mathf.Max(Mathf.Max(minAcc, maxAcc), curAcc);
                    return ChartCached.FloatToStr(value, acc, m_ForceENotation);
                }
                else if (value - (int)value == 0)
                    return ChartCached.IntToStr((int)value);
                else
                    return ChartCached.FloatToStr(value, 1);
            }
            else if (m_Formatter.Contains("{value"))
            {
                var content = m_Formatter;
                if (content.Contains("{value:f0}"))
                    content = m_Formatter.Replace("{value:f0}", ChartCached.IntToStr((int)value));
                if (content.Contains("{value:f2}"))
                    content = m_Formatter.Replace("{value:f2}", ChartCached.FloatToStr(value, 2));
                else if (content.Contains("{value:f1}"))
                    content = m_Formatter.Replace("{value:f1}", ChartCached.FloatToStr(value, 1));
                else if (content.Contains("{value}"))
                {
                    if (value - (int)value == 0)
                        content = m_Formatter.Replace("{value}", ChartCached.IntToStr((int)value));
                    else
                        content = m_Formatter.Replace("{value}", ChartCached.FloatToStr(value, 1));
                }

                content = content.Replace("\\n", "\n");
                content = content.Replace("<br/>", "\n");
                return content;
            }
            else
            {
                return value.ToString(m_Formatter);
            }
        }
    }
}
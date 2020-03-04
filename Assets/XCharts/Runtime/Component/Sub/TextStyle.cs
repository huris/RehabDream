/******************************************/
/*                                        */
/*     Copyright (c) 2018 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System;
using UnityEngine;

namespace XCharts
{
    /// <summary>
    /// Settings related to text.
    /// 文本的相关设置。
    /// </summary>
    [Serializable]
    public class TextStyle : SubComponent, IEquatable<TextStyle>
    {
        [SerializeField] private Font m_Font;
        [SerializeField] private float m_Rotate = 0;
        [SerializeField] private Vector2 m_Offset = Vector2.zero;
        [SerializeField] private Color m_Color = Color.clear;
        [SerializeField] private Color m_BackgroundColor = Color.clear;
        [SerializeField] private int m_FontSize = 18;
        [SerializeField] private FontStyle m_FontStyle = FontStyle.Normal;
        [SerializeField] private float m_LineSpacing = 1f;
        [SerializeField] private float m_PaddingLeft = 0f;
        [SerializeField] private float m_PaddingRight = 0f;
        [SerializeField] private float m_PaddingTop = 0f;
        [SerializeField] private float m_PaddingBottom = 0f;

        /// <summary>
        /// Rotation of text.
        /// 文本的旋转。
        /// </summary>
        public float rotate { get { return m_Rotate; } set { m_Rotate = value; } }
        /// <summary>
        /// the offset of position.
        /// 坐标偏移。
        /// </summary>
        public Vector2 offset { get { return m_Offset; } set { m_Offset = value; } }

        public Vector3 offsetv3 { get { return new Vector3(m_Offset.x, m_Offset.y, 0); } }

        /// <summary>
        /// the color of text. 
        /// 文本的颜色。
        /// </summary>
        public Color color { get { return m_Color; } set { m_Color = value; } }
        /// <summary>
        /// the color of text. 
        /// 文本的背景颜色。
        /// </summary>
        public Color backgroundColor { get { return m_BackgroundColor; } set { m_BackgroundColor = value; } }
        /// <summary>
        /// the font of text.
        /// 文本字体
        /// </summary>
        public Font font { get { return m_Font; } set { m_Font = value; } }
        /// <summary>
        /// font size.
        /// 文本字体大小。
        /// </summary>
        public int fontSize { get { return m_FontSize; } set { m_FontSize = value; } }
        /// <summary>
        /// font style.
        /// 文本字体的风格。
        /// </summary>
        public FontStyle fontStyle { get { return m_FontStyle; } set { m_FontStyle = value; } }
        /// <summary>
        /// text line spacing.
        /// 行间距。
        /// </summary>
        public float lineSpacing { get { return m_LineSpacing; } set { m_LineSpacing = value; } }

        public TextStyle()
        {
        }

        public TextStyle(int fontSize)
        {
            this.fontSize = fontSize;
        }

        public TextStyle(int fontSize, FontStyle fontStyle)
        {
            this.fontSize = fontSize;
            this.fontStyle = fontStyle;
        }

        public TextStyle(int fontSize, FontStyle fontStyle, Color color)
        {
            this.fontSize = fontSize;
            this.fontStyle = fontStyle;
            this.color = color;
        }

        public TextStyle(int fontSize, FontStyle fontStyle, Color color, int rorate)
        {
            this.fontSize = fontSize;
            this.fontStyle = fontStyle;
            this.color = color;
            this.rotate = rotate;
        }

        public void Copy(TextStyle style)
        {
            this.fontSize = style.fontSize;
            this.fontStyle = style.fontStyle;
            this.color = style.color;
            this.backgroundColor = style.backgroundColor;
            this.rotate = style.rotate;
            this.offset = style.offset;
            this.lineSpacing = style.lineSpacing;
        }

        public TextStyle Clone()
        {
            var textStyle = new TextStyle();
            textStyle.rotate = rotate;
            textStyle.color = color;
            textStyle.backgroundColor = backgroundColor;
            textStyle.fontSize = fontSize;
            textStyle.fontStyle = fontStyle;
            textStyle.offset = offset;
            textStyle.lineSpacing = lineSpacing;
            return textStyle;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else if (obj is TextStyle)
            {
                return Equals((TextStyle)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(TextStyle other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return rotate == other.rotate &&
                fontSize == other.fontSize &&
                fontStyle == other.fontStyle &&
                offset == other.offset &&
                lineSpacing == other.lineSpacing &&
                ChartHelper.IsValueEqualsColor(m_BackgroundColor, other.backgroundColor) &&
                ChartHelper.IsValueEqualsColor(m_Color, other.color);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
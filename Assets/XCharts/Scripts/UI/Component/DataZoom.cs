﻿using UnityEngine;
using UnityEngine.UI;

namespace XCharts
{
    /// <summary>
    /// DataZoom component is used for zooming a specific area,
    /// which enables user to investigate data in detail,
    /// or get an overview of the data, or get rid of outlier points.
    /// <para>DataZoom 组件 用于区域缩放，从而能自由关注细节的数据信息，或者概览数据整体，或者去除离群点的影响。</para>
    /// </summary>
    [System.Serializable]
    public class DataZoom
    {
        /// <summary>
        /// Generally dataZoom component zoom or roam coordinate system through data filtering
        /// and set the windows of axes internally.
        /// Its behaviours vary according to filtering mode settings.
        /// dataZoom 的运行原理是通过 数据过滤 来达到 数据窗口缩放 的效果。数据过滤模式的设置不同，效果也不同。
        /// </summary>
        public enum FilterMode
        {
            /// <summary>
            /// data that outside the window will be filtered, which may lead to some changes of windows of other axes.
            /// For each data item, it will be filtered if one of the relevant dimensions is out of the window.
            /// 当前数据窗口外的数据，被 过滤掉。即 会 影响其他轴的数据范围。每个数据项，只要有一个维度在数据窗口外，整个数据项就会被过滤掉。
            /// </summary>
            Filter,
            /// <summary>
            /// data that outside the window will be filtered, which may lead to some changes of windows of other axes.
            /// For each data item, it will be filtered only if all of the relevant dimensions are out of the same side of the window.
            /// 当前数据窗口外的数据，被 过滤掉。即 会 影响其他轴的数据范围。每个数据项，只有当全部维度都在数据窗口同侧外部，整个数据项才会被过滤掉。
            /// </summary>
            WeakFilter,
            /// <summary>
            /// data that outside the window will be set to NaN, which will not lead to changes of windows of other axes. 
            /// 当前数据窗口外的数据，被 设置为空。即 不会 影响其他轴的数据范围。
            /// </summary>
            Empty,
            /// <summary>
            /// Do not filter data.
            /// 不过滤数据，只改变数轴范围。
            /// </summary>
            None
        }
        /// <summary>
        /// The value type of start and end.取值类型
        /// </summary>
        public enum RangeMode
        {
            //Value,
            /// <summary>
            /// percent value. 百分比
            /// </summary>
            Percent
        }
        [SerializeField] private bool m_Enable;
        [SerializeField] private FilterMode m_FilterMode;
        [SerializeField] private int m_XAxisIndex;
        [SerializeField] private int m_YAxisIndex;
        [SerializeField] private bool m_SupportInside;
        [SerializeField] private bool m_SupportSlider;
        [SerializeField] private bool m_SupportSelect;
        [SerializeField] private bool m_ShowDataShadow;
        [SerializeField] private bool m_ShowDetail;
        [SerializeField] private bool m_ZoomLock;
        [SerializeField] private bool m_Realtime;
        [SerializeField] private Color m_BackgroundColor;
        [SerializeField] private float m_Height;
        [SerializeField] private float m_Bottom;
        [SerializeField] private RangeMode m_RangeMode;
        [SerializeField] private float m_Start;
        [SerializeField] private float m_End;
        [SerializeField] private float m_StartValue;
        [SerializeField] private float m_EndValue;
        [Range(1f, 20f)]
        [SerializeField] private float m_ScrollSensitivity = 1.1f;
        [SerializeField] private int m_FontSize = 18;
        [SerializeField] private FontStyle m_FontStyle;

        /// <summary>
        /// Whether to show dataZoom. 
        /// 是否显示缩放区域。
        /// </summary>
        public bool enable { get { return m_Enable; } set { m_Enable = value; } }
        /// <summary>
        /// The mode of data filter. 
        /// 数据过滤类型。
        /// </summary>
        public FilterMode filterMode { get { return m_FilterMode; } set { m_FilterMode = value; } }
        /// <summary>
        /// Specify which xAxis is controlled by the dataZoom. 
        /// 控制哪一个 x 轴。
        /// </summary>
        public int xAxisIndex { get { return m_XAxisIndex; } set { m_XAxisIndex = value; } }
        /// <summary>
        /// Specify which yAxis is controlled by the dataZoom. 
        /// 控制哪一个 y 轴。
        /// </summary>
        public int yAxisIndex { get { return m_YAxisIndex; } set { m_YAxisIndex = value; } }
        /// <summary>
        /// 是否支持内置。内置于坐标系中，使用户可以在坐标系上通过鼠标拖拽、鼠标滚轮、手指滑动（触屏上）来缩放或漫游坐标系。
        /// </summary>
        public bool supportInside { get { return m_SupportInside; } set { m_SupportInside = value; } }
        /// <summary>
        /// 是否支持滑动条。有单独的滑动条，用户在滑动条上进行缩放或漫游。
        /// </summary>
        public bool supportSlider { get { return m_SupportSlider; } set { m_SupportSlider = value; } }
        /// <summary>
        /// 是否支持框选。提供一个选框进行数据区域缩放。
        /// </summary>
        private bool supportSelect { get { return m_SupportSelect; } set { m_SupportSelect = value; } }
        /// <summary>
        /// Whether to show data shadow, to indicate the data tendency in brief.
        /// default:true
        /// 是否显示数据阴影。数据阴影可以简单地反应数据走势。
        /// </summary>
        public bool showDataShadow { get { return m_ShowDataShadow; } set { m_ShowDataShadow = value; } }
        /// <summary>
        /// Whether to show detail, that is, show the detailed data information when dragging.
        /// 是否显示detail，即拖拽时候显示详细数值信息。
        /// </summary>
        public bool showDetail { get { return m_ShowDetail; } set { m_ShowDetail = value; } }
        /// <summary>
        /// Specify whether to lock the size of window (selected area).
        /// default:false
        /// 是否锁定选择区域（或叫做数据窗口）的大小。
        /// 如果设置为 true 则锁定选择区域的大小，也就是说，只能平移，不能缩放。
        /// </summary>
        public bool zoomLock { get { return m_ZoomLock; } set { m_ZoomLock = value; } }
        /// <summary>
        /// Whether to show data shadow in dataZoom-silder component, to indicate the data tendency in brief.
        /// default:true
        /// 拖动时，是否实时更新系列的视图。如果设置为 false，则只在拖拽结束的时候更新。默认为true，暂不支持修改。
        /// </summary>
        public bool realtime { get { return true; } }
        /// <summary>
        /// The background color of the component.
        /// 组件的背景颜色。
        /// </summary>
        private Color backgroundColor { get { return m_BackgroundColor; } set { m_BackgroundColor = value; } }
        /// <summary>
        /// Distance between dataZoom component and the bottom side of the container.
        /// bottom value is a instant pixel value like 10.
        /// default:10
        /// 组件离容器下侧的距离。
        /// </summary>
        public float bottom { get { return m_Bottom; } set { m_Bottom = value; } }
        /// <summary>
        /// The height of dataZoom component.
        /// height value is a instant pixel value like 10.
        /// default:50
        /// 组件高度。
        /// </summary>
        public float height { get { return m_Height; } set { m_Height = value; } }
        /// <summary>
        /// Use absolute value or percent value in DataZoom.start and DataZoom.end.
        /// default:RangeMode.Percent.
        /// 取绝对值还是百分比。
        /// </summary>
        public RangeMode rangeMode { get { return m_RangeMode; } set { m_RangeMode = value; } }
        /// <summary>
        /// The start percentage of the window out of the data extent, in the range of 0 ~ 100.
        /// default:30
        /// 数据窗口范围的起始百分比。范围是：0 ~ 100。
        /// </summary>
        public float start
        {
            get { return m_Start; }
            set { m_Start = value; if (m_Start < 0) m_Start = 0; if (m_Start > 100) m_Start = 100; }
        }
        /// <summary>
        /// The end percentage of the window out of the data extent, in the range of 0 ~ 100.
        /// default:70
        /// 数据窗口范围的结束百分比。范围是：0 ~ 100。
        /// </summary>
        public float end
        {
            get { return m_End; }
            set { m_End = value; if (m_End < 0) m_End = 0; if (m_End > 100) m_End = 100; }
        }
        /// <summary>
        /// The sensitivity of dataZoom scroll.
        /// The larger the number, the more sensitive it is.
        /// default:10
        /// 缩放区域组件的敏感度。值越高每次缩放所代表的数据越多。
        /// </summary>
        public float scrollSensitivity { get { return m_ScrollSensitivity; } set { m_ScrollSensitivity = value; } }
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
        /// DataZoom is in draging.
        /// 正在拖拽组件。
        /// </summary>
        public bool isDraging { get; set; }
        /// <summary>
        /// The start label.
        /// 组件的开始信息文本。
        /// </summary>
        public Text startLabel { get; set; }
        /// <summary>
        /// The end label.
        /// 组件的结束信息文本。
        /// </summary>
        public Text endLabel { get; set; }

        public static DataZoom defaultDataZoom
        {
            get
            {
                return new DataZoom()
                {
                    filterMode = FilterMode.None,
                    xAxisIndex = 0,
                    yAxisIndex = 0,
                    showDataShadow = true,
                    showDetail = false,
                    zoomLock = false,
                    m_Height = 0,
                    m_Bottom = 10,
                    rangeMode = RangeMode.Percent,
                    start = 30,
                    end = 70,
                    m_ScrollSensitivity = 10,
                };
            }
        }

        /// <summary>
        /// 给定的坐标是否在缩放区域内
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="startX"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool IsInZoom(Vector2 pos, float startX, float width)
        {
            Rect rect = Rect.MinMaxRect(startX, m_Bottom, startX + width, m_Bottom + m_Height);
            return rect.Contains(pos);
        }

        /// <summary>
        /// 给定的坐标是否在选中区域内
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="startX"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool IsInSelectedZoom(Vector2 pos, float startX, float width)
        {
            var start = startX + width * m_Start / 100;
            var end = startX + width * m_End / 100;
            Rect rect = Rect.MinMaxRect(start, m_Bottom, end, m_Bottom + m_Height);
            return rect.Contains(pos);
        }

        /// <summary>
        /// 给定的坐标是否在开始活动条触发区域内
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="startX"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool IsInStartZoom(Vector2 pos, float startX, float width)
        {
            var start = startX + width * m_Start / 100;
            Rect rect = Rect.MinMaxRect(start - 10, m_Bottom, start + 10, m_Bottom + m_Height);
            return rect.Contains(pos);
        }

        /// <summary>
        /// 给定的坐标是否在结束活动条触发区域内
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="startX"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool IsInEndZoom(Vector2 pos, float startX, float width)
        {
            var end = startX + width * m_End / 100;
            Rect rect = Rect.MinMaxRect(end - 10, m_Bottom, end + 10, m_Bottom + m_Height);
            return rect.Contains(pos);
        }

        /// <summary>
        /// 是否显示文本
        /// </summary>
        /// <param name="flag"></param>
        public void SetLabelActive(bool flag)
        {
            if (startLabel && startLabel.gameObject.activeInHierarchy != flag)
            {
                startLabel.gameObject.SetActive(flag);
            }
            if (endLabel && endLabel.gameObject.activeInHierarchy != flag)
            {
                endLabel.gameObject.SetActive(flag);
            }
        }

        /// <summary>
        /// 设置开始文本内容
        /// </summary>
        /// <param name="text"></param>
        public void SetStartLabelText(string text)
        {
            if (startLabel) startLabel.text = text;
        }

        /// <summary>
        /// 设置结束文本内容
        /// </summary>
        /// <param name="text"></param>
        public void SetEndLabelText(string text)
        {
            if (endLabel) endLabel.text = text;
        }

        /// <summary>
        /// 获取DataZoom的高，当height设置为0时，自动计算合适的偏移。
        /// </summary>
        /// <param name="gridBottom"></param>
        /// <returns></returns>
        public float GetHeight(float gridBottom)
        {
            if (height <= 0)
            {
                height = gridBottom - bottom - 30;
                if (height < 10) height = 10;
                return height;
            }
            else return height;
        }
    }
}
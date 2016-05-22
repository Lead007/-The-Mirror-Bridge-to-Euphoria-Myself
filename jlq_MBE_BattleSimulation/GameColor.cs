using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>静态颜色类</summary>
    public static class GameColor
    {
        /// <summary>控件的基础颜色</summary>
        public static Brush BaseColor => Brushes.LightGray;
        /// <summary>棋盘按钮的颜色</summary>
        public static Brush PadBackground => Brushes.LightYellow;
        /// <summary>角色标签的默认颜色</summary>
        public static Brush LabelDefalutBackground => Brushes.White;
        /// <summary>角色标签改变后的颜色</summary>
        public static Brush LabelBackground => Brushes.LightBlue;
        /// <summary>角色标签改变后的颜色2</summary>
        public static Brush LabelBackground2 => Brushes.LawnGreen;
        /// <summary>棋盘网格的颜色</summary>
        public static Brush PadBrush => Brushes.Blue;
        /// <summary>对话框中的按钮颜色</summary>
        public static Brush DialogButtonBrush => Brushes.SaddleBrown;
        /// <summary>按钮单击后的颜色</summary>
        public static Brush LinearBrush { get; private set; }

        /// <summary>生成颜色</summary>
        public static void GenerateColors()
        {
            LinearBrush = new LinearGradientBrush(
                new GradientStopCollection(new List<GradientStop>
                {
                    new GradientStop(Colors.Lime, 0),
                    new GradientStop(Colors.Yellow, 0.5),
                    new GradientStop(Colors.Turquoise, 1)
                }));
        }
    }
}

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
        /// <summary>游戏按钮单击后的颜色</summary>
        public static Brush GameButtonLinearBrush { get; }
        #region 符卡
        /// <summary>符卡按钮的文字颜色</summary>
        public static Brush ScButtonForeBrush => Brushes.White;
        /// <summary>符卡按钮的默认颜色</summary>
        public static Brush ScButtonDefaultBrush => Brushes.DarkSlateGray;
        /// <summary>符卡按钮单击后的颜色</summary>
        public static Brush ScButtonLinearBrush { get; }
        #endregion
        
        /// <summary>生成颜色</summary>
        static GameColor()
        {
            GameButtonLinearBrush = new LinearGradientBrush(
                new GradientStopCollection(new List<GradientStop>
                {
                    new GradientStop(Colors.Lime, 0),
                    new GradientStop(Colors.Yellow, 0.5),
                    new GradientStop(Colors.Turquoise, 1)
                }));
            ScButtonLinearBrush = new LinearGradientBrush(
                new GradientStopCollection(new List<GradientStop>
                {
                    new GradientStop(Colors.Crimson, 0),
                    new GradientStop(Colors.Fuchsia, 0.5),
                    new GradientStop(Colors.Violet, 1)
                }));
        }
    }
}

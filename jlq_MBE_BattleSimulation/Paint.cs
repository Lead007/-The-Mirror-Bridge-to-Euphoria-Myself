using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    public static class Paint
    {
        /// <summary>将角色标签颜色设为淡蓝色</summary>
        /// <param name="c">角色</param>
        public static void SetLabelBackground(this Character c)
        {
            c.LabelDisplay.Background = GameColor.LabelBackground;
        }

        /// <summary>将角色标签颜色设为指定的颜色</summary>
        /// <param name="c">角色</param>
        /// <param name="color">指定的颜色</param>
        public static void SetLabelBackground(this Character c, Brush color)
        {
            c.LabelDisplay.Background = color;
        }

        /// <summary>将角色枚举集合内所有角色标签颜色设为淡蓝色</summary>
        /// <param name="characters">角色枚举集合</param>
        public static void SetLabelBackground(this IEnumerable<Character> characters)
        {
            foreach (var c in characters)
            {
                c.SetLabelBackground();
            }
        }

        /// <summary>将角色枚举集合内所有角色标签颜色设为指定的颜色</summary>
        /// <param name="characters">角色枚举集合</param>
        /// <param name="color">指定的颜色</param>
        public static void SetLabelBackground(this IEnumerable<Character> characters, Brush color)
        {
            foreach (var c in characters)
            {
                c.SetLabelBackground(color);
            }
        }

        /// <summary>将按钮颜色设为淡黄色</summary>
        /// <param name="button">按钮</param>
        public static void SetButtonColor(this Button button)
        {
            button.Opacity = 1;
        }

        /// <summary>将按钮枚举集合内所有按钮颜色设为淡黄色</summary>
        /// <param name="buttons">按钮枚举集合</param>
        public static void SetButtonColor(this IEnumerable<Button> buttons)
        {
            buttons.Aggregate(0.0, (c, b) => b.Opacity = 1);
        }

        /// <summary>将按钮颜色设为无色</summary>
        /// <param name="button">按钮</param>
        public static void ResetButtonColor(this Button button)
        {
            button.Opacity = 0;
        }

        /// <summary>将按钮枚举集合内所有按钮颜色设为无色</summary>
        /// <param name="buttons">按钮枚举集合</param>
        public static void ResetButtonColor(this IEnumerable<Button> buttons)
        {
            buttons.Aggregate(0.0, (c, b) => b.Opacity = 0);
        }
    }
}

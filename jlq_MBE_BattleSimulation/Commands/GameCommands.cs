using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace jlq_MBE_BattleSimulation.Commands
{
    /// <summary>游戏命令</summary>
    public class GameCommands
    {
        /// <summary>退出命令</summary>
        public static RoutedUICommand Exit { get; }

        /// <summary>转为战斗模式命令</summary>
        public static RoutedUICommand TurnBattle { get; }

        /// <summary>清空棋盘命令</summary>
        public static RoutedUICommand Clear { get; }

        /// <summary>查看Mods命令</summary>
        public static RoutedUICommand Mods { get; }

        /// <summary>设置命令</summary>
        public static RoutedUICommand Settings { get; }

        static GameCommands()
        {
            #region 退出命令
            var inputs = new InputGestureCollection {new KeyGesture(Key.Escape)};
            Exit = new RoutedUICommand("退出", "退出", typeof (GameCommands), inputs);
            #endregion
            #region 转为战斗模式命令
            var inputs2 = new InputGestureCollection {new KeyGesture(Key.T, ModifierKeys.Control, "Ctrl+T")};
            TurnBattle = new RoutedUICommand("转为战斗模式", "转为战斗模式", typeof(GameCommands), inputs2);
            #endregion
            #region 清空棋盘命令
            var inputs3 = new InputGestureCollection {new KeyGesture(Key.C, ModifierKeys.Alt, "Alt+C")};
            Clear = new RoutedUICommand("清除所有单位", "清除所有单位", typeof (GameCommands), inputs3);
            #endregion
            #region 查看Mods命令
            var inputs4 = new InputGestureCollection {new KeyGesture(Key.M, ModifierKeys.Alt, "Alt+M")};
            Mods = new RoutedUICommand("Mods", "Mods", typeof (GameCommands), inputs4);
            #endregion
            #region 设置命令
            var inputs5 = new InputGestureCollection {new KeyGesture(Key.O, ModifierKeys.Alt, "Alt+O")};
            Settings = new RoutedUICommand("设置", "设置", typeof (GameCommands), inputs5);
            #endregion
        }
    }
}

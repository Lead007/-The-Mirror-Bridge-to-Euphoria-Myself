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

        static GameCommands()
        {
            #region 退出命令
            var inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Escape));
            Exit = new RoutedUICommand("退出", "退出", typeof (GameCommands), inputs);
            #endregion
            #region 转为战斗模式命令
            var inputs2 = new InputGestureCollection();
            inputs2.Add(new KeyGesture(Key.T, ModifierKeys.Control, "Ctrl+T"));
            TurnBattle = new RoutedUICommand("转为战斗模式", "转为战斗模式", typeof(GameCommands), inputs2);
            #endregion
        }
    }
}

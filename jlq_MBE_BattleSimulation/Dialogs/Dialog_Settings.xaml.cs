using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JLQ_GameBase;

namespace jlq_MBE_BattleSimulation.Dialogs
{
    /// <summary>
    /// Dialog_Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Dialog_Settings : Window
    {
        public Dialog_Settings(Game game)
        {
            InitializeComponent();
            this.game = game;
        }

        /// <summary>游戏对象</summary>
        private Game game { get; }

        /// <summary>游戏设置</summary>
        private GameSettings Settings => game.Settings;

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            //TODO Save Settings
            this.DialogResult = true;
            this.Close();
        }
    }
}

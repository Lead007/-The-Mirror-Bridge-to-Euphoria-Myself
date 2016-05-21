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

namespace JLQ_MBE_BattleSimulation.Dialogs
{
    /// <summary>
    /// Dialog_LyricaSC02.xaml 的交互逻辑
    /// </summary>
    public partial class Dialog_LyricaSC02 : Window
    {
        private Game game;
        public Dialog_LyricaSC02(Game game)
        {
            InitializeComponent();
            this.game = game;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }
    }
}

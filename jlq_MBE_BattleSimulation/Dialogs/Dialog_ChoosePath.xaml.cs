using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using JLQ_GameBase;

namespace JLQ_MBE_BattleSimulation.Dialogs
{
    /// <summary>
    /// Dialog_ChoosePath.xaml 的交互逻辑
    /// </summary>
    public partial class Dialog_ChoosePath : Window
    {
        /// <summary>构造函数</summary>
        public Dialog_ChoosePath(Game game)
        {
            InitializeComponent();
            this.game = game;
            labelPath.Content = game.SavePath;
            textBoxName.Text = "Pad" + game.SaveTimes;
        }

        private Game game;

        public string FileSavePath { get; set; }

        private void textBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxName.Text.Last() == '\\')
            {
                textBoxName.Text = textBoxName.Text.Substring(0, textBoxName.Text.Length - 1);
            }
            labelPath.Content = game.SavePath + textBoxName.Text + ".pad\t";
            if (!string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                textBoxName.BorderBrush = GameColor.BaseColor;
            }
        }

        private void buttonExplore_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            game.SavePath = dialog.SelectedPath;
            if (game.SavePath.Last() != '\\') game.SavePath += "\\";
            labelPath.Content = game.SavePath + textBoxName.Text + ".pad\t";
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                textBoxName.BorderBrush = Brushes.Red;
                return;
            }
            var filePath = game.SavePath + textBoxName.Text + ".pad";
            if (File.Exists(filePath))
            {
                Game.ErrorMessageBox("该文件已经存在！");
                return;
            }
            var formatter = new BinaryFormatter();
            FileSavePath = filePath;
            using (var writer = File.Create(filePath))
            {
                formatter.Serialize(writer, game.CInfos);
            }
            Thread.Sleep(2000);
            MessageBox.Show("保存成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            this.Close();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }
    }
}

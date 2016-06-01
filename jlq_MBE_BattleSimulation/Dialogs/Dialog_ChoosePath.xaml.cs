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
    /// Window1.xaml 的交互逻辑
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

        private void textBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxName.Text.Last() == '\\')
            {
                textBoxName.Text = textBoxName.Text.Substring(0, textBoxName.Text.Length - 1);
            }
            labelPath.Content = game.SavePath + textBoxName.Text + "   ";
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
            labelPath.Content = game.SavePath + textBoxName.Text + "\\\t";
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                textBoxName.BorderBrush = Brushes.Red;
                return;
            }
            var path = game.SavePath + textBoxName.Text;
            if (Directory.Exists(path))
            {
                Game.ErrorMessageBox("该路径已经存在！");
                return;
            }
            Directory.CreateDirectory(path);
            path += "\\";
            var formatter = new BinaryFormatter();
            foreach (var c in game.Characters)
            {
                var pathc = path + c.ID;
                Directory.CreateDirectory(pathc);
                pathc += "\\";
                using (Stream writer = File.Create(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Data))
                {
                    formatter.Serialize(writer, Game.CharacterDataListShow.First(cd => cd.Name == c.Name));
                }
                using (Stream writer = File.Create(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Group))
                {
                    formatter.Serialize(writer, c.Group);
                }
                using (Stream writer = File.Create(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Position))
                {
                    formatter.Serialize(writer, c.Position);
                }
            }
            using (var writer = new StreamWriter(path + textBoxName.Text + ".pad"))
            {
                writer.WriteLine("Name: {0}", textBoxName.Text);
                writer.WriteLine("Number of Characters: {0}", game.Characters.Count);
                writer.WriteLine("Save Time: {0}",
                    DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
            }
            Thread.Sleep(2000);
            MessageBox.Show("保存成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }
    }
}

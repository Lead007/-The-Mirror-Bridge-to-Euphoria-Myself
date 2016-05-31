using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using Bitmap;
using Data;
using JLQ_MBE_BattleSimulation.Dialogs;
using JLQ_GameBase;
using JLQ_GameResources.Characters.SingleCharacter;
using MoreEnumerable;
using RandomHelper;
using static JLQ_GameBase.GameColor;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 静态
        /// <summary>当前文件的路径</summary>
        private static string CurrentPath { get; } = Directory.GetCurrentDirectory();
        #endregion

        /// <summary>game对象</summary>
        private Game game { get; } = new Game();

        /// <summary>构造函数</summary>
        public MainWindow()
        {
            InitializeComponent();
            #region 待完成
            //this.Icon = BitmapConverter.BitmapToBitmapImage(TODO Add Icon);
            //Application.Current.DispatcherUnhandledException += (s, e) =>
            //{
            //    MessageBox.Show(e.Exception.ToString());
            //    e.Handled = true;
            //};
            #endregion

            #region 读取角色各数据

            var data = new XmlDocument();
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create("Resources/Data/data.xml",
                    new XmlReaderSettings {IgnoreComments = true /*忽略注释*/});
                data.Load(reader);
            }
            catch (Exception)
            {
                Game.ErrorMessageBox("找不到数据文件，程序无法运行。请联系开发者。");
                this.Close();
                return;
            }
            Calculate.CharacterDataList = DataLoader.LoadDatas(data);
            reader.Close();
            Calculate.CharacterDataList.DoAction(cd => comboBoxDisplay.Items.Add(cd.Display));
            comboBoxDisplay.Items.Remove("芙分");

            #endregion

            #region 初始化game对象

            game.EventGridPadClick += GridPadMouseDown;

            #endregion

            #region 生成棋盘按钮事件

            foreach (var button in game.Buttons)
            {
                var column = (int) button.GetValue(Grid.ColumnProperty);
                var row = (int) button.GetValue(Grid.RowProperty);

                #region MouseMove

                button.MouseMove += (s, ev) =>
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        button.ToolTip = game.StringShow(new Point(column, row));
                    }
                    else
                    {
                        button.ToolTip = game.TipShow(new Point(column, row));
                    }
                };

                #endregion

                #region MouseEnter

                button.MouseEnter += (s, ev) =>
                {
                    game.MousePoint = new Point(column, row);
                };

                #endregion

                #region MouseLeave

                button.MouseLeave += (s, ev) =>
                {
                    game.MousePoint = new Point(-1, -1);
                };

                #endregion

                #region KeyDown

                button.KeyDown += (s, ev) =>
                {
                    //如果shift和ctrl都没被按下或不在行动阶段或不在棋盘内或该点无角色或该点角色为当前角色则无效
                    if ((!(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
                           Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) ||
                        GameSection != Section.Round || game.MousePoint == new Point(-1, -1) ||
                        game.Characters.All(c => c.Position != game.MousePoint) ||
                        game.MousePoint == game.CurrentPosition) return;
                    //如果shift被按下
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        var character = game.MouseCharacter;
                        if (character == null) return;
                        //清屏
                        game.DefaultButtonAndLabels();
                        game.SetButtonBackground(game.MousePoint, character.AttackRange);
                        character.SetLabelBackground();
                    }
                    //如果ctrl被按下
                    else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        var character = game.MouseCharacter;
                        if (character == null) return;
                        //清屏
                        game.DefaultButtonAndLabels();

                        game.SetButtonBackground(game.MousePoint, character.MoveAbility);
                        character.SetLabelBackground();
                    }
                };

                #endregion

                #region KeyUp

                button.KeyUp += (s, ev) =>
                {
                    //如果不在行动阶段或仍有shift或ctrl在棋盘内则无效
                    if (GameSection != Section.Round) return;
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
                        Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) return;
                    //恢复原本显示
                    game.HandleResetShow();
                };

                #endregion
            }

            #endregion

            #region 加载mods

            var p = CurrentPath + "\\Resources\\Mods";
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
            try
            {
                foreach (var assembly in
                    Directory.GetFiles(p).Where(s => string.Compare(s, s.Length - 4, ".dll", 0, 4, true) == 0))
                {
                    try
                    {
                        Assemblies.Add(Assembly.LoadFile(assembly));
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {
            }

            #endregion
        }

        /// <summary>当前行动角色</summary>
        private Character CurrentCharacter => game.CurrentCharacter;
        /// <summary>当前游戏阶段</summary>
        private Section? GameSection => game.GameSection;

        /// <summary>外置程序集</summary>
        private List<Assembly> Assemblies { get; } = new List<Assembly>(); 

        /// <summary>添加角色</summary>
        /// <param name="point">添加的位置</param>
        /// <param name="group">角色的阵营</param>
        /// <param name="display">显示的字符串</param>
        private void AddCharacter(Point point, Group group, string display)
        {
            var name = Calculate.CharacterDataList.First(c => c.Display == display).Name;
            Type type;
            try
            {
                type = typeof(Reimu).Assembly.GetTypes().First(t => t.Name == name);
            }
            catch (Exception)
            {
                try
                {
                    type = Assemblies.SelectMany(a => a.GetTypes()).First(t => t.Name == name);
                }
                catch
                {
                    Game.ErrorMessageBox("未找到包含该角色的程序集，请联系开发者。");
                    return;
                }
            }
            game.AddCharacter(point, group, type);
            menuBackout.IsEnabled = true;
            var labelTemp = game.LabelsGroup[(int)group + 1];
            labelTemp.Content = Convert.ToInt32(labelTemp.Content) + 1;
        }



        /// <summary>网格单击事件</summary>
        /// <param name="leftButton">鼠标左键状态</param>
        /// <param name="middleButton">鼠标中键状态</param>
        private void GridPadMouseDown(MouseButtonState leftButton, MouseButtonState middleButton)
        {
            if (!game.IsBattle)
            {
                #region 加人模式
                //如果没选角色Display则操作非法
                if (string.IsNullOrEmpty(comboBoxDisplay.Text))
                {
                    Game.IllegalMessageBox("请选择角色！");
                    return;
                }
                //如果这个位置已添加则操作非法
                if (game.Characters.Any(c => c.Position == game.MousePoint))
                {
                    Game.IllegalMessageBox("此位置已有角色！");
                    return;
                }
                //添加角色
                var group = (leftButton == MouseButtonState.Pressed)
                    ? Group.Friend
                    : ((middleButton == MouseButtonState.Pressed) ? Group.Middle : Group.Enemy);
                AddCharacter(game.MousePoint, group, comboBoxDisplay.Text);
                #endregion
            }
            else
            {
                #region 战斗模式
                #region 如果不是行动阶段则操作非法
                if (GameSection != Section.Round) return;
                #endregion
                if (game.ScSelect != 0)
                {
                    #region 符卡
                    //如果单击位置不合法
                    if (!game.HandleIsLegalClick(game.MousePoint))
                    {
                        Game.IllegalMessageBox("符卡选择位置非法");
                        return;
                    }
                    DoSC();
                    game.EndSC();
                    #endregion
                }
                else if (game.IsMoving)
                {
                    #region 如果正在移动中
                    if (!game.CanReachPoint[game.MouseColumn, game.MouseRow]) return;
                    #region 移动
                    CurrentCharacter.Move(game.MousePoint);
                    game.HasMoved = true;
                    game.IsMoving = false;
                    #region 绘制屏幕
                    game.DefaultButtonAndLabels();
                    if (game.HasMoved)
                    {
                        game.ResetPadButtons();
                    }
                    else
                    {
                        game.PaintButtons();
                    }
                    game.UpdateLabelBackground();
                    #endregion
                    #endregion
                    #region 如果同时已经攻击过则进入结束阶段
                    if (!game.HasAttacked || !game.HasMoved) return;
                    //Thread.Sleep(500);
                    game.EndSection();
                    #endregion
                    #endregion
                }
                else if (game.IsAttacking)
                {
                    #region 如果正在攻击中
                    if (game.MousePoint != CurrentCharacter.Position &&
                        !game.EnemyCanAttack.Contains(game.MouseCharacter)) return;
                    if (game.EnemyCanAttack.Contains(game.MouseCharacter))
                    {
                        //获取目标
                        var target = game.MouseCharacter;
                        //攻击
                        CurrentCharacter.HandleDoAttack(target);
                        //死人提示
                        game.HandleIsDead();
                    }
                    game.HasAttacked = true;
                    game.IsAttacking = false;
                    #region 绘制屏幕
                    if (game.HasAttacked)
                    {
                        game.EnemyCanAttack.SetLabelBackground(GameColor.LabelDefalutBackground);
                    }
                    game.Generate_CanReachPoint();
                    game.PaintButtons();
                    #endregion
                    #region 如果同时已经移动过则进入结束阶段
                    if (!game.HasAttacked || !game.HasMoved) return;
                    //Thread.Sleep(500);
                    game.EndSection();
                    #endregion
                    #endregion
                }
                else
                {
                    #region 单击位置非法，操作非法
                    Game.IllegalMessageBox("请先选择操作类型");
                    #endregion
                }
                #endregion
            }
        }

        /// <summary>随机添加角色</summary>
        /// <param name="group">角色阵营</param>
        /// <param name="number">添加数量</param>
        private void RandomlyAddCharacters(Group group, int number)
        {
            var points = new List<Point>();
            for (var i = 0; i < Game.Column; i++)
            {
                for (var j = 0; j < Game.Row; j++)
                {
                    points.Add(new Point(i, j));
                }
            }
            var pointsCanAdd = points.Where(p => game[p] == null).ToList();
            if (pointsCanAdd.Count < number)
            {
                MessageBox.Show("空格不足！", "添加失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            game.random.RandomElements(number, pointsCanAdd)
                .DoAction(p => AddCharacter(p, group, game.random.RandomElement(Calculate.CharacterDataList).Display));
            if (checkBox.IsChecked == false) return;
            MessageBox.Show("生成成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>清除所有角色</summary>
        private void ClearCharacters()
        {
            game.Characters.SelectMany(c => c.ListControls).DoAction(c => game.GridPad.Children.Remove(c));
            game.Characters.Clear();
            game.characterLastAdd = null;
            menuBackout.IsEnabled = false;
            game.ID = 1;
            game.LabelID.Content = "1";
            game.LabelsGroup.DoAction(l => l.Content = "0");
        }

        private void SC(int index)
        {
            if (GameSection != Section.Round) return;
            if (!game.IsSCing)
            {
                if (!CurrentCharacter.IsMpEnough(CurrentCharacter.SCMpUse[index - 1]))
                {
                    Game.IllegalMessageBox("灵力不足！");
                    return;
                }
                game.IsSCing = true;
                game.SC(index);
                if (game.HandleIsLegalClick != null) return;
                DoSC();
            }
            else
            {
                game.IsSCing = false;
                game.EndSC();
            }
        }

        private void DoSC()
        {
            #region 如果已攻击过则操作非法
            if (game.HasAttacked)
            {
                Game.IllegalMessageBox("已攻击过不能使用符卡");
                return;
            }
            #endregion
            game.HandleSelf?.Invoke();
            game.Characters.Where(c => game.HandleIsTargetLegal(c, game.MousePoint))
                .ToList().DoAction(c => game.HandleTarget(c));
            game.EndSC();
            game.HasAttacked = true;
            game.HandleIsDead();

            game.DefaultButtonAndLabels();
            game.Generate_CanReachPoint();
            game.PaintButtons();
            if (!game.HasAttacked)
            {
                game.UpdateLabelBackground();
            }
            #region 如果同时已经移动过则进入结束阶段
            if (!game.HasMoved) return;
            //Thread.Sleep(500);
            game.EndSection();
            #endregion
        }

        private void buttonJump_Click(object sender, RoutedEventArgs e)
        {
            if (GameSection != Section.Round || game.IsMoving || game.IsAttacking || game.IsSCing) return;
            game.DefaultButtonAndLabels();
            game.EndSection();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("是否退出？", "退出", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        #region Menus
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuClear_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确认清除所有角色？", "确认", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                ClearCharacters();
            }
        }

        private void menuPattern_Click(object sender, RoutedEventArgs e)
        {
            #region 合法性
            if (game.Characters.Count == 0)
            {
                Game.ErrorMessageBox("还未加人！");
                return;
            }
            if (!game.FriendCharacters.Any())
            {
                Game.ErrorMessageBox("还未加己方角色！");
                return;
            }
            if (!game.EnemyCharacters.Any())
            {
                Game.ErrorMessageBox("还未加敌方角色！");
                return;
            }
            #endregion
            #region 控件操作
            menuPattern.IsEnabled = false;
            menuBackout.IsEnabled = false;
            menuClear.IsEnabled = false;
            labelShow.Content = "战斗模式";
            label2.Visibility = Visibility.Hidden;
            game.LabelID.Visibility = Visibility.Hidden;
            comboBoxDisplay.Text = "";
            comboBoxDisplay.IsEnabled = false;
            comboBoxEnemy.IsEnabled = false;
            comboBoxEnemy.Text = "";
            comboBoxFriend.IsEnabled = false;
            comboBoxFriend.Text = "";
            comboBoxMiddle.IsEnabled = false;
            comboBoxMiddle.Text = "";
            buttonGenerateFriend.IsEnabled = false;
            buttonGenerateEnemy.IsEnabled = false;
            buttonGenerateMiddle.IsEnabled = false;
            buttonSave.IsEnabled = false;
            buttonLoad.IsEnabled = false;
            game.ButtonAttack.IsEnabled = true;
            game.ButtonMove.IsEnabled = true;
            buttonJump.IsEnabled = true;
            game.ButtonSC.DoAction(b => b.IsEnabled = true);
            labelShow.Foreground = Brushes.Black;
            game.TurnToBattle();
            game.PreparingSection();
            #endregion
        }

        private void menuBackout_Click(object sender, RoutedEventArgs e)
        {
            if (game.characterLastAdd == null) return;
            game.RemoveCharacter(game.characterLastAdd);
            game.ID--;
            if (game.Characters.Count == 0)
            {
                game.characterLastAdd = null;
                menuBackout.IsEnabled = false;
            }
            else
            {
                game.characterLastAdd = game.Characters.Last();
            }
        }

        private void menuHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(CurrentPath + @"\Resources\Help\Operators.txt");
            }
            catch
            {
                Game.ErrorMessageBox(jlq_MBE_BattleSimulation.Properties.Resources.HelpError);
            }
        }

        private void menuShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(CurrentPath + @"\Resources\Help\Show.txt");
            }
            catch
            {
                Game.ErrorMessageBox(jlq_MBE_BattleSimulation.Properties.Resources.HelpError);
            }
        }
        #endregion

        #region 随机生成
        private void buttonGenerateFriend_Click(object sender, RoutedEventArgs e)
        {
            RandomlyAddCharacters(Group.Friend, int.Parse(comboBoxFriend.Text));
        }

        private void buttonGenerateEnemy_Click(object sender, RoutedEventArgs e)
        {
            RandomlyAddCharacters(Group.Enemy, int.Parse(comboBoxEnemy.Text));
        }

        private void buttonGenerateMiddle_Click(object sender, RoutedEventArgs e)
        {
            RandomlyAddCharacters(Group.Middle, int.Parse(comboBoxMiddle.Text));
        }
        #endregion

        #region Loaded
        private void borderPad_Loaded(object sender, RoutedEventArgs e)
        {
            borderPad.Child = game.GridPad;
        }

        private void gridWindow_Loaded(object sender, RoutedEventArgs e)
        {
            gridWindow.Children.Add(game.LabelSection);
        }

        private void gridGame_Loaded(object sender, RoutedEventArgs e)
        {
            gridGame.Children.Add(game.ButtonAttack);
            gridGame.Children.Add(game.ButtonMove);
        }

        private void gridCount_Loaded(object sender, RoutedEventArgs e)
        {
            game.LabelsGroup.DoAction(l => gridCount.Children.Add(l));
        }

        private void gridSC01_Loaded(object sender, RoutedEventArgs e)
        {
            gridSC01.Children.Add(game.ButtonSC[0]);
            gridSC02.Children.Add(game.ButtonSC[1]);
            gridSC03.Children.Add(game.ButtonSC[2]);
            for (var i = 0; i < 3; i++)
            {
                var j = i + 1;
                game.ButtonSC[i].Click += (s, ev) =>
                {
                    if (game.IsMoving || game.IsAttacking) return;
                    SC(j);
                };
            }
        }

        private void gridGameInformation_Loaded(object sender, RoutedEventArgs e)
        {
            gridGameInformation.Children.Add(game.LabelID);
        }
        #endregion

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!game.Characters.Any())
            {
                Game.ErrorMessageBox("还未添加角色！");
                return;
            }
            var pathChoosing = new Dialog_ChoosePath(game);
            pathChoosing.ShowDialog();
        }

        private void buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = game.LoadPath,
                Title = "选择棋盘文件(*.pad)",
                Multiselect = false,
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            var result = MessageBox.Show("将清空棋盘，确定？", "确认", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result != MessageBoxResult.OK) return;
            ClearCharacters();
            var index = dialog.FileName.LastIndexOf('\\');
            game.LoadPath = dialog.FileName.Substring(0, index + 1);
            var p = new Point(0, 0);
            const Group g = Group.Friend;
            var d = Calculate.CharacterDataList.First(cd => cd.Name == "Reimu");
            var formatter = new BinaryFormatter();
            for (var i = 1; Directory.Exists(game.LoadPath + i); i++)
            {
                var pathc = game.LoadPath + i + "\\";
                Point pp;
                if (File.Exists(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Position))
                {
                    using (Stream reader = File.OpenRead(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Position))
                    {
                        pp = (Point)formatter.Deserialize(reader);
                    }
                }
                else
                {
                    pp = p;
                    p = p.Y == Game.Row - 1 ? new Point(p.X + 1, 0) : new Point(p.X, p.Y + 1);
                }
                Group gg;
                if (File.Exists(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Group))
                {
                    using (Stream reader = File.OpenRead(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Group))
                    {
                        gg = (Group)formatter.Deserialize(reader);
                    }
                }
                else
                {
                    gg = g;
                }
                CharacterData cd;
                if (File.Exists(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Data))
                {
                    using (Stream reader = File.OpenRead(pathc + jlq_MBE_BattleSimulation.Properties.Resources.Data))
                    {
                        cd = (CharacterData)formatter.Deserialize(reader);
                    }
                }
                else
                {
                    cd = d;
                }
                AddCharacter(pp, gg, cd.Display);
            }
            MessageBox.Show("载入成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

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
using ExceptionHelper;
using FileHelper;
using jlq_MBE_BattleSimulation.Dialogs;
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

            #region 会在ComboBox中显示的数据
            var data = new XmlDocument();
            IEnumerable<string> xmls;
            try
            {
                xmls = Directory.GetFiles(CurrentPath + "\\Resources\\Data\\ShowInComboBox")
                    .GetPathsWithFileExtension(".xml");
            }
            catch (Exception)
            {
                Game.ErrorMessageBox("找不到数据文件，程序无法运行。请联系开发者。");
                this.Close();
                return;
            }
            Game.CharacterDataListShow = DataLoader.LoadDatas(xmls);
            if (!Game.CharacterDataListShow.Any())
            {
                Game.ErrorMessageBox("找不到数据文件，程序无法运行。请联系开发者。");
                this.Close();
                return;
            }
            Game.CharacterDataListShow.DoAction(cd => comboBoxDisplay.Items.Add(cd.Display));
            #endregion
            #region 不会在ComboBox显示的数据
            var unshowPath = CurrentPath + "\\Resources\\Data\\Unshow";
            if (!Directory.Exists(unshowPath)) Directory.CreateDirectory(unshowPath);
            xmls = Directory.GetFiles(unshowPath).GetPathsWithFileExtension(".xml");
            Game.CharacterDataListUnshow = DataLoader.LoadDatas(xmls);
            #endregion

            #endregion

            #region 初始化game对象

            game.EventGridPadClick += GridPadMouseDown;

            #endregion

            #region 界面初始化
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
            labelFriend.Content = 1;
            labelMiddle.Content = 1;
            labelEnemy.Content = 1;
            #endregion

            #region 样式
            var roundStyle = buttonJump.Style;
            game.ButtonAttack.Style = roundStyle;
            game.ButtonMove.Style = roundStyle;
            #endregion

            #region 加载mods

            var p = CurrentPath + "\\Resources\\Mods";
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
            try
            {
                foreach (var assembly in Directory.GetFiles(p).GetPathsWithFileExtension(".dll"))
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

        #region SaveLoad相关
        /// <summary>当前加载的棋盘地址</summary>
        private string PadLoadedNow { get; set; }

        /// <summary>是否保存当前棋盘</summary>
        private bool HasSaved { get; set; }
        
        /// <summary>是否可撤销添加角色</summary>
        private bool CanBackout { get; set; }
        #endregion

        /// <summary>添加角色</summary>
        /// <param name="point">添加的位置</param>
        /// <param name="group">角色的阵营</param>
        /// <param name="display">显示的字符串</param>
        private void AddCharacter(Point point, Group group, string display)
        {
            var name = Game.CharacterDataListShow.First(c => c.Display == display).Name;
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
            CanBackout = true;
            var labelTemp = game.LabelsGroup[(int)group + 1];
            labelTemp.Content = (int)labelTemp.Content + 1;
            HasSaved = false;
        }
        /// <summary>添加角色</summary>
        /// <param name="info">添加的角色信息</param>
        private void AddCharacter(CharacterInfo info) => AddCharacter(info.Position, info.CGroup, info.Display);


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
                .DoAction(p => AddCharacter(p, group, game.random.RandomElement(Game.CharacterDataListShow).Display));
            if (checkBox.IsChecked == false) return;
            MessageBox.Show("生成成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>清除所有角色</summary>
        private void ClearCharacters()
        {
            game.Characters.SelectMany(c => c.ListControls).DoAction(c => game.GridPad.Children.Remove(c));
            game.Characters.Clear();
            game.CharacterLastAdd = null;
            CanBackout = false;
            game.ID = 1;
            game.LabelsGroup.DoAction(l => l.Content = 0);
            HasSaved = false;
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

        #region 窗体事件处理程序
        private void buttonJump_Click(object sender, RoutedEventArgs e)
        {
            if (GameSection != Section.Round || game.IsMoving || game.IsAttacking || game.IsSCing) return;
            game.DefaultButtonAndLabels();
            game.EndSection();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("是否退出？", "退出", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        #region Menus
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
            => RandomlyAddCharacters(Group.Friend, (int)labelFriend.Content);

        private void buttonGenerateEnemy_Click(object sender, RoutedEventArgs e)
            => RandomlyAddCharacters(Group.Enemy, (int)labelEnemy.Content);

        private void buttonGenerateMiddle_Click(object sender, RoutedEventArgs e)
            => RandomlyAddCharacters(Group.Middle, (int)labelMiddle.Content);

        private void LabelRandom_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var label = sender as Label;
            var i = (int)label.Content;
            label.Content = e.Delta > 0 ? Math.Min(10, i + 1) : Math.Max(1, i - 1);
        }
        #endregion

        #region Loaded
        private void borderPad_Loaded(object sender, RoutedEventArgs e)
            => borderPad.Child = game.GridPad;

        private void gridGame_Loaded(object sender, RoutedEventArgs e)
        {
            gridGame.Children.Add(game.ButtonAttack);
            gridGame.Children.Add(game.ButtonMove);
        }

        private void gridCount_Loaded(object sender, RoutedEventArgs e)
            => game.LabelsGroup.DoAction(l => gridCount.Children.Add(l));

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
                    if (game.IsMoving || game.IsAttacking)
                    {
                        game.EndSC();
                        return;
                    }
                    SC(j);
                };
            }
        }

        private void gridGameInformation_Loaded(object sender, RoutedEventArgs e)
            => gridGameInformation.Children.Add(game.LabelID);

        private void gridSection_Loaded(object sender, RoutedEventArgs e) => gridSection.Children.Add(game.LabelSection);
        #endregion
        #endregion

        #region 命令
        #region Execute
        private void Command_New(object sender, RoutedEventArgs e)
        {
            if (!game.Characters.Any()) return;
            if (!HasSaved)
            {
                var result = MessageBox.Show("将清空棋盘，确定？", "确认", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result != MessageBoxResult.OK) return;
            }
            ClearCharacters();
        }

        private void Command_Save(object sender, RoutedEventArgs e)
        {
            if (PadLoadedNow == null)
            {
                Command_SaveAs(sender, e);
            }
            else
            {
                if (HasSaved) return;
                var formatter = new BinaryFormatter();
                using (var writer = File.OpenWrite(PadLoadedNow))
                {
                    formatter.Serialize(writer, game.CInfos);
                }
                Thread.Sleep(2000);
                MessageBox.Show("保存成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                HasSaved = true;
            }
        }

        private void Command_SaveAs(object sender, RoutedEventArgs e)
        {
            if (!game.Characters.Any())
            {
                Game.ErrorMessageBox("还未添加角色！");
                return;
            }
            var pathChoosing = new Dialog_ChoosePath(game);
            var result = pathChoosing.ShowDialog();
            if (result != true) return;
            HasSaved = true;
            PadLoadedNow = pathChoosing.FileSavePath;
        }

        private void Command_Open(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = game.LoadPath,
                Title = "选择棋盘文件(*.pad)",
                Multiselect = false,
                Filter = "棋盘文件|*.pad"
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            if (game.Characters.Any() && !HasSaved)
            {
                var result = MessageBox.Show("将清空棋盘，确定？", "确认", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result != MessageBoxResult.OK) return;
                ClearCharacters();
            }
            var formatter = new BinaryFormatter();
            using (var reader = File.OpenRead(dialog.FileName))
            {
                try
                {
                    var infos = formatter.Deserialize(reader) as List<CharacterInfo>;
                    foreach (var info in infos)
                    {
                        AddCharacter(info);
                    }
                    game.LoadPath = dialog.FileName;
                    PadLoadedNow = dialog.FileName;
                    HasSaved = true;
                    MessageBox.Show("载入成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ex.Log();
                    return;
                }
            }
        }

        private void Command_Exit(object sender, RoutedEventArgs e) => this.Close();

        private void Command_TurnBattle(object seder, RoutedEventArgs e)
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
            CanBackout = false;
            labelShow.Content = "战斗模式";
            labelStaticID.Visibility = Visibility.Hidden;
            game.LabelID.Visibility = Visibility.Hidden;
            comboBoxDisplay.Text = "";
            comboBoxDisplay.IsEnabled = false;
            expanderGenerate.IsExpanded = false;
            expanderGenerate.IsEnabled = false;
            expanderSaveLoad.IsExpanded = false;
            expanderSaveLoad.IsEnabled = false;
            labelEnemy.IsEnabled = false;
            labelEnemy.Content = "";
            labelFriend.IsEnabled = false;
            labelFriend.Content = "";
            labelMiddle.IsEnabled = false;
            labelMiddle.Content = "";
            buttonGenerateFriend.IsEnabled = false;
            buttonGenerateEnemy.IsEnabled = false;
            buttonGenerateMiddle.IsEnabled = false;
            game.ButtonAttack.IsEnabled = true;
            game.ButtonMove.IsEnabled = true;
            buttonJump.IsEnabled = true;
            game.ButtonSC.DoAction(b => b.IsEnabled = true);
            labelShow.Foreground = Brushes.Black;
            game.TurnToBattle();
            game.PreparingSection();
            #endregion
        }

        private void Command_Undo(object seder, RoutedEventArgs e)
        {
            if (game.CharacterLastAdd == null) return;
            game.RemoveCharacter(game.CharacterLastAdd);
            game.ID--;
            if (!game.Characters.Any())
            {
                game.CharacterLastAdd = null;
                CanBackout = false;
            }
            else
            {
                game.CharacterLastAdd = game.Characters.Last();
            }
        }

        private void Command_Clear(object seder, RoutedEventArgs e)
        {
            if (!game.Characters.Any()) return;
            var result = MessageBox.Show("确认清除所有角色？", "确认", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                ClearCharacters();
            }
        }

        private void Command_Mods(object seder, RoutedEventArgs e)
        {
            var mods = Assemblies.Aggregate(string.Format("已加载的Mods：{0}个。", Assemblies.Count),
                (s, a) => s += ("\n" + a.GetName()));
            MessageBox.Show(mods, "已加载的Mods", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Command_Settings(object seder, RoutedEventArgs e)
        {
            return;
            var dialog = new Dialog_Settings(game);
            var result = dialog.ShowDialog();
            if (result != true) return;
            //TODO Options
        }
        #endregion
        #region CanExecute
        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !game.IsBattle;

        private void Command_AllCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void Command_CanBackout(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = CanBackout;
        #endregion
        #endregion
    }
}

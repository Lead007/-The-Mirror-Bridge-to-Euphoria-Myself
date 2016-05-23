using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using Data;
using JLQ_MBE_BattleSimulation.Dialogs;
using RandomHelper;
using static JLQ_MBE_BattleSimulation.GameColor;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>game对象</summary>
        private readonly Game game;

        /// <summary>构造函数</summary>
        public MainWindow()
        {
            InitializeComponent();

            #region 读取角色各数据
            var data = new XmlDocument();
            var reader = XmlReader.Create("Resources/Data/data.xml", new XmlReaderSettings {IgnoreComments = true /*忽略注释*/});
            data.Load(reader);
            Calculate.CharacterDataList = DataLoader.LoadDatas(data);
            reader.Close();
            foreach (var cd in Calculate.CharacterDataList)
            {
                comboBoxDisplay.Items.Add(cd.Display);
            }
            comboBoxDisplay.Items.Remove("芙分");
            #endregion

            #region 初始化颜色
            GenerateColors();
            #endregion
            #region 初始化game对象
            game = new Game();
            game.EventGridPadClick += GridPadMouseDown;
            #endregion

            #region 生成棋盘按钮事件
            foreach (var button in game.Buttons)
            {
                var column = (int)button.GetValue(Grid.ColumnProperty);
                var row = (int)button.GetValue(Grid.RowProperty);
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
                button.MouseEnter += (s, ev) =>
                {
                    game.MousePoint = new Point(column, row);
                };
                button.MouseLeave += (s, ev) =>
                {
                    game.MousePoint = new Point(-1, -1);
                };
                button.KeyDown += (s, ev) =>
                {
                    //如果shift和ctrl都没被按下或不在行动阶段或不在棋盘内或该点无角色或该点角色为当前角色则无效
                    if ((!(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
                           Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) ||
                        GameSection != JLQ_MBE_BattleSimulation.Section.Round || game.MousePoint == new Point(-1, -1) ||
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
                        character.LabelDisplay.Background = LabelBackground;
                    }
                    //如果ctrl被按下
                    else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        var character = game.MouseCharacter;
                        if (character == null) return;
                        //清屏
                        game.DefaultButtonAndLabels();

                        game.SetButtonBackground(game.MousePoint, character.MoveAbility);
                        character.LabelDisplay.Background = LabelBackground;
                    }

                };
                button.KeyUp += (s, ev) =>
                {
                    //如果不在行动阶段或仍有shift或ctrl在棋盘内则无效
                    if (GameSection != JLQ_MBE_BattleSimulation.Section.Round) return;
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) ||
                        Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) return;
                    //恢复原本显示
                    game.ResetShow();
                };
            }
            #endregion
        }

        /// <summary>当前行动角色</summary>
        private Character CurrentCharacter => game.CurrentCharacter;
        /// <summary>当前游戏阶段</summary>
        private Section? GameSection => game.GameSection;

        /// <summary>添加角色</summary>
        /// <param name="point">添加的位置</param>
        /// <param name="group">角色的阵营</param>
        /// <param name="display">显示的字符串</param>
        private void AddCharacter(Point point, Group group, string display)
        {
            game.AddCharacter(point, group, display);
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
                if (String.IsNullOrEmpty(comboBoxDisplay.Text))
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
                if (GameSection != JLQ_MBE_BattleSimulation.Section.Round) return;
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
                        game.EnemyCanAttack.Aggregate(BaseColor, (cu, c) => c.LabelDisplay.Background = LabelDefalutBackground);
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
            foreach (var p in game.random.RandomElements(number, pointsCanAdd))
            {
                AddCharacter(p, group, game.random.RandomElement(Calculate.CharacterDataList).Display);
            }
            if (checkBox.IsChecked == false) return;
            MessageBox.Show("生成成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>清除所有角色</summary>
        private void ClearCharacters()
        {
            IEnumerable<FrameworkElement> controls = new List<FrameworkElement>();
            controls = game.Characters.Select(c => c.ListControls).Aggregate(controls, (current, l) => current.Concat(l));
            foreach (var c in controls)
            {
                game.GridPad.Children.Remove(c);
            }
            game.Characters.Clear();
            game.characterLastAdd = null;
            menuBackout.IsEnabled = false;
            game.ID = 1;
            game.LabelID.Content = "1";
            foreach (var l in game.LabelsGroup)
            {
                l.Content = "0";
            }
        }

        private void SC(int index)
        {
            if (GameSection != JLQ_MBE_BattleSimulation.Section.Round) return;
            if (!game.IsSCing)
            {
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
            foreach (var c in game.Characters.Where(c => game.HandleIsTargetLegal(c, game.MousePoint)))
            {
                game.HandleTarget(c);
            }
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


        /// <summary>帮助-操作菜单</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuHelp_Click(object sender, RoutedEventArgs e)
        {
            //TODO Pause
            MessageBox.Show(
                "加人模式左键添加己方单位，中键添加中立单位，右键单击敌方单位；\n鼠标悬停在单位上方显示数据，按住shift微移鼠标显示单位详细信息；\n点击自身可跳过行动阶段；\n若暴击则会beep。", "操作",
                MessageBoxButton.OK, MessageBoxImage.Question);
            //TODO Continue
        }

        /// <summary>关闭时询问是否关闭窗口</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("是否退出？", "退出", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        /// <summary>退出菜单</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>清除已添加的所有单位</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClear_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确认清除所有角色？", "确认", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                ClearCharacters();
            }
        }

        /// <summary>模式切换</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            game.LabelGameTip.Visibility = Visibility.Visible;
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
            game.ButtonSC.Aggregate(false, (c, b) => b.IsEnabled = true);
            labelShow.Foreground = Brushes.Black;
            game.TurnToBattle();
            game.PreparingSection();
            #endregion
        }

        /// <summary>撤销上一次添加的角色</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void menuShow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "红色字体为己方单位，黑色字体为中立单位，绿色字体为敌方单位；\n" + "战斗模式下：\n淡粉色为当前行动单位；\n淡蓝色为当前行动单位可攻击的单位；\n淡黄色为可以移动至的位置\n" +
                "鼠标悬停在单位上方：\n按下Shift显示该角色的攻击范围；\n按下Ctrl显示该角色的移动范围。", "显示", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void buttonGenerateFriend_Click(object sender, RoutedEventArgs e)
        {
            RandomlyAddCharacters(Group.Friend, Int32.Parse(comboBoxFriend.Text));
        }

        private void buttonGenerateEnemy_Click(object sender, RoutedEventArgs e)
        {
            RandomlyAddCharacters(Group.Enemy, Int32.Parse(comboBoxEnemy.Text));
        }

        private void buttonGenerateMiddle_Click(object sender, RoutedEventArgs e)
        {
            RandomlyAddCharacters(Group.Middle, Int32.Parse(comboBoxMiddle.Text));
        }

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
            game.LabelsGroup.Aggregate(0, (c, l) => gridGame.Children.Add(l));
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
            gridGameInformation.Children.Add(game.LabelGameTip);
        }

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
                if (File.Exists(pathc + Properties.Resources.Position))
                {
                    using (Stream reader = File.OpenRead(pathc + Properties.Resources.Position))
                    {
                        pp = (Point) formatter.Deserialize(reader);
                    }
                }
                else
                {
                    pp = p;
                    p = p.Y == Game.Row - 1 ? new Point(p.X + 1, 0) : new Point(p.X, p.Y + 1);
                }
                Group gg;
                if (File.Exists(pathc + Properties.Resources.Group))
                {
                    using (Stream reader = File.OpenRead(pathc + Properties.Resources.Group))
                    {
                        gg = (Group)formatter.Deserialize(reader);
                    }
                }
                else
                {
                    gg = g;
                }
                CharacterData cd;
                if (File.Exists(pathc + Properties.Resources.Data))
                {
                    using (Stream reader = File.OpenRead(pathc + Properties.Resources.Data))
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

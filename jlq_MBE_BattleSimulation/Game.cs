using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media.TextFormatting;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>游戏类</summary>
    public class Game
    {
        /// <summary>默认点</summary>
        public static Point DefaultPoint => new Point(-1, -1);
        /// <summary>棋盘中心</summary>
        public static Point CenterPoint => new Point(4, 4);

        /// <summary>棋盘点集</summary>
        public static List<Point> PadPoints
        {
            get
            {
                var points = new List<Point>();
                for (var i = 0; i < MainWindow.Column; i++)
                    for (var j = 0; j < MainWindow.Row; j++)
                        points.Add(new Point(i, j));
                return points;
            }
        }

        /// <summary>随机数对象</summary>
        public Random Random = new Random();

        /// <summary>当前行动者</summary>
        public Character CurrentCharacter = null;

        /// <summary>是否为战斗模式</summary>
        public bool IsBattle { get; private set; }

        /// <summary>鼠标的网格位置</summary>
        public Point MousePoint { get; set; } = new Point(-1, -1);

        /// <summary>鼠标网格位置的Column值</summary>
        public int MouseColumn => (int) MousePoint.X;

        /// <summary>鼠标网格位置的Row值</summary>
        public int MouseRow => (int) MousePoint.Y;

        private Section? _section;

        /// <summary>当前回合所在的阶段</summary>
        public Section? Section
        {
            get { return _section; }
            set
            {
                _section = value;
                LabelSection.Content = Calculate.Convert(value);
            }
        }

        private int _id = 1;

        /// <summary>加人模式的当前ID</summary>
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                LabelID.Content = value.ToString();
            }
        }


        /// <summary>当前行动者是否已移动</summary>
        public bool HasMoved
        {
            get { return CurrentCharacter?.HasMoved ?? false; }
            set
            {
                if (CurrentCharacter == null) return;
                CurrentCharacter.HasMoved = value;
                LabelMove.Content = value ? "已移动" : "还未移动";
            }
        }

        /// <summary>当前行动者是否已攻击</summary>
        public bool HasAttacked
        {
            get { return CurrentCharacter?.HasAttacked ?? false; }
            set
            {
                if (CurrentCharacter == null) return;
                CurrentCharacter.HasAttacked = value;
                LabelAttack.Content = value ? "已攻击" : "还未攻击";
                ButtonSC.Aggregate(false, (c, b) => b.IsEnabled = !value);
            }
        }


        /// <summary>游戏中所有角色列表</summary>
        public List<Character> Characters { get; } = new List<Character>();

        /// <summary>加人模式上一个添加的角色</summary>
        public Character characterLastAdd { get; set; }= null;


    /// <summary>每个格子能否被到达</summary>
        public bool[,] CanReachPoint { get; } = new bool[MainWindow.Column, MainWindow.Row];

        /// <summary>可能死亡的角色列表</summary>
        public List<AttackModel> CharactersMayDie { get; } = new List<AttackModel>();

        /// <summary>准备阶段是否继续或需等待单击</summary>
        public bool IsPreparingSectionContinue { get; set; } = true;
        /// <summary>结束阶段是否继续或需等待单击</summary>
        public bool IsEndSectionContinue { get; set; } = true;

        /// <summary>应用路径</summary>
        public string ApplicationPath { get; }
        /// <summary>资源文件夹路径</summary>
        public string ResourcePath { get; }
        /// <summary>Save按钮的路径</summary>
        public string SavePath { get; set; }
        /// <summary>保存次数</summary>
        public int SaveTimes { get; set; } = 1;
        /// <summary>Load按钮的路径</summary>
        public string LoadPath { get; set; }

        //窗体显示
        /// <summary>当前阶段</summary>
        public Label LabelSection { get; }
        /// <summary>是否已攻击</summary>
        public Label LabelAttack { get; }
        /// <summary>是否已移动</summary>
        public Label LabelMove { get; }
        /// <summary>用以响应鼠标事件的按钮</summary>
        public Button[,] Buttons { get; } = new Button[MainWindow.Column, MainWindow.Row];
        /// <summary>棋盘按钮单击事件</summary>
        public event DGridPadClick EventGridPadClick;
        /// <summary>符卡按钮</summary>
        public Button[] ButtonSC { get; }
        /// <summary>棋盘网格控件</summary>
        public Grid GridPad { get; }
        /// <summary>棋盘网格线</summary>
        private Border[,] borders { get; } = new Border[MainWindow.Column, MainWindow.Row];
        /// <summary>显示每阵营剩余人数的标签</summary>
        public Label[] LabelsGroup { get; } = new Label[3];
        /// <summary>显示当前添加ID的标签</summary>
        public Label LabelID { get; }
        /// <summary>游戏提示标签</summary>
        public Label LabelGameTip { get; }
        /// <summary>生成可到达点矩阵</summary>
        public DAssignPointCanReach HandleAssignPointCanReach;
        /// <summary>判断是否死亡</summary>
        public DIsDead HandleIsDead;


        //符卡相关
        /// <summary>传递参数，如何获取目标以及所需参数列表</summary>
        public DIsTargetLegal HandleIsTargetLegal;
        /// <summary>传递参数，如何处理自己</summary>
        public DHandleSelf HandleSelf;
        /// <summary>传递参数，如何处理目标</summary>
        public DHandleTarget HandleTarget;
        /// <summary>传递参数，判断单击位置是否有效</summary>
        public DIsLegalClick HandleIsLegalClick;
        /// <summary>当前符卡序号，0为不处于符卡状态</summary> 
        public int ScSelect { get; set; }

        /// <summary>Game类的构造函数</summary>
        public Game()
        {
            HandleAssignPointCanReach = AssignPointCanReach;
            HandleIsDead = IsDead;

            //LabelSection
            LabelSection = new Label
            {
                Content = "游戏还未开始",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(197, 47, 0, 0),
                Width = 115,
                FontWeight = FontWeights.SemiBold,
                Height = 25
            };
            LabelSection.SetValue(Grid.RowProperty, 2);
            var binding = new Binding
            {
                Source = LabelSection,
                Path = new PropertyPath("Content"),
                Converter = new ConverterContentToColor()
            };
            LabelSection.SetBinding(Label.ForegroundProperty, binding);
            //LabelMove
            LabelMove = new Label
            {
                Content = "还未移动",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 2, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 115,
                FontWeight = FontWeights.SemiBold,
                Height = 25,
            };
            LabelMove.SetValue(Grid.ColumnProperty, 1);
            LabelMove.SetValue(Grid.ColumnSpanProperty, 2);
            var binding2 = new Binding
            {
                Source = LabelMove,
                Path = new PropertyPath("Content"),
                Converter = new ConverterHasMovedToColor()
            };
            LabelMove.SetBinding(Label.ForegroundProperty, binding2);
            //LabelAttack
            LabelAttack = new Label
            {
                Content = "还未攻击",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 2, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 115,
                FontWeight = FontWeights.SemiBold,
                Height = 25,
            };
            LabelAttack.SetValue(Grid.ColumnProperty, 4);
            var binding3 = new Binding
            {
                Source = LabelAttack,
                Path = new PropertyPath("Content"),
                Converter = new ConverterHasAttackedToColor()
            };
            LabelAttack.SetBinding(Label.ForegroundProperty, binding3);
            //Buttons
            for (var i = 0; i < MainWindow.Column; i++)
            {
                for (var j = 0; j < MainWindow.Row; j++)
                {
                    Buttons[i, j] = new Button
                    {
                        Margin = new Thickness(1),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Background = Brushes.LightYellow,
                        Opacity = 0
                    };
                    Buttons[i, j].SetValue(Grid.ColumnProperty, i);
                    Buttons[i, j].SetValue(Grid.RowProperty, j);
                    Buttons[i, j].SetValue(Grid.ColumnSpanProperty, 1);
                    Buttons[i, j].SetValue(Grid.RowSpanProperty, 1);
                    Buttons[i, j].SetValue(Panel.ZIndexProperty, 1);
                    Buttons[i, j].MouseDown += (s, ev) =>
                    {
                        if (ev.LeftButton == MouseButtonState.Released)
                        {
                            EventGridPadClick(MouseButtonState.Released, ev.MiddleButton);
                        }
                    };
                    Buttons[i, j].Click += (s, ev) =>
                        EventGridPadClick(MouseButtonState.Pressed, MouseButtonState.Released);
                }
            }
            //ButtonSC
            ButtonSC = new Button[3];
            for (var i = 0; i < 3; i++)
            {
                ButtonSC[i] = new Button
                {
                    Content = "SC0" + (i + 1),
                    IsEnabled = false,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
            }
            //LabelsGroup
            for (var i = 0; i < 3; i++)
            {
                LabelsGroup[i] = new Label
                {
                    Content = 0,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top
                };
                LabelsGroup[i].SetValue(Grid.RowProperty, 1);
                LabelsGroup[i].SetValue(Grid.ColumnProperty, 5 - 2*i);
            }
            //PadBorders
            for (var i = 0; i < MainWindow.Column; i++)
            {
                for (var j = 0; j < MainWindow.Row; j++)
                {
                    //生成网格线
                    borders[i, j] = new Border
                    {
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        BorderThickness = new Thickness(1),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                    };
                    borders[i, j].SetValue(Grid.ColumnProperty, i);
                    borders[i, j].SetValue(Grid.RowProperty, j);
                    borders[i, j].SetValue(Grid.ColumnSpanProperty, 1);
                    borders[i, j].SetValue(Grid.RowSpanProperty, 1);
                    borders[i, j].SetValue(Panel.ZIndexProperty, 0);
                }
            }
            //LabelID
            LabelID = new Label
            {
                Content = "1",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontWeight = FontWeights.SemiBold
            };
            LabelID.SetValue(Grid.ColumnProperty, 1);
            LabelID.SetValue(Grid.RowProperty, 1);
            //LabelGameTip
            LabelGameTip = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Hidden,
                Foreground = Brushes.Blue
            };
            LabelGameTip.SetValue(Grid.RowProperty, 1);
            LabelGameTip.SetValue(Grid.ColumnSpanProperty, 2);
            //GridPad
            GridPad = new Grid();
            for (var i = 0; i < MainWindow.Column; i++)
            {
                GridPad.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (var i = 0; i < MainWindow.Row; i++)
            {
                GridPad.RowDefinitions.Add(new RowDefinition());
            }
            GridPad.Loaded += (sender, e) =>
            {
                foreach (var border in borders)
                {
                    GridPad.Children.Add(border);
                }
                foreach (var button in Buttons)
                {
                    GridPad.Children.Add(button);
                }
            };

            this.ApplicationPath = Directory.GetCurrentDirectory();
            this.ResourcePath = ApplicationPath + @"\Resources";
            this.SavePath = ApplicationPath;
            this.LoadPath = ApplicationPath;
            if (this.SavePath.Last() != '\\') this.SavePath += "\\";
        }

        /// <summary>确定在某位置的角色，若没有则返回null</summary>
        /// <param name="position">需要搜索角色的位置</param>
        /// <returns>在该位置的角色</returns>
        public Character this[Point position] => Characters.FirstOrDefault(c => c.Position == position);

        //当前行动者属性

        /// <summary>当前行动者的位置</summary>
        public Point CurrentPosition => CurrentCharacter.Position;
        /// <summary>当前行动者的scName</summary>
        public string[] ScName => CurrentCharacter.Data.ScName;
        /// <summary>当前行动者的scDisc</summary>
        public string[] ScDisc => CurrentCharacter.Data.ScDisc;

        /// <summary>友军列表</summary>
        public IEnumerable<Character> FriendCharacters => Characters.Where(c => c.Group == Group.Friend);
        /// <summary>中立列表</summary>
        public IEnumerable<Character> MiddleCharacters => Characters.Where(c => c.Group == Group.Middle);
        /// <summary>敌军列表</summary>
        public IEnumerable<Character> EnemyCharacters => Characters.Where(c => c.Group == Group.Enemy);

        /// <summary>攻击范围内的可攻击角色</summary>
        public IEnumerable<Character> EnemyCanAttack =>
            EnemyAsCurrent.Where(c => Calculate.Distance(CurrentCharacter, c) <= CurrentCharacter.AttackRange);

        /// <summary>对当前行动者的阻挡列表</summary>
        public virtual IEnumerable<Point> EnemyBlock => CurrentCharacter.EnemyBlock;

        /// <summary>对当前行动者的敌人列表</summary>
        public IEnumerable<Character> EnemyAsCurrent => CurrentCharacter.Enemy;
        /// <summary>棋盘按钮数组的一维数组形式</summary>
        public Button[] ArrayButtons
        {
            get
            {
                var result = new Button[MainWindow.Column * MainWindow.Row];
                for(var i = 0; i < MainWindow.Column; i++) 
                    for (var j = 0; j < MainWindow.Row; j++)
                    {
                        result[j*MainWindow.Column + i] = Buttons[i, j];
                    }
                return result;
            }
        }

        /// <summary>添加角色</summary>
        /// <param name="point">添加的位置</param>
        /// <param name="group">角色的阵营</param>
        /// <param name="display">显示的字符串</param>
        public void AddCharacter(Point point, Group group, string display)
        {
            //以下你肯定凌乱了不过就是调用对应的构造函数创建角色对象而已
            var characterData = Calculate.CharacterDataList.First(cd => cd.Display == display);
            Type[] constructorTypes =
            {
                    typeof (int), typeof (Point), typeof (Group), typeof (Random), typeof (Game)
                };
            object[] parameters = { ID, point, group, Random, this };
            characterLastAdd =
                (Character)
                    Type.GetType("JLQ_MBE_BattleSimulation." + characterData.Name).GetConstructors()[0].Invoke(
                        parameters);
            //各种加入列表
            characterLastAdd.ListControls.Aggregate(0, (cu, c) => GridPad.Children.Add(c));
            Characters.Add(characterLastAdd);
            ID++;
        }

        /// <summary>移除角色</summary>
        /// <param name="target">待移除的角色</param>
        public void RemoveCharacter(Character target)
        {
            var labelTemp = LabelsGroup[(int)target.Group + 1];
            labelTemp.Content = Convert.ToInt32(labelTemp.Content) - 1;
            foreach (var c in target.ListControls)
            {
                GridPad.Children.Remove(c);
            }
            Characters.Remove(target);
        }

        /// <summary>更新下个行动的角色,取currentTime最小的角色中Interval最大的角色中的随机一个</summary>
        public void GetNextRoundCharacter()
        {
            CurrentCharacter?.ResetSCShow();
            //currentCharacter?.Reset();
            HasMoved = false;
            HasAttacked = false;
            var stack = new Stack<Character>();
            stack.Push(Characters.First());
            foreach (var character in Characters)
            {
                var temp = stack.Peek();
                if (character.CurrentTime < temp.CurrentTime)
                {
                    stack.Clear();
                    stack.Push(character);
                }
                else if (character.CurrentTime == temp.CurrentTime)
                {
                    if (character.Interval > temp.Interval)
                    {
                        stack.Clear();
                        stack.Push(character);
                    }
                    else if (character.Interval == temp.Interval)
                    {
                        stack.Push(character);
                    }
                }
            }
            var i = Random.Next(stack.Count);
            CurrentCharacter = stack.ElementAt(i);
            UpdateLabelBackground();

            var ct = CurrentCharacter.CurrentTime;
            Characters.Aggregate(0, (cu, c) => c.CurrentTime -= ct);
            var tempList = (from l in Characters.Select(c => c.BuffList) from b in l where b.Round(ct) select b).ToList();
            foreach (var b in tempList)
            {
                b.BuffEnd();
            }
            CurrentCharacter.CurrentTime = CurrentCharacter.Interval;

            Generate_CanReachPoint();

            CurrentCharacter.SCShow();
        }

        /// <summary>死亡结算</summary>
        public void IsDead()
        {
            foreach (var model in CharactersMayDie.Where(m => m.Target.IsDead))
            {
                if (model.Attacker != null)
                {
                    MessageBox.Show(
                        string.Format("{0}号{1}{2}被{3}号{4}{5}杀死", model.Target.ID, Calculate.Convert(model.Target.Group),
                            model.Target.Name, model.Attacker.ID, Calculate.Convert(model.Attacker.Group),
                            model.Attacker.Name), "死亡", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(
                        string.Format("{0}号{1}{2}被无来源伤害杀死", model.Target.ID, Calculate.Convert(model.Target.Group),
                            model.Target.Name), "死亡", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                RemoveCharacter(model.Target);
            }
            //游戏是否结束
            if (!FriendCharacters.Any())
            {
                MessageBox.Show("敌方获胜！", "游戏结束", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (EnemyCharacters.Any()) return;
            MessageBox.Show("己方获胜！", "游戏结束", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary>格子的文字显示</summary>
        /// <param name="position">格子</param>
        /// <returns>文字显示</returns>
        public string StringShow(Point position)
        {
            return Characters.FirstOrDefault(c => c.Position == position)?.ToString() ?? null;
        }
        /// <summary>格子的信息提示</summary>
        /// <param name="position">格子</param>
        /// <returns>信息提示</returns>
        public string TipShow(Point position)
        {
            if (CurrentCharacter == null) return null;
            return Characters.FirstOrDefault(c => c.Position == position)?.Tip(CurrentCharacter) ?? null;
        }

        /// <summary>错误提示</summary>
        /// <param name="message">提示信息</param>
        public static void ErrorMessageBox(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>非法操作提示</summary>
        /// <param name="message">提示信息</param>
        public static void IllegalMessageBox(string message)
        {
            MessageBox.Show(message, "操作非法", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>生成bool二维数组</summary>
        public void Generate_CanReachPoint()
        {
            for (var i = 0; i < MainWindow.Column; i++)
            {
                for (var j = 0; j < MainWindow.Row; j++)
                {
                    CanReachPoint[i, j] = false;
                }
            }
            HandleAssignPointCanReach(CurrentCharacter.Position, CurrentCharacter.MoveAbility);
            Characters.Where(c => c.Position != CurrentCharacter.Position)
                .Aggregate(false, (cu, c) => CanReachPoint[c.X, c.Y] = false);
        }

        /// <summary>将所有可以到达的点在bool二维数组中置为true</summary>
        /// <param name="origin">起点</param>
        /// <param name="step">步数</param>
        private void AssignPointCanReach(Point origin, int step)
        {
            CanReachPoint[(int)origin.X, (int)origin.Y] = true;
            if (step == 0) return;
            var enm = this.EnemyBlock.ToList();
            if (origin.Y < MainWindow.Row - 1 && !enm.Contains(new Point(origin.X, origin.Y + 1)))
            {
                AssignPointCanReach(new Point(origin.X, origin.Y + 1), step - 1);
            }
            if (origin.Y > 0 && !enm.Contains(new Point(origin.X, origin.Y - 1)))
            {
                AssignPointCanReach(new Point(origin.X, origin.Y - 1), step - 1);
            }
            if (origin.X < MainWindow.Column - 1 && !enm.Contains(new Point(origin.X + 1, origin.Y)))
            {
                AssignPointCanReach(new Point(origin.X + 1, origin.Y), step - 1);
            }
            if (origin.X > 0 && !enm.Contains(new Point(origin.X - 1, origin.Y)))
            {
                AssignPointCanReach(new Point(origin.X - 1, origin.Y), step - 1);
            }
        }

        /// <summary>结算buff</summary>
        /// <param name="section">当前结算的阶段</param>
        public void BuffSettle(Section section)
        {
            foreach (var buff in
                    CurrentCharacter.BuffList.Where(buff => buff is BuffExecuteInSection)
                        .Cast<BuffExecuteInSection>()
                        .Where(buff => buff.ExecuteSection == section))
            {
                buff.BuffTrigger();
                Thread.Sleep(200);
            }
        }

        /// <summary>获得对应点的按钮</summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Button GetButton(Point point)
        {
            return Buttons[(int) point.X, (int) point.Y];
        }

        /// <summary>改为战斗模式</summary>
        public void TurnToBattle()
        {
            IsBattle = true;
        }

        //棋盘绘制相关
        /// <summary>恢复棋盘和角色正常颜色</summary>
        public void DefaultButtonAndLabels()
        {
            ResetPadButtons();
            Characters.Where(c => c != CurrentCharacter)
                .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.White);
            SetCurrentLabel();
        }

        /// <summary>生成与起始点距离小于等于范围的按钮的颜色</summary>
        /// <param name="origin">起始点</param>
        /// <param name="range">范围</param>
        public void SetButtonBackground(Point origin, int range)
        {
            PadPoints.Where(point => Calculate.Distance(point, origin) <= range && this[point] == null)
                .Aggregate(0.0, (c, point) => GetButton(point).Opacity = 1);
        }

        /// <summary>生成可到达点的按钮颜色</summary>
        public void PaintButton()
        {
            if (HasMoved) return;
            for (var i = 0; i < MainWindow.Column; i++)
            {
                for (var j = 0; j < MainWindow.Row; j++)
                {
                    if ((!CanReachPoint[i, j]) || CurrentPosition == new Point(i, j)) continue;
                    Buttons[i, j].Opacity = 1;
                }
            }
        }

        /// <summary>将全部棋盘按钮置透明</summary>
        public void ResetPadButtons()
        {
            ArrayButtons.Aggregate(0.0, (c, b) => b.Opacity = 0);
        }

        /// <summary>更新角色标签颜色</summary>
        public void UpdateLabelBackground()
        {
            Characters.Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.White);
            SetCurrentLabel();
            if (HasAttacked) return;
            EnemyCanAttack.Aggregate((Brush)Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
        }

        /// <summary>将当前行动角色标签颜色设为淡粉色</summary>
        public void SetCurrentLabel()
        {
            CurrentCharacter.LabelDisplay.Background = Brushes.LightPink;
        }

        /// <summary>恢复全盘显示</summary>
        public void ResetShow()
        {
            ResetPadButtons();
            PaintButton();
            UpdateLabelBackground();
        }

        //SC
        /// <summary>SC</summary>
        public void SC(int index)
        {
            ScSelect = index;
            ButtonSC[index - 1].Background = Brushes.Red;
            switch (index)
            {
                case 1:
                    CurrentCharacter.SC01();
                    return;
                case 2:
                    CurrentCharacter.SC02();
                    return;
                case 3:
                    CurrentCharacter.SC03();
                    return;
            }
        }

        /// <summary>结束符卡结算</summary>
        public void EndSC()
        {
            HandleIsTargetLegal = null;
            HandleTarget = null;
            HandleIsLegalClick = null;
            ButtonSC.Aggregate((Brush) Brushes.White, (c, b) => b.Background = Brushes.LightGray);
            LabelGameTip.Content = "";
            switch (ScSelect)
            {
                case 1:
                    CurrentCharacter.EndSC01();
                    break;
                case 2:
                    CurrentCharacter.EndSC02();
                    break;
                case 3:
                    CurrentCharacter.EndSC03();
                    break;
            }
            ScSelect = 0;
        }


        //TODO save&load

        /// <summary>播放声音</summary>
        /// <param name="uType"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool MessageBeep(uint uType);

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Data;
using ExceptionHelper;
using RandomHelper;
using JLQ_GameBase.Buffs;
using MoreEnumerable;
using static JLQ_GameBase.GameColor;

namespace JLQ_GameBase
{
    /// <summary>游戏类</summary>
    public class Game
    {
        #region 静态只读属性或常量
        /// <summary>棋盘列数</summary>
        public const int Column = 9;
        /// <summary>棋盘行数</summary>
        public const int Row = 9;

        /// <summary>默认点</summary>
        public static Point DefaultPoint { get; } = new Point(-1, -1);
        /// <summary>棋盘中心</summary>
        public static Point CenterPoint { get; } = new Point(4, 4);

        /// <summary>棋盘点集</summary>
        public static List<Point> PadPoints { get; } = new List<Point>();

        /// <summary>显示在ComboBox中的角色数据列表</summary>
        public static IEnumerable<CharacterData> CharacterDataListShow { get; set; } = new List<CharacterData>();
        /// <summary>不显示在ComboBox中的角色数据列表</summary>
        public static IEnumerable<CharacterData> CharacterDataListUnshow { get; set; } = new List<CharacterData>();
        /// <summary>所有角色列表</summary>
        public static IEnumerable<CharacterData> CharacterDatas => CharacterDataListShow.Concat(CharacterDataListUnshow); 
        static Game()
        {
            #region PadPoints
            for (var i = 0; i < Column; i++)
                for (var j = 0; j < Row; j++)
                    PadPoints.Add(new Point(i, j));
            #endregion
        }
        #endregion

        /// <summary>随机数对象</summary>
        public Random random { get; } = new Random();

        /// <summary>当前行动者</summary>
        public Character CurrentCharacter { get; set; } = null;

        /// <summary>是否为战斗模式</summary>
        public bool IsBattle { get; private set; }

        #region 鼠标位置相关
        /// <summary>鼠标的网格位置</summary>
        public Point MousePoint { get; set; } = Game.DefaultPoint;

        /// <summary>鼠标网格位置的Column值</summary>
        public int MouseColumn => (int)MousePoint.X;
        /// <summary>鼠标网格位置的Row值</summary>
        public int MouseRow => (int)MousePoint.Y;
        #endregion

        /// <summary>鼠标网格位置的角色</summary>
        public Character MouseCharacter => this[this.MousePoint];

        private Section? _section;

        /// <summary>当前回合所在的阶段</summary>
        public Section? GameSection
        {
            get { return _section; }
            private set
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
                LabelID.Content = value;
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
                ButtonMove.IsEnabled = !HasMoved;
                ButtonMove.Background = BaseColor;
            }
        }
        /// <summary>是否正在移动中</summary>
        public bool IsMoving { get; set; }

        /// <summary>当前行动者是否已攻击</summary>
        public bool HasAttacked
        {
            get { return CurrentCharacter?.HasAttacked ?? false; }
            set
            {
                if (CurrentCharacter == null) return;
                CurrentCharacter.HasAttacked = value;
                ButtonAttack.IsEnabled = !HasAttacked;
                ButtonAttack.Background = BaseColor;
                ButtonSC.DoAction(b => b.IsEnabled = !value);
            }
        }
        /// <summary>是否正在攻击中</summary>
        public bool IsAttacking { get; set; }

        /// <summary>游戏中所有角色列表</summary>
        public List<Character> Characters { get; } = new List<Character>();
        /// <summary>游戏中所有角色的信息列表</summary>
        public List<CharacterInfo> CInfos => Characters.Select(c => c.Info).ToList(); 

        /// <summary>加人模式上一个添加的角色</summary>
        public Character CharacterLastAdd { get; set; }

        /// <summary>每个格子能否被到达</summary>
        public bool[,] CanReachPoint { get; } = new bool[Column, Row];

        /// <summary>可能死亡的角色列表</summary>
        public List<AttackModel> CharactersMayDie { get; } = new List<AttackModel>();

        /// <summary>准备阶段是否继续或需等待单击</summary>
        public bool IsPreparingSectionContinue { get; set; } = true;
        /// <summary>结束阶段是否继续或需等待单击</summary>
        public bool IsEndSectionContinue { get; set; } = true;

        #region Save与Load相关
        /// <summary>应用路径</summary>
        public string ApplicationPath { get; } = Directory.GetCurrentDirectory();
        /// <summary>资源文件夹路径</summary>
        public string ResourcePath { get; }
        /// <summary>Save按钮的路径</summary>
        public string SavePath { get; set; }
        /// <summary>保存次数</summary>
        public int SaveTimes { get; set; } = 1;
        /// <summary>Load按钮的路径</summary>
        public string LoadPath { get; set; }
        #endregion

        #region 窗体显示
        /// <summary>当前阶段</summary>
        public Label LabelSection { get; }
        /// <summary>攻击按钮</summary>
        public Button ButtonAttack { get; }
        /// <summary>移动按钮</summary>
        public Button ButtonMove { get; }
        /// <summary>用以响应鼠标事件的按钮</summary>
        public Button[,] Buttons { get; } = new Button[Column, Row];
        /// <summary>棋盘按钮单击事件</summary>
        public event DGridPadClick EventGridPadClick;
        /// <summary>符卡按钮</summary>
        public Button[] ButtonSC { get; }
        /// <summary>棋盘网格控件</summary>
        public Grid GridPad { get; }
        /// <summary>棋盘网格线</summary>
        private Border[,] borders { get; } = new Border[Column, Row];
        /// <summary>显示每阵营剩余人数的标签</summary>
        public Label[] LabelsGroup { get; } = new Label[3];
        /// <summary>显示当前添加ID的标签</summary>
        public Label LabelID { get; }
        #endregion

        /// <summary>生成可到达点矩阵</summary>
        public Action<Point,int> HandleAssignPointCanReach { get; set; }
        /// <summary>判断是否死亡</summary>
        public DIsDead HandleIsDead { get; set; }

        private Action _handleResetShow;
        /// <summary>重置显示</summary>
        public Action HandleResetShow
        {
            get { return _handleResetShow; }
            set
            {
                _handleResetShow = value;
                _handleResetShow?.Invoke();
            }
        }


        #region 符卡相关
        /// <summary>所有符卡相关的委托</summary>
        public Delegate[] ScDelegates
            => new Delegate[] {HandleIsLegalClick, HandleIsTargetLegal, HandleSelf, HandleTarget};
        /// <summary>传递参数，判断单击位置是否有效</summary>
        public Func<Point, bool> HandleIsLegalClick { get; set; }
        /// <summary>传递参数，如何获取目标以及所需参数列表</summary>
        public Func<Character, Point, bool> HandleIsTargetLegal { get; set; }
        /// <summary>传递参数，如何处理自己</summary>
        public Action HandleSelf { get; set; }
        /// <summary>传递参数，如何处理目标</summary>
        public Action<Character> HandleTarget { get; set; }
        /// <summary>当前符卡序号，0为不处于符卡状态</summary> 
        public int ScSelect { get; set; }
        /// <summary>是否处于符卡状态</summary>
        public bool IsSCing { get; set; }
        #endregion

        /// <summary>Game类的构造函数</summary>
        public Game()
        {

            #region 窗体显示

            #region LabelSection

            LabelSection = new Label
            {
                Content = "游戏还未开始",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 115,
                FontWeight = FontWeights.SemiBold,
                Height = 25
            };
            LabelSection.SetValue(Grid.ColumnProperty, 1);
            var binding = new Binding
            {
                Source = LabelSection,
                Path = new PropertyPath("Content"),
                Converter = new ConverterContentToColor()
            };
            LabelSection.SetBinding(Label.ForegroundProperty, binding);

            #endregion

            #region ButtonMove

            ButtonMove = new Button
            {
                Content = "移动",
                IsEnabled = false,
            };
            ButtonMove.Click += (s, ev) =>
            {
                if (IsSCing || IsAttacking) return;
                ButtonMove.Background = IsMoving ? BaseColor : GameButtonLinearBrush;
                IsMoving = !IsMoving;
            };

            #endregion

            #region ButtonAttack

            ButtonAttack = new Button
            {
                Content = "攻击",
                IsEnabled = false
            };
            ButtonAttack.SetValue(Grid.ColumnProperty, 1);
            ButtonAttack.Click += (s, ev) =>
            {
                if (IsSCing || IsMoving) return;
                ButtonAttack.Background = IsAttacking ? BaseColor : GameButtonLinearBrush;
                IsAttacking = !IsAttacking;
            };

            #endregion

            #region Buttons

            for (var i = 0; i < Column; i++)
            {
                for (var j = 0; j < Row; j++)
                {
                    Buttons[i, j] = new Button
                    {
                        Margin = new Thickness(1),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Background = PadBackground,
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

            #endregion

            #region ButtonSC

            ButtonSC = new Button[3];
            for (var i = 0; i < 3; i++)
            {
                ButtonSC[i] = new Button
                {
                    Background = ScButtonDefaultBrush,
                    Content = "SC0" + (i + 1),
                    Foreground = Brushes.White,
                    FontSize = 18,
                    IsEnabled = false,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
            }

            #endregion

            #region LabelsGroup

            for (var i = 0; i < 3; i++)
            {
                LabelsGroup[i] = new Label
                {
                    Content = 0,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top
                };
                LabelsGroup[i].SetValue(Grid.ColumnProperty, 5 - 2*i);
            }

            #endregion

            #region PadBorders

            for (var i = 0; i < Column; i++)
            {
                for (var j = 0; j < Row; j++)
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

            #endregion

            #region LabelID

            LabelID = new Label
            {
                Content = "1",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold
            };
            LabelID.SetValue(Grid.ColumnProperty, 1);
            LabelID.SetValue(Grid.RowProperty, 1);

            #endregion

            #region GridPad

            GridPad = new Grid();
            for (var i = 0; i < Column; i++)
            {
                GridPad.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (var i = 0; i < Row; i++)
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

            #endregion

            #endregion

            #region Save与Load相关

            this.ResourcePath = ApplicationPath + @"\Resources";
            this.SavePath = ApplicationPath;
            this.LoadPath = ApplicationPath + @"\Resources\Pads";
            if (this.SavePath.Last() != '\\') this.SavePath += "\\";

            #endregion

            #region 初始化委托
            HandleAssignPointCanReach = AssignPointCanReach;
            HandleIsDead = IsDead;
            _handleResetShow = ResetShow;
            #endregion
        }

        /// <summary>确定在某位置的角色，若没有则返回null</summary>
        /// <param name="position">需要搜索角色的位置</param>
        /// <returns>在该位置的角色</returns>
        public Character this[Point position] => Characters.FirstOrDefault(c => c.Position == position);
        
        /// <summary>获取特定ID的角色，若没有则返回null</summary>
        /// <param name="ID">需要搜索角色的ID</param>
        /// <returns>该ID的角色</returns>
        public Character this[int ID] => ID < 1 ? null : Characters.FirstOrDefault(c => c.ID == ID);

        #region 当前行动者属性
        /// <summary>当前行动者的位置</summary>
        public Point CurrentPosition => CurrentCharacter.Position;
        /// <summary>当前行动者的scName</summary>
        public string[] ScName => CurrentCharacter.ScName;
        /// <summary>当前行动者的scDisc</summary>
        public string[] ScDisc => CurrentCharacter.ScDisc;
        #endregion

        #region 宏观整个游戏的各方列表
        /// <summary>友军列表</summary>
        public IEnumerable<Character> FriendCharacters => Characters.Where(c => c.Group == Group.Friend);
        /// <summary>中立列表</summary>
        public IEnumerable<Character> MiddleCharacters => Characters.Where(c => c.Group == Group.Middle);
        /// <summary>敌军列表</summary>
        public IEnumerable<Character> EnemyCharacters => Characters.Where(c => c.Group == Group.Enemy);
        #endregion

        /// <summary>当前角色攻击范围内的可攻击角色</summary>
        public IEnumerable<Character> EnemyCanAttack =>
            EnemyAsCurrent.Where(c => CurrentCharacter.Distance(c) <= CurrentCharacter.AttackRange);

        /// <summary>对当前行动者的阻挡列表</summary>
        public IEnumerable<Point> EnemyBlock => CurrentCharacter.EnemyBlock;
        /// <summary>对当前行动者的敌人列表</summary>
        public IEnumerable<Character> EnemyAsCurrent => CurrentCharacter.Enemies;

        /// <summary>棋盘按钮数组的一维数组形式</summary>
        public Button[] ArrayButtons
        {
            get
            {
                var result = new Button[Column * Row];
                for(var i = 0; i < Column; i++) 
                    for (var j = 0; j < Row; j++)
                    {
                        result[j*Column + i] = Buttons[i, j];
                    }
                return result;
            }
        }

        /// <summary>添加角色</summary>
        /// <param name="point">添加的位置</param>
        /// <param name="group">角色的阵营</param>
        /// <param name="cType">添加的角色类型</param>
        public void AddCharacter(Point point, Group group, Type cType)
        {
            #region 调用对应的构造函数创建角色对象而已
            try
            {
                object[] parameters = {ID, point, group, this};
                CharacterLastAdd = cType.GetConstructors()[0].Invoke(parameters) as Character;
                ID++;
            }
            catch (Exception ex)
            {
                ex.Log();
                return;
            }
            #endregion
            AddCharacterToList(CharacterLastAdd);
        }

        /// <summary>添加角色</summary>
        /// <param name="point">添加的位置</param>
        /// <param name="group">角色的阵营</param>
        /// <param name="cType">添加的角色类型</param>
        /// <param name="parameters">该角色的构造函数的参数列表</param>
        public void AddCharacter(Point point, Group group, Type cType, params object[] parameters)
        {
            #region 调用对应的构造函数创建角色对象而已
            try
            {
                CharacterLastAdd =
                    cType.GetConstructor(parameters.Select(o => o.GetType()).ToArray()).Invoke(parameters) as Character;
            }
            catch (Exception ex)
            {
                ex.Log();
                return;
            }
            #endregion
            AddCharacterToList(CharacterLastAdd);
        }

        private void AddCharacterToList(Character c)
        {
            CharacterLastAdd.ListControls.DoAction(cc => GridPad.Children.Add(cc));
            Characters.Add(CharacterLastAdd);
        }

        /// <summary>移除角色</summary>
        /// <param name="target">待移除的角色</param>
        public void RemoveCharacter(Character target)
        {
            var labelTemp = LabelsGroup[(int)target.Group + 1];
            labelTemp.Content = (int)labelTemp.Content - 1;
            target.ListControls.DoAction(c => GridPad.Children.Remove(c));
            Characters.Remove(target);
        }

        #region 游戏流程
        /// <summary>准备阶段</summary>
        public void PreparingSection()
        {
            #region 获取下个行动的角色
            GetNextRoundCharacter();
            for (var i = 0; i < 3; i++)
            {
                ButtonSC[i].Content = CurrentCharacter.ScName[i + 1];
                ButtonSC[i].ToolTip = CurrentCharacter.ScDisc[i + 1];
            }
            PaintButtons();
            #endregion

            #region 跳转阶段
            GameSection = Section.Preparing;
            BuffSettle(Section.Preparing);
            CurrentCharacter.HandlePreparingSection();
            if (!IsPreparingSectionContinue) return;
            //Thread.Sleep(500);
            GameSection = Section.Round;
            #endregion
        }
        /// <summary>结束阶段</summary>
        public void EndSection()
        {
            IsSCing = false;
            CharactersMayDie.Clear();
            GameSection = Section.End;
            BuffSettle(Section.End);
            //Thread.Sleep(1000);
            CurrentCharacter.HandleEndSection();
            if (!IsEndSectionContinue) return;
            CurrentCharacter?.ResetSCShow();
            HasMoved = false;
            HasAttacked = false;

            IsPreparingSectionContinue = true;
            IsEndSectionContinue = true;
            PreparingSection();
        }
        #endregion


        /// <summary>更新下个行动的角色,取currentTime最小的角色中Interval最大的角色中的随机一个</summary>
        public void GetNextRoundCharacter()
        {
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
            CurrentCharacter = random.RandomElement(stack);
            UpdateLabelBackground();

            var ct = CurrentCharacter.CurrentTime;
            Characters.DoAction(c => c.CurrentTime -= ct);
            var tempList = (from l in Characters.Select(c => c.BuffList) from b in l where b.Round(ct) select b).ToList();
            tempList.DoAction(b => b.BuffEnd());
            CurrentCharacter.CurrentTime = CurrentCharacter.Interval;

            Generate_CanReachPoint();

            CurrentCharacter.SCShow();
        }

        /// <summary>死亡结算</summary>
        public void IsDead()
        {
            foreach (var model in CharactersMayDie.Where(m => m.Target.IsDead).Distinct(new AttackModelEqualityComparer()))
            {
                var s = model.Attacker != null
                    ? string.Format("{0}号{1}{2}被{3}号{4}{5}杀死", model.Target.ID, Calculate.Convert(model.Target.Group),
                        model.Target.Name, model.Attacker.ID, Calculate.Convert(model.Attacker.Group),
                        model.Attacker.Name)
                    : string.Format("{0}号{1}{2}被无来源伤害杀死", model.Target.ID, Calculate.Convert(model.Target.Group),
                        model.Target.Name);
                MessageBox.Show(s, "死亡", MessageBoxButton.OK, MessageBoxImage.Warning);
                RemoveCharacter(model.Target);
            }
            #region 游戏是否结束
            if (!FriendCharacters.Any())
            {
                MessageBox.Show("敌方获胜！", "游戏结束", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (EnemyCharacters.Any()) return;
            MessageBox.Show("己方获胜！", "游戏结束", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            #endregion
        }

        /// <summary>格子的文字显示</summary>
        /// <param name="position">格子</param>
        /// <returns>文字显示</returns>
        public string StringShow(Point position) => this[position]?.ToString() ?? null;
        /// <summary>格子的信息提示</summary>
        /// <param name="position">格子</param>
        /// <returns>信息提示</returns>
        public string TipShow(Point position)
        {
            if (CurrentCharacter == null) return null;
            return this[position]?.Tip(CurrentCharacter) ?? null;
        }

        /// <summary>错误提示</summary>
        /// <param name="message">提示信息</param>
        public static void ErrorMessageBox(string message)
            => MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        /// <summary>非法操作提示</summary>
        /// <param name="message">提示信息</param>
        public static void IllegalMessageBox(string message)
            => MessageBox.Show(message, "操作非法", MessageBoxButton.OK, MessageBoxImage.Error);

        /// <summary>生成bool二维数组</summary>
        public void Generate_CanReachPoint()
        {
            for (var i = 0; i < Column; i++)
            {
                for (var j = 0; j < Row; j++)
                {
                    CanReachPoint[i, j] = false;
                }
            }
            HandleAssignPointCanReach(CurrentCharacter.Position, CurrentCharacter.MoveAbility);
            Characters.Where(c => c.Position != CurrentCharacter.Position).DoAction(c=> CanReachPoint[c.X, c.Y] = false);
        }

        /// <summary>将所有可以到达的点在bool二维数组中置为true</summary>
        /// <param name="origin">起点</param>
        /// <param name="step">步数</param>
        private void AssignPointCanReach(Point origin, int step)
        {
            CanReachPoint[(int)origin.X, (int)origin.Y] = true;
            if (step == 0) return;
            var enm = this.EnemyBlock.ToList();
            if (origin.Y < Row - 1 && !enm.Contains(new Point(origin.X, origin.Y + 1)))
            {
                AssignPointCanReach(new Point(origin.X, origin.Y + 1), step - 1);
            }
            if (origin.Y > 0 && !enm.Contains(new Point(origin.X, origin.Y - 1)))
            {
                AssignPointCanReach(new Point(origin.X, origin.Y - 1), step - 1);
            }
            if (origin.X < Column - 1 && !enm.Contains(new Point(origin.X + 1, origin.Y)))
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
            foreach (var buff in CurrentCharacter.BuffList.OfType<BuffExecuteInSection>()
                .Where(buff => buff.ExecuteSection == section))
            {
                buff.BuffTrigger();
                //Thread.Sleep(200);
            }
        }

        /// <summary>获得对应点的按钮</summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Button GetButton(Point point) => Buttons[(int)point.X, (int)point.Y];

        /// <summary>改为战斗模式</summary>
        public void TurnToBattle() => IsBattle = true;

        #region 棋盘绘制相关
        /// <summary>恢复棋盘和角色正常颜色</summary>
        public void DefaultButtonAndLabels()
        {
            ResetPadButtons();
            Characters.SetLabelBackground(LabelDefalutBackground);
            SetCurrentLabel();
        }

        /// <summary>生成与起始点距离小于等于范围的按钮的颜色</summary>
        /// <param name="origin">起始点</param>
        /// <param name="range">范围</param>
        public void SetButtonBackground(Point origin, int range)
            => PadPoints.Where(point => origin.IsInRange(point, range) && this[point] == null)
                .Select(GetButton)
                .SetButtonColor();

        /// <summary>生成可到达点的按钮颜色</summary>
        public void PaintButtons()
        {
            if (HasMoved) return;
            for (var i = 0; i < Column; i++)
            {
                for (var j = 0; j < Row; j++)
                {
                    if ((!CanReachPoint[i, j]) || CurrentPosition == new Point(i, j)) continue;
                    Buttons[i, j].SetButtonColor();
                }
            }
        }

        /// <summary>将全部棋盘按钮置透明</summary>
        public void ResetPadButtons() => ArrayButtons.ResetButtonColor();

        /// <summary>更新角色标签颜色</summary>
        public void UpdateLabelBackground()
        {
            Characters.SetLabelBackground(LabelDefalutBackground);
            SetCurrentLabel();
            if (HasAttacked) return;
            EnemyCanAttack.SetLabelBackground();
        }

        /// <summary>将当前行动角色标签颜色设为淡粉色</summary>
        public void SetCurrentLabel() => CurrentCharacter.SetLabelBackground(Brushes.LightPink);

        /// <summary>恢复全盘显示</summary>
        public void ResetShow()
        {
            ResetPadButtons();
            PaintButtons();
            UpdateLabelBackground();
        }
        #endregion

        #region 符卡
        /// <summary>SC</summary>
        public void SC(int index)
        {
            ScSelect = index;
            ButtonSC[index - 1].Background = ScButtonLinearBrush;
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
            ButtonSC.DoAction(b => b.Background = ScButtonDefaultBrush);
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
            IsSCing = false;
        }
        #endregion

        //TODO 播放声音
        /// <summary>播放声音</summary>
        /// <param name="uType"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool MessageBeep(uint uType);
    }
}

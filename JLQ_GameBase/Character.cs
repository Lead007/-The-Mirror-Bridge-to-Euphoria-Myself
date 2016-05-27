using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Data;
using Number;
using RandomHelper;

namespace JLQ_GameBase
{
    /// <summary>角色类</summary>
    public abstract class Character
    {
        //以下为字段
        #region 只读属性字段
        /// <summary>ID</summary>
        public int ID { get; }
        #region 角色数据
        /// <summary>角色数据</summary>
        private CharacterData Data { get; }
        /// <summary>角色名字</summary>
        public string Name => Data.Name;
        /// <summary>角色最大血量</summary>
        public int MaxHp => Data.MaxHp;
        /// <summary>角色显示</summary>
        public string Display => Data.Display;
        /// <summary>角色符卡名字</summary>
        public string[] ScName => Data.ScName;
        /// <summary>角色符卡提示</summary>
        public string[] ScDisc => Data.ScDisc;
        #endregion
        /// <summary>最大灵力</summary>
        public int MaxMp { get; }

        /// <summary>阵营</summary>
        public Group Group { get; }

        /// <summary>随机数对象</summary>
        protected Random random { get; }
        /// <summary>游戏对象</summary>
        protected Game game { get; }
        #endregion

        #region 角色数据增益与增量
        /// <summary>攻击增益</summary>
        private double _attackX = 1.0f;
        public double AttackX
        {
            set { _attackX *= (1 + value); }
        }
        /// <summary>攻击增量</summary>
        private int _attackAdd = 0;
        public int AttackAdd
        {
            set { _attackAdd += value; }
        }

        /// <summary>防御增益</summary>
        private double _defenceX = 1.0f;
        public double DefenceX
        {
            set { _defenceX *= (1 + value); }
        }
        /// <summary>防御增量</summary>
        public int _defenceAdd = 0;
        public int DefenceAdd
        {
            set { _defenceAdd += value; }
        }

        /// <summary>命中率增益</summary>
        private double _hitRateX = 1.0f;
        public double HitRateX
        {
            set { _hitRateX *= (1 + value); }
        }
        /// <summary>命中率增量</summary>
        private int _hitRateAdd = 0;
        public int HitRateAdd
        {
            set { _hitRateAdd += value; }
        }

        /// <summary>闪避率增益</summary>
        private double _dodgeRateX = 1.0f;
        public double DodgeRateX
        {
            set { _dodgeRateX *= (1 + value); }
        }
        /// <summary>闪避率增益</summary>
        private int _dodgeRateAdd = 0;
        public int DodgeRateAdd
        {
            set { _dodgeRateAdd += value; }
        }

        /// <summary>近战补正增益</summary>
        private double _closeAmendmentX = 1.0f;
        public double CloseAmendmentX
        {
            set { _closeAmendmentX *= (1 + value); }
        }
        /// <summary>近战补正增量</summary>
        private double _clostAmendmentAdd = 0;
        public double CloseAmendmentAdd
        {
            set { _clostAmendmentAdd += value; }
        }

        /// <summary>行动间隔增益</summary>
        private double __intervalX = 1.0f;
        private double _intervalX
        {
            get { return __intervalX; }
            set
            {
                __intervalX = value;
                CurrentTime = Math.Min(CurrentTime, Interval);
                BarTime.Maximum = Interval;
            }
        }
        public double IntervalX
        {
            set { _intervalX *= (1 + value); }
        }
        /// <summary>行动间隔增量</summary>
        private int __intervalAdd;
        private int _intervalAdd
        {
            get { return __intervalAdd; }
            set
            {
                var i = Interval;
                __intervalAdd = Math.Max(0, value);
                BarTime.Maximum = Interval;
                CurrentTime = Math.Max(0, CurrentTime - i + Interval);
            }
        }
        public int IntervalAdd
        {
            set { _intervalAdd += value; }
        }

        /// <summary>机动增量</summary>
        private int _moveAbilityAdd = 0;
        public int MoveAbilityAdd
        {
            set { _moveAbilityAdd += value; }
        }

        /// <summary>攻击范围增量</summary>
        private int _attackRangeAdd = 0;
        public int AttackRangeAdd
        {
            set { _attackRangeAdd += value; }
        }
        #endregion

        #region 与ProgressBar绑定的属性
        private int _hp;
        /// <summary>血量</summary>
        public int Hp
        {
            get { return _hp; }
            set
            {
                _hp = value;
                BarHp.Value = value;
            }
        }

        private int _mp;
        /// <summary>灵力</summary>
        public virtual int Mp
        {
            get { return _mp; }
            set
            {
                _mp = value;
                BarMp.Value = value;
            }
        }

        private int _currentTime;
        /// <summary>当前剩余冷却时间</summary>
        public int CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                BarTime.Value = value;
            }
        }
        #endregion

        /// <summary>位置，X为Grid.Column，Y为Grid.Row</summary>
        public Point Position { get; private set; }

        /// <summary>是否已移动</summary>
        public virtual bool HasMoved { get; set; }
        /// <summary>是否已攻击</summary>
        public virtual bool HasAttacked { get; set; }

        /// <summary>作为buff承受者的buff列表</summary>
        public List<Buff> BuffList { get; protected set; } = new List<Buff>();

        #region 显示
        /// <summary>所有在GUI上显示的控件</summary>
        public IEnumerable<FrameworkElement> ListControls => StaticControls.Concat(StateControls);
        /// <summary>固定显示的控件</summary>
        private List<FrameworkElement> StaticControls { get; } = new List<FrameworkElement>();
        /// <summary>显示Display的Label</summary>
        public Label LabelDisplay { get; }
        /// <summary>显示Hp的ProgressBar</summary>
        private ProgressBar BarHp { get; }
        /// <summary>显示Mp的ProgressBar</summary>
        private ProgressBar BarMp { get; }
        /// <summary>显示剩余时间的ProgressBar</summary>
        private ProgressBar BarTime { get; }
        /// <summary>显示状态的控件</summary>
        private List<FrameworkElement> StateControls { get; } = new List<FrameworkElement>();
        /// <summary>显示状态的控件</summary>
        public IEnumerable<FrameworkElement> StatesControls => StateControls; 
        #endregion


        #region 只读属性
        #region 游戏内角色数据
        /// <summary>攻击</summary>
        public int Attack => Math.Max(0, (Data.Attack*_attackX).Floor() + _attackAdd);
        /// <summary>防御</summary>
        public int Defence => Math.Max(0, (Data.Defence*_defenceX).Floor() + _defenceAdd);
        /// <summary>命中率</summary>
        public int HitRate => Math.Max(0, (Data.HitRate*_hitRateX).Floor() + _hitRateAdd);
        /// <summary>闪避率</summary>
        public int DodgeRate => Math.Max(0, (Data.DodgeRate*_dodgeRateX).Floor() + _dodgeRateAdd);
        /// <summary>近战补正</summary>
        public double CloseAmendment => Math.Max(0, Data.CloseAmendment*_closeAmendmentX + _clostAmendmentAdd);
        /// <summary>行动间隔</summary>
        public int Interval => Math.Max(1, (Data.Interval*_intervalX + _intervalAdd).Floor());
        /// <summary>默认Buff时间（3*Interval）</summary>
        public int BuffTime => 3*Interval;
        /// <summary>机动</summary>
        public int MoveAbility => Math.Max(0, Data.MoveAbility + _moveAbilityAdd);
        /// <summary>攻击范围</summary>
        public int AttackRange => Math.Max(0, Data.AttackRange + _attackRangeAdd);
        /// <summary>暴击增益</summary>
        protected float CriticalHitGain => 1.5f;
        /// <summary>暴击率</summary>
        protected float CriticalHitRate => 0.2f;
        /// <summary>伤害浮动</summary>
        protected float DamageFloat => 0.1f;
        #endregion
        /// <summary>是否死亡</summary>
        public bool IsDead => 0 >= Hp;
        /// <summary>Column坐标</summary>
        public int X => (int)this.Position.X;
        /// <summary>Row坐标</summary>
        public int Y => (int)this.Position.Y;

        /// <summary>对此角色而言的敌人列表</summary>
        public IEnumerable<Character> Enemy => game.Characters.Where(IsEnemy);
        /// <summary>阻挡行动的敌人列表</summary>
        public virtual IEnumerable<Point> EnemyBlock => HandleEnemyBlock(Enemy.Select(c => c.Position));
        #endregion

        #region 游戏相关委托
        /// <summary>攻击结算的委托对象</summary>
        public DDoAttack HandleDoAttack { get; set; }
        /// <summary>弹幕攻击结算的委托对象</summary>
        public DDoAttack HandleDoDanmakuAttack { get; set; }
        /// <summary>命中后攻击结算的委托对象</summary>
        public DDoAttack HandleDoingAttack { get; set; }
        /// <summary>被攻击结算的委托对象</summary>
        public DBeAttacked HandleBeAttacked { get; set; }
        /// <summary>是否命中的委托对象</summary>
        protected DIsHit HandleIsHit { get; set; }
        /// <summary>近战增益的委托对象</summary>
        protected DCloseGain HandleCloseGain { get; set; }
        /// <summary>是否暴击的委托对象</summary>
        protected DIsCriticalHit HandleIsCriticalHit { get; set; }
        /// <summary>准备阶段委托</summary>
        public DPreparingSection HandlePreparingSection { get; set; }
        /// <summary>结束阶段委托</summary>
        public DEndSection HandleEndSection { get; protected set; }
        /// <summary>修改阻挡的敌人列表的委托</summary>
        public DEnemyBlock HandleEnemyBlock { get; set; }
        #endregion

        #region 符卡相关委托
        /// <summary>进入符卡按钮01的委托</summary>
        protected MouseEventHandler[] enterButton { get; set; } = new MouseEventHandler[3];
        /// <summary>离开符卡按钮01的委托</summary>
        protected MouseEventHandler[] leaveButton { get; set; } = new MouseEventHandler[3];
        /// <summary>单击符卡按钮01后进入棋盘按钮的委托</summary>
        protected MouseEventHandler[] enterPad { get; set; } = new MouseEventHandler[3];
        /// <summary>单击符卡按钮01后离开棋盘按钮的委托</summary>
        protected MouseEventHandler[] leavePad { get; set; } = new MouseEventHandler[3];
        #endregion


        /// <summary>Character类的构造函数</summary>
        /// <param name="id">角色ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
        protected Character(int id, Point position, Group group, Random random, Game game)
        {
            this.ID = id;
            this.Position = position;
            this.Group = group;
            this.Data =
                Calculate.CharacterDataList.First(cd => cd.Name == this.GetType().Name);
            this.MaxMp = 1000;
            #region 初始化显示
            #region LabelDisplay
            this.LabelDisplay = new Label
            {
                Margin = new Thickness(2, 2, 2, 11),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Top,
                Content = this.Display,
                Padding = new Thickness(0),
                FontSize = 23
            };
            switch (this.Group)
            {
                case Group.Friend:
                    LabelDisplay.Foreground = Brushes.Red;
                    break;
                case Group.Middle:
                    LabelDisplay.Foreground = Brushes.Black;
                    break;
                default:
                    LabelDisplay.Foreground = Brushes.Green;
                    break;
            }
            #endregion
            #region BarHp
            this.BarHp = new ProgressBar
            {
                Margin = new Thickness(2, 0, 2, 8),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 2,
                Foreground = Brushes.Red,
                Maximum = this.MaxHp,
                Value = this.Hp
            };
            #endregion
            #region BarTime
            this.BarTime = new ProgressBar
            {
                Margin = new Thickness(2, 0, 2, 2),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 2,
                Foreground = Brushes.Green,
                Maximum = this.Interval,
                Value = this.CurrentTime
            };
            #endregion
            #region BarMp
            this.BarMp = new ProgressBar
            {
                Margin = new Thickness(2, 0, 2, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 2,
                Foreground = Brushes.Blue,
                Maximum = this.MaxMp,
                Value = this.Mp
            };
            #endregion
            #region 将控件添加至ListControls
            StaticControls.Add(LabelDisplay);
            StaticControls.Add(BarHp);
            StaticControls.Add(BarMp);
            StaticControls.Add(BarTime);
            Set();
            #endregion
            #endregion

            this.Hp = this.MaxHp;
            this.Mp = MaxMp;
            this.CurrentTime = this.Data.Interval;

            this.random = random;
            this.game = game;
            #region 初始化委托
            HandleDoAttack = DoAttack;
            HandleDoDanmakuAttack = DoDanmakuAttack;
            HandleDoingAttack = DoingAttack;
            HandleBeAttacked = BeAttacked;
            HandleIsHit = IsHit;
            HandleCloseGain = t => 1.0f;
            HandleIsCriticalHit = IsCriticalHit;
            HandlePreparingSection = PreparingSection;
            HandleEndSection = EndSection;
            HandleEnemyBlock = e => from p in e select p;
            #endregion
        }

        #region 治疗
        /// <summary>治疗</summary>
        /// <param name="hp">治疗的体力值</param>
        public void Cure(int hp)
        {
            this.Hp = Math.Min(this.MaxHp, this.Hp + hp);
        }

        /// <summary>治疗</summary>
        /// <param name="hp">治疗的体力值</param>
        public void Cure(double hp)
        {
            Cure((int)hp);
        }

        /// <summary>治疗最大体力值的一定比值</summary>
        /// <param name="x">治疗量与最大体力值的比值</param>
        public void Cure(RationalNumber x)
        {
            Cure(x.Value*MaxHp);
        }
        #endregion

        /// <summary>被攻击</summary>
        /// <param name="damage">伤害值</param>
        /// <param name="attacker">伤害来源</param>
        public virtual void BeAttacked(int damage, Character attacker)
        {
            Damage(damage);
            game.CharactersMayDie.Add(new AttackModel(attacker, this));
        }

        #region 造成伤害
        /// <summary>攻击</summary>
        /// <param name="target">攻击目标</param>
        /// <param name="times">伤害值增益</param>
        /// <returns>是否暴击</returns>
        public virtual bool DoAttack(Character target, float times = 1.0f)
        {
            //判断是否命中
            if (HandleIsHit(target)) return false;
            //计算近战补正
            var closeGain = HandleCloseGain(target);
            //造成伤害
            return DoingAttack(target, times*closeGain);
        }

        /// <summary>弹幕攻击</summary>
        /// <param name="target">攻击目标</param>
        /// <param name="times">伤害值增益</param>
        /// <returns>是否暴击</returns>
        public virtual bool DoDanmakuAttack(Character target, float times = 1.0f)
        {
            return !HandleIsHit(target) && DoingAttack(target, times);
        }

        /// <summary>命中后的伤害结算</summary>
        /// <param name="target">攻击目标</param>
        /// <param name="times">伤害值增益</param>
        /// <returns>是否暴击</returns>
        public virtual bool DoingAttack(Character target, float times = 1.0f)
        {
            //判断是否近战
            var closeGain = HandleCloseGain(target);
            //计算基础伤害
            var damage = /*基础伤害*/ Calculate.Damage(this.Attack, target.Defence) * /*近战补正*/closeGain * FloatDamage * times;
            //判断是否暴击
            var isCriticalHit = HandleIsCriticalHit(target);
            if (isCriticalHit)
            {
                damage *= this.CriticalHitGain;
            }
            target.HandleBeAttacked((int)damage, this);
            return isCriticalHit;
        }
        #endregion

        #region 信息字符串化
        /// <summary>将各数据转化为字符串显示</summary>
        /// <returns>各数据字符串化的结果</returns>
        public override string ToString()
        {
            var result = string.Format("HP: {0} / {1}\nMP: {2} / {3}\n攻击: {4}\n防御: {5}\n" +
                                          "命中率: {6}\n闪避率: {7}\n近战补正: {8}{9}\n" +
                                          "行动间隔: {10}\n机动: {11}\n攻击范围: {12}\n剩余冷却时间: {13}", Hp, MaxHp, Mp,
                MaxMp, Attack, Defence, HitRate, DodgeRate, CloseAmendment, (CloseAmendment%1 == 0) ? ".0" : "",
                Interval, MoveAbility, AttackRange, CurrentTime);
            if (!BuffList.Any()) return result;
            result += "\nBUFF:\n";
            result = BuffList.Aggregate(result, (current, buff) => current + buff.ToString() + "\n");
            return result;
        }

        /// <summary>命中率和伤害值的信息提示</summary>
        /// <param name="target">攻击接受者</param>
        /// <returns></returns>
        public string Tip(Character target)
        {
            return string.Format("命中几率: {0}%\n平均伤害值: {1}\n<按下Shift查看详细信息>",
                (this.HitRate(target)*100).Floor(),
                Calculate.Damage(this.Attack, target.Defence));
        }
        #endregion

        #region 移动
        /// <summary>移动至指定坐标</summary>
        /// <param name="end">移动的目标坐标</param>
        public virtual void Move(Point end)
        {
            this.Position = end;
            Set();
        }

        /// <summary>在各方向移动指定的值，若超限则取边界</summary>
        /// <param name="relativeX">移动的列向相对坐标</param>
        /// <param name="relativeY">移动的行向相对坐标</param>
        public virtual void Move(int relativeX, int relativeY)
        {
            Move(new Point(GetValidPosition((int)this.X + relativeX, Game.Column),
                GetValidPosition((int)this.Y + relativeY, Game.Row)));
        }
        #endregion

        #region 显示更新
        /// <summary>更新显示display的位置</summary>
        public void Set()
        {
            foreach (var c in ListControls)
            {
                c.SetValue(Grid.ColumnProperty, this.X);
                c.SetValue(Grid.RowProperty, this.Y);
            }
        }

        /// <summary>添加显示状态的控件</summary>
        /// <param name="item">待添加的控件</param>
        public void AddStateControl(FrameworkElement item)
        {
            var count = StateControls.Count;
            item.Margin = new Thickness(0, 2 + 12*count, 2, 0);
            this.StateControls.Add(item);
            this.Set();
            game.GridPad.Children.Add(item);
        }

        /// <summary>移除显示状态的控件</summary>
        /// <param name="item">待移除的控件</param>
        public void RemoveStateControl(FrameworkElement item)
        {
            game.GridPad.Children.Remove(item);
            this.StateControls.Remove(item);
            for (int i = 0, count = StateControls.Count; i < count; i++)
            {
                StateControls[i].Margin = new Thickness(0, 2 + 12*i, 2, 0);
            }
        }
        #endregion

        /// <summary>准备阶段</summary>
        public virtual void PreparingSection() { }
        /// <summary>结束阶段</summary>
        public virtual void EndSection() { }



        /// <summary>检测灵力是否足够</summary>
        /// <param name="mp">消耗的灵力量</param>
        /// <returns>灵力是否足够</returns>
        public bool IsMpEnough(int mp)
        {
            return Mp >= mp;
        }

        /// <summary>灵力消耗</summary>
        /// <param name="mp">消耗的灵力量</param>
        /// <returns>灵力是否足够</returns>
        public bool MpUse(int mp)
        {
            if (!IsMpEnough(mp)) return false;
            Mp -= mp;
            return true;
        }

        /// <summary>灵力获取</summary>
        /// <param name="mp">获得的灵力量</param>
        public virtual void MpGain(int mp)
        {
            Mp = Math.Min(MaxMp, Mp + mp);
        }

        /// <summary>判断角色是否为敌人</summary>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否为敌人</returns>
        public bool IsEnemy(Character c)
        {
            if (c == null) return false;
            return /*当前角色中立且c非中立*/ (this.Group == Group.Middle && c.Group != Group.Middle) ||
                /*当前角色非中立且c与之敌对*/ (this.Group != Group.Middle && c.Group == (Group)(-(int)this.Group));
        }

        /// <summary>判断角色是否为队友</summary>
        /// <param name="c">待判断的角色</param>
        /// <param name="containThis">自己是否返回true</param>
        /// <returns>是否为队友</returns>
        public bool IsFriend(Character c, bool containThis = true)
        {
            if (containThis) return c.Group == this.Group;
            return c != this && c.Group == this.Group;
        }

        /// <summary>是否是在自己周围某范围内的敌人</summary>
        /// <param name="range">范围</param>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否符合</returns>
        public bool IsInRangeAndEnemy(int range, Character c)
        {
            return IsInRangeAndEnemy(this.Position, range, c);
        }

        public virtual void AddBuff(Buff buff)
        {
            this.BuffList.Add(buff);
        }


        #region 符卡
        /// <summary>符卡01</summary>
        public abstract void SC01();
        /// <summary>结束使用符卡01</summary>
        public virtual void EndSC01()
        {
            EndSC();
        }

        /// <summary>符卡02</summary>
        public abstract void SC02();
        /// <summary>结束使用符卡02</summary>
        public virtual void EndSC02()
        {
            EndSC();
        }

        /// <summary>符卡03</summary>
        public abstract void SC03();
        /// <summary>结束使用符卡03</summary>
        public virtual void EndSC03()
        {
            EndSC();
        }

        /// <summary>成为当前行动角色时符卡按钮的移动响应</summary>
        public virtual void SCShow()
        {
            for (var i = 0; i < 3; i++)
            {
                if (enterButton[i] != null) AddSCButtonEvent(i);
            }
        }
        /// <summary>符卡按钮移动响应复位</summary>
        public virtual void ResetSCShow()
        {
            for (var i = 0; i < 3; i++)
            {
                if (enterButton[i] != null) RemoveSCButtonEvent(i);
            }
        }
        #endregion



        #region 私有或受保护函数
        /// <summary>受伤</summary>
        /// <param name="damage">伤害值</param>
        private void Damage(int damage)
        {
            Hp -= damage;
        }

        /// <summary>将不合法的Position坐标项转化为合法值</summary>
        /// <param name="coordinate">待转化值</param>
        /// <param name="max">最大值</param>
        /// <returns>合法的坐标值</returns>
        private static int GetValidPosition(int coordinate, int max)
        {
            if (coordinate < 0)
            {
                return 0;
            }
            return coordinate > max ? max : coordinate;
        }

        #region 造成伤害时的计算
        /// <summary>是否命中</summary>
        /// <param name="target">攻击目标</param>
        /// <returns>是否命中</returns>
        protected virtual bool IsHit(Character target)
        {
            return !random.NextBool(this.HitRate(target));
        }
        /// <summary>近战增益</summary>
        /// <param name="target">攻击目标</param>
        /// <returns>近战增益</returns>
        protected virtual double CloseGain(Character target)
        {
            double closeGain;
            var distance = this.Distance(target);
            if (distance == 1)
            {
                closeGain = this.CloseAmendment;
            }
            else
            {
                closeGain = 1.0f;
            }
            return closeGain;
        }
        /// <summary>是否暴击</summary>
        /// <param name="target">攻击目标</param>
        /// <returns>是否暴击</returns>
        protected virtual bool IsCriticalHit(Character target)
        {
            return random.NextBool(this.CriticalHitRate);
        }

        #region 重置
        /// <summary>恢复是否命中的委托对象</summary>
        protected void ResetHandleIsHit()
        {
            HandleIsHit = IsHit;
        }
        /// <summary>恢复近战增益的委托对象</summary>
        protected void ResetHandleCloseGain()
        {
            HandleCloseGain = t => 1.0f;
        }
        /// <summary>恢复是否暴击的委托对象</summary>
        protected void ResetHandleIsCriticalHit()
        {
            HandleIsCriticalHit = IsCriticalHit;
        }
        /// <summary>恢复被攻击结算的委托对象</summary>
        protected void ResetBeAttacked()
        {
            HandleBeAttacked = BeAttacked;
        }
        #endregion

        /// <summary>伤害浮动</summary>
        /// <returns>浮动带来的伤害系数</returns>
        protected double FloatDamage => (2*random.NextDouble() - 1)*this.DamageFloat + 1;
        #endregion

        /// <summary>结束符卡结算</summary>
        private void EndSC()
        {
            for (int i = 0, length = game.ScDelegates.Length; i < length; i++)
            {
                game.ScDelegates[i] = null;
            }
        }

        #region 符卡桌面事件相关
        /// <summary>为符卡按钮事件添加对应委托</summary>
        /// <param name="index">符卡按钮索引</param>
        protected void AddSCButtonEvent(int index)
        {
            game.ButtonSC[index].MouseEnter += enterButton[index];
            game.ButtonSC[index].MouseLeave += leaveButton[index];
        }
        /// <summary>为符卡按钮事件去除对应委托</summary>
        /// <param name="index">符卡按钮索引</param>
        protected void RemoveSCButtonEvent(int index)
        {
            game.ButtonSC[index].MouseEnter -= enterButton[index];
            game.ButtonSC[index].MouseLeave -= leaveButton[index];
        }

        /// <summary>为棋盘按钮事件添加对应委托</summary>
        /// <param name="index">符卡索引</param>
        protected void AddPadButtonEvent(int index)
        {
            foreach (var b in game.Buttons)
            {
                b.MouseEnter += enterPad[index];
                b.MouseLeave += leavePad[index];
            }
        }
        /// <summary>为棋盘按钮事件去除对应委托</summary>
        /// <param name="index">符卡索引</param>
        protected void RemovePadButtonEvent(int index)
        {
            foreach (var b in game.Buttons)
            {
                b.MouseEnter -= enterPad[index];
                b.MouseLeave -= leavePad[index];
            }
        }

        /// <summary>设置离开棋盘按钮事件所添加的委托</summary>
        /// <param name="index">符卡索引</param>
        protected void SetDefaultLeaveSCButtonDelegate(int index)
        {
            leaveButton[index] = (s, ev) => this.game.ResetShow();
        }

        /// <summary>设置离开棋盘按钮事件所添加的委托</summary>
        /// <param name="index">符卡索引</param>
        protected void SetDefaultLeavePadButtonDelegate(int index)
        {
            leavePad[index] = (s, ev) => this.game.ResetShow();
        }
        #endregion

        /// <summary>是否是在某点周围某范围内的敌人</summary>
        /// <param name="origin">点</param>
        /// <param name="range">范围</param>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndEnemy(Point origin, int range, Character c)
        {
            if (c == null) return false;
            return origin.Distance(c) <= range && IsEnemy(c);
        }

        /// <summary>某点处的角色是否是在某点周围某范围内的敌人</summary>
        /// <param name="origin">点</param>
        /// <param name="range">范围</param>
        /// <param name="p">待判断的点</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndEnemy(Point origin, int range, Point p)
        {
            return origin.Distance(p) <= range && IsEnemy(game[p]);
        }

        /// <summary>某点处的角色是否是在某点周围某范围内的敌人</summary>
        /// <param name="range">范围</param>
        /// <param name="p">待判断的点</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndEnemy(int range, Point p)
        {
            return p.Distance(this) <= range && IsEnemy(game[p]);
        }

        /// <summary>是否是在自己周围某范围内的队友</summary>
        /// <param name="range">范围</param>
        /// <param name="c">待判断的角色</param>
        /// <param name="containThis">自己是否返回true</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndFriend(int range, Character c, bool containThis = true)
        {
            if (c == null) return false;
            return c.Distance(this) <= range && IsFriend(c, containThis);
        }

        /// <summary>某点处的角色是否是在某点周围某范围内的队友</summary>
        /// <param name="origin">点</param>
        /// <param name="range">范围</param>
        /// <param name="p">待判断的点</param>
        /// <param name="containThis">自己是否返回true</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndFriend(Point origin, int range, Point p, bool containThis = true)
        {
            return origin.Distance(p) <= range && IsFriend(game[p], containThis);
        }

        /// <summary>某点处的角色是否是在某点周围某范围内的队友</summary>
        /// <param name="origin">点</param>
        /// <param name="range">范围</param>
        /// <param name="p">待判断的点</param>
        /// <param name="containThis">自己是否返回true</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndFriend(int range, Point p, bool containThis = true)
        {
            return p.Distance(this) <= range && IsFriend(game[p], containThis);
        }
        #endregion
    }
}

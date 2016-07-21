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
using MoreEnumerable;
using RandomHelper;

namespace JLQ_GameBase
{
    /// <summary>角色类</summary>
    public abstract class Character : CharacterBase
    {
        #region 静态属性
        /// <summary>表示状态的控件样式</summary>
        public static Style StateStyle { get; } = new Style();

        static Character()
        {
            #region StateStyle
            StateStyle.Setters.Add(new Setter(Control.HorizontalAlignmentProperty, HorizontalAlignment.Right));
            StateStyle.Setters.Add(new Setter(Control.VerticalAlignmentProperty, VerticalAlignment.Top));
            #endregion
        }
        #endregion
        #region 只读属性
        /// <summary>ID</summary>
        public int ID { get; }
        #region 角色数据
        /// <summary>角色数据</summary>
        private CharacterData Data { get; }
        /// <summary>角色名字</summary>
        public string Name => Data.Name;
        /// <summary>角色最大血量</summary>
        public virtual int MaxHp => Data.MaxHp;
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
        /// <summary>将死事件</summary>
        public event Action EWillDie;
        /// <summary>血量</summary>
        public override int Hp
        {
            get { return base.Hp; }
            protected set
            {
                base.Hp = value;
                BarHp.Value = value;
                if (value <= 0) EWillDie?.Invoke();
            }
        }

        /// <summary>灵力</summary>
        public override int Mp
        {
            get { return base.Mp; }
            protected set
            {
                base.Mp = value;
                BarMp.Value = value;
                if (!game.IsBattle || game.CurrentCharacter != this) return;
                for (var i = 0; i < 3; i++)
                {
                    game.ButtonSC[i].IsEnabled = IsMpEnough(SCMpUse[i]());
                }
            }
        }

        /// <summary>当前剩余冷却时间</summary>
        public override int CurrentTime
        {
            get { return base.CurrentTime; }
            set
            {
                base.CurrentTime = value;
                BarTime.Value = value;
            }
        }
        #endregion

        /// <summary>位置</summary>
        public PadPoint Position { get; private set; }

        /// <summary>是否已移动</summary>
        public virtual bool HasMoved { get; set; }
        /// <summary>是否已攻击</summary>
        public virtual bool HasAttacked { get; set; }

        /// <summary>作为buff承受者的buff列表</summary>
        public List<Buff> BuffList { get; } = new List<Buff>();

        public Level CharacterLevel { get; set; } = Level.Easy;

        #region 显示
        /// <summary>所有在GUI上显示的控件</summary>
        public IEnumerable<FrameworkElement> ListControls => StaticControls.Concat(StateControls);
        /// <summary>固定显示的控件</summary>
        private List<FrameworkElement> StaticControls { get; } = new List<FrameworkElement>();
        /// <summary>显示Display的Label</summary>
        public Label LabelDisplay { get; }
        /// <summary>显示Hp的ProgressBar</summary>
        protected ProgressBar BarHp { get; }
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
        public int Attack => Math.Max(0, (Data.Attack * _attackX).Floor() + _attackAdd);
        /// <summary>防御</summary>
        public virtual int Defence => Math.Max(0, (Data.Defence * _defenceX).Floor() + _defenceAdd);
        /// <summary>命中率</summary>
        public int HitRate => Math.Max(0, (Data.HitRate * _hitRateX).Floor() + _hitRateAdd);
        /// <summary>闪避率</summary>
        public int DodgeRate => Math.Max(0, (Data.DodgeRate * _dodgeRateX).Floor() + _dodgeRateAdd);
        /// <summary>近战补正</summary>
        public double CloseAmendment => Math.Max(0, Data.CloseAmendment * _closeAmendmentX + _clostAmendmentAdd);
        /// <summary>行动间隔</summary>
        public int Interval => Math.Max(1, (Data.Interval * _intervalX + _intervalAdd).Floor());
        /// <summary>默认Buff时间（3*Interval）</summary>
        public int BuffTime => 3 * Interval;
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
        public int Column => this.Position.Column;
        /// <summary>Row坐标</summary>
        public int Row => this.Position.Row;

        /// <summary>对此角色而言的敌人列表</summary>
        public IEnumerable<Character> Enemies => game.Characters.Where(IsEnemy);
        /// <summary>阻挡行动的敌人列表</summary>
        public virtual IEnumerable<PadPoint> EnemyBlock => HandleEnemyBlock(Enemies.Select(c => c.Position));

        /// <summary>符卡的灵力消耗</summary>
        public virtual Func<int>[] SCMpUse { get; } = new Func<int>[]
        {
            () => 0,
            () => 0,
            () => 0
        };//TODO SC Mp Use

        /// <summary>用于保存的角色信息</summary>
        internal CharacterInfo Info => new CharacterInfo
        {
            Position = this.Position,
            CGroup = this.Group,
            Display = this.Display
        };
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
        public Action HandlePreparingSection { get; set; }
        /// <summary>结束阶段委托</summary>
        public Action HandleEndSection { get; protected set; }
        /// <summary>修改阻挡的敌人列表的委托</summary>
        public DEnemyBlock HandleEnemyBlock { get; set; }
        #endregion

        #region 符卡相关委托
        /// <summary>进入符卡按钮01的委托</summary>
        protected MouseEventHandler[] enterButton { get; set; } = new MouseEventHandler[] { null, null, null };
        /// <summary>离开符卡按钮01的委托</summary>
        protected MouseEventHandler[] leaveButton { get; set; } = new MouseEventHandler[] { null, null, null };
        /// <summary>单击符卡按钮01后进入棋盘按钮的委托</summary>
        protected MouseEventHandler[] enterPad { get; set; } = new MouseEventHandler[] { null, null, null };
        /// <summary>单击符卡按钮01后离开棋盘按钮的委托</summary>
        protected MouseEventHandler[] leavePad { get; set; } = new MouseEventHandler[] { null, null, null };
        #endregion


        /// <summary>Character类的构造函数</summary>
        /// <param name="id">角色ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="game">游戏对象</param>
        protected Character(int id, PadPoint position, Group group, Game game)
        {
            this.ID = id;
            this.Position = position;
            this.Group = group;
            this.game = game;
            this.Data =
                Game.CharacterDatas.First(cd => cd.Name == this.GetType().Name);
            this.MaxMp = 1000;

            #region 初始化显示

            #region LabelDisplay

            this.LabelDisplay = new Label
            {
                Margin = new Thickness(2, 2, 2, 11),
                Style = Game.StretchStyle,
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
            this.Mp = this.MaxMp;
            this.CurrentTime = this.Data.Interval;

            this.random = game.random;

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
        public void Cure(int hp) => this.Hp = Math.Min(this.MaxHp, this.Hp + hp);

        /// <summary>治疗</summary>
        /// <param name="hp">治疗的体力值</param>
        public void Cure(double hp) => Cure((int)hp);

        /// <summary>治疗最大体力值的一定比例</summary>
        /// <param name="x">治疗量与最大体力值的比值</param>
        public void Cure(PercentOfMaxHp x) => Cure(x.Percent * MaxHp);
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
        /// <summary>造成伤害事件</summary>
        public event Action<Character, int> EAttackDone;
        /// <summary>暴击事件</summary>
        public event Action<Character> ECriticalHitted;
        protected void HandleEAttackDone(Character target, int damage) => EAttackDone?.Invoke(target, damage);
        /// <summary>攻击</summary>
        /// <param name="target">攻击目标</param>
        /// <param name="times">伤害值增益</param>
        /// <returns>是否暴击</returns>
        public virtual bool DoAttack(Character target, float times = 1.0f)
        {
            //判断是否命中
            if (!HandleIsHit(target)) return false;
            //计算近战补正
            var closeGain = HandleCloseGain(target);
            //造成伤害
            return HandleDoingAttack(target, times * closeGain);
        }

        /// <summary>弹幕攻击</summary>
        /// <param name="target">攻击目标</param>
        /// <param name="times">伤害值增益</param>
        /// <returns>是否暴击</returns>
        public virtual bool DoDanmakuAttack(Character target, float times = 1.0f)
            => HandleIsHit(target) && HandleDoingAttack(target, times);

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
                ECriticalHitted?.Invoke(target);
            }
            target.HandleBeAttacked((int)damage, this);
            EAttackDone?.Invoke(target, (int)damage);
            return isCriticalHit;
        }
        #endregion

        #region 信息字符串化
        /// <summary>将各数据转化为字符串显示</summary>
        /// <returns>各数据字符串化的结果</returns>
        public override string ToString()
        {
            var result = string.Format("HP： {0} / {1}\nMP： {2} / {3}\n攻击： {4}\n防御： {5}\n" +
                                       "命中率： {6}\n闪避率： {7}\n近战补正： {8}\n" +
                                       "行动间隔： {9}\n机动： {10}\n攻击范围： {11}\n剩余冷却时间： {12}", Hp, MaxHp, Mp,
                MaxMp, Attack, Defence, HitRate, DodgeRate, CloseAmendment, Interval, MoveAbility, AttackRange,
                CurrentTime);
            if (!BuffList.Any()) return result;
            result += "\nBUFF:\n";
            result = BuffList.Aggregate(result, (current, buff) => current + buff.ToString() + "\n");
            return result;
        }

        /// <summary>命中率和伤害值的信息提示</summary>
        /// <param name="target">攻击接受者</param>
        /// <returns></returns>
        public string Tip(Character target)
            => string.Format("命中几率： {0}%\n平均伤害值： {1}\n<按下Shift查看详细信息>",
                (this.HitRate(target) * 100).Floor(),
                Calculate.Damage(this.Attack, target.Defence));
        #endregion

        #region 移动
        /// <summary>移动至指定坐标</summary>
        /// <param name="end">移动的目标坐标</param>
        public virtual void Move(PadPoint end)
        {
            this.Position = end;
            Set();
        }

        /// <summary>在各方向移动指定的值，若超限则取边界</summary>
        /// <param name="relativeX">移动的列向相对坐标</param>
        /// <param name="relativeY">移动的行向相对坐标</param>
        public virtual void Move(int relativeX, int relativeY)
        {
            var end = new PadPoint(GetValidPosition(this.Column + relativeX, Game.Column),
                GetValidPosition(this.Row + relativeY, Game.Row));
            if (game[end] != null) Move(end);
        }
        #endregion

        #region 显示更新
        /// <summary>更新显示display的位置</summary>
        public void Set()
        {
            foreach (var c in ListControls)
            {
                c.SetValue(Grid.ColumnProperty, this.Column);
                c.SetValue(Grid.RowProperty, this.Row);
            }
        }

        /// <summary>添加显示状态的控件</summary>
        /// <param name="item">待添加的控件</param>
        public void AddStateControl(FrameworkElement item)
        {
            var count = StateControls.Count;
            item.Margin = new Thickness(0, 2 + 12 * count, 2, 0);
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
                StateControls[i].Margin = new Thickness(0, 2 + 12 * i, 2, 0);
            }
        }
        #endregion

        /// <summary>准备阶段</summary>
        public virtual void PreparingSection()
        {
            for (var i = 0; i < 3; i++)
            {
                if (!IsMpEnough(SCMpUse[i]())) game.ButtonSC[i].IsEnabled = false;
            }
        }

        /// <summary>结束阶段</summary>
        public virtual void EndSection() => game.ButtonSC.DoAction(b => b.IsEnabled = true);

        /// <summary>攻击后的执行代码</summary>
        public virtual void ActionWhileAttacked() { }

        /// <summary>移动后的执行代码</summary>
        public virtual void ActionWhileMoved() { }


        /// <summary>检测灵力是否足够</summary>
        /// <param name="mp">消耗的灵力量</param>
        /// <returns>灵力是否足够</returns>
        public bool IsMpEnough(int mp) => Mp >= mp;

        /// <summary>灵力消耗</summary>
        /// <param name="mp">消耗的灵力量</param>
        /// <returns>灵力是否足够</returns>
        public virtual bool MpUse(int mp)
        {
            if (!IsMpEnough(mp)) return false;
            Mp -= mp;
            return true;
        }

        /// <summary>灵力获取</summary>
        /// <param name="mp">获得的灵力量</param>
        public virtual void MpGain(int mp) => Mp = Math.Min(MaxMp, Mp + mp);

        /// <summary>判断角色是否为敌人</summary>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否为敌人</returns>
        public bool IsEnemy(Character c) => c != null && this.Group != c.Group;

        /// <summary>判断角色是否为队友</summary>
        /// <param name="c">待判断的角色</param>
        /// <param name="containThis">自己是否返回true</param>
        /// <returns>是否为队友</returns>
        public bool IsFriend(Character c, bool containThis = true)
        {
            if (c == null) return false;
            if (containThis) return c.Group == this.Group;
            return c != this && c.Group == this.Group;
        }

        /// <summary>是否是在自己周围某范围内的敌人</summary>
        /// <param name="range">范围</param>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否符合</returns>
        public bool IsInRangeAndEnemy(int range, Character c) => IsInRangeAndEnemy(this.Position, range, c);

        protected internal virtual void AddBuff(Buff buff) => this.BuffList.Add(buff);


        #region 符卡
        /// <summary>符卡01</summary>
        public abstract void SC01();
        /// <summary>结束使用符卡01</summary>
        public virtual void EndSC01() => EndSC();

        /// <summary>符卡02</summary>
        public abstract void SC02();
        /// <summary>结束使用符卡02</summary>
        public virtual void EndSC02() => EndSC();

        /// <summary>符卡03</summary>
        public abstract void SC03();
        /// <summary>结束使用符卡03</summary>
        public virtual void EndSC03() => EndSC();

        /// <summary>成为当前行动角色时符卡按钮的移动响应</summary>
        public virtual void SCShow()
        {
            for (var i = 0; i < 3; i++)
            {
                AddSCButtonEvent(i);
            }
        }
        /// <summary>符卡按钮移动响应复位</summary>
        public virtual void ResetSCShow()
        {
            for (var i = 0; i < 3; i++)
            {
                RemoveSCButtonEvent(i);
            }
        }
        #endregion


        #region 私有或受保护函数
        /// <summary>受伤</summary>
        /// <param name="damage">伤害值</param>
        private void Damage(int damage) => Hp -= damage;

        /// <summary>将不合法的Position坐标项转化为合法值</summary>
        /// <param name="coordinate">待转化值</param>
        /// <param name="max">最大值</param>
        /// <returns>合法的坐标值</returns>
        private static int GetValidPosition(int coordinate, int max) => coordinate < 0 ? 0 : Math.Min(coordinate, max);

        #region 造成伤害时的计算
        /// <summary>是否命中</summary>
        /// <param name="target">攻击目标</param>
        /// <returns>是否命中</returns>
        protected virtual bool IsHit(Character target)
        {
            return random.NextBool(this.HitRate(target));
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
        protected double FloatDamage => (2 * random.NextDouble() - 1) * this.DamageFloat + 1;
        #endregion

        /// <summary>结束符卡结算</summary>
        private void EndSC()
        {
            game.HandleResetShow = game.ResetShow;
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
            if (enterButton[index] == null) return;
            game.ButtonSC[index].MouseEnter += enterButton[index];
            game.ButtonSC[index].MouseLeave += leaveButton[index];
        }
        /// <summary>为符卡按钮事件去除对应委托</summary>
        /// <param name="index">符卡按钮索引</param>
        protected void RemoveSCButtonEvent(int index)
        {
            if (enterButton[index] == null) return;
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
            leaveButton[index] = (s, ev) => game.HandleResetShow();
        }

        /// <summary>设置离开棋盘按钮事件所添加的委托</summary>
        /// <param name="index">符卡索引</param>
        protected void SetDefaultLeavePadButtonDelegate(int index)
        {
            leavePad[index] = (s, ev) => game.HandleResetShow();
        }
        #endregion

        /// <summary>是否是在某点周围某范围内的敌人</summary>
        /// <param name="origin">点</param>
        /// <param name="range">范围</param>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndEnemy(PadPoint origin, int range, Character c)
            => IsEnemy(c) && origin.IsInRange(c, range);

        /// <summary>某点处的角色是否是在某点周围某范围内的敌人</summary>
        /// <param name="origin">点</param>
        /// <param name="range">范围</param>
        /// <param name="p">待判断的点</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndEnemy(PadPoint origin, int range, PadPoint p)
            => origin.IsInRange(p, range) && IsEnemy(game[p]);

        /// <summary>某点处的角色是否是在某点周围某范围内的敌人</summary>
        /// <param name="range">范围</param>
        /// <param name="p">待判断的点</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndEnemy(int range, PadPoint p) => p.IsInRange(this, range) && IsEnemy(game[p]);

        /// <summary>是否是在自己周围某范围内的队友</summary>
        /// <param name="range">范围</param>
        /// <param name="c">待判断的角色</param>
        /// <param name="containThis">自己是否返回true</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndFriend(int range, Character c, bool containThis = true)
            => IsFriend(c, containThis) && this.IsInRange(c, range);

        /// <summary>某点处的角色是否是在某点周围某范围内的队友</summary>
        /// <param name="origin">点</param>
        /// <param name="range">范围</param>
        /// <param name="p">待判断的点</param>
        /// <param name="containThis">自己是否返回true</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndFriend(PadPoint origin, int range, PadPoint p, bool containThis = true)
            => origin.IsInRange(p, range) && IsFriend(game[p], containThis);

        /// <summary>某点处的角色是否是在某点周围某范围内的队友</summary>
        /// <param name="range">范围</param>
        /// <param name="p">待判断的点</param>
        /// <param name="containThis">自己是否返回true</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndFriend(int range, PadPoint p, bool containThis = true)
            => p.IsInRange(this, range) && IsFriend(game[p], containThis);

        /// <summary>在范围内的敌人</summary>
        /// <param name="origin">起始点</param>
        /// <param name="range">范围</param>
        /// <returns>在范围内的敌人</returns>
        protected IEnumerable<Character> EnemyInRange(PadPoint origin, int range)
            => Enemies.Where(c => origin.IsInRange(c, range));

        /// <summary>在鼠标点范围内的敌人</summary>
        /// <param name="range">范围</param>
        /// <returns>在范围内的敌人</returns>
        protected IEnumerable<Character> EnemyInMouseRange(int range)
            => Enemies.Where(c => game.MousePoint.IsInRange(c, range));

        /// <summary>在范围内的敌人</summary>
        /// <param name="range">范围</param>
        /// <returns>在范围内的敌人</returns>
        protected IEnumerable<Character> EnemyInRange(int range) => Enemies.Where(c => this.IsInRange(c, range));
        #endregion
    }
}

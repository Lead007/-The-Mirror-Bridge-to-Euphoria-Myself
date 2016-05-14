using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Data;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>角色类</summary>
    public abstract class Character
    {
        //以下为字段
        //只读字段
        /// <summary>ID</summary>
        public readonly int ID;
        /// <summary>角色数据</summary>
        public readonly CharacterData Data;
        /// <summary>最大灵力</summary>
        public readonly int _maxMp;
        /// <summary>阵营</summary>
        public readonly Group Group;

        //可变字段
        //增益
        /// <summary>攻击增益</summary>
        public virtual double _attackX { get; set; } = 1.0f;
        /// <summary>攻击增量</summary>
        public int _attackAdd { get; set; } = 0;
        /// <summary>防御增益</summary>
        public double _defenceX { get; set; } = 1.0f;
        /// <summary>防御增量</summary>
        public int _defenceAdd { get; set; } = 0;
        /// <summary>命中率增益</summary>
        public double _hitRateX { private get; set; } = 1.0f;
        /// <summary>闪避率增益</summary>
        public double _dodgeRateX { private get; set; } = 1.0f;
        /// <summary>近战补正增益</summary>
        public double _closeAmendmentX { private get; set; } = 1.0f;

        private double __intervalX = 1.0f;
        /// <summary>行动间隔增益</summary>
        public double _intervalX
        {
            get { return __intervalX; }
            set
            {
                __intervalX = value;
                CurrentTime = Math.Min(CurrentTime, Interval);
                BarTime.Maximum = Interval;
            }
        }

        private int __intervalAdd;
        /// <summary>行动间隔增量</summary>
        public int _intervalAdd
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
        /// <summary>机动增量</summary>
        public int _moveAbilityAdd { get; set; } = 0;
        /// <summary>攻击范围增量</summary>
        public int _attackRangeAdd { get; set; } = 0;

        /// <summary>随机数对象</summary>
        protected Random random;
        /// <summary>游戏对象</summary>
        protected Game game;

        //属性
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
        /// <summary>位置，X为Grid.Column，Y为Grid.Row</summary>
        public Point Position { get; protected set; }
        /// <summary>是否已移动</summary>
        public virtual bool HasMoved { get; set; }
        /// <summary>是否已攻击</summary>
        public virtual bool HasAttacked { get; set; }
        /// <summary>作为buff承受者的buff列表</summary>
        public List<Buff> BuffList { get; protected set; }

        //显示
        /// <summary>所有在GUI上显示的控件</summary>
        public List<FrameworkElement> ListControls = new List<FrameworkElement>();
        /// <summary>显示Display的Label</summary>
        public Label LabelDisplay { get; set; }
        /// <summary>显示Hp的ProgressBar</summary>
        public ProgressBar BarHp { get; set; }
        /// <summary>显示Mp的ProgressBar</summary>
        public ProgressBar BarMp { get; set; }
        /// <summary>显示剩余时间的ProgressBar</summary>
        public ProgressBar BarTime { get; set; }


        //只读属性
        /// <summary>攻击</summary>
        public int Attack => Math.Max(0, Calculate.Floor(Data.Attack*_attackX) + _attackAdd);
        /// <summary>防御</summary>
        public int Defence => Math.Max(0, Calculate.Floor(Data.Defence*_defenceX) + _defenceAdd);
        /// <summary>命中率</summary>
        public int HitRate => Calculate.Floor(Data.HitRate*_hitRateX);
        /// <summary>闪避率</summary>
        public int DodgeRate => Calculate.Floor(Data.DodgeRate*_dodgeRateX);
        /// <summary>近战补正</summary>
        public int CloseAmendment => Calculate.Floor(Data.CloseAmendment*_closeAmendmentX);
        /// <summary>行动间隔</summary>
        public int Interval => Math.Max(1, Calculate.Floor(Data.Interval*_intervalX + _intervalAdd));
        /// <summary>机动</summary>
        public int MoveAbility => Math.Max(0, Data.MoveAbility + _moveAbilityAdd);
        /// <summary>攻击范围</summary>
        public int AttackRange => Math.Max(0, Data.AttackRange + _attackRangeAdd);
        /// <summary>暴击增益</summary>
        protected float CriticalHitGain => 1.5f;
        /// <summary>暴击率</summary>
        private float CriticalHitRate => 0.2f;
        /// <summary>伤害浮动</summary>
        private float DamageFloat => 0.1f;
        /// <summary>是否死亡</summary>
        public bool IsDead => 0 >= Hp;
        /// <summary>名字</summary>
        public string Name => Data.Name;
        /// <summary>Column坐标</summary>
        public int X => (int)this.Position.X;
        /// <summary>Row坐标</summary>
        public int Y => (int)this.Position.Y;

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

        //符卡相关委托
        /// <summary>进入符卡按钮01的委托</summary>
        protected MouseEventHandler[] enterButton = new MouseEventHandler[3];
        /// <summary>离开符卡按钮01的委托</summary>
        protected MouseEventHandler[] leaveButton = new MouseEventHandler[3];
        /// <summary>单击符卡按钮01后进入棋盘按钮的委托</summary>
        protected MouseEventHandler[] enterPad = new MouseEventHandler[3];
        /// <summary>单击符卡按钮01后离开棋盘按钮的委托</summary>
        protected MouseEventHandler[] leavePad = new MouseEventHandler[3];


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
            HasMoved = false;
            HasAttacked = false;
            this.Data =
                Calculate.CharacterDataList.First(cd => cd.Name == this.GetType().ToString().Substring(25));
            this._maxMp = 1000;
            //初始化显示
            this.LabelDisplay = new Label
            {
                Margin = new Thickness(2, 2, 2, 11),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Top,
                Content = this.Data.Display,
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

            this.BarHp = new ProgressBar
            {
                Margin = new Thickness(2, 0, 2, 8),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 2,
                Foreground = Brushes.Red,
                Maximum = this.Data.MaxHp,
                Value = this.Hp
            };
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
            this.BarMp = new ProgressBar
            {
                Margin = new Thickness(2, 0, 2, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 2,
                Foreground = Brushes.Blue,
                Maximum = this._maxMp,
                Value = this.Mp
            };
            ListControls.Add(LabelDisplay);
            ListControls.Add(BarHp);
            ListControls.Add(BarMp);
            ListControls.Add(BarTime);
            Set();

            this.Hp = this.Data.MaxHp;
            this.Mp = _maxMp;
            this.CurrentTime = this.Data.Interval;

            BuffList = new List<Buff>();
            this.random = random;
            this.game = game;
            //初始化委托
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
        }

        /// <summary>治疗</summary>
        /// <param name="hp">治疗的体力值</param>
        public void Cure(int hp)
        {
            this.Hp = Math.Min(this.Data.MaxHp, this.Hp + hp);
        }

        /// <summary>治疗</summary>
        /// <param name="hp">治疗的体力值</param>
        public void Cure(double hp)
        {
            Cure((int)hp);
        }

        /// <summary>被攻击</summary>
        /// <param name="damage">伤害值</param>
        /// <param name="attacker">伤害来源</param>
        public virtual void BeAttacked(int damage, Character attacker)
        {
            Damage(damage);
            game.CharactersMayDie.Add(new AttackModel(attacker, this));
        }

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

        /// <summary>将各数据转化为字符串显示</summary>
        /// <returns>各数据字符串化的结果</returns>
        public override string ToString()
        {
            var result = string.Format("HP: {0} / {1}\nMP: {2} / {3}\n攻击: {4}\n防御: {5}\n" +
                                          "命中率: {6}\n闪避率: {7}\n近战补正: {8}{9}\n" +
                                          "行动间隔: {10}\n机动: {11}\n攻击范围: {12}\n剩余冷却时间: {13}", Hp, Data.MaxHp, Mp,
                _maxMp, Attack, Defence, HitRate, DodgeRate, CloseAmendment, (CloseAmendment%1 == 0) ? ".0" : "",
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
                Calculate.Floor(Calculate.HitRate(this, target)*100),
                Calculate.Damage(this.Attack, target.Defence));
        }

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

        /// <summary>对此角色而言的敌人列表</summary>
        public IEnumerable<Character> Enemy => game.Characters.Where(IsEnemy);

        /// <summary>准备阶段</summary>
        public virtual void PreparingSection() { }
        /// <summary>结束阶段</summary>
        public virtual void EndSection() { }

        /// <summary>阻挡行动的敌人列表</summary>
        public virtual IEnumerable<Point> EnemyBlock => HandleEnemyBlock(Enemy.Select(c => c.Position));


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
            Mp = Math.Min(_maxMp, Mp + mp);
        }


        //以下为符卡

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
        ///// <summary>成为当前行动角色时按钮的移动响应</summary>
        //public abstract void SCShow();
        ///// <summary>按钮移动响应复位</summary>
        //public abstract void ResetSCShow();
        //TODO back abstract
        /// <summary>成为当前行动角色时按钮的移动响应</summary>
        public virtual void SCShow(){ }

        /// <summary>按钮移动响应复位</summary>
        public virtual void ResetSCShow(){ }



        //以下为私有函数

        /// <summary>受伤</summary>
        /// <param name="damage">伤害值</param>
        private void Damage(int damage)
        {
            Hp -= damage;
        }

        /// <summary>将不合法的Position坐标项转化为合法值</summary>
        /// <param name="coordinate">待转化值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        private static int GetValidPosition(int coordinate, int max)
        {
            if (coordinate < 0)
            {
                return 0;
            }
            return coordinate > max ? max : coordinate;
        }

        //显示更新
        /// <summary>更新显示display的位置</summary>
        public void Set()
        {
            foreach (var c in ListControls)
            {
                c.SetValue(Grid.ColumnProperty, this.X);
                c.SetValue(Grid.RowProperty, this.Y);
            }
        }

        /// <summary>是否命中</summary>
        /// <param name="target">攻击目标</param>
        /// <returns>是否命中</returns>
        protected virtual bool IsHit(Character target)
        {
            return random.NextDouble() > Calculate.HitRate(this, target);
        }
        /// <summary>近战增益</summary>
        /// <param name="target">攻击目标</param>
        /// <returns>近战增益</returns>
        protected virtual float CloseGain(Character target)
        {
            float closeGain;
            var distance = Calculate.Distance(this, target);
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
            return random.NextDouble() <= this.CriticalHitRate;
        }

        //RESET
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

        /// <summary>伤害浮动</summary>
        /// <returns>浮动带来的伤害系数</returns>
        protected double FloatDamage => (2*random.NextDouble() - 1)*this.DamageFloat + 1;

        /// <summary>结束符卡结算</summary>
        private void EndSC()
        {
            game.HandleIsLegalClick = null;
            game.HandleIsTargetLegal = null;
            game.HandleTarget = null;
        }

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

        /// <summary>是否是在某点周围某范围内的敌人</summary>
        /// <param name="origin">点</param>
        /// <param name="range">范围</param>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndEnemy(Point origin, int range, Character c)
        {
            if (c == null) return false;
            return Calculate.Distance(origin, c) <= range && IsEnemy(c);
        }

        /// <summary>是否是在自己周围某范围内的敌人</summary>
        /// <param name="range">范围</param>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否符合</returns>
        protected bool IsInRangeAndEnemy(int range, Character c)
        {
            return IsInRangeAndEnemy(this.Position, range, c);
        }

        /// <summary>判断角色是否为敌人</summary>
        /// <param name="c">待判断的角色</param>
        /// <returns>是否为敌人</returns>
        protected bool IsEnemy(Character c)
        {
            if (c == null) return false;
            return /*当前角色中立且c非中立*/ (this.Group == Group.Middle && c.Group != Group.Middle) ||
                /*当前角色非中立且c与之敌对*/ (this.Group != Group.Middle && c.Group == (Group) (-(int)this.Group));
        }
    }
}

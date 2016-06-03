using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_BaseBuffs.Gain.Sealed;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using MoreEnumerable;
using RandomHelper;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>上白泽慧音</summary>
    public class Keine : Character, IHuman
	{
		public Keine(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
		    enterButton[0] = (s, ev) =>
		    {
		        if (!IsBaize) return;
		        game.DefaultButtonAndLabels();
		        var cs = game.Characters.Where(c => this.IsInRange(c, SC01RangeBaize)).ToList();
		        cs.Where(SC01BaizeFriendIsTargetLegal).SetLabelBackground();
		        cs.Where(SC01BaizeEnemyIsTargetLegal).SetLabelBackground(GameColor.LabelBackground2);
		    };
            SetDefaultLeaveSCButtonDelegate(0);
		    enterPad[0] = (s, ev) =>
		    {
		        if (IsBaize) return;
		        var c = game.MouseCharacter;
		        if (c == null) return;
		        game.DefaultButtonAndLabels();
		        c.SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
		    enterButton[1] = (s, ev) =>
		    {
		        if (IsBaize)
		        {
		            game.DefaultButtonAndLabels();
		            Enemies.Where(c => Math.Abs(c.X - this.X) <= 1).SetLabelBackground();
		        }
		    };
            SetDefaultLeaveSCButtonDelegate(1);
		    enterPad[2] = (s, ev) =>
		    {
		        if (IsBaize)
		        {

		        }
		        else
		        {
		            var cc = game.MouseCharacter;
		            if (!IsFriend(cc)) return;
		            game.DefaultButtonAndLabels();
		            EnemyInRange(cc.Position, cc.AttackRange + cc.MoveAbility).SetLabelBackground();
		        }
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        public Human HumanKind => Human.HalfHuman;

        private const int SC01RangeBaize = 4;

        private bool _isBaize;
        private bool IsBaize
        {
            get { return _isBaize; }
            set
            {
                _isBaize = value;
                this.LabelDisplay.Content = value ? "白学家" : "慧";
                this.CloseAmendmentX = value ? (2/1.1) : (1.1/2);
                if (value) return;
                SC01Buffs.DoAction(b => b.BuffEnd());
                SC01Buffs.Clear();
            }
        }

        private List<Buff> SC01Buffs { get; }= new List<Buff>();

        //天赋
        public override void EndSection()
        {
            base.EndSection();
            IsBaize = random.NextBool(1.0/6);
        }

        public override string ToString() => (IsBaize ? "白泽状态\n" : string.Empty) + base.ToString();

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            if (IsBaize)
            {
                game.HandleIsTargetLegal = (SCee, point) =>
                    this.IsInRange(SCee, SC01RangeBaize) &&
                    (SC01BaizeFriendIsTargetLegal(SCee) ||
                     SC01BaizeEnemyIsTargetLegal(SCee));
                game.HandleTarget = SCee =>
                {
                    if (IsFriend(SCee))
                    {
                        var buff1 = new BuffGainAttack(SCee, this, int.MaxValue, 0.5, game);
                        buff1.BuffTrigger();
                        SC01Buffs.Add(buff1);
                        var buff2 = new BuffSlowDownGain(SCee, this, int.MaxValue, -1.0/3, game);
                        buff2.BuffTrigger();
                        SC01Buffs.Add(buff2);
                    }
                    else
                    {
                        var buff1 = new BuffGainAttack(SCee, this, int.MaxValue, -0.5, game);
                        buff1.BuffTrigger();
                        SC01Buffs.Add(buff1);
                        var buff2 = new BuffSlowDownGain(SCee, this, int.MaxValue, 1.0/3, game);
                        buff2.BuffTrigger();
                        SC01Buffs.Add(buff2);
                    }
                };
            }
            else
            {
                game.HandleIsLegalClick = point => game[point] != null;
                game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
                game.HandleTarget = SCee =>
                {
                    var buff = new BuffCannotAttackAndBeAttacked(SCee, this, this.Interval, game);
                    buff.BuffTrigger();
                };
                game.HandleResetShow = () =>
                {
                    game.DefaultButtonAndLabels();
                    game.Characters.SetLabelBackground();
                    game.SetCurrentLabel();
                };
            }
            AddPadButtonEvent(0);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            if (IsBaize)
            {
                game.HandleIsTargetLegal = (SCee, point) => IsEnemy(SCee) && Math.Abs(SCee.X - this.X) <= 1;
                game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 2);
            }
            else
            {
                
            }
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            if (IsBaize)
            {
                game.HandleIsLegalClick = point => IsFriend(game[point]);
                game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
                game.HandleTarget = SCee =>
                {

                };
            }
            else
            {
                game.HandleIsLegalClick = point => IsFriend(game[point]);
                game.HandleIsTargetLegal = (SCee, point) =>
                {
                    var c = game[point];
                    return point.IsInRange(SCee, c.MoveAbility + c.AttackRange);
                };
                game.HandleTarget = SCee =>
                {
                    SCee.HandleBeAttacked = (damage, attacker) => SCee.BeAttacked(damage*3/2, attacker);
                };
                game.HandleResetShow = () =>
                {
                    game.DefaultButtonAndLabels();
                    game.Characters.Where(c => IsFriend(c)).SetLabelBackground();
                };
            }
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        private bool SC01BaizeFriendIsTargetLegal(Character SCee)
        {
            return SCee is IHuman && IsFriend(SCee);
        }

        private bool SC01BaizeEnemyIsTargetLegal(Character SCee)
        {
            return IsEnemy(SCee) && !(SCee is IHuman);
        }
    }
}

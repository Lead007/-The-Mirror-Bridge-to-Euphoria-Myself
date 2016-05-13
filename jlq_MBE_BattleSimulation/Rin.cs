using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>冴月麟</summary>
    class Rin : Character
	{
		public Rin(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡01
            //显示有效单击点
		    enterButton[0] = (s, ev) =>
		    {
		        this.game.DefaultButtonAndLabels();
		        Game.PadPoints.Where(SC01IsLegalClick)
		            .Aggregate(GameColor.BaseColor, (c, point) => game[point].LabelDisplay.Background = GameColor.LabelBackground);
		        pointTemp1 = Game.DefaultPoint;
		    };
            SetDefaultLeaveSCButtonDelegate(0);
            //显示将瞬移到的点和将被攻击的目标
		    enterPad[0] = (s, ev) =>
		    {
		        if (!this.game.HandleIsLegalClick(this.game.MousePoint)) return;
		        this.game.DefaultButtonAndLabels();
		        if (this.Position != pointTemp1)
		        {
		            game.GetButton(pointTemp1).Opacity = 1;
		        }
		        game.Characters.Where(SCee => IsInRangeAndEnemy(pointTemp1, SC01Range2, SCee))
		            .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
                pointTemp1 = Game.DefaultPoint;
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示有效单击点
            enterButton[1] = (s, ev) =>
		    {
		        this.game.DefaultButtonAndLabels();
		        game.Characters.Where(c => IsInRangeAndEnemy(this.Position, SC02Range, c))
		            .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
		    };
            SetDefaultLeaveSCButtonDelegate(1);
            //显示将被攻击的目标
		    enterPad[1] = (s, ev) =>
		    {
		        if (!this.game.HandleIsLegalClick(game.MousePoint)) return;
		        this.game.DefaultButtonAndLabels();
		        game[game.MousePoint].LabelDisplay.Background = GameColor.LabelBackground;
		    };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将被攻击的目标
            enterPad[2] = (s, ev) =>
		    {
		        this.game.DefaultButtonAndLabels();
		        game.Characters.Where(c => IsInRangeAndEnemy(game.MousePoint, SC03Range, c))
		            .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        /// <summary>天赋范围</summary>
        private const int skillRange = 2;
        /// <summary>符卡01的参数</summary>
	    private const int SC01Range = 3;
	    private const int SC01Range2 = 2;
        private const float SC01DamageGain = 0.5f;
        /// <summary>符卡02的参数</summary>
        private const int SC02Range = 4;
        private const float SC02DamageGain = 2.0f;
        /// <summary>符卡03的参数</summary>
        private const int SC03Range = 1;
        private const float SC03DamageGain = 0.7f;

	    private Point pointTemp1 = Game.DefaultPoint;

        /// <summary>天赋：当你受到攻击时，对2格内随机一名敌方单位造成所受伤害30%的真实伤害</summary>
        /// <param name="damage">伤害值</param>
        /// <param name="attacker">攻击者</param>
        public override void BeAttacked(int damage, Character attacker)
        {
            base.BeAttacked(damage, attacker);
            var legalTarget = game.Characters.Where(c => IsInRangeAndEnemy(this.Position, skillRange, c)).ToArray();
            if (legalTarget.Length == 0) return;
            var index = random.Next(legalTarget.Length);
            var target = legalTarget[index];
            //判断是否命中
            if (HandleIsHit(target)) return;
            //造成无来源伤害
            var damageNew = (int) (damage*0.3*FloatDamage);
            target.BeAttacked(damageNew, null);
        }

        //符卡
        /// <summary>符卡01：乘着风，瞬移到3格内一名敌方角色面前，并释放旋风对自身2格内所有敌方单位造成0.5倍率的伤害</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(pointTemp1, SC01Range2, SCee);
            game.HandleSelf = () => Move(pointTemp1);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC01DamageGain);
            AddPadButtonEvent(0);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
        }

        /// <summary>符卡02：孤独绽放的，对4格内一名敌方单位造成2.0倍率的伤害</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point =>
            {
                var c = game[point];
                return c != null && IsInRangeAndEnemy(this.Position, SC02Range, c);
            };
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC02DamageGain);
            enterButton[1](null, null);

        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03：对一点周围1格内所有敌方角色造成0.7倍率的伤害</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => true;
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, SC03Range, SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03DamageGain);
            AddPadButtonEvent(2);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
            RemovePadButtonEvent(2);
        }

        public override void SCShow()
        {
            for (var i = 0; i < 2; i++)
            {
                AddSCButtonEvent(i);
            }
        }

        public override void ResetSCShow()
        {
            for (var i = 0; i < 2; i++)
            {
                RemoveSCButtonEvent(i);
            }
        }

        private bool SC01IsLegalClick(Point point)
        {
            var c = game[point];
            if (c == null || (!IsInRangeAndEnemy(this.Position, SC01Range, c))) return false;
            pointTemp1 = c.Y == this.Y
                ? new Point(c.X + (c.X > this.X ? -1 : 1), c.Y)
                : new Point(c.X, c.Y + (c.Y > this.Y ? -1 : 1));
            if (this.Position == pointTemp1 || game[pointTemp1] == null) return true;
            pointTemp1 = Game.DefaultPoint;
            return false;
        }
    }
}

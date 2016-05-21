using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>小恶魔</summary>
    class Koakuma : Character
	{
		public Koakuma(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //符卡02
            //显示将获得buff的队友
		    enterPad[1] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsFriend(c)) return;
		        game.DefaultButtonAndLabels();
		        c.LabelDisplay.Background = GameColor.LabelBackground;
		    };
            SetDefaultLeavePadButtonDelegate(1);
		}

	    private const int SC01Range = 4;

	    public override void PreparingSection()
	    {
	        MpGain(this.Mp/10);
            Cure(this.Hp/10);
	    }

	    //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => Calculate.Distance(point, this) <= SC01Range && IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                var buff = new BuffGainBeDamaged(SCee, this, this.BuffTime, 0.2f, game);
                buff.BuffTrigger();
            };
            //显示可攻击目标
            game.DefaultButtonAndLabels();
            game.SetCurrentLabel();
            Enemy.Where(c => Calculate.Distance(c, this) <= SC01Range)
                .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => IsFriend(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                var buff1 = new BuffCure(SCee, this, this.BuffTime, SCee.MaxHp/10, game);
                var buff2 = new BuffMpGain(SCee, this, this.BuffTime, SCee.MaxMp/10, game);
            };
            AddPadButtonEvent(1);
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            RemovePadButtonEvent(1);
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            //TODO SC03
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {

        }
    }
}

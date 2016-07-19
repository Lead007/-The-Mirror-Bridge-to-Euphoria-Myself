using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>小恶魔</summary>
    public class Koakuma : Character
	{
		public Koakuma(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示将受影响的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (!IsInRangeAndEnemy(SC01Range, game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        game.MouseCharacter.SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将受影响的角色
		    enterPad[1] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsFriend(c)) return;
		        game.DefaultButtonAndLabels();
		        c.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
		}
        
	    public override void PreparingSection()
	    {
            base.PreparingSection();
	        MpGain(this.Mp/10);
	    }

        //符卡
        private const int SC01Range = 4;
        private float SC01Parameter => (1 + (int)this.CharacterLevel)*0.1f;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(SC01Range, point);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                var buff = new BuffGainBeDamaged(SCee, this, this.BuffTime, SC01Range, game);
                buff.BuffTrigger();
            };
            //显示可指定目标
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                Enemies.Where(c => c.Distance(this) <= SC01Range).SetLabelBackground();
            };
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
                var buff1 = new BuffCure(SCee, this, this.BuffTime, SCee.MaxHp/20, game);
                var buff2 = new BuffMpGain(SCee, this, this.BuffTime, SCee.MaxMp/20, game);
            };
            AddPadButtonEvent(1);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                game.Characters.Where(c => IsFriend(c)).SetLabelBackground();
            };
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
            base.EndSC03();
        }
    }
}

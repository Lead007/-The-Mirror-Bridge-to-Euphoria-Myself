using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>冴月麟</summary>
    public class Rin : CharacterHitBack
	{
		public Rin(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示将瞬移到的点和将被攻击的目标
		    enterPad[0] = (s, ev) =>
		    {
		        if (!game.HandleIsLegalClick(this.game.MousePoint)) return;
		        this.game.DefaultButtonAndLabels();
		        var p = this.Position.FacePoint(game.MousePoint);
		        if (this.Position != p)
		        {
                    game.GetButton(p).SetButtonColor();
                }
		        EnemyInRange(p, SC01Range2).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被攻击的目标
		    enterPad[1] = (s, ev) =>
		    {
		        if (!this.game.HandleIsLegalClick(game.MousePoint)) return;
		        this.game.DefaultButtonAndLabels();
		        game.MouseCharacter.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将被攻击的目标
            enterPad[2] = (s, ev) =>
		    {
		        this.game.DefaultButtonAndLabels();
		        EnemyInMouseRange(SC03Range).SetLabelBackground();
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

        //天赋
        protected override float HitBackGain => 0.3f;

        protected override IEnumerable<Character> LegalHitBackTarget => EnemyInRange(skillRange);

        //符卡
        /// <summary>符卡01：乘着风，瞬移到3格内一名敌方角色面前，并释放旋风对自身2格内所有敌方单位造成0.5倍率的伤害</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point =>
            {
                if (!IsInRangeAndEnemy(SC01Range, game[point])) return false;
                var ct = game[this.Position.FacePoint(point)];
                return ct == null || ct == this;
            };
            game.HandleIsTargetLegal =
                (SCee, point) => IsInRangeAndEnemy(this.Position.FacePoint(point), SC01Range2, SCee);
            game.HandleSelf = () => Move(this.Position.FacePoint(game.MousePoint));
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC01DamageGain);
            AddPadButtonEvent(0);
            game.HandleResetShow = () =>
            {
                this.game.DefaultButtonAndLabels();
                Game.PadPoints.Where(game.HandleIsLegalClick).Select(p => game[p]).SetLabelBackground();
            };
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
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(SC02Range, point);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC02DamageGain);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                EnemyInRange(SC02Range).SetLabelBackground();
            };
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
    }
}

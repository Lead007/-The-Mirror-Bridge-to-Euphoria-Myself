using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>大妖精</summary>
    public class Daiyousei : Character
	{
		public Daiyousei(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示将瞬移到的点和将回血的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (!game.HandleIsLegalClick(game.MousePoint)) return;
		        this.game.DefaultButtonAndLabels();
		        if (this.Position == game.MousePoint) return;
		        if (this.Position != pointTemp1)
		        {
		            game.GetButton(pointTemp1).SetButtonColor();
		        }
		        game.MouseCharacter.SetLabelBackground();
		        pointTemp1 = Game.DefaultPoint;
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被攻击的角色
		    enterPad[1] = (s, ev) =>
		    {
		        if (!IsInRangeAndEnemy(SC02Range, game.MousePoint)) return;
		        this.game.DefaultButtonAndLabels();
		        game.MouseCharacter.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将回血的角色
		    enterButton[2] = (s, ev) =>
		    {
                this.game.DefaultButtonAndLabels();
		        game.Characters.Where(c => IsInRangeAndFriend(SC03Range, c, false)).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private const int skillRange = 2;
        private static PercentOfMaxHp skillGain { get; } = new PercentOfMaxHp(0.05f);
        private const int SC01Range = 4;
        private const int SC02Range = 4;
        private const float SC02Gain = 1.5f;
        private const int SC03Range = 2;

        private PadPoint pointTemp1 = Game.DefaultPoint;

        /// <summary>天赋：雾之湖的恩惠</summary>
        public override void PreparingSection()
        {
            base.PreparingSection();
            game.Characters.Where(c => IsInRangeAndFriend(skillRange, c, false)).DoAction(c => c.Cure(skillGain));
        }

        //符卡
        /// <summary>符卡01：贴心的妖精，选择4格内一个目标（可以是自己），瞬移到他背后（如果是自己就不用瞬移），并使目标回复自己攻击力0.7倍率的目标血量。</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () => Move(pointTemp1);
            game.HandleTarget = SCee => SCee.Cure(0.7*this.Attack);
            AddPadButtonEvent(0);
            game.HandleResetShow = () =>
            {
                this.game.DefaultButtonAndLabels();
                Game.PadPoints.Where(point => this.Position != point && SC01IsLegalClick(point))
                    .Select(p => game[p])
                    .SetLabelBackground();
                pointTemp1 = Game.DefaultPoint;
            };
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
        }

        /// <summary>符卡02：花仙炮，对4格内一敌方目标造成1.5倍率的伤害。</summary>
        public override void SC02()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(SC02Range, point);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, 1.5f);
            AddPadButtonEvent(1);
            game.HandleResetShow = () =>
            {
                this.game.DefaultButtonAndLabels();
                EnemyInRange(SC02Range).SetLabelBackground();
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            RemovePadButtonEvent(1);
        }
        /// <summary>符卡03：妖精狂欢，使2格内所有己方角色恢复自己攻击力1.5倍率的血量</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndFriend(SC03Range, SCee);
            game.HandleTarget = SCee => SCee.Cure(SCee.Attack*SC02Gain);
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

        private bool SC01IsLegalClick(PadPoint point)
        {
            if (!point.IsInRange(this, SC01Range) || game[point] == null) return false;
            if (point == this.Position)
            {
                pointTemp1 = point;
                return true;
            }
            if (point.Row == this.Row)
            {
                if (point.Column > this.Column)
                {
                    if (point.Column == 8) return false;
                    pointTemp1 = new PadPoint(point.Column + 1, point.Row);
                    if (game[pointTemp1] == null) return true;
                    pointTemp1 = Game.DefaultPoint;
                    return false;
                }
                if (point.Column == 0) return false;
                pointTemp1 = new PadPoint(point.Column - 1, point.Row);
                if (game[pointTemp1] == null) return true;
                pointTemp1 = Game.DefaultPoint;
                return false;
            }
            if (point.Row > this.Row)
            {
                if (point.Row == 8) return false;
                pointTemp1 = new PadPoint(point.Column, point.Row + 1);
                if (game[pointTemp1] == null) return true;
                pointTemp1 = Game.DefaultPoint;
                return false;
            }
            if (point.Row == 0) return false;
            pointTemp1 = new PadPoint(point.Column, point.Row - 1);
            if (game[pointTemp1] == null) return true;
            pointTemp1 = Game.DefaultPoint;
            return false;
        }
    }
}

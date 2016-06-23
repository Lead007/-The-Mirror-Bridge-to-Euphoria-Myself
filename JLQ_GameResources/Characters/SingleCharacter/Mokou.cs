using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>藤原妹红</summary>
    public class Mokou : Character, IHuman
    {
        public Mokou(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示有效单击点
            enterButton[0] = (s, ev) =>
            {
                this.game.DefaultButtonAndLabels();
                Game.PadPoints.Where(SC01IsLegalClick).Select(p => game[p]).SetLabelBackground();
                pointTemp1 = Game.DefaultPoint;
            };
            SetDefaultLeaveSCButtonDelegate(0);
            //显示将瞬移到的点和将被攻击的目标
            enterPad[0] = (s, ev) =>
            {
                if (!SC01IsLegalClick(game.MousePoint)) return;
                game.DefaultButtonAndLabels();
                if (this.Position != pointTemp1)
                {
                    game.GetButton(pointTemp1).SetButtonColor();
                }
                game.MouseCharacter.SetLabelBackground();
                pointTemp1 = Game.DefaultPoint;
            };
            SetDefaultLeavePadButtonDelegate(0);
        }

        public Human HumanKind => Human.Mokou;

        private PadPoint pointTemp1 = Game.DefaultPoint;

        //TODO 天赋
        public override void PreparingSection()
        {
            base.PreparingSection();
            this.Cure(new PercentOfMaxHp(0.1f));
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = SC01IsLegalClick;
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleSelf = () =>
            {
                this.Move(pointTemp1);
                HandleBeAttacked(this.MaxHp/10, this);
            };
            game.HandleTarget = SCee => HandleDoAttack(SCee, 1.2f);
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
            //TODO SC02
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {

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

        private bool SC01IsLegalClick(PadPoint point)
        {
            var c = game[point];
            if (!IsInRangeAndEnemy(3, c)) return false;
            pointTemp1 = c.Row == this.Row
                ? new PadPoint(c.Column + (c.Column > this.Column ? -1 : 1), c.Row)
                : new PadPoint(c.Column, c.Row + (c.Row > this.Row ? -1 : 1));
            if (this.Position == pointTemp1 || game[pointTemp1] == null) return true;
            pointTemp1 = Game.DefaultPoint;
            return false;
        }

    }
}

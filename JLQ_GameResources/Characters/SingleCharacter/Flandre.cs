using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using JLQ_GameResources.Characters.CharacterBeCalled;
using RandomHelper;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>芙兰</summary>
    public class Flandre : Character
	{
		public Flandre(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
		    enterButton[1] = (s, ev) =>
		    {
		        SC02points.Select(game.GetButton).SetButtonColor();
		    };
            SetDefaultLeaveSCButtonDelegate(1);
		    enterPad[2] = (s, ev) =>
		    {
		        if (!game.HandleIsLegalClick(game.MousePoint)) return;
		        game.DefaultButtonAndLabels();
		        Enemies.Where(c => game.MousePoint.IsIn33(c)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(2);
		}

        public override void PreparingSection()
        {
            base.PreparingSection();
            game.ButtonSC[0].IsEnabled = false;
        }

        //天赋
        public override void BeAttacked(int damage, Character attacker)
        {
            base.BeAttacked(damage, attacker);
            if (attacker == null) return;
            var buff = new BuffBlooding(attacker, this, SC01Parameter*this.Interval, game);
        }

        //符卡
        private int SC01Parameter => 3 + (int)this.CharacterLevel;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            throw new NotImplementedException();
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        private const int SC02Range = 2;
        private int SC02Num => Math.Min(SC02points.Count, this.CharacterLevel > Level.Normal ? 3 : 2);
        private List<FlandreLittle> FList { get; } = new List<FlandreLittle>();
        private List<PadPoint> SC02points
            => Game.PadPoints.Where(p => p.Distance(this) <= SC02Range && game[p] == null).ToList();
        private float SC02Parameter => (1 + (int) this.CharacterLevel)*0.1f;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                foreach (var p in random.RandomElements(SC02Num, SC02points))
                {
                    game.AddCharacter(p, this.Group, typeof (FlandreLittle), p, this.Group, SC02Parameter, this.game);
                    this.FList.Add(game.Characters.Last() as FlandreLittle);
                }
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }

        private const float SC03Gain = 1.7f;
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick =
                point => point.Column > 0 && point.Column < Game.Column - 1 && point.Row > 0 && point.Row < Game.Row - 1;
            game.HandleIsTargetLegal = (SCee, point) => point.IsIn33(SCee) && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain);
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

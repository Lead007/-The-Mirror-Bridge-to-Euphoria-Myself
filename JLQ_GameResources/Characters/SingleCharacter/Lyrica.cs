using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Dialogs.GamePad.ChooseLines;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>莉莉卡·普莉兹姆利巴</summary>
    public class Lyrica : CharacterPoltergeist
	{
		public Lyrica(int id, Point position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示将被攻击的角色
		    enterButton[0] = (s, ev) =>
		    {
                game.DefaultButtonAndLabels();
		        Enemy.Where(c => c.Y > this.Y).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(0);
		}

        private const float SC01Gain = 0.3f;

        public override void PreparingSection()
        {
            base.PreparingSection();
            game.Characters.Where(c => IsInRangeAndEnemy(skillRange, c)).DoAction(c => c.BeAttacked(10, this));
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => SCee.Y > this.Y && IsEnemy(SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC01Gain);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        /// <summary>符卡02</summary>
        public override void SC02()
        {
            var dialog = new GamePad_LyricaSC02(game);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                game.HandleIsTargetLegal = (SCee, point) => dialog.LinesChoose.Contains(SCee.Y);
                game.HandleTarget = SCee =>
                {
                    if (IsFriend(SCee))
                    {
                        var buff = new BuffCure(SCee, this, BuffTime, SCee.MaxHp/10, game);
                    }
                    else if (IsEnemy(SCee))
                    {
                        var buff = new BuffBeAttacked(SCee, this, BuffTime, SCee.MaxHp/10, this, game);
                    }
                };
            }
            else
            {
                game.HandleIsLegalClick = point => false;
            }
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
	}
}

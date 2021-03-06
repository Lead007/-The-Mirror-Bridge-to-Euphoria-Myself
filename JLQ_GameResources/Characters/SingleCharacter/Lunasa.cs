using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Dialogs.GamePad.ChooseLines;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>露娜萨·普莉兹姆利巴</summary>
    public class Lunasa : CharacterPoltergeist
	{
		public Lunasa(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //符卡01
            //显示将被攻击的角色
		    enterPad[0] = (s, ev) =>
		    {
		        var c = game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        c.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(0);
		}

        private int SC01Parameter => 3 + ((int)this.CharacterLevel << 1);
        public override void PreparingSection()
        {
            base.PreparingSection();
            EnemyInRange(skillRange).Select(c => new BuffSlowDown(c, this, this.Interval, SC01Parameter, game))
                .DoAction(b => b.BuffTrigger());
        }

        //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => IsEnemy(game[point]);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee);
            AddPadButtonEvent(0);
            game.HandleResetShow = () =>
            {
                game.DefaultButtonAndLabels();
                Enemies.SetLabelBackground();
            };
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
            var dialog = new GamePad_LunasaSC02(game);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                game.HandleIsTargetLegal = (SCee, point) => dialog.LinesChoose.Contains(SCee.Column);
                game.HandleTarget = SCee =>
                {
                    if (IsFriend(SCee))
                    {
                        var buff = BuffGainProperty.BuffGainHitRate(SCee, this, 2*this.Interval, 0.1f, game);
                        buff.BuffTrigger();
                    }
                    else if (IsEnemy(SCee))
                    {
                        var buff = BuffGainProperty.BuffGainAttack(SCee, this, 2*this.Interval, -0.1f, game);
                        buff.BuffTrigger();
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

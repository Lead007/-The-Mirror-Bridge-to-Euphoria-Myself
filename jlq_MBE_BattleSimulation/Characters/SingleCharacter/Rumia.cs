using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_MBE_BattleSimulation.Buffs.SingleBuff;

namespace JLQ_MBE_BattleSimulation.Characters.SingleCharacter
{
    /// <summary>露米娅</summary>
    public class Rumia : Character
	{
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">位置</param>
        /// <param name="group">阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
		public Rumia(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
            //显示将被攻击的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (game.MouseCharacter != null) return;
		        game.DefaultButtonAndLabels();
		        game.Characters.Where(c => IsInRangeAndEnemy(game.MousePoint, SC01Range, c)).SetLabelBackground();
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //显示将被攻击的角色
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        game.Characters.Where(SCee => IsInRangeAndEnemy(SC03Range, SCee)).SetLabelBackground();
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

	    private const int SC01Range = 2;
	    private const int SC03Range = 2;
	    private const float SC03Gain = 1.5f;

        /// <summary>天赋的标记数量</summary>
        public int SkillNum = 1;
	    private List<Character> _skillBeSymboled = new List<Character>(); 

        /// <summary>天赋</summary>
	    public override void PreparingSection()
	    {
	        _skillBeSymboled.Clear();
	        var num = Math.Min(SkillNum, Enemy.Count());
	        var cList = Enemy.OrderBy(c => c.Hp, new IntRandomComparer(random));
	        for (var i = 0; i < num; i++)
	        {
	            var c = cList.ElementAt(i);
	            var buff = new BuffGainBeDamaged(c, this, this.Interval, 0.5f, game);
	            buff.BuffTrigger();
	        }
	    }

	    //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => game[point] == null;
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, SC01Range, SCee);
            game.HandleSelf = () => Move(game.MousePoint);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, 0.7f);
                var buff = new BuffShield(this, this, 3*Interval, game);
                buff.BuffTrigger();
            };
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
            //TODO NO CONSUME
            game.HandleIsTargetLegal = (SCee, point) => SCee == this;
            game.HandleTarget = SCee =>
            {
                var buff = new BuffAddRumiaSkillNum(this, game);
                buff.BuffTrigger();
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal =
                (SCee, point) => IsInRangeAndEnemy(SC03Range, SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain);
            //TODO back mp
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
    }
}

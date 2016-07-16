using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using RandomHelper;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>露米娅</summary>
    public class Rumia : Character
    {
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">位置</param>
        /// <param name="group">阵营</param>
        /// <param name="game">游戏对象</param>
		public Rumia(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {
            //符卡01
            //显示将瞬移到的点和将被攻击的角色
            enterPad[0] = (s, ev) =>
            {
                if (game.MouseCharacter != null) return;
                game.DefaultButtonAndLabels();
                game.GetButton(game.MousePoint).SetButtonColor();
                EnemyInMouseRange(SC01Range).SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡03
            //显示将被攻击的角色
            enterButton[2] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                EnemyInRange(SC03Range).SetLabelBackground();
            };
            SetDefaultLeaveSCButtonDelegate(2);
        }
        
        /// <summary>天赋的标记数量</summary>
        public int SkillNum = 1;
        private const float SkillGain = 1.5f;
        private List<Character> _skillBeSymboled = new List<Character>();

        public override void PreparingSection()
        {
            _skillBeSymboled.Clear();
            var cList = Enemies.OrderBy(c => c.Hp, new RandomComparer<int>(random)).ToList();
            var num = Math.Min(SkillNum, cList.Count);
            _skillBeSymboled = cList.GetRange(0, num);
        }

        public override bool DoingAttack(Character target, float times = 1)
            => base.DoingAttack(target, times * (_skillBeSymboled.Contains(target) ? SkillGain : 1));

        public override string ToString()
        {
            var bs = base.ToString() + "\n标记角色：\n";
            if (_skillBeSymboled == null) return bs + "暂无。";
            return _skillBeSymboled.Aggregate(bs,
                (cu, c) => cu + string.Format("{0}号{1}[位置：{2}]\n", c.ID, c.Name, c.Position));
        }

        //符卡
        private const int SC01Range = 2;
        private const float SC01Gain = 0.7f; 
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => game[point] == null;
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, SC01Range, SCee);
            game.HandleSelf = () => Move(game.MousePoint);
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, SC01Gain);
                var buff = BuffAddProperty.BuffAddDodgeRate(this, this, this.Interval, 10, game);
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

        private const float SC02Gain1 = 0.1f;
        private const float SC02Gain2 = 0.1f;
        private int SC02Parameter => (int)this.CharacterLevel + 1;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                this.BeAttacked((int)(SC02Gain1 * this.MaxHp), this);
                this.MpGain((int)(SC02Gain2 * this.MaxMp));
                var buff = new BuffAddRumiaSkillNum(this, SC02Parameter, game);
                buff.BuffTrigger();
            };
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }

        private const int SC03Range = 2;
        private const float SC03Gain1 = 1.5f;
        private float SC03Gain2 => 0.2f*(1 + (int)this.CharacterLevel);
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal =
                (SCee, point) => IsInRangeAndEnemy(SC03Range, SCee);
            game.HandleTarget = SCee => HandleDoDanmakuAttack(SCee, SC03Gain1);
            //TODO back mp
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
    }
}

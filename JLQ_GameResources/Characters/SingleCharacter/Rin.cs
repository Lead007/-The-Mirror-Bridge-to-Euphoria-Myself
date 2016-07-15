using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>冴月麟</summary>
    public class Rin : Character
	{
		public Rin(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
		}

        //天赋
        /// <summary>回合内是否移动</summary>
        private bool HasMovedInRound { get; set; } = false;
        /// <summary>回合内是否攻击</summary>
        private bool HasAttackedInRound { get; set; } = false;

        public override void ActionWhileAttacked()
        {
            base.ActionWhileAttacked();
            HasAttackedInRound = true;
        }

        public override void ActionWhileMoved()
        {
            base.ActionWhileMoved();
            HasMovedInRound = true;
        }

        /// <summary>天赋增益</summary>
        private const float SkillGain1 = 0.1f;
        public override void EndSection()
        {
            base.EndSection();
            if (!HasMovedInRound)
            {
                
            }
            if (!HasAttackedInRound) this.MpGain((int)(SkillGain1*this.MaxMp));
            this.HasAttackedInRound = false;
            this.HasMovedInRound = false;
        }

        /// <summary>是否执行天赋的伤害增益</summary>
        private bool DoDamageGain { get; set; }
        /// <summary>天赋的伤害增益</summary>
        private const float SkillGain2 = 0.5f;
        public override bool DoingAttack(Character target, float times = 1)
        {
            return base.DoingAttack(target, times*(DoDamageGain ? (1 + SkillGain2) : 1));
        }

        //符卡
        private const float SC01Gain1 = 1.5f;
        private const int SC01Gain2 = 1;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => this.Position.RelativeDirection(point) != null;
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                var direction = (Direction) this.Position.RelativeDirection(game.MousePoint);
                while (MoveNext(direction) == true){ }
                if (MoveNext(direction) == false)
                {
                    HandleDoDanmakuAttack(game[NextPoint(this.Position, direction)], SC01Gain1);
                }

            };
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        #region 符卡01相关函数
        /// <summary>下一个点</summary>
        /// <param name="origin">起点</param>
        /// <param name="direction">方向</param>
        /// <returns>下一个点</returns>
        private static PadPoint NextPoint(PadPoint origin, Direction direction)
        {
            var pointNext = origin;
            switch (direction)
            {
                case Direction.Up:
                    pointNext.Row--;
                    break;
                case Direction.UpRight:
                    pointNext.Row--;
                    pointNext.Column++;
                    break;
                case Direction.Right:
                    pointNext.Column++;
                    break;
                case Direction.DownRight:
                    pointNext.Row++;
                    pointNext.Column++;
                    break;
                case Direction.Down:
                    pointNext.Row++;
                    break;
                case Direction.DownLeft:
                    pointNext.Row++;
                    pointNext.Column--;
                    break;
                case Direction.Left:
                    pointNext.Column--;
                    break;
                case Direction.UpLeft:
                    pointNext.Row--;
                    pointNext.Column--;
                    break;
            }
            return pointNext;
        }

        /// <summary>指定方向向下一格移动，返回是否移动成功</summary>
        /// <param name="direction">指定的方向</param>
        /// <returns>true则移动成功，false则碰到敌人，null则碰到队友或出边界</returns>
        private bool? MoveNext(Direction direction)
        {
            var pointNext = NextPoint(this.Position, direction);
            var isSuccess = IsSuccess(pointNext);
            if (isSuccess == true) this.Move(pointNext);
            return isSuccess;
        }

        /// <summary>目标点移动是否成功</summary>
        /// <param name="=target">目标点</param>
        /// <returns>true则成功，false则为敌人，null则为队友或边界</returns>
        private bool? IsSuccess(PadPoint target)
        {
            if (target.Column < 0 || target.Column > Game.Column || target.Row < 0 || target.Row > Game.Row)
                return null;
            var c = game[target];
            if (c == null) return true;
            if (IsEnemy(c)) return false;
            return null;
        }
        #endregion

        /// <summary>符卡02</summary>
        public override void SC02()
        {
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }
        /// <summary>符卡03：对一点周围1格内所有敌方角色造成0.7倍率的伤害</summary>
        public override void SC03()
        {
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
    }
}

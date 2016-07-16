using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameResources.Dialogs.GamePad.ChoosePoints;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>冴月麟</summary>
    public class Rin : Character
    {
        public Rin(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {
            //符卡01
            //显示将移动到的位置将受到影响的角色
            enterPad[0] = (s, ev) =>
            {
                var d = this.Position.RelativeDirection(game.MousePoint);
                if (d == null) return;
                game.DefaultButtonAndLabels();
                var direction = (Direction)d;
                var point = this.Position;
                while (IsSuccess(NextPoint(point, direction)) == true)
                {
                    point = NextPoint(point, direction);
                }
                if (this.Position != point) game.GetButton(point).SetButtonColor();
                point = NextPoint(point, direction);
                if (IsSuccess(point) == null) return;
                game[point].SetLabelBackground();
                NextEnemy(point, direction)?.SetLabelBackground(GameColor.LabelBackground2);
            };
            SetDefaultLeavePadButtonDelegate(0);
            //符卡02
            //显示将被移动的角色
            enterPad[1] = (s, ev) =>
            {
                var c = game.MouseCharacter;
                if (c == null) return;
                game.DefaultButtonAndLabels();
                c.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(1);
            //符卡03
            //显示将被攻击的角色
            enterPad[2] = (s, ev) =>
            {
                var c = game.MouseCharacter;
                if (!IsInRangeAndEnemy(SC03Range, c)) return;
                game.DefaultButtonAndLabels();
                c.SetLabelBackground();
            };
            SetDefaultLeavePadButtonDelegate(2);
        }

        public override void PreparingSection()
        {
            base.PreparingSection();
            if (game[SC02Point] != null) game.ButtonSC[1].IsEnabled = false;
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
                //TODO Unseeable
            }
            if (!HasAttackedInRound) this.MpGain((int)(SkillGain1 * this.MaxMp));
            this.HasAttackedInRound = false;
            this.HasMovedInRound = false;
            if (_cAttack == null) return;
            if (_cAttack.IsDead)
            {
                //TODO back mp
            }
        }

        /// <summary>是否执行天赋的伤害增益</summary>
        private bool DoDamageGain { get; set; }
        /// <summary>天赋的伤害增益</summary>
        private const float SkillGain2 = 0.5f;
        public override bool DoingAttack(Character target, float times = 1)
        {
            return base.DoingAttack(target, times * (DoDamageGain ? (1 + SkillGain2) : 1));
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
                var direction = (Direction)this.Position.RelativeDirection(game.MousePoint);
                while (MoveNext(direction) == true) { }
                if (MoveNext(direction) != false) return;
                var p = NextPoint(this.Position, direction);
                HandleDoDanmakuAttack(game[p], SC01Gain1);
                var c = NextEnemy(p, direction);
                if (c == null) return;
                HandleDoDanmakuAttack(c, SC01Gain2);
            };
            AddPadButtonEvent(0);
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
            RemovePadButtonEvent(0);
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
        /// <param name="target">目标点</param>
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

        /// <summary>指定方向的下一个敌人</summary>
        /// <param name="origin">起点</param>
        /// <param name="direction">方向</param>
        /// <returns>敌人，若不存在则返回null</returns>
        private Character NextEnemy(PadPoint origin, Direction direction)
        {
            var p = NextPoint(origin, direction);
            while (IsSuccess(p) == true || (IsSuccess(p) == null && game[p] != null))
            {
                p = NextPoint(p, direction);
            }
            return game[p];
        }
        #endregion

        /// <summary>麟之印记</summary>
        public PadPoint SC02Point { private get; set; } = Game.DefaultPoint;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            if (SC02Point == Game.DefaultPoint)
            {
                var dialog = new GamePad_RinSC02(this, game);
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    SC02Point = dialog.PointsChoose.First();
                }
                else
                {
                    game.HandleIsLegalClick = point => false;
                    return;
                }
            }
            game.HandleIsLegalClick = point => game[point] != null;
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee => SCee.Move(SC02Point);
            AddPadButtonEvent(1);
        }

        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
            RemovePadButtonEvent(1);
        }

        public override string ToString()
        {
            if (SC02Point == Game.DefaultPoint) return base.ToString();
            return base.ToString() + string.Format("\n麟之印记：{0}", SC02Point);
        }

        private const int SC03Range = 2;
        private const float SC03Parameter = 0.3f;
        private Character _cAttack;
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsLegalClick = point => IsInRangeAndEnemy(SC03Range, point);
            game.HandleIsTargetLegal = (SCee, point) => SCee.Position == point;
            game.HandleTarget = SCee =>
            {
                HandleDoDanmakuAttack(SCee, 1 + SC03Parameter * (SCee.MaxHp - SCee.Hp));
                _cAttack = SCee;
            };
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

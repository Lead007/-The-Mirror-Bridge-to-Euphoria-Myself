using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_BaseBuffs.SingleBuff;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>橙</summary>
    public class Chen : Character
	{
		public Chen(int id, PadPoint position, Group group, Game game)
			: base(id, position, group, game)
		{
            //天赋
		    _skillMove = (l, m) =>
		    {
		        if (!game.IsMoving) return;
		        var c = this.game.MouseCharacter;
		        if (!IsEnemy(c)) return;
		        var p = this.Column == c.Column
		            ? new PadPoint(c.Column, c.Row + (this.Row > c.Row ? 1 : -1))
		            : new PadPoint(c.Column + (this.Column > c.Column ? 1 : -1), c.Row);
		        if (game[p] != null) return;
		        this.Move(p);
		        HandleDoAttack(c, 0.5f);
		        game.HasMoved = true;
		        game.IsMoving = false;
		        game.ResetPadButtons();
		        game.UpdateLabelBackground();
		        //如果同时已经攻击过则进入结束阶段
		        if (!game.HasAttacked || !game.HasMoved) return;
		        //Thread.Sleep(500);
		        game.EndSection();
		    };
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        Enemies.SetLabelBackground();
            };
            SetDefaultLeaveSCButtonDelegate(2);
		}

        private readonly DGridPadClick _skillMove;
        private float _maxHpGain = 1;
        private float MaxHpGain
        {
            get { return _maxHpGain;}
            set
            {
                _maxHpGain = value;
                this.BarHp.Maximum = this.MaxHp;
            }
        }

        //天赋
        public override void PreparingSection()
        {
            base.PreparingSection();
            if (_sc02Flag)
            {
                game.ButtonSC[1].IsEnabled = false;
                EnemyInRange(SC02Range).DoAction(c => this.HandleDoDanmakuAttack(c, SC02Gain2));
            }
            if (_sc03Flag)
            {
                game.ButtonSC[2].IsEnabled = false;
                switch (random.Next(3))
                {
                    case 0:
                        SC03Action1();
                        break;
                    case 1:
                        SC03Action2();
                        break;
                    case 2:
                        SC03Action3();
                        break;
                }
            }
            game.EventGridPadClick += _skillMove;
        }

        public override void EndSection()
        {
            base.EndSection();
            game.EventGridPadClick -= _skillMove;
        }

        //符卡
        private int SC01Parameter2 => (2 + (int) this.CharacterLevel)*2;
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                var buff2 = new BuffSlowDown(this, this, this.BuffTime, SC01Parameter2, game);
                buff2.BuffTrigger();
            };
        }

        /// <summary>结束符卡01</summary>
        public override void EndSC01()
        {
            base.EndSC01();
        }

        private bool _sc02Flag = false;
        private float SC02Gain1 => 0.5f + 0.2f*(int)this.CharacterLevel;
        private const int SC02Range = 1;
        private float SC02Gain2 => (4 + (int)this.CharacterLevel)*0.05f;
        /// <summary>符卡02</summary>
        public override void SC02()
        {
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () =>
            {
                _sc02Flag = true;
                var maxHpOld = this.MaxHp;
                this.MaxHpGain = 1 + SC02Gain1;
                this.Cure(this.MaxHp - maxHpOld);
            };
        }
        
        /// <summary>结束符卡02</summary>
        public override void EndSC02()
        {
            base.EndSC02();
        }

        public override int MaxHp => (int)(base.MaxHp*MaxHpGain);

        private bool _sc03Flag = false;
        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal = (SCee, point) => false;
            game.HandleSelf = () => _sc03Flag = true;
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

        #region 符卡03相关函数
        private void SC03Action1()
        {
            Enemies.Select(c => new BuffBlooding(c, this, this.Interval, game)).DoAction(b => { });
        }

        private void SC03Action2()
        {
            //TODO
        }

        private void SC03Action3()
        {
            Enemies.Select(c => new BuffSlowDown(c, this, this.Interval, 5, game)).DoAction(b => b.BuffTrigger());
        }
        #endregion

        public override string ToString()
        {
            var s = base.ToString();
            if (_sc02Flag) s += "\n符卡02已使用";
            if (_sc03Flag) s += "\n符卡03已使用";
            return s;
        }
    }
}

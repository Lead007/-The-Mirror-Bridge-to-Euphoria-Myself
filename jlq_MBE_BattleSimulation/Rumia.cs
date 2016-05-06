using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JLQ_MBE_BattleSimulation
{
	class Rumia : Character
	{
		public Rumia(int id, Point position, Group group, Random random, Game game)
			: base(id, position, group, random, game)
		{
		    //IsPreventing = new Image
		    //{
		    //    HorizontalAlignment = HorizontalAlignment.Right,
		    //    VerticalAlignment = VerticalAlignment.Top,
		    //    Margin = new Thickness(0, 2, 2, 0),
		    //    Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Resources/Characters/Rumia/Shield.jpg")),
      //          Visibility = Visibility.Hidden,
		    //    Width = 15,
		    //    Height = 15
		    //};
		    //IsPreventing.SetValue(Panel.ZIndexProperty, 6);
      //      ListControls.Add(IsPreventing);
      //      Set();
            //显示将被攻击的角色
		    enterPad[0] = (s, ev) =>
		    {
		        if (game[game.MousePoint] != null) return;
		        game.DefaultButtonAndLabels();
		        game.Characters.Where(c => IsInRangeAndEnemy(game.MousePoint, SC01Range, c))
		            .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
		    };
            SetDefaultLeavePadButtonDelegate(0);
            //显示将被攻击的角色
		    enterButton[2] = (s, ev) =>
		    {
		        game.DefaultButtonAndLabels();
		        game.Characters.Where(SCee => IsInRangeAndEnemy(this.Position, SC03Range, SCee))
		            .Aggregate((Brush) Brushes.White, (cu, c) => c.LabelDisplay.Background = Brushes.LightBlue);
		    };
            SetDefaultLeaveSCButtonDelegate(2);
		}

	    private const int SC01Range = 2;
	    private const int SC03Range = 2;
	    private const float SC03Gain = 1.5f;

	    //private Image IsPreventing;
        public int SkillNum = 1;
	    //private bool __doPrevent;
	    //private bool _doPrevent
	    //{
     //       get { return __doPrevent; }
	    //    set
	    //    {
	    //        __doPrevent = value;
	    //        IsPreventing.Visibility = value ? Visibility.Visible : Visibility.Hidden;
	    //    }
	    //}
	    private List<Character> _skillBeSymboled = new List<Character>(); 

	    public override void PreparingSection()
	    {
	        _skillBeSymboled.Clear();
	        var num = Math.Min(SkillNum, Enemy.Count());
	        var cList = Enemy.OrderBy(c => c.Hp, new IntRandomComparer(random));
	        for (var i = 0; i < num; i++)
	        {
	            var c = cList.ElementAt(i);
	            var buff = new BuffAddDamage(c, this, this.Interval, 0.5f, game);
                c.BuffList.Add(buff);
	            buff.BuffTrigger();
	        }
	    }

	    //public override void BeAttacked(int damage, Character attacker)
	    //{
	    //    if (_doPrevent)
	    //    {
	    //        _doPrevent = false;
	    //        return;
	    //    }
	    //    base.BeAttacked(damage, attacker);
	    //}

	    //符卡
        /// <summary>符卡01</summary>
        public override void SC01()
        {
            game.HandleIsLegalClick = point => game[point] == null;
            game.HandleIsTargetLegal = (SCee, point) => IsInRangeAndEnemy(point, SC01Range, SCee);
            game.HandleSelf = () => Move(game.MousePoint);
            game.HandleTarget = SCee =>
            {
                DoAttack(SCee, 0.7f);
                var buff = new BuffShield(this, 3*Interval, game);
                this.BuffList.Add(buff);
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
                this.BuffList.Add(buff);
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
                (SCee, point) => IsInRangeAndEnemy(this.Position, SC03Range, SCee);
            game.HandleTarget = SCee => DoAttack(SCee, SC03Gain);
            //TODO back mp
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

	    public override void SCShow()
	    {
	        AddSCButtonEvent(2);
	    }

	    public override void ResetSCShow()
	    {
	        RemoveSCButtonEvent(2);
	    }
    }
}

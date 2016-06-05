using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using JLQ_GameBase;
using JLQ_GameBase.Buffs;

namespace JLQ_GameResources.Buffs.SingleBuff
{
    /// <summary>获得持续一段时间的护盾，可抵消这段时间内受到的第一个伤害</summary>
    public class BuffShield : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="game">游戏对象</param>
        public BuffShield(Character buffee, Character buffer, int time, Game game)
            : base(buffee, buffer, time, string.Format("护盾：{0}时间单位内防止受到的第一次伤害", time), true, game)
        {
            _imageShield = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Width = 10,
                Height = 10
            };
            try
            {
                _imageShield.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Resources/Image/Buff/Shield.jpg"));
            }
            catch
            {
                _imageShield.Source = new BitmapImage(new Uri("Images\\Error.jpg", UriKind.Relative));
            }
            _imageShield.SetValue(Panel.ZIndexProperty, 6);
        }

        private readonly Image _imageShield;
        private DBeAttacked temp;

        protected override void BuffAffect()
        {
            Buffee.AddStateControl(_imageShield);
            temp = Buffee.HandleBeAttacked.Clone() as DBeAttacked;
            Buffee.HandleBeAttacked = (damage, attacker) => this.BuffEnd();
        }

        protected override void Cancel()
        {
            Buffee.RemoveStateControl(_imageShield);
            Buffee.HandleBeAttacked = temp;
            base.Cancel();
        }
    }
}

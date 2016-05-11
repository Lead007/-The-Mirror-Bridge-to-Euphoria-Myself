using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JLQ_MBE_BattleSimulation
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
                Margin = new Thickness(0, 2, 2, 0),
                Width = 15,
                Height = 15
            };
            try
            {
                _imageShield.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Resources/Image/Buff/Shield.jpg"));
            }
            catch
            {
                _imageShield.Source = BitmapConverter.BitmapConverter.BitmapToBitmapImage(Properties.Resources.Error);
            }
            _imageShield.SetValue(Panel.ZIndexProperty, 6);
            HasPrevented = true;
            BuffAffect += (bee, ber) =>
            {
                bee.ListControls.Add(_imageShield);
                bee.Set();
                game.GridPad.Children.Add(_imageShield);
                temp = (DBeAttacked) bee.HandleBeAttacked.Clone();
                bee.HandleBeAttacked = (damage, attacker) =>
                {
                    if (HasPrevented)
                    {
                        HasPrevented = false;
                        return;
                    }
                    temp(damage, attacker);
                };
            };
            BuffCancels += (bee, ber) =>
            {
                game.GridPad.Children.Remove(_imageShield);
                Buffer.ListControls.Remove(_imageShield);
                bee.HandleBeAttacked = temp;
            };
        }

        private readonly Image _imageShield;
        private bool _hasPrevented;
        private DBeAttacked temp;

        private bool HasPrevented
        {
            get { return _hasPrevented;}
            set
            {
                _hasPrevented = value;
                _imageShield.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /// <summary>重写ToString方法</summary>
        /// <returns>字符串化结果</returns>
        public override string ToString()
            => string.Format("{0}({1}使用) By:{2} 剩余时间：{3}", Name, HasPrevented ? "未" : "已", Buffer.Data.Name, Time);

    }
}

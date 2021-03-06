﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JLQ_GameBase;

namespace JLQ_GameResources.Dialogs
{
    /// <summary>
    /// Dialog_GamePad.xaml 的交互逻辑
    /// </summary>
    public abstract partial class Dialog_GamePad : Window
    {
        protected const string tick = "√";
        public readonly Grid[] Grids;
        protected Game game { get; }
        protected Dialog_GamePad(Game game)
        {
            InitializeComponent();

            this.game = game;

            Grids = new[] {GridLeft, GridRight, GridBottom, GridTop};
        }

        protected void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        protected void GridPad_Loaded(object sender, RoutedEventArgs e)
        {
            #region PadBorders
            var borders = new Border[Game.Column, Game.Row];
            for (var i = 0; i < Game.Column; i++)
                for (var j = 0; j < Game.Row; j++)
                {
                    var b = borders[i, j];
                    b = new Border
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = GameColor.PadBrush,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    b.SetValue(Grid.ColumnProperty, i);
                    b.SetValue(Grid.RowProperty, j);
                    GridPad.Children.Add(b);
                }
            #endregion
            #region Character List Controls
            foreach (var ui in game.Characters.SelectMany(c => c.ListControls))
            {
                game.GridPad.Children.Remove(ui);
                GridPad.Children.Add(ui);
            }
            #endregion
        }

        protected void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var ui in game.Characters.SelectMany(c => c.ListControls))
            {
                GridPad.Children.Remove(ui);
                game.GridPad.Children.Add(ui);
            }
            game.Characters.SetLabelBackground(GameColor.LabelDefalutBackground);
            game.SetCurrentLabel();
            if (!game.HasAttacked) game.UpdateLabelBackground();
        }

    }
}

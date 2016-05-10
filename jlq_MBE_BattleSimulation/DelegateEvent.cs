using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JLQ_MBE_BattleSimulation
{
    //委托声明

    //窗体相关
    /// <summary>棋盘按钮单击事件的委托</summary>
    /// <param name="leftButton">左键状态</param>
    /// <param name="middleButton">中键状态</param>
    public delegate void DGridPadClick(MouseButtonState leftButton, MouseButtonState middleButton);

    //buff相关
    /// <summary>buff效果的委托</summary>
    /// <param name="buffee">buff承受者</param>
    /// <param name="buffer">buff发出者</param>
    public delegate void DBuffAffect(Character buffee, Character buffer);
    /// <summary>取消buff的委托</summary>
    /// <param name="buffee">buff承受者</param>
    /// <param name="buffer">buff发出者</param>
    public delegate void DBuffCancel(Character buffee, Character buffer);
    //符卡相关
    /// <summary>如何处理目标的委托</summary>
    /// <param name="SCee">被使用符卡者</param>
    public delegate void DHandleTarget(Character SCee);
    /// <summary>如何处理自己的委托</summary>
    public delegate void DHandleSelf();
    /// <summary>如何选择目标的委托</summary>
    /// <param name="SCee">符卡目标</param>
    /// <param name="clickPoint">单击位置</param>
    /// <returns>该目标是否是符卡的合法目标</returns>
    public delegate bool DIsTargetLegal(Character SCee, Point clickPoint);
    /// <summary>所选位置是否为合法目标的委托</summary>
    /// <param name="clickPoint">单击位置</param>
    /// <returns>单击位置是否合法</returns>
    public delegate bool DIsLegalClick(Point clickPoint);
    //伤害结算相关
    /// <summary>计算近战增益的委托</summary>
    /// <param name="target">攻击目标</param>
    /// <returns>近战增益</returns>
    public delegate float DCloseGain(Character target);
    /// <summary>判断是否命中的委托</summary>
    /// <param name="target">攻击目标</param>
    /// <returns>是否命中</returns>
    public delegate bool DIsHit(Character target);
    /// <summary>判断是否暴击的委托</summary>
    /// <param name="target">攻击目标</param>
    /// <returns>是否暴击</returns>
    public delegate bool DIsCriticalHit(Character target);

    /// <summary>攻击结算的委托</summary>
    /// <param name="target">攻击目标</param>
    /// <param name="times">伤害值增益</param>
    /// <returns>是否暴击</returns>
    public delegate bool DDoAttack(Character target, float times = 1.0f);
    /// <summary>被攻击结算的委托</summary>
    /// <param name="damage">伤害值</param>
    /// <param name="attacker">伤害来源</param>
    public delegate void DBeAttacked(int damage, Character attacker);
    //回合制相关
    /// <summary>生成可到达点</summary>
    /// <param name="origin">起点</param>
    /// <param name="step">步数</param>
    public delegate void DAssignPointCanReach(Point origin, int step);
    /// <summary>角色独立的准备阶段</summary>
    public delegate void DPreparingSection();
    /// <summary>角色独立的结束阶段</summary>
    public delegate void DEndSection();
    /// <summary>判断死亡角色的委托</summary>
    public delegate void DIsDead();
    /// <summary>修改阻挡的敌人列表</summary>
    /// <param name="enemies">原敌人列表</param>
    /// <returns>修改后的敌人列表</returns>
    public delegate IEnumerable<Point> DEnemyBlock(IEnumerable<Point> enemies);
    //buff相关
    //BuffLimit
    /// <summary>判断点是否为虚拟敌人</summary>
    /// <param name="origin">原点</param>
    /// <param name="point">待判断点</param>
    /// <returns>是否为虚拟敌人</returns>
    public delegate bool DIsPointWall(Point origin, Point point);
}

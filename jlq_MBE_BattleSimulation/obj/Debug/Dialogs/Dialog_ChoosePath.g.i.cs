﻿#pragma checksum "..\..\..\Dialogs\Dialog_ChoosePath.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "68136F426ED6085AEF29BD9B720D0879"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using JLQ_MBE_BattleSimulation;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace JLQ_MBE_BattleSimulation.Dialogs {
    
    
    /// <summary>
    /// Dialog_ChoosePath
    /// </summary>
    public partial class Dialog_ChoosePath : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonOK;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonCancel;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelPath;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonExplore;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxName;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/jlq_MBE_BattleSimulation;component/dialogs/dialog_choosepath.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
            ((JLQ_MBE_BattleSimulation.Dialogs.Dialog_ChoosePath)(target)).LostFocus += new System.Windows.RoutedEventHandler(this.Window_LostFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.buttonOK = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
            this.buttonOK.Click += new System.Windows.RoutedEventHandler(this.buttonOK_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.buttonCancel = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
            this.buttonCancel.Click += new System.Windows.RoutedEventHandler(this.buttonCancel_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.labelPath = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.buttonExplore = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
            this.buttonExplore.Click += new System.Windows.RoutedEventHandler(this.buttonExplore_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.label = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.textBoxName = ((System.Windows.Controls.TextBox)(target));
            
            #line 29 "..\..\..\Dialogs\Dialog_ChoosePath.xaml"
            this.textBoxName.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.textBoxName_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


﻿#pragma checksum "..\..\..\windows\Changelog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0763C54E53B7CC699B10030C6B9A377F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MyUserControl;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
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


namespace XviD4PSP {
    
    
    /// <summary>
    /// Changelog
    /// </summary>
    public partial class Changelog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\windows\Changelog.xaml"
        internal XviD4PSP.Changelog Window;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\windows\Changelog.xaml"
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\windows\Changelog.xaml"
        internal System.Windows.Controls.Grid grid_main;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\windows\Changelog.xaml"
        internal System.Windows.Controls.GroupBox group_muxing;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\windows\Changelog.xaml"
        internal System.Windows.Controls.TextBox changelog_text;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\windows\Changelog.xaml"
        internal System.Windows.Controls.Grid grid_buttons;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\windows\Changelog.xaml"
        internal System.Windows.Controls.Button button_ok;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/XviD4PSP;component/windows/changelog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\windows\Changelog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Window = ((XviD4PSP.Changelog)(target));
            return;
            case 2:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.grid_main = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.group_muxing = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 5:
            this.changelog_text = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.grid_buttons = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.button_ok = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\windows\Changelog.xaml"
            this.button_ok.Click += new System.Windows.RoutedEventHandler(this.button_ok_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
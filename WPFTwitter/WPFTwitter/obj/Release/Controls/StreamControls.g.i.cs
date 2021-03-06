﻿#pragma checksum "..\..\..\Controls\StreamControls.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6529CF17F5E8B1D00DFDEA7A70DB0AA3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Converters;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;


namespace WPFTwitter {
    
    
    /// <summary>
    /// StreamControls
    /// </summary>
    public partial class StreamControls : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\Controls\StreamControls.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.DockingManager dMan;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Controls\StreamControls.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox filterTextbox;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Controls\StreamControls.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button startStreamButton;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Controls\StreamControls.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restartStreamButton;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Controls\StreamControls.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button stopStreamButton;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFTwitter;component/controls/streamcontrols.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\StreamControls.xaml"
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
            this.dMan = ((Xceed.Wpf.AvalonDock.DockingManager)(target));
            return;
            case 2:
            this.filterTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.startStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\Controls\StreamControls.xaml"
            this.startStreamButton.Click += new System.Windows.RoutedEventHandler(this.startStreamButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.restartStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\Controls\StreamControls.xaml"
            this.restartStreamButton.Click += new System.Windows.RoutedEventHandler(this.restartStreamButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.stopStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\..\Controls\StreamControls.xaml"
            this.stopStreamButton.Click += new System.Windows.RoutedEventHandler(this.stopStreamButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


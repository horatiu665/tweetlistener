﻿#pragma checksum "..\..\..\Windows\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4C7CAE619002926EE946E55B644DFDC8"
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
using WPFTwitter;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Converters;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;


namespace WPFTwitter.Windows {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 37 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.DockingManager dMan;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.Layout.LayoutAnchorablePane panelLogAnchorablePane;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable panelLogAnchorable;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid logView;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox databaseConnectionComboBox;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox databaseDestinationComboBox;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox logPathTextBox;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_Log;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_database;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_logCounters;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_databaseMessages;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox filterTextbox;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button startStreamButton;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restartStreamButton;
        
        #line default
        #line hidden
        
        
        #line 119 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button stopStreamButton;
        
        #line default
        #line hidden
        
        
        #line 136 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox restFilterTextBox;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restQueryButton;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restRateLimitButton;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restAddToDatabase;
        
        #line default
        #line hidden
        
        
        #line 149 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox rest_filter_recent;
        
        #line default
        #line hidden
        
        
        #line 153 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid restView;
        
        #line default
        #line hidden
        
        
        #line 189 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Access_Token;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label longestString;
        
        #line default
        #line hidden
        
        
        #line 193 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Access_Token_Secret;
        
        #line default
        #line hidden
        
        
        #line 197 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Consumer_Key;
        
        #line default
        #line hidden
        
        
        #line 201 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Consumer_Secret;
        
        #line default
        #line hidden
        
        
        #line 203 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button setCredentialsButton;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFTwitter;component/windows/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\MainWindow.xaml"
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
            
            #line 16 "..\..\..\Windows\MainWindow.xaml"
            ((WPFTwitter.Windows.MainWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 20 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.menu_Options_OpenConsole_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 23 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.menu_View_Log_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 24 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.menu_View_LogSettings_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 25 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.menu_View_StreamingToolbox_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 26 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.menu_View_Rest_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 33 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.LogIn_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.dMan = ((Xceed.Wpf.AvalonDock.DockingManager)(target));
            
            #line 37 "..\..\..\Windows\MainWindow.xaml"
            this.dMan.Loaded += new System.Windows.RoutedEventHandler(this.dMan_Loaded);
            
            #line default
            #line hidden
            
            #line 37 "..\..\..\Windows\MainWindow.xaml"
            this.dMan.Unloaded += new System.Windows.RoutedEventHandler(this.dMan_Unloaded);
            
            #line default
            #line hidden
            return;
            case 9:
            this.panelLogAnchorablePane = ((Xceed.Wpf.AvalonDock.Layout.LayoutAnchorablePane)(target));
            return;
            case 10:
            this.panelLogAnchorable = ((Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable)(target));
            return;
            case 11:
            this.logView = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 12:
            this.databaseConnectionComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 13:
            this.databaseDestinationComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 14:
            this.logPathTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 15:
            this.checkBox_Log = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 16:
            this.checkBox_database = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 17:
            this.checkBox_logCounters = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 18:
            this.checkBox_databaseMessages = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 19:
            this.filterTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 20:
            this.startStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 117 "..\..\..\Windows\MainWindow.xaml"
            this.startStreamButton.Click += new System.Windows.RoutedEventHandler(this.startStreamButton_Click);
            
            #line default
            #line hidden
            return;
            case 21:
            this.restartStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 118 "..\..\..\Windows\MainWindow.xaml"
            this.restartStreamButton.Click += new System.Windows.RoutedEventHandler(this.restartStreamButton_Click);
            
            #line default
            #line hidden
            return;
            case 22:
            this.stopStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 119 "..\..\..\Windows\MainWindow.xaml"
            this.stopStreamButton.Click += new System.Windows.RoutedEventHandler(this.stopStreamButton_Click);
            
            #line default
            #line hidden
            return;
            case 23:
            this.restFilterTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 24:
            this.restQueryButton = ((System.Windows.Controls.Button)(target));
            
            #line 140 "..\..\..\Windows\MainWindow.xaml"
            this.restQueryButton.Click += new System.Windows.RoutedEventHandler(this.restQueryButton_Click);
            
            #line default
            #line hidden
            return;
            case 25:
            this.restRateLimitButton = ((System.Windows.Controls.Button)(target));
            
            #line 141 "..\..\..\Windows\MainWindow.xaml"
            this.restRateLimitButton.Click += new System.Windows.RoutedEventHandler(this.restRateLimitButton_Click);
            
            #line default
            #line hidden
            return;
            case 26:
            this.restAddToDatabase = ((System.Windows.Controls.Button)(target));
            
            #line 142 "..\..\..\Windows\MainWindow.xaml"
            this.restAddToDatabase.Click += new System.Windows.RoutedEventHandler(this.restAddToDatabase_Click);
            
            #line default
            #line hidden
            return;
            case 27:
            this.rest_filter_recent = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 28:
            this.restView = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 29:
            this.Access_Token = ((System.Windows.Controls.TextBox)(target));
            return;
            case 30:
            this.longestString = ((System.Windows.Controls.Label)(target));
            return;
            case 31:
            this.Access_Token_Secret = ((System.Windows.Controls.TextBox)(target));
            return;
            case 32:
            this.Consumer_Key = ((System.Windows.Controls.TextBox)(target));
            return;
            case 33:
            this.Consumer_Secret = ((System.Windows.Controls.TextBox)(target));
            return;
            case 34:
            this.setCredentialsButton = ((System.Windows.Controls.Button)(target));
            
            #line 203 "..\..\..\Windows\MainWindow.xaml"
            this.setCredentialsButton.Click += new System.Windows.RoutedEventHandler(this.setCredentialsButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


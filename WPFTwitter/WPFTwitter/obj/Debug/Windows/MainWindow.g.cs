﻿#pragma checksum "..\..\..\Windows\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "047E204CE4799940E3A8F9FBC5A34D91"
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace WPFTwitter.Windows {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 14 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.DockingManager dMan;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable logLayout;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid logView;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView tweetView;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView tweetViewGrid;
        
        #line default
        #line hidden
        
        
        #line 116 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable logSettingsLayout;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel logSettingsStackPanel;
        
        #line default
        #line hidden
        
        
        #line 120 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox databaseConnectionComboBox;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox databaseDestinationComboBox;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox databasePhpPath;
        
        #line default
        #line hidden
        
        
        #line 139 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox logPathTextBox;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_Log;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_database;
        
        #line default
        #line hidden
        
        
        #line 146 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox databaseRetries;
        
        #line default
        #line hidden
        
        
        #line 148 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_saveToTextFile;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_logCounters;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_databaseMessages;
        
        #line default
        #line hidden
        
        
        #line 152 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox checkBox_streamJsonSpammer;
        
        #line default
        #line hidden
        
        
        #line 165 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable streamingToolboxLayout;
        
        #line default
        #line hidden
        
        
        #line 167 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button startStreamButton;
        
        #line default
        #line hidden
        
        
        #line 168 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restartStreamButton;
        
        #line default
        #line hidden
        
        
        #line 169 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button stopStreamButton;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox restFilterTextBox;
        
        #line default
        #line hidden
        
        
        #line 195 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label restStartDateLabel;
        
        #line default
        #line hidden
        
        
        #line 196 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.DateTimePicker restStartDate;
        
        #line default
        #line hidden
        
        
        #line 200 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.DateTimePicker restEndDate;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restQueryButton;
        
        #line default
        #line hidden
        
        
        #line 205 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restExhaustiveQueryButton;
        
        #line default
        #line hidden
        
        
        #line 206 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restRateLimitButton;
        
        #line default
        #line hidden
        
        
        #line 213 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox rest_filter_simpleQuery;
        
        #line default
        #line hidden
        
        
        #line 214 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox rest_filter_recent;
        
        #line default
        #line hidden
        
        
        #line 219 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid restView;
        
        #line default
        #line hidden
        
        
        #line 257 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label restExpansionStatusLabel;
        
        #line default
        #line hidden
        
        
        #line 260 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button restExpansionButton;
        
        #line default
        #line hidden
        
        
        #line 264 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox restExpansionFilters;
        
        #line default
        #line hidden
        
        
        #line 268 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox restExpansionsMaxExpansionsTextbox;
        
        #line default
        #line hidden
        
        
        #line 290 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button keywordsUseAllButton;
        
        #line default
        #line hidden
        
        
        #line 291 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button keywordsUseNoneButton;
        
        #line default
        #line hidden
        
        
        #line 292 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button keywordsResetSelectionButton;
        
        #line default
        #line hidden
        
        
        #line 299 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox keywordAddTextbox;
        
        #line default
        #line hidden
        
        
        #line 302 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView keywordListView;
        
        #line default
        #line hidden
        
        
        #line 313 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView restExpansionListViewGrid;
        
        #line default
        #line hidden
        
        
        #line 351 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable credentialsPanelLayout;
        
        #line default
        #line hidden
        
        
        #line 354 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel credentialsPanel;
        
        #line default
        #line hidden
        
        
        #line 357 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Access_Token;
        
        #line default
        #line hidden
        
        
        #line 360 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label longestString;
        
        #line default
        #line hidden
        
        
        #line 361 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Access_Token_Secret;
        
        #line default
        #line hidden
        
        
        #line 365 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Consumer_Key;
        
        #line default
        #line hidden
        
        
        #line 369 "..\..\..\Windows\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Consumer_Secret;
        
        #line default
        #line hidden
        
        
        #line 371 "..\..\..\Windows\MainWindow.xaml"
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
            
            #line 7 "..\..\..\Windows\MainWindow.xaml"
            ((WPFTwitter.Windows.MainWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.dMan = ((Xceed.Wpf.AvalonDock.DockingManager)(target));
            
            #line 14 "..\..\..\Windows\MainWindow.xaml"
            this.dMan.Loaded += new System.Windows.RoutedEventHandler(this.dMan_Loaded);
            
            #line default
            #line hidden
            
            #line 14 "..\..\..\Windows\MainWindow.xaml"
            this.dMan.Unloaded += new System.Windows.RoutedEventHandler(this.dMan_Unloaded);
            
            #line default
            #line hidden
            return;
            case 3:
            this.logLayout = ((Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable)(target));
            return;
            case 4:
            
            #line 28 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.logClearButtonClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.logView = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 6:
            
            #line 61 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.tweetView_ResetSelection);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 62 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.tweetView_DeleteAll);
            
            #line default
            #line hidden
            return;
            case 8:
            this.tweetView = ((System.Windows.Controls.ListView)(target));
            return;
            case 10:
            this.tweetViewGrid = ((System.Windows.Controls.GridView)(target));
            return;
            case 12:
            this.logSettingsLayout = ((Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable)(target));
            return;
            case 13:
            this.logSettingsStackPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 14:
            this.databaseConnectionComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 15:
            this.databaseDestinationComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 16:
            this.databasePhpPath = ((System.Windows.Controls.TextBox)(target));
            
            #line 134 "..\..\..\Windows\MainWindow.xaml"
            this.databasePhpPath.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.databasePhpPath_TextChanged);
            
            #line default
            #line hidden
            return;
            case 17:
            this.logPathTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 18:
            this.checkBox_Log = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 19:
            this.checkBox_database = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 20:
            this.databaseRetries = ((System.Windows.Controls.TextBox)(target));
            return;
            case 21:
            this.checkBox_saveToTextFile = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 22:
            this.checkBox_logCounters = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 23:
            this.checkBox_databaseMessages = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 24:
            this.checkBox_streamJsonSpammer = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 25:
            this.streamingToolboxLayout = ((Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable)(target));
            return;
            case 26:
            this.startStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 167 "..\..\..\Windows\MainWindow.xaml"
            this.startStreamButton.Click += new System.Windows.RoutedEventHandler(this.startStreamButton_Click);
            
            #line default
            #line hidden
            return;
            case 27:
            this.restartStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 168 "..\..\..\Windows\MainWindow.xaml"
            this.restartStreamButton.Click += new System.Windows.RoutedEventHandler(this.restartStreamButton_Click);
            
            #line default
            #line hidden
            return;
            case 28:
            this.stopStreamButton = ((System.Windows.Controls.Button)(target));
            
            #line 169 "..\..\..\Windows\MainWindow.xaml"
            this.stopStreamButton.Click += new System.Windows.RoutedEventHandler(this.stopStreamButton_Click);
            
            #line default
            #line hidden
            return;
            case 29:
            this.restFilterTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 30:
            this.restStartDateLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 31:
            this.restStartDate = ((Xceed.Wpf.Toolkit.DateTimePicker)(target));
            return;
            case 32:
            this.restEndDate = ((Xceed.Wpf.Toolkit.DateTimePicker)(target));
            return;
            case 33:
            this.restQueryButton = ((System.Windows.Controls.Button)(target));
            
            #line 204 "..\..\..\Windows\MainWindow.xaml"
            this.restQueryButton.Click += new System.Windows.RoutedEventHandler(this.restQueryButton_Click);
            
            #line default
            #line hidden
            return;
            case 34:
            this.restExhaustiveQueryButton = ((System.Windows.Controls.Button)(target));
            
            #line 205 "..\..\..\Windows\MainWindow.xaml"
            this.restExhaustiveQueryButton.Click += new System.Windows.RoutedEventHandler(this.restExhaustiveQueryButton_Click);
            
            #line default
            #line hidden
            return;
            case 35:
            this.restRateLimitButton = ((System.Windows.Controls.Button)(target));
            
            #line 206 "..\..\..\Windows\MainWindow.xaml"
            this.restRateLimitButton.Click += new System.Windows.RoutedEventHandler(this.restRateLimitButton_Click);
            
            #line default
            #line hidden
            return;
            case 36:
            this.rest_filter_simpleQuery = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 37:
            this.rest_filter_recent = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 38:
            this.restView = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 39:
            this.restExpansionStatusLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 40:
            this.restExpansionButton = ((System.Windows.Controls.Button)(target));
            
            #line 260 "..\..\..\Windows\MainWindow.xaml"
            this.restExpansionButton.Click += new System.Windows.RoutedEventHandler(this.restExpansionButton_Click);
            
            #line default
            #line hidden
            return;
            case 41:
            this.restExpansionFilters = ((System.Windows.Controls.TextBox)(target));
            return;
            case 42:
            this.restExpansionsMaxExpansionsTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 43:
            this.keywordsUseAllButton = ((System.Windows.Controls.Button)(target));
            
            #line 290 "..\..\..\Windows\MainWindow.xaml"
            this.keywordsUseAllButton.Click += new System.Windows.RoutedEventHandler(this.keywordsView_UseAll);
            
            #line default
            #line hidden
            return;
            case 44:
            this.keywordsUseNoneButton = ((System.Windows.Controls.Button)(target));
            
            #line 291 "..\..\..\Windows\MainWindow.xaml"
            this.keywordsUseNoneButton.Click += new System.Windows.RoutedEventHandler(this.keywordsView_UseNone);
            
            #line default
            #line hidden
            return;
            case 45:
            this.keywordsResetSelectionButton = ((System.Windows.Controls.Button)(target));
            
            #line 292 "..\..\..\Windows\MainWindow.xaml"
            this.keywordsResetSelectionButton.Click += new System.Windows.RoutedEventHandler(this.keywordsView_ResetSelection);
            
            #line default
            #line hidden
            return;
            case 46:
            
            #line 293 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.keywordsView_DeleteAll);
            
            #line default
            #line hidden
            return;
            case 47:
            
            #line 296 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.keywordAddButtonClick);
            
            #line default
            #line hidden
            return;
            case 48:
            this.keywordAddTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 49:
            this.keywordListView = ((System.Windows.Controls.ListView)(target));
            return;
            case 52:
            this.restExpansionListViewGrid = ((System.Windows.Controls.GridView)(target));
            return;
            case 54:
            this.credentialsPanelLayout = ((Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable)(target));
            return;
            case 55:
            this.credentialsPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 56:
            this.Access_Token = ((System.Windows.Controls.TextBox)(target));
            return;
            case 57:
            this.longestString = ((System.Windows.Controls.Label)(target));
            return;
            case 58:
            this.Access_Token_Secret = ((System.Windows.Controls.TextBox)(target));
            return;
            case 59:
            this.Consumer_Key = ((System.Windows.Controls.TextBox)(target));
            return;
            case 60:
            this.Consumer_Secret = ((System.Windows.Controls.TextBox)(target));
            return;
            case 61:
            this.setCredentialsButton = ((System.Windows.Controls.Button)(target));
            
            #line 371 "..\..\..\Windows\MainWindow.xaml"
            this.setCredentialsButton.Click += new System.Windows.RoutedEventHandler(this.setCredentialsButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            System.Windows.EventSetter eventSetter;
            switch (connectionId)
            {
            case 9:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.MouseUpEvent;
            
            #line 67 "..\..\..\Windows\MainWindow.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.tweetView_Headers_MouseUp);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 11:
            
            #line 94 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.tweetView_Item_DeleteButton);
            
            #line default
            #line hidden
            break;
            case 50:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.MouseUpEvent;
            
            #line 305 "..\..\..\Windows\MainWindow.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.keywordListView_Headers_MouseUp);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 51:
            eventSetter = new System.Windows.EventSetter();
            eventSetter.Event = System.Windows.UIElement.MouseUpEvent;
            
            #line 308 "..\..\..\Windows\MainWindow.xaml"
            eventSetter.Handler = new System.Windows.Input.MouseButtonEventHandler(this.keywordListView_Item_MouseUp);
            
            #line default
            #line hidden
            ((System.Windows.Style)(target)).Setters.Add(eventSetter);
            break;
            case 53:
            
            #line 334 "..\..\..\Windows\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.keywordView_Item_DeleteButton);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}


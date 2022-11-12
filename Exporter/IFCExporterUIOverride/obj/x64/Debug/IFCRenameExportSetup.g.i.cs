﻿#pragma checksum "..\..\..\IFCRenameExportSetup.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "34ABB6697FF7719187CEAEB75F86EC6F849A943AE95274A53304D6F210D33947"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Autodesk.UI.Windows;
using BIM.IFC.Export.UI.Properties;
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
using System.Windows.Shell;


namespace BIM.IFC.Export.UI {
    
    
    /// <summary>
    /// RenameExportSetupWindow
    /// </summary>
    public partial class RenameExportSetupWindow : Autodesk.UI.Windows.ChildWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\IFCRenameExportSetup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal BIM.IFC.Export.UI.RenameExportSetupWindow Rename;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\..\IFCRenameExportSetup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonCancel;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\IFCRenameExportSetup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonOK;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\IFCRenameExportSetup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxOriginalName;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\IFCRenameExportSetup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelOriginalName;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\IFCRenameExportSetup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBoxNewName;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\IFCRenameExportSetup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelNewName;
        
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
            System.Uri resourceLocater = new System.Uri("/LoggerIFCExportUIOverride;component/ifcrenameexportsetup.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\IFCRenameExportSetup.xaml"
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
            this.Rename = ((BIM.IFC.Export.UI.RenameExportSetupWindow)(target));
            return;
            case 2:
            this.buttonCancel = ((System.Windows.Controls.Button)(target));
            
            #line 8 "..\..\..\IFCRenameExportSetup.xaml"
            this.buttonCancel.Click += new System.Windows.RoutedEventHandler(this.buttonCancel_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.buttonOK = ((System.Windows.Controls.Button)(target));
            
            #line 9 "..\..\..\IFCRenameExportSetup.xaml"
            this.buttonOK.Click += new System.Windows.RoutedEventHandler(this.buttonOK_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.textBoxOriginalName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.labelOriginalName = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.textBoxNewName = ((System.Windows.Controls.TextBox)(target));
            
            #line 12 "..\..\..\IFCRenameExportSetup.xaml"
            this.textBoxNewName.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.textBoxNewName_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.labelNewName = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

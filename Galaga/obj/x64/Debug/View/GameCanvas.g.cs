﻿#pragma checksum "C:\Users\kg00281\Desktop\TeamGalaga\TeamCGalaga\Galaga\View\GameCanvas.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "F9E50177C4B08D82E750B7BDD2F1D9631A179AA48F9FEF03449DA417DC767C9E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Galaga.View
{
    partial class GameCanvas : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // View\GameCanvas.xaml line 12
                {
                    this.canvas = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                }
                break;
            case 3: // View\GameCanvas.xaml line 14
                {
                    this.livesTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4: // View\GameCanvas.xaml line 15
                {
                    this.scoreTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 5: // View\GameCanvas.xaml line 16
                {
                    this.levelTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.19041.685")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}


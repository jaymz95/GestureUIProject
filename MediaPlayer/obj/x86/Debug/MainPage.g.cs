﻿#pragma checksum "C:\Users\james\source\repos\GestureUIProject\MediaPlayer\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "19750BFDA9939FD42700A4E2B55F9AAF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MediaPlayer
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainPage.xaml line 18
                {
                    this.cePreview = (global::Windows.UI.Xaml.Controls.CaptureElement)(target);
                }
                break;
            case 3: // MainPage.xaml line 19
                {
                    this.cvsFaceOverlay = (global::Windows.UI.Xaml.Controls.Canvas)(target);
                }
                break;
            case 4: // MainPage.xaml line 26
                {
                    this.mediaSimple = (global::Windows.UI.Xaml.Controls.MediaPlayerElement)(target);
                }
                break;
            case 5: // MainPage.xaml line 30
                {
                    this.mediaPlayer = (global::Windows.UI.Xaml.Controls.MediaPlayerElement)(target);
                }
                break;
            case 6: // MainPage.xaml line 36
                {
                    global::Windows.UI.Xaml.Controls.Button element6 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element6).Click += this.Button_Click;
                }
                break;
            case 7: // MainPage.xaml line 21
                {
                    this.btnCamera = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnCamera).Click += this.btnCamera_Click;
                }
                break;
            case 8: // MainPage.xaml line 22
                {
                    this.btnDetectFaces = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnDetectFaces).Click += this.btnDetectFaces_Click;
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

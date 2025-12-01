using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Windows;
 
namespace KioskApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
        }
 
        private async void InitializeWebView()
        {
            string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SurveyApp", "index.html");
 
            var env = await CoreWebView2Environment.CreateAsync();
            await webView.EnsureCoreWebView2Async(env);
 
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
 
            webView.Source = new Uri(localPath);
        }
    }
}
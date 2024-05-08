using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Dokumentaci k šabloně položky Dialog obsahu najdete na adrese https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pong.UWP
{
    public sealed partial class DesktopEditionPrompt : ContentDialog
    {
        private ApplicationDataContainer _localSettings;

        public DesktopEditionPrompt(ApplicationDataContainer localSettings)
        {
            _localSettings = localSettings;
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        { }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _localSettings.Values["showEditionPrompt"] = false;
        }
    }
}

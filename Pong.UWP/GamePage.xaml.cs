using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Pong.UWP
{
    public sealed partial class GamePage : Page
    {
        Game.Game1 _game;
        readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public GamePage()
        {
            this.InitializeComponent();

            // Disable Xbox overscan (https://learn.microsoft.com/en-us/windows/uwp/xbox-apps/turn-off-overscan)
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bool showEditionPrompt = _localSettings.Values["showEditionPrompt"] as bool? ?? true;

            // Check if user is on Windows, then prompt install of Desktop edition
            if (showEditionPrompt && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop"
                && Package.Current.Id.Architecture != Windows.System.ProcessorArchitecture.Arm)
            {
                DesktopEditionPrompt prompt = new DesktopEditionPrompt(_localSettings);
                await prompt.ShowAsync();
            }

            // Create the game.
            var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game.Game1>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
        }
    }
}

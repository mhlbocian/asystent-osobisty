using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{

    public sealed partial class Rozklad : Page
    {
        public static string PageTitle = "Rozkład jazdy";
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public Rozklad()
        {
            this.InitializeComponent();
            this.initTimetable();
        }

        private async void initTimetable()
        {
            try
            {
                string adress = localSettings.Values["timetableSite"].ToString();
                webviewTimetable.Navigate(new Uri(adress));
            }
            catch
            {
                var dialog = new MessageDialog("Przejdż do ustawień, aby wybrać adres rozkładu jazdy.");
                await dialog.ShowAsync();
                webviewProgress.IsActive = false;
            }
        }

        private void webviewTimetable_Loading(FrameworkElement sender, object args)
        {
            webviewProgress.IsActive = true;
        }

        private void webviewTimetable_LoadCompleted(object sender, NavigationEventArgs e)
        {
            webviewProgress.IsActive = false;
        }
    }
}

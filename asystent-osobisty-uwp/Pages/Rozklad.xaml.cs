using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        public static string PageTitle = "Podglad strony";

        private static SQLiteConnection DbConnection
        {
            get
            {
                return new SQLiteConnection(
                    new SQLitePlatformWinRT(),
                    Path.Combine(ApplicationData.Current.LocalFolder.Path,
                    "db.sqlite"));
            }
        }

        public Rozklad()
        {
            this.InitializeComponent();
        }

        private async void initTimetable(int pageId)
        {
            try {
                Sites s = (from p in DbConnection.Table<Sites>()
                           where p.Id == pageId
                           select p).FirstOrDefault();
                webviewTimetable.Navigate(new Uri(s.Url));
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog("Coś poszło nie tak. Sprawdź" +
                    " ustawienia aplikacji");
                await dialog.ShowAsync();
                webviewProgress.IsActive = false;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int id = Convert.ToInt32(e.Parameter.ToString());
            this.initTimetable(id);
            base.OnNavigatedTo(e);
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

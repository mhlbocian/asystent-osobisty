using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Email;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System.IO;
using Windows.Storage;
using SQLite.Net.Attributes;
using System.Linq;

namespace App1
{
    public sealed partial class Ustawienia : Page
    {

        public static string PageTitle = "Ustawienia";
        private Dictionary<TextBox, TextBox> sitesDict =
            new Dictionary<TextBox, TextBox>();

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

        ApplicationDataContainer localSettings =
            ApplicationData.Current.LocalSettings;

        public Ustawienia()
        {
            this.InitializeComponent();

            try
            {
                DbConnection.CreateTable<Sites>();
                this.RefreshSitesList(stackOfPages);
            }
            catch (SQLiteException e)
            {
                var dialog = new MessageDialog("Błąd bazy danych");
                dialog.ShowAsync();
            }
        }

        private void RefreshSitesList(StackPanel stackOfPages)
        {
            sitesDict.Clear();
            stackOfPages.Children.Clear();
            stackOfPages.Children.Add((new TextBlock()));

            List<Sites> sites = (from p in DbConnection.Table<Sites>()
                                 select p).ToList();
            foreach (Sites s in sites)
            {
                this.AddSiteToList(s, stackOfPages);
            }
        }

        private void AddSiteToList(Sites s,
            StackPanel listName)
        {

            Button hpDel = new Button
            {
                Content = "X",
                Name = "shb_" + s.Id,
            };
            hpDel.Click += deleteSite;

            TextBox tbt = new TextBox
            {
                Text = s.Name,
                Name = "stt_" + s.Id,
                BorderThickness = new Thickness(0),
                MaxLength = 15,
            };
            tbt.TextChanging += SettingChanged;

            TextBox tb = new TextBox
            {
                Name = "stb_" + s.Id,
                Text = s.Url,
            };
            tb.TextChanging += SettingChanged;

            sitesDict.Add(tbt, tb);

            Grid grd = new Grid();
            grd.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            grd.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Auto)
            });

            grd.Children.Add(tb);
            Grid.SetColumn(tb, 0);
            grd.Children.Add(hpDel);
            Grid.SetColumn(hpDel, 1);

            listName.Children.Add(tbt);
            listName.Children.Add(grd);
            listName.Children.Add(new TextBlock { });
        }

        private void deleteSite(object sender, RoutedEventArgs e)
        {
            var btn = (sender as Button);
            int id = Convert.ToInt32(btn.Name.Replace("shb_", ""));

            Sites s = (from p in DbConnection.Table<Sites>()
                       where p.Id == id
                       select p).FirstOrDefault();
            DbConnection.Delete(s);
            this.RefreshSitesList(stackOfPages);
        }

        private async void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Uri uriResult;
            bool success = true;
            foreach (KeyValuePair<TextBox, TextBox> t in sitesDict)
            {
                int id = Convert.ToInt32(t.Key.Name.Replace("stt_", ""));

                if (t.Key.Text == string.Empty)
                    t.Key.Text = "Bez nazwy";

                if (Uri.TryCreate(t.Value.Text, UriKind.Absolute, out uriResult))
                {
                    Sites s = new Sites();
                    s.Id = id;
                    s.Name = t.Key.Text;
                    s.Url = t.Value.Text;
                    DbConnection.Update(s);
                }
                else
                {
                    success = false;
                    string msg = "Adres " + t.Value.Text + " jest błędny!";
                    var dialog = new MessageDialog(msg);
                    await dialog.ShowAsync();
                }
            }

            if (success)
            {
                var dialog = new MessageDialog("Ustawienia zapisane!");
                await dialog.ShowAsync();
                btnSaveSettings.Visibility = Visibility.Collapsed;
            }
            else
            {
                var dialog = new MessageDialog("Proszę poprawić dane!");
                await dialog.ShowAsync();
            }

        }

        private void webLicense_LoadCompleted(object sender, NavigationEventArgs e)
        {
            pbarLoadingLicense.Visibility = Visibility.Collapsed;
        }

        private async void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("bocian.michal@outlook.com"));
            string messageBody = "";
            emailMessage.Body = messageBody;
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private void PivotItem_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            btnSaveSettings.Visibility = Visibility.Visible;
        }

        private void SettingChanged(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            btnSaveSettings.Visibility = Visibility.Visible;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string msg = string.Empty;
            try
            {
                Sites s = new Sites();
                s.Name = "Bez nazwy";
                s.Url = "http://zmien.to";
                DbConnection.Insert(s);

                msg = "Strona dodana!";
                this.RefreshSitesList(stackOfPages);
            }
            catch (SQLiteException ex)
            {
                msg = "Błąd bazy danych";
            }
            finally
            {
                await (new MessageDialog(msg)).ShowAsync();
            }

        }

        private void webLicense_Loaded(object sender, RoutedEventArgs e)
        {
            pbarLoadingLicense.Visibility = Visibility.Collapsed;
        }
    }
}

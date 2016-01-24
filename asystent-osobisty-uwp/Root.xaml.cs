using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace App1
{
    public sealed partial class Root : Page
    {

        Frame frmRootFrame;
        Collection<string> titlesStack = new Collection<string>();

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

        private void RootNavigate<NavPage>(object navargs = null)
        {
            titlesStack.Add(tbxTitlebarPageName.Text);
            frmRootFrame.Navigate(typeof(NavPage), navargs);
            RefreshPagesList();

            string pageTitle = string.Empty;
            try
            {
                FieldInfo f = typeof(NavPage).GetField("PageTitle");
                pageTitle = (string)f.GetValue(null);
            }
            finally
            {
                tbxTitlebarPageName.Text = pageTitle;
            }
            slvRootPanes.IsPaneOpen = false;

            listHamburger.SelectedIndex = -1;
            listHome.SelectedIndex = -1;
            listSites.SelectedIndex = -1;
            listSettings.SelectedIndex = -1;
        }

        public void SetBackPageTitle()
        {
            tbxTitlebarPageName.Text = titlesStack.Last();
            titlesStack.RemoveAt(titlesStack.Count - 1);
        }

        public Root(Frame frame)
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            this.frmRootFrame = frame;
            this.grdContent.Children.Add(this.frmRootFrame);
            Grid.SetRow(this.frmRootFrame, 1);
            this.RootNavigate<Home>();
            this.RefreshPagesList();
        }

        /* HAMBURGER */
        private void ToggleHamburgerMenu(object sender, TappedRoutedEventArgs e)
        {
            slvRootPanes.IsPaneOpen = !slvRootPanes.IsPaneOpen;
        }

        private void RefreshPagesList()
        {
            listSites.Items.Clear();
            List<Sites> sites = (from p in DbConnection.Table<Sites>()
                                 select p).ToList();
            foreach (Sites s in sites)
            {
                ListViewItem item = new ListViewItem
                {
                    Padding = new Thickness(0),
                    Name = "site_" + s.Id,
                };
                item.Tapped += listSites_Tapped;

                StackPanel stack = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,

                };

                TextBlock ticon = new TextBlock
                {
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 50,
                    FontSize = 18,
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Text = "\U0000E718",
                };

                TextBlock tname = new TextBlock
                {
                    Padding = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = s.Name,
                };

                stack.Children.Add(ticon);
                stack.Children.Add(tname);
                item.Content = stack;

                listSites.Items.Add(item);
            }
        }

        private void listMenuHome_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RootNavigate<Home>();
        }

        private void listSites_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int id = Convert.ToInt32(
                (sender as ListViewItem).Name.Replace("site_", ""));
            RootNavigate<Rozklad>(id);
        }

        /* USTAWIENIA */
        private void listSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RootNavigate<Ustawienia>();
        }
    }
}

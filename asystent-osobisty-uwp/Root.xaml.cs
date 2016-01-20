using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        private void RootNavigate<NavPage>()
        {
            titlesStack.Add(tbxTitlebarPageName.Text);

            frmRootFrame.Navigate(typeof(NavPage));

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

            listHamburger.SelectedIndex = -1;
            listMenu.SelectedIndex = -1;
            listSettings.SelectedIndex = -1;
            slvRootPanes.IsPaneOpen = false;
        }

        public void SetBackPageTitle()
        {
            tbxTitlebarPageName.Text = titlesStack.Last();
            titlesStack.RemoveAt(titlesStack.Count - 1);
        }

        public Root(Frame frame)
        {
            this.InitializeComponent();
            this.frmRootFrame = frame;
            this.grdContent.Children.Add(this.frmRootFrame);
            Grid.SetRow(this.frmRootFrame, 1);
            this.RootNavigate<Home>();
        }

        /* HAMBURGER */
        private void ToggleHamburgerMenu(object sender, TappedRoutedEventArgs e)
        {
            slvRootPanes.IsPaneOpen = !slvRootPanes.IsPaneOpen;
        }

        private void listMenuHome_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RootNavigate<Home>();
        }

        private void listMenuTimetable_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RootNavigate<Rozklad>();
        }

        /* USTAWIENIA */
        private void listSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RootNavigate<Ustawienia>();
        }
    }
}

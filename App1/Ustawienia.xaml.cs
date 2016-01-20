using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
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

namespace App1
{
    public sealed partial class Ustawienia : Page
    {

        public static string PageTitle = "Ustawienia";

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public Ustawienia()
        {
            this.InitializeComponent();

            if (localSettings.Values["timetableSite"] == null)
                inpTimetableAdress.Text = "";
            else
                inpTimetableAdress.Text = localSettings.Values["timetableSite"].ToString();
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

        private void inpTimetableAdress_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            btnSaveSettings.Visibility = Visibility.Visible;
        }

        private async void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(inpTimetableAdress.Text, UriKind.Absolute, out uriResult);
            if(result)
            {
                localSettings.Values["timetableSite"] = inpTimetableAdress.Text;
                var dialog = new MessageDialog("Ustawienia zostały pomyślnie zapisane!");
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new MessageDialog("Wprowadzono nieprawidłowy adres URL!");
                await dialog.ShowAsync();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Strona dodana!");
            await dialog.ShowAsync();
        }

        private void webLicense_Loaded(object sender, RoutedEventArgs e)
        {
            pbarLoadingLicense.Visibility = Visibility.Collapsed;
        }
    }
}

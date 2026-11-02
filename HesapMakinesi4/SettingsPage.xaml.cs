using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace HesapMakinesi4
{
    public partial class SettingsPage : ContentPage
    {
        //orijinal boyutu ayarladık
        double suankiBoyut = 1.0;

        public SettingsPage()
        {
            InitializeComponent();
            //kullanıcı dah önce bir tema sectiyse o devam eder ancak ilk defa giriş yapıyorsa sistem teması secilir
            int kayitliTema = Preferences.Default.Get("AppTheme", 2);
            ThemePicker.SelectedIndex = kayitliTema;

            //aynı sekilde secilen boyut varsa o devam eder
            suankiBoyut = Preferences.Default.Get("AppScale", 1.0);
            boyutYazisiniGuncelle();
        }

        private void temaDegisimYapildi(object sender, EventArgs e)
        {
            int secilen = ThemePicker.SelectedIndex;
            Preferences.Default.Set("AppTheme", secilen);

            //seçilen sıraya göre temanın rengi değiştiriliyor
            switch (secilen)
            {
                case 0: Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Light; break;
                case 1: Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Dark; break;
                case 2: Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Unspecified; break;
            }
        }

        private void boyutKucultTiklandi(object sender, EventArgs e)
        {
            //ekran cok küçültülüp kötü görğntü oluşturulamaz alt sınır belirledik
            if (suankiBoyut > 0.90) // Alt sınır
            {
                suankiBoyut -= 0.05;
                boyutuGuncelle();
            }
        }

        private void boyutBuyutTiklandi(object sender, EventArgs e)
        {
            //burada da büyütme işlemi yapılıyor ama butonlar ekran boyutunu geçmeyecek şekilde ayarlanıyor
            if (suankiBoyut < 1.05) // Üst sınır (Ekrandan büyük olmasını engeller)
            {
                suankiBoyut += 0.05;
                boyutuGuncelle();
            }
        }

        private void boyutuGuncelle()
        {
            //değişen yeni boyutu hafızaya kaydediyor
            Preferences.Default.Set("AppScale", suankiBoyut);
            boyutYazisiniGuncelle();
            this.Scale = suankiBoyut;
        }

        private void boyutYazisiniGuncelle()
        {
            SizeLabel.Text = $"{Math.Round(suankiBoyut, 2):0.00}x";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Scale = Preferences.Default.Get("AppScale", 1.0);
        }
    }
}
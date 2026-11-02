using System;
using System.Data;
using Microsoft.Maui.Controls;

namespace HesapMakinesi4
{
    public partial class StandardPage : ContentPage
    {

        //hafızada değişkenleri tutuyoruz
        string islemDizesi = "";
        string ekranGecmisi = "";
        string suankiDeger = "";
        bool yeniGirisMi = true;//rakamları yan yana mı yazalım yoksa ekranı temizleyelim mi sorusu
        //virgüllü sayılarda ondalık ayracı nedeniyle TR formatına çevirdik
        System.Globalization.CultureInfo trCulture = new System.Globalization.CultureInfo("tr-TR");

        public StandardPage()
        {
            InitializeComponent();
            DisplayLabel.Text = "0";//uygulama ilk açıldığında ekran boş kalmasın diye 0 atadık
        }

        private void sayiButonunaBasildi(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (yeniGirisMi || DisplayLabel.Text == "0" || DisplayLabel.Text == "Tanımsız" || DisplayLabel.Text == "Sıfıra bölünemez")
            {
                DisplayLabel.Text = button.Text;
                yeniGirisMi = false;
            }
            else
            {
                DisplayLabel.Text += button.Text;
            }
            //noktalar binlik ayracı olma nedeniyle sorun yaşatmasın diye değişim yapıyoruz
            suankiDeger = DisplayLabel.Text.Replace(".", "");
        }

        private void islemButonunaBasildi(object sender, EventArgs e)
        {
            if (DisplayLabel.Text == "Tanımsız" || DisplayLabel.Text == "Sıfıra bölünemez") return;

            Button button = (Button)sender;
            string displayOp = button.Text;
            string calcOp = displayOp;

            //DataTable anlamadığı için arka planda bunu değiştiriyoruz
            if (displayOp == "÷") calcOp = "/";
            if (displayOp == "×") calcOp = "*";
            if (displayOp == "−") calcOp = "-";

            if (yeniGirisMi && islemDizesi.Length > 0 && string.IsNullOrEmpty(suankiDeger))
            {
                islemDizesi = islemDizesi.Substring(0, islemDizesi.Length - 2) + calcOp + " ";
                ekranGecmisi = ekranGecmisi.Substring(0, ekranGecmisi.Length - 2) + displayOp + " ";
                HistoryLabel.Text = ekranGecmisi;
                return;
            }

            if (string.IsNullOrEmpty(suankiDeger)) suankiDeger = DisplayLabel.Text.Replace(".", "");

            //hata çözümü, Virgüllü sayıları CultureInfo ile güvenli hsle getiriyoruz
            double.TryParse(DisplayLabel.Text.Replace(".", ""), System.Globalization.NumberStyles.Any, trCulture, out double val);
            string valStr = val.ToString(System.Globalization.CultureInfo.InvariantCulture);

            islemDizesi += valStr + " " + calcOp + " ";
            ekranGecmisi += suankiDeger + " " + displayOp + " ";
            HistoryLabel.Text = ekranGecmisi;//üstteki küçük alana yapılan işlemin adımlarını yazıyoruz

            yeniGirisMi = true;
            suankiDeger = "";
        }

        private void sonucuHesapla(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(islemDizesi)) return;//hesaplanacak birşey yoksa returnluyorz

            if (string.IsNullOrEmpty(suankiDeger)) suankiDeger = DisplayLabel.Text.Replace(".", "");

            //hata çözümü, Virgüllü sayıları CultureInfo ile güvenli hsle getiriyoruz
            double.TryParse(DisplayLabel.Text.Replace(".", ""), System.Globalization.NumberStyles.Any, trCulture, out double val);
            string valStr = val.ToString(System.Globalization.CultureInfo.InvariantCulture);

            islemDizesi += valStr;
            ekranGecmisi += suankiDeger + " =";
            HistoryLabel.Text = ekranGecmisi;

            try
            {
                //işlemleri çözme işini DataTableye gönderiyoruz
                var dataTable = new DataTable();
                var result = Convert.ToDouble(dataTable.Compute(islemDizesi, null));

                if (double.IsInfinity(result) || double.IsNaN(result))
                    DisplayLabel.Text = "Sıfıra bölünemez";//sıfıra bölünemediğini ekrana yazdırıyoruz
                else
                    DisplayLabel.Text = result.ToString("#,0.##########", trCulture);
            }
            catch
            {
                DisplayLabel.Text = "Tanımsız";
            }

            //işlem bitti sonraki işlemler için ekranı temizliyoruz
            islemDizesi = "";
            ekranGecmisi = "";
            suankiDeger = "";
            yeniGirisMi = true;
        }

        private void temizleButonuSecildi(object sender, EventArgs e)
        {
            //CE sadece anlık yazılan sayıyı silerken C butonu bütün geçmişi siliyor
            Button button = (Button)sender;
            if (button.Text == "CE")
            {
                DisplayLabel.Text = "0";
                suankiDeger = "0";
                yeniGirisMi = true;
            }
            else
            {
                DisplayLabel.Text = "0";
                HistoryLabel.Text = "";
                islemDizesi = "";
                ekranGecmisi = "";
                suankiDeger = "";
                yeniGirisMi = true;
            }
        }

        private void silmeTusunaBasildi(object sender, EventArgs e)
        {
            if (yeniGirisMi || DisplayLabel.Text == "Tanımsız" || DisplayLabel.Text == "Sıfıra bölünemez") return;

            //ekrandaki sayının son karakterini siliyoruz
            DisplayLabel.Text = DisplayLabel.Text.Substring(0, DisplayLabel.Text.Length - 1);
            if (DisplayLabel.Text == "" || DisplayLabel.Text == "-") DisplayLabel.Text = "0";//ekranda hicbi sey kalmadıysa 0 yazıyoruz
            suankiDeger = DisplayLabel.Text.Replace(".", "");
        }

        private void yuzdeHesapla(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(islemDizesi))
            {
                DisplayLabel.Text = "0";
                suankiDeger = "0";
                yeniGirisMi = true;
                return;
            }

            //yüzde hesaplarken o ana kadar girilmiş işlemin sonucunu bulup ona göre yüzde aldırıyor
            try
            {
                var dataTable = new DataTable();
                string tempExpr = islemDizesi.Trim();
                if (tempExpr.EndsWith("+") || tempExpr.EndsWith("-") || tempExpr.EndsWith("*") || tempExpr.EndsWith("/"))
                    tempExpr = tempExpr.Substring(0, tempExpr.Length - 1).Trim();

                double baseValue = Convert.ToDouble(dataTable.Compute(tempExpr, null));

                
                double.TryParse(DisplayLabel.Text.Replace(".", ""), System.Globalization.NumberStyles.Any, trCulture, out double currentVal);

                double percentValue = baseValue * (currentVal / 100.0);
                DisplayLabel.Text = percentValue.ToString("#,0.##########", trCulture);
                suankiDeger = percentValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                yeniGirisMi = true;
            }
            catch { }
        }

        private void karekokKareAlTiklandi(object sender, EventArgs e)
        {
            if (DisplayLabel.Text == "Tanımsız" || DisplayLabel.Text == "Sıfıra bölünemez") return;
            Button button = (Button)sender;

            // MANTIKSAL HATA ÇÖZÜMÜ: Virgüllü sayıları CultureInfo ile güvenli Parse etmek
            double.TryParse(DisplayLabel.Text.Replace(".", ""), System.Globalization.NumberStyles.Any, trCulture, out double val);
            double result = 0;

            if (string.IsNullOrEmpty(suankiDeger)) suankiDeger = DisplayLabel.Text.Replace(".", "");

            //tek butona basıldığında anında hesaplancak işlemler bloğu
            switch (button.Text)
            {
                case "x²":
                    result = Math.Pow(val, 2);
                    suankiDeger = $"sqr({suankiDeger})";
                    break;
                case "²√x":
                    if (val < 0) { DisplayLabel.Text = "Tanımsız"; return; }//negatif olanların karakökü alınmaz
                    result = Math.Sqrt(val);
                    suankiDeger = $"√({suankiDeger})";
                    break;
                case "1/x":
                    if (val == 0) { DisplayLabel.Text = "Sıfıra bölünemez"; return; }
                    result = 1 / val;
                    suankiDeger = $"1/({suankiDeger})";
                    break;
            }

            DisplayLabel.Text = result.ToString("#,0.##########", trCulture);
            yeniGirisMi = true;
        }

        private void isaretDegistir(object sender, EventArgs e)
        {
            if (DisplayLabel.Text == "0" || DisplayLabel.Text == "Tanımsız") return;

            //sayının başında eksi ve artı olmasına göre negatif ya da pozitif hale çeviriyor
            if (DisplayLabel.Text.StartsWith("-"))
                DisplayLabel.Text = DisplayLabel.Text.Substring(1);
            else
                DisplayLabel.Text = "-" + DisplayLabel.Text;

            if (suankiDeger.StartsWith("negate("))
                suankiDeger = suankiDeger.Substring(7, suankiDeger.Length - 8);
            else
                suankiDeger = $"negate({suankiDeger})";
        }

        private void virgulEkle(object sender, EventArgs e)
        {
            //kullanıcı peş peşe virgüller ekleyemesin diye kontrol var
            if (yeniGirisMi)
            {
                DisplayLabel.Text = "0,";
                yeniGirisMi = false;
            }
            else if (!DisplayLabel.Text.Contains(","))
            {
                DisplayLabel.Text += ",";
            }
            suankiDeger = DisplayLabel.Text.Replace(".", "");
        }

        protected override void OnAppearing()
        {
            //boyutlandırma ayarı yapılıyor
            base.OnAppearing();
            this.Scale = Microsoft.Maui.Storage.Preferences.Default.Get("AppScale", 1.0);
        }
    }
}
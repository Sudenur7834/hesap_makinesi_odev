using System;
using System.Data;
using Microsoft.Maui.Controls;

namespace HesapMakinesi4
{
    public partial class ScientificPage : ContentPage
    {
        //burada hafıza da değişkenleri tutuyoruz
        string islemDizesi = "";
        string ekranGecmisi = "";
        string suankiDeger = "";
        bool yeniGirisMi = true;
        bool ikinciModAktif = false;//normal menüde ikinci mod aktif
        bool trigonometriTersi = false; // Trigonometri menusunde ikinci fonksiyonun aktif olması
        
        //burada TR formatını tanımladık
        System.Globalization.CultureInfo trCulture = new System.Globalization.CultureInfo("tr-TR");

        public ScientificPage()
        {
            InitializeComponent();
            DisplayLabel.Text = "0";//ekran ilk açıldığında ekranda 0 yazar
        }

        private void menuleriKapat()
        {
            if (TrigMenuBorder.IsVisible) TrigMenuBorder.IsVisible = false;
            if (FuncMenuBorder.IsVisible) FuncMenuBorder.IsVisible = false;
        }

        private void trigonometriMenuTiklandi(object sender, EventArgs e)
        {
            TrigMenuBorder.IsVisible = !TrigMenuBorder.IsVisible;
            FuncMenuBorder.IsVisible = false;//trigonometri ve işlev butonları ikisi aynı anda çık kalmıyor biri açıldığında diğeri kapanuyr şeklinde ayarlama yaptık
        }

        private void islevMenuTiklandi(object sender, EventArgs e)
        {
            FuncMenuBorder.IsVisible = !FuncMenuBorder.IsVisible;
            TrigMenuBorder.IsVisible = false;//trigonometri menusunde yaptığımız gibi burada da buneklemeyi yapıyoruz
        }

        private void sayiButonunaBasildi(object sender, EventArgs e)
        {
            menuleriKapat();
            Button button = (Button)sender;

            //eğer yeni giriş varsa diğeri silinip ekrana bu yeni veri giriş yapıyor
            if (yeniGirisMi || DisplayLabel.Text == "0" || DisplayLabel.Text == "Tanımsız" || DisplayLabel.Text == "Sıfıra bölünemez")
            {
                DisplayLabel.Text = button.Text;
                yeniGirisMi = false;
            }
            else
            {
                //yeni giriş yoksa sayının yanına yeni eklemeler yapıyoruz
                DisplayLabel.Text += button.Text;
            }

            suankiDeger = DisplayLabel.Text.Replace(".", "");
        }

        private void sabitSayiTiklandi(object sender, EventArgs e)
        {
            menuleriKapat();
            Button button = (Button)sender;
            double val = button.Text == "π" ? Math.PI : Math.E;
            DisplayLabel.Text = val.ToString("#,0.##########", trCulture);//virgülden sonra en fazla 10 basamak göstermesi için
            //trCulture ile ondalik sayıyı virgül ile gösteriyoruz
            suankiDeger = val.ToString(System.Globalization.CultureInfo.InvariantCulture);
            //bu girisler zaten sabit sayı oldugı iç,n yanına sayı ekleme ihticaı duymuyoruz
            yeniGirisMi = false;
        }

        private void parantezTiklandi(object sender, EventArgs e)
        {
            //parantez butonuna tıklandı ve ekranda bu gözüküyor
            menuleriKapat();
            Button button = (Button)sender;
            if (button.Text == ")" && !yeniGirisMi)
            {
                double.TryParse(DisplayLabel.Text.Replace(".", ""), System.Globalization.NumberStyles.Any, trCulture, out double val);
                string valStr = val.ToString(System.Globalization.CultureInfo.InvariantCulture);

                islemDizesi += valStr + " ";
                ekranGecmisi += suankiDeger + " ";

                yeniGirisMi = true;
                suankiDeger = "";
            }
            islemDizesi += button.Text + " ";
            ekranGecmisi += button.Text + " ";
            HistoryLabel.Text = ekranGecmisi;
        }

        private void islemButonunaBasildi(object sender, EventArgs e)
        {
            menuleriKapat();
            if (DisplayLabel.Text == "Tanımsız" || DisplayLabel.Text == "Sıfıra bölünemez") return;

            Button button = (Button)sender;
            string displayOp = button.Text;
            string calcOp = displayOp;


            //ekranda gercek hesap makineisne benzemesi için karakterleri bilgisayarın
            //kullandıgı gibi ekrana yazmadık bu yüzden burda karakterleri olması greketiği gibi çevirdik
            if (displayOp == "÷") calcOp = "/";
            if (displayOp == "×") calcOp = "*";
            if (displayOp == "−") calcOp = "-";
            if (displayOp == "mod") calcOp = "%"; // DataTable % ile mod alır

            // DataTable üs, kök ve logaritmayı desteklemediği için formata özel etiket veriyoruz
            if (displayOp == "xʸ") calcOp = "^";
            if (displayOp == "ʸ√x") calcOp = "root";
            if (displayOp == "logᵧx") calcOp = "logbase";

            // burada yanlıs bir operator seçtiğimizde silme tusuna basmadan sadece son iki karakteri siliyoruz ve yeni operatoru ekrana yazıyoruz
            if (yeniGirisMi && islemDizesi.Length > 0 && string.IsNullOrEmpty(suankiDeger))
            {
                // HATA ÇÖZÜMÜ: Eğer son girilen şey bir kapanış paranteziyse, silme işlemi yapmadan 
                // bölme/çarpma operatörünü direkt yanına ekletiyoruz.
                if (islemDizesi.EndsWith(") "))
                {
                    islemDizesi += calcOp + " ";
                    ekranGecmisi += displayOp + " ";
                    HistoryLabel.Text = ekranGecmisi;
                    return;
                }
                else // Parantez değilse eski kuralımız çalışmaya devam ediyor (Yanlış operatörü silip yenisini yazma).
                {
                    islemDizesi = islemDizesi.Substring(0, islemDizesi.Length - 3) + calcOp + " ";
                    ekranGecmisi = ekranGecmisi.Substring(0, ekranGecmisi.Length - 3) + displayOp + " ";
                    HistoryLabel.Text = ekranGecmisi;
                    return;
                }
            }

            if (string.IsNullOrEmpty(suankiDeger)) suankiDeger = DisplayLabel.Text.Replace(".", "");

            
            double.TryParse(DisplayLabel.Text.Replace(".", ""), System.Globalization.NumberStyles.Any, trCulture, out double val);
            string valStr = val.ToString(System.Globalization.CultureInfo.InvariantCulture);

            //işlemi hafızaya atıp yeni sayı girmek için hazırlık yapıyoruz
            islemDizesi += valStr + " " + calcOp + " ";
            ekranGecmisi += suankiDeger + " " + displayOp + " ";
            HistoryLabel.Text = ekranGecmisi;

            yeniGirisMi = true;
            suankiDeger = "";
        }

        private void sonucuHesapla(object sender, EventArgs e)
        {
            menuleriKapat();
            if (string.IsNullOrEmpty(islemDizesi)) return;//hesaplanacak bir şey yoksa geri döndürüyoruz

            // Eğer işlem kapanış parantezi ile bitiyorsa, ekrandaki sayıyı tekrar eklemeye gerek yoktur.
            if (islemDizesi.TrimEnd().EndsWith(")"))
            {
                ekranGecmisi += "=";
            }
            else
            {
                if (string.IsNullOrEmpty(suankiDeger)) suankiDeger = DisplayLabel.Text.Replace(".", "");

                double.TryParse(DisplayLabel.Text.Replace(".", ""), System.Globalization.NumberStyles.Any, trCulture, out double val);
                string valStr = val.ToString(System.Globalization.CultureInfo.InvariantCulture);

                islemDizesi += valStr;
                ekranGecmisi += suankiDeger + " =";
            }

            HistoryLabel.Text = ekranGecmisi;

            try
            {
                string hesapDizesi = islemDizesi;

                // 1. Metni boşluklara göre ayırıp bir listeye çeviriyoruz.
                List<string> parcalar = new List<string>(hesapDizesi.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                // 2. xʸ (Üs alma) işlemi
                while (parcalar.Contains("^"))
                {
                    int i = parcalar.IndexOf("^"); // İşaretin listedeki sırasını bul
                    double taban = double.Parse(parcalar[i - 1], System.Globalization.CultureInfo.InvariantCulture);
                    double us = double.Parse(parcalar[i + 1], System.Globalization.CultureInfo.InvariantCulture);

                    double powRes = Math.Pow(taban, us);

                    parcalar.RemoveRange(i - 1, 3);
                    parcalar.Insert(i - 1, powRes.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }

                // 3. ʸ√x (Y dereceden kök alma) işlemi
                while (parcalar.Contains("root"))
                {
                    int i = parcalar.IndexOf("root");
                    double kok = double.Parse(parcalar[i - 1], System.Globalization.CultureInfo.InvariantCulture);
                    double derece = double.Parse(parcalar[i + 1], System.Globalization.CultureInfo.InvariantCulture);
                    double rootRes = Math.Pow(kok, 1.0 / derece);

                    parcalar.RemoveRange(i - 1, 3);
                    parcalar.Insert(i - 1, rootRes.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }

                // 4. logᵧx (Y tabanında X logaritması) işlemi
                while (parcalar.Contains("logbase"))
                {
                    int i = parcalar.IndexOf("logbase");
                    double xDegeri = double.Parse(parcalar[i - 1], System.Globalization.CultureInfo.InvariantCulture);
                    double yTabani = double.Parse(parcalar[i + 1], System.Globalization.CultureInfo.InvariantCulture);
                    double logRes = Math.Log(xDegeri, yTabani);

                    parcalar.RemoveRange(i - 1, 3);
                    parcalar.Insert(i - 1, logRes.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }

                hesapDizesi = string.Join(" ", parcalar);

                // Kalan basit işlemleri DataTable'e yolluyoruz
                var dataTable = new DataTable();
                var result = Convert.ToDouble(dataTable.Compute(hesapDizesi, null));

                if (double.IsInfinity(result) || double.IsNaN(result))
                    DisplayLabel.Text = "Sıfıra bölünemez";
                else
                    DisplayLabel.Text = result.ToString("#,0.##########", trCulture);
            }
            catch
            {
                DisplayLabel.Text = "Tanımsız";
            }

            //hafızayı sıfırladık ki bir sonraki işlemde karısıklık olmasın
            islemDizesi = "";
            ekranGecmisi = "";
            suankiDeger = "";
            yeniGirisMi = true;
        }

        //temizle butonu C her şeyi ilk haline getiriyor
        private void temizleButonuSecildi(object sender, EventArgs e)
        {
            menuleriKapat();
            DisplayLabel.Text = "0";
            HistoryLabel.Text = "";
            islemDizesi = "";
            ekranGecmisi = "";
            suankiDeger = "";
            yeniGirisMi = true;
        }

        //del yani silme son yazılan rakami siliyor
        private void silmeTusunaBasildi(object sender, EventArgs e)
        {
            menuleriKapat();
            if (yeniGirisMi || DisplayLabel.Text == "Tanımsız" || DisplayLabel.Text == "Sıfıra bölünemez") return;

            DisplayLabel.Text = DisplayLabel.Text.Substring(0, DisplayLabel.Text.Length - 1);
            if (DisplayLabel.Text == "" || DisplayLabel.Text == "-") DisplayLabel.Text = "0";
            suankiDeger = DisplayLabel.Text.Replace(".", "");
        }

        //artı eksi butonu negatif ise pozitif , pozitif ise negatif yapıyor
        private void isaretDegistir(object sender, EventArgs e)
        {
            menuleriKapat();
            if (DisplayLabel.Text == "0" || DisplayLabel.Text == "Tanımsız") return;

            if (DisplayLabel.Text.StartsWith("-"))
                DisplayLabel.Text = DisplayLabel.Text.Substring(1);
            else
                DisplayLabel.Text = "-" + DisplayLabel.Text;

            suankiDeger = $"negate({suankiDeger})";
        }

        //ondalık sayı girmek için virgül ekleme 
        private void virgulEkle(object sender, EventArgs e)
        {
            menuleriKapat();
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

        //bilimsel hesap makinesind eolan 2nd yani ikinci fonksiyona geçme tuşunu burada oluşturduk
        private void ikinciFonksiyonDegistir(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Text == "2nd")
            {
                ikinciModAktif = !ikinciModAktif;

                if (ikinciModAktif)
                {
                    //2. mod aktif oldugunda butonların üstünde ki yazılar değişti
                    Btn2nd.BackgroundColor = Color.FromArgb("#DBA5E9");//basıldıgında butonun rengi değişiyır
                    Btn2nd.TextColor = Colors.Black;

                    BtnRow1.Text = "x³";
                    BtnRow2.Text = "³√x";
                    BtnRow3.Text = "ʸ√x";
                    BtnRow4.Text = "2ˣ";
                    BtnRow5.Text = "logᵧx";
                    BtnRow6.Text = "eˣ";
                }
                else
                {
                    Btn2nd.BackgroundColor = App.Current.RequestedTheme == AppTheme.Dark ? Color.FromArgb("#323232") : Color.FromArgb("#F9F9F9");
                    Btn2nd.TextColor = App.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;

                    BtnRow1.Text = "x²";
                    BtnRow2.Text = "²√x";
                    BtnRow3.Text = "xʸ";
                    BtnRow4.Text = "10ˣ";
                    BtnRow5.Text = "log";
                    BtnRow6.Text = "ln";
                }
            }
        }

        //aynı şekilde burada da trigonometri blogunda 2nd (2. fonk.) için de aynı işlemi yapıyoruz
        private void trigIkinciFonksiyonDegistir(object sender, EventArgs e)
        {
            trigonometriTersi = !trigonometriTersi;

            if (trigonometriTersi)
            {
                Trig2ndBtn.BackgroundColor = Color.FromArgb("#DBA5E9");
                Trig2ndBtn.TextColor = Colors.Black;

                BtnSin.Text = "sin⁻¹";
                BtnCos.Text = "cos⁻¹";
                BtnTan.Text = "tan⁻¹";
                BtnSec.Text = "sec⁻¹";
                BtnCsc.Text = "csc⁻¹";
                BtnCot.Text = "cot⁻¹";
            }
            else
            {
                Trig2ndBtn.BackgroundColor = App.Current.RequestedTheme == AppTheme.Dark ? Color.FromArgb("#323232") : Color.FromArgb("#E6E6E6");
                Trig2ndBtn.TextColor = App.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black;

                BtnSin.Text = "sin";
                BtnCos.Text = "cos";
                BtnTan.Text = "tan";
                BtnSec.Text = "sec";
                BtnCsc.Text = "csc";
                BtnCot.Text = "cot";
            }
        }

        //bilimsel butonların tetiklendiği ana yer burası
        private void bilimselislemYap(object sender, EventArgs e)
        {
            menuleriKapat();
            if (DisplayLabel.Text == "Tanımsız" || DisplayLabel.Text == "Sıfıra bölünemez") return;
            Button button = (Button)sender;
            string op = button.Text;

            // logᵧx işlemi bilimsel menüde olsa da aslında 2 sayılı bir işlem (X ve Y) olduğu için
            // Eşittir butonuna kadar bekletmesi adına 'islemButonunaBasildi' metoduna yönlendiriyoruz.
            if (op == "logᵧx")
            {
                ikinciModAktif = false;
                islemButonunaBasildi(sender, e);
                return;
            }

            if (string.IsNullOrEmpty(suankiDeger)) suankiDeger = DisplayLabel.Text.Replace(".", "");

            double.TryParse(DisplayLabel.Text.Replace(".", ""), System.Globalization.NumberStyles.Any, trCulture, out double val);
            double result = 0;

            //2nd butonu aktif ise yapılacak ekstra fonk.ları switch ile yönetiyoruz
            if (ikinciModAktif)
            {
                switch (op)
                {
                    case "x³":
                        result = Math.Pow(val, 3);
                        suankiDeger = $"cube({suankiDeger})";
                        break;
                    case "³√x":
                        result = Math.Cbrt(val);
                        suankiDeger = $"cbrt({suankiDeger})";
                        break;
                    case "2ˣ":
                        result = Math.Pow(2, val);
                        suankiDeger = $"2^({suankiDeger})";
                        break;
                    case "eˣ":
                        result = Math.Exp(val);
                        suankiDeger = $"e^({suankiDeger})";
                        break;
                }
                ikinciModAktif = false;
            }
            //trigo da 2nd aktif ise bu alan çalışıyor
            else if (trigonometriTersi)
            {
                switch (op)
                {
                    case "sin⁻¹":
                        if (val < -1 || val > 1) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = Math.Asin(val) * 180 / Math.PI;//dereceye döndürüyoruz
                        suankiDeger = $"asin({suankiDeger})";
                        break;
                    case "cos⁻¹":
                        if (val < -1 || val > 1) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = Math.Acos(val) * 180 / Math.PI;
                        suankiDeger = $"acos({suankiDeger})";
                        break;
                    case "tan⁻¹":
                        result = Math.Atan(val) * 180 / Math.PI;
                        suankiDeger = $"atan({suankiDeger})";
                        break;
                    case "sec⁻¹":
                        if (val > -1 && val < 1) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = Math.Acos(1 / val) * 180 / Math.PI;
                        suankiDeger = $"asec({suankiDeger})";
                        break;
                    case "csc⁻¹":
                        if (val > -1 && val < 1) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = Math.Asin(1 / val) * 180 / Math.PI;
                        suankiDeger = $"acsc({suankiDeger})";
                        break;
                    case "cot⁻¹":
                        result = (Math.PI / 2 - Math.Atan(val)) * 180 / Math.PI;
                        suankiDeger = $"acot({suankiDeger})";
                        break;
                }
            }
            else
            {
                //normal hesaplamalar bölümümüz
                switch (op)
                {
                    case "sin": result = Math.Sin(val * Math.PI / 180); suankiDeger = $"sin({suankiDeger})"; break;
                    case "cos": result = Math.Cos(val * Math.PI / 180); suankiDeger = $"cos({suankiDeger})"; break;
                    case "tan": result = Math.Tan(val * Math.PI / 180); suankiDeger = $"tan({suankiDeger})"; break;
                    case "sec":
                        if (Math.Cos(val * Math.PI / 180) == 0) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = 1 / Math.Cos(val * Math.PI / 180); suankiDeger = $"sec({suankiDeger})"; break;
                    case "csc":
                        if (Math.Sin(val * Math.PI / 180) == 0) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = 1 / Math.Sin(val * Math.PI / 180); suankiDeger = $"csc({suankiDeger})"; break;
                    case "cot":
                        if (Math.Tan(val * Math.PI / 180) == 0) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = 1 / Math.Tan(val * Math.PI / 180); suankiDeger = $"cot({suankiDeger})"; break;

                    case "rand":
                        result = new Random().NextDouble(); suankiDeger = "rand"; break;
                    case "⌊x⌋"://aşağı yuvarlama
                        result = Math.Floor(val); suankiDeger = $"floor({suankiDeger})"; break;
                    case "⌈x⌉"://yukarı yuvarlama
                        result = Math.Ceiling(val); suankiDeger = $"ceil({suankiDeger})"; break;
                    case "→dms"://ondaliklı sayıyı dms formatına çevirme
                        double d = Math.Floor(val);
                        double m = Math.Floor((val - d) * 60);
                        double s = ((val - d) * 60 - m) * 60;
                        result = d + (m / 100.0) + (s / 10000.0);
                        suankiDeger = $"dms({suankiDeger})";
                        break;
                    case "→deg"://tam tersi dms formatını ondalikli dereceye cevirme
                        double degD = Math.Floor(val);
                        double degM = Math.Floor((val - degD) * 100);
                        double degS = Math.Round(((val - degD) * 100 - degM) * 100);
                        result = degD + (degM / 60.0) + (degS / 3600.0);
                        suankiDeger = $"deg({suankiDeger})";
                        break;

                    case "log":
                        if (val <= 0) { DisplayLabel.Text = "Tanımsız"; return; }//negatif oldugunda kabul olmaz
                        result = Math.Log10(val); suankiDeger = $"log({suankiDeger})"; break;
                    case "ln":
                        if (val <= 0) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = Math.Log(val); suankiDeger = $"ln({suankiDeger})"; break;
                    case "²√x":
                        if (val < 0) { DisplayLabel.Text = "Tanımsız"; return; }//çift kök içi negatif olamaz
                        result = Math.Sqrt(val); suankiDeger = $"√({suankiDeger})"; break;
                    case "x²": result = Math.Pow(val, 2); suankiDeger = $"sqr({suankiDeger})"; break;
                    case "1/x":
                        if (val == 0) { DisplayLabel.Text = "Sıfıra bölünemez"; return; }
                        result = 1 / val; suankiDeger = $"1/({suankiDeger})"; break;
                    case "|x|": result = Math.Abs(val); suankiDeger = $"abs({suankiDeger})"; break;
                    case "10ˣ": result = Math.Pow(10, val); suankiDeger = $"10^({suankiDeger})"; break;
                    case "n!"://faktoriyel hesabını for döngüsü ile yaptığımız yer
                        if (val < 0 || val % 1 != 0) { DisplayLabel.Text = "Tanımsız"; return; }
                        result = 1; for (int i = 1; i <= val; i++) result *= i;
                        suankiDeger = $"fact({suankiDeger})"; break;
                    case "exp": result = Math.Exp(val); suankiDeger = $"exp({suankiDeger})"; break;
                }
            }
            //sonucu hesapladıktan sonra formatımıza uygun şekilde ekrana yazdırıyoruz
            DisplayLabel.Text = result.ToString("#,0.##########", trCulture);
            HistoryLabel.Text = ekranGecmisi + suankiDeger;
            yeniGirisMi = true;//yeni sayı girilirse sıfırdan başlatmak için
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Scale = Microsoft.Maui.Storage.Preferences.Default.Get("AppScale", 1.0);
        }
    }
}
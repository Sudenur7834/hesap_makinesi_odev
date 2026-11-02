namespace HesapMakinesi4
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            
            int kayitliTema = Microsoft.Maui.Storage.Preferences.Default.Get("AppTheme", 2);
            Application.Current.UserAppTheme = kayitliTema switch
            {
                0 => AppTheme.Light,
                1 => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            const int makineGenisligi = 350;
            const int makineYuksekligi = 600;
            window.Width = makineGenisligi;
            window.Height = makineYuksekligi;
            window.MinimumWidth = makineGenisligi;
            window.MinimumHeight = makineYuksekligi;
            return window;
        }
    }
}
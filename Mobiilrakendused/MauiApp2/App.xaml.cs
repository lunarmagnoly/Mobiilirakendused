using Microsoft.Extensions.DependencyInjection;

namespace MauiApp2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Loome esimese lehe (nt StartPage)
            var startPage = new StartPage();
            // Pakime selle NavigationPage sisse, et tekiks ülemine riba ja "tagasi"

            var navPage = new NavigationPage(startPage)
            {
                BarBackgroundColor = Colors.Blue,
                BarTextColor = Colors.White
            };

            return new Window(navPage);
        }
    }
}
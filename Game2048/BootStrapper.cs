using Game2048.Game.Views;
using Game2048.Infrastructure.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Windows;

namespace Game2048
{
    public class BootStrapper
    {        
        [System.STAThreadAttribute()]
        static void Main()
        {
            Application app = new Application();
            app.Startup += app_Startup;
            app.Run();

        }

        private static void ConfigureContainer()
        {
            var container = new UnityContainer().LoadConfiguration();
            var locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static GameShell CreateShell()
        {
            return ServiceLocator.Current.GetInstance<GameShell>();
        }

        private static void Initialize()
        {
            GameShell mainWindow = CreateShell();
            Application.Current.MainWindow = mainWindow;

            IBrowserManager browserManager = ServiceLocator.Current.GetInstance<IBrowserManager>();
            browserManager.Upgrade();
            browserManager.Navigated = () => browserManager.InjectScript();
            //browserManager.NavigateTo("http://gabrielecirulli.github.io/2048/");
            browserManager.NavigateTo("http://2048game.com/");
            browserManager.DeactivateErrors();
            Application.Current.MainWindow.Show();
        }

        static void app_Startup(object sender, StartupEventArgs e)
        {
            ConfigureContainer();
            Initialize();
        }
    }
}

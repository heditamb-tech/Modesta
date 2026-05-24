using System.Windows;

namespace Modesta
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnLastWindowClose;
            var login = new Modesta.Views.LoginWindow();
            login.Show();
        }
    }
}
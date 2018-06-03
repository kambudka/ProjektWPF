using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProjektWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Utworzenie anonimowego użytkownika podczas startu aplikacji.
            CustomPrincipal customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            base.OnStartup(e);

            //Otworzenie głównego okna
            AuthenticationViewModel viewModel = new AuthenticationViewModel(new AuthenticationService());
            IView startWindow = new MainWindow(viewModel);
            startWindow.Show();


        }
    }
}

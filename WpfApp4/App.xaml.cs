using System;
using System.Windows;

namespace FahrzeugVerwaltung
{
 
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Ein unerwarteter Fehler ist aufgetreten:\n{e.Exception.Message}",
                "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Ein kritischer Fehler ist aufgetreten:\n{((Exception)e.ExceptionObject).Message}",
                "Kritischer Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
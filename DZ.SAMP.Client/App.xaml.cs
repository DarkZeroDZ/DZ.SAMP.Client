using Autofac;
using DZ.SAMP.Client.Domain;
using DZ.SAMP.Client.UI.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DZ.SAMP.Client
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static IContainer Container;
        public static ContainerBuilder Builder;
        private Models.Settings _settings;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new Mutex(true, "San Andreas Multiplayer Launcher", out var onlyInstance);
            if (!onlyInstance && !e.Args.Contains("IgnoreRunningInstance"))
            {
                try
                {
                    var other = User32.FindWindow(null, "San Andreas Multiplayer Launcher");
                    if (other != IntPtr.Zero)
                    {
                        User32.SetForegroundWindow(other);

                        if (User32.IsIconic(other))
                            User32.OpenIcon(other);
                    }
                    else
                    {
                        User32.SetForegroundWindow(other);
                    }
                }
                finally
                {
                    Current?.Shutdown();
                }
            }
            else
            {
                Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
                this.Dispatcher.UnhandledException += Dispatcher_UnhandledException1;

                this._settings = new Models.Settings();

                if (this._settings.Language != null)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(this._settings.Language.Culture);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(this._settings.Language.Culture);
                }
                else
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                }

                if (string.IsNullOrEmpty(this._settings.SingleplayerLocation))
                    this._settings.SingleplayerLocation = AppDomain.CurrentDomain.BaseDirectory;

                App.Builder = new ContainerBuilder();
                App.Builder.RegisterType<MainViewModel>().As<MainViewModel>();
                App.Container = App.Builder.Build();

                var window = new MainWindow { DataContext = App.Container.Resolve<MainViewModel>() };
                window.ShowDialog();
            }
        }

        #region Unhandled exception
        private static void UnhandledException(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException(e.Exception);
            e.Handled = true;
        }

        private static void Dispatcher_UnhandledException1(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException(e.Exception);
            e.Handled = true;
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            UnhandledException(e.Exception);
            e.SetObserved();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException((Exception)e.ExceptionObject);
        }
        #endregion
    }
}

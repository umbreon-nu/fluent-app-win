using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using WpfApp1.Views;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MutexName = "IODATA_DSIG";
        private const int SW_SHOWNORMAL = 1;

        private readonly Mutex _mutex = new(false, MutexName);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetForegroundWindow(IntPtr hWnd);

        // 正常時はコンストラクタで設定される
        public ServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();

            Startup += App_Startup;
            Exit += App_Exit;
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // View
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var existingProcess = GetExistingProcess();
            // すでに起動している場合、ウィンドウを前面に表示する
            if (existingProcess != null)
            {
                ShowWindow(existingProcess.MainWindowHandle, SW_SHOWNORMAL);
                SetForegroundWindow(existingProcess.MainWindowHandle);
                Current.Shutdown();
                return;
            }

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            _mutex?.Close();
        }

        private Process? GetExistingProcess()
        {
            if (_mutex.WaitOne(0, false)) return null;
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            return processes.FirstOrDefault(x => x.Id != currentProcess.Id && x.MainWindowHandle != IntPtr.Zero);
        }
    }
}

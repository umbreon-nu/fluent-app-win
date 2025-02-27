using System.Windows;
using AutoUpdaterDotNET;

namespace WpfApp1.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // FIXME: AutoUpdater.NETのサンプルなので、アップデートサーバーに配置したXMLを指定してください
            AutoUpdater.Start("https://rbsoft.org/updates/AutoUpdaterTest.xml");
        }
    }
}

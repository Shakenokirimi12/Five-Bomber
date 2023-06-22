using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Five_Bomber.Pages
{
    /// <summary>
    /// Lobby.xaml の相互作用ロジック
    /// </summary>
    public partial class Lobby : Page
    {
        public Lobby()
        {
            InitializeComponent();
            // ホスト名を取得する
            string hostname = Dns.GetHostName();

            // ホスト名からIPアドレスを取得する
            IPAddress[] adrList = Dns.GetHostAddresses(hostname);
            foreach (IPAddress address in adrList)
            {
                IPaddress.Content = "IP:" + address.ToString();
            }
            Roop = true;
        }

        private async Task ExecuteNodeScriptAsync(string directoryPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = directoryPath;
            process.StartInfo.Arguments = "/C node app.js";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.OutputDataReceived += (sender, e) =>
            {
                // コマンドプロンプトの出力を処理するためのコード
                // ここでは、出力をConsoleに表示していますが、適宜変更してください。
                Console.WriteLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();

            // 非同期に待機してプロセスの終了を待つ
            process.WaitForExit();
        }

        private void BackGround_Lobby_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void GameStart()
        {
            try
            {
                Frame frame = new Frame();
                NavigationService navigationService = frame.NavigationService;
                this.NavigationService.Navigate(new Uri("Pages/GamePage.xaml", UriKind.Relative));
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.ToString()); ;
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private async void Label_Lobby_Loaded(object sender, RoutedEventArgs e)
        {
            Roop = true;
            string directoryPath = "RemoteControl";
            await ExecuteNodeScriptAsync(directoryPath);
            while (Roop)
            {
                Label_Lobby.Content = "Waiting for start.";
                GameStatusReader();
                await Task.Delay(500);
                Label_Lobby.Content = "Waiting for start..";
                GameStatusReader();
                await Task.Delay(500);
                Label_Lobby.Content = "Waiting for start...";
                GameStatusReader();
                await Task.Delay(500);
                Label_Lobby.Content = "Waiting for start....";
                GameStatusReader();
                await Task.Delay(500);
                Label_Lobby.Content = "Waiting for start.....";
                GameStatusReader();
                await Task.Delay(500);
                Label_Lobby.Content = "Waiting for start......";
                GameStatusReader();
            }
        }

        static bool Roop = true;

        public void GameStatusReader()
        {
            var program = new Lobby();
            using (var reader = program.OpenReadFile("RemoteControl/GameStatus.txt"))
            {
                string readString = reader.ReadLine();
                if (readString == "1")
                {
                    var program2 = new Lobby();
                    using (var fileStream = new FileStream("RemoteControl/GameStatus.txt", FileMode.Open))
                    {
                        fileStream.SetLength(0);
                    }
                    Roop = false;
                    GameStart();
                }
            }
        }

        private StreamReader OpenReadFile(string filePathName)
        {
            // 第4引数のFileShareは"ReadWrite"にします。
            var fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var reader = new StreamReader(fileStream);
            return reader;
        }

        private StreamWriter OpenWriteFile(string filePathName)
        {
            var fileStream = new FileStream(filePathName, FileMode.Create, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(fileStream);
            return writer;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // 実行中のタスクを停止する
            Roop = false;

            // イベントハンドラの解除
            Label_Lobby.Loaded -= Label_Lobby_Loaded;

            // その他の後処理が必要なリソースやオブジェクトがあればここで解放する
        }

    }
}

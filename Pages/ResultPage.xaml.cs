using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace Five_Bomber.Pages
{
    /// <summary>
    /// ResultPage.xaml の相互作用ロジック
    /// </summary>
    public partial class ResultPage : Page
    {
        public ResultPage()
        {
            InitializeComponent();
            
        }

        static string TeamName;
        private void BackGround_Lobby_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void GameEnd()
        {
            var Destination = new Pages.Lobby();
            NavigationService.Navigate(Destination);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var GamePoint = Application.Current.Properties["Point"];
            TeamName = Application.Current.Properties["TeamName"].ToString();
            ResultShowBox.Text = $"チーム名:{TeamName}" + "\r\n" + $"正解数:{GamePoint.ToString()}問" + "\r\n" + "ランキングを読み込み中です。お待ちください...";
            await SendPointToAPI(TeamName, Int32.Parse(GamePoint.ToString()));
            await getRank();
            if (TeamRank == 0)
            {
                ResultShowBox.Text = $"チーム名:{TeamName}" + "\r\n" + $"正解数:{GamePoint.ToString()}問";
            }
            else if(TeamRank == 1)
            {
                ResultShowBox.Text = $"チーム名:{TeamName}" + "\r\n" + $"正解数:{GamePoint.ToString()}問" + "\r\n" + $"チームの最高記録は{TeamRank.ToString()}位です。おめでとうございます！";
            }
            else
            {
                ResultShowBox.Text = $"チーム名:{TeamName}" + "\r\n" + $"正解数:{GamePoint.ToString()}問" + "\r\n" + $"チームの最高記録は{TeamRank.ToString()}位です。";
            }
            while (true)
            {
                GameStatusReader();
                await Task.Delay(500);
            }
        }

        public async void GameStatusReader()
        {
            var program = new ResultPage();
            using (var reader = program.OpenReadFile("RemoteControl/GameStatus.txt"))
            {
                string readString = reader.ReadLine();
                if (readString == "2")
                {
                    var program2 = new ResultPage();
                    using (var fileStream = new FileStream("RemoteControl/GameStatus.txt", FileMode.Open))
                    {
                        fileStream.SetLength(0);
                    }
                    GameEnd();
                }
            }
            await Task.Delay(1);
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

        private async Task SendPointToAPI(string TeamName,int Point)
        {
            try
            {
                HttpResponseMessage responsePost = await SendPostRequest(apiUrl, TeamName, Point);
                if (responsePost.IsSuccessStatusCode)
                {
                    string responseBody = await responsePost.Content.ReadAsStringAsync();
                    Console.WriteLine("POST Response:");
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine("POST Request failed.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static async Task<HttpResponseMessage> SendGetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                return response;
            }
        }

        static async Task<HttpResponseMessage> SendPostRequest(string url, string holder, int score)
        {
            using (HttpClient client = new HttpClient())
            {
                // POSTデータを作成
                var postData = new { holder = holder, score = score };
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postData), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                return response;
            }
        }

        public async Task getRank()
        {
            try
            {
                // GETリクエストの例
                HttpResponseMessage responseGet = await SendGetRequest(apiUrl + "?holder=" + TeamName);
                if (responseGet.IsSuccessStatusCode)
                {
                    string responseBody = await responseGet.Content.ReadAsStringAsync();
                    Console.WriteLine("GET Response:");

                    // JSONをパース
                    dynamic data = JsonConvert.DeserializeObject(responseBody);

                    // specificHolderRank の rank を取得
                    TeamRank = data.specificHolderRank.rank;
                }
                else
                {
                    Console.WriteLine("GET Request failed.");
                }

            }
            catch(Exception)
            {
                TeamRank = 0;
            }
        }

        static int TeamRank;
    }
}

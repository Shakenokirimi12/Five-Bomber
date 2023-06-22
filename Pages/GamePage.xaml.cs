using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml;
using Microsoft.DirectX.AudioVideoPlayback;
using System.Drawing;
using System.Threading;
using System.IO;
using System.ComponentModel.Design;
using System.Windows.Interop;
using static System.Net.Mime.MediaTypeNames;

namespace Five_Bomber.Pages
{
    /// <summary>
    /// GamePage.xaml の相互作用ロジック
    /// </summary>
    public partial class GamePage : Page
    {
        public GamePage()
        {
            InitializeComponent();
        }

        static string TeamName;

        public void InitGame()
        {
            var program = new GamePage();
            using (var reader = program.OpenReadFile("RemoteControl/PlayerNumber.txt"))
            {
                PlayerNumber = Int32.Parse(reader.ReadLine());
            }
            var program2 = new GamePage();
            using (var reader = program2.OpenReadFile("RemoteControl/TeamName.txt"))
            {
                TeamName = reader.ReadLine();
            }
            BackGroundImage.Visibility = Visibility.Visible;
            Red_Bar.Visibility = Visibility.Visible;
            Blue_Bar.Visibility = Visibility.Visible;
            Yellow_Bar.Visibility = Visibility.Visible;
            Green_Bar.Visibility = Visibility.Visible;
            Orange_Bar.Visibility = Visibility.Visible;
            Bomb.Visibility = Visibility.Visible;
            using (var fileStream = new FileStream("RemoteControl/Wrong.txt", FileMode.Open))
            {
                fileStream.SetLength(0);
            }
            using (var fileStream = new FileStream("RemoteControl/Collect.txt", FileMode.Open))
            {
                fileStream.SetLength(0);
            }
            var program3 = new GamePage();
            using (var reader = program3.OpenReadFile("RemoteControl/BombSpeed.txt"))
            {
                string readString = reader.ReadLine();
                BarSpeed = Int32.Parse(readString);
            }
            PointBox.Text = TeamName + ":" + "0Point";
            GamePoint = 0;
        }

        static int GamePoint;


        static bool Explode;
        static int BarSpeed;

        public async void GameStart()
        {
            var audio4 = new Microsoft.DirectX.AudioVideoPlayback.Audio("Switch.wav");
            audio4.Play();
            while (true)
            {
                Blue_Bar.Margin = new Thickness(323, Ycoordinate, 0, 0);
                Green_Bar.Margin = new Thickness(586, Ycoordinate, 0, 0);
                Yellow_Bar.Margin = new Thickness(853, Ycoordinate, 0, 0);
                Orange_Bar.Margin = new Thickness(1104, Ycoordinate, 0, 0);
                Red_Bar.Margin = new Thickness(1365, Ycoordinate, 0, 0);
                Bomb.Margin = new Thickness(BombXCoordinate, -153 + Ycoordinate, 0, 0);
                Ycoordinate = Ycoordinate + 5;
                if (Ycoordinate == 325)
                {
                    Blue_Bar.Margin = new Thickness(323, 325, 0, 0);
                    Green_Bar.Margin = new Thickness(586, 325, 0, 0);
                    Yellow_Bar.Margin = new Thickness(853, 325, 0, 0);
                    Orange_Bar.Margin = new Thickness(1104, 325, 0, 0);
                    Red_Bar.Margin = new Thickness(1365, 325, 0, 0);
                    Bomb.Margin = new Thickness(BombXCoordinate, 170, 0, 0);
                    Ycoordinate = 0;
                    break;
                }
                else
                {
                    await Task.Delay(1);
                }
            }
            await Task.Delay(5000);
            CountDownText.Visibility = Visibility.Visible;
            CountDownText.Content = "3";
            await Task.Delay(1000);
            CountDownText.Content = "2";
            await Task.Delay(1000);
            CountDownText.Content = "1";
            await Task.Delay(1000);
            CountDownText.Content = "Start!";
            await Task.Delay(1000);
            CountDownText.Visibility = Visibility.Hidden;
            DownAllBar();
            audio1.Play();
        }
        Microsoft.DirectX.AudioVideoPlayback.Audio audio1 = new Microsoft.DirectX.AudioVideoPlayback.Audio("BGM.wav");
        Microsoft.DirectX.AudioVideoPlayback.Audio audio2;
        Microsoft.DirectX.AudioVideoPlayback.Audio audio3;



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        public void DownBlueBar(int Ycoordinate)
        {
            Blue_Bar.Margin = new Thickness(323, 323 + Ycoordinate, 0, 0);
        }

        public void DownGreenBar(int Ycoordinate)
        {
            Green_Bar.Margin = new Thickness(586, 323 + Ycoordinate, 0, 0);
        }

        public void DownYellowBar(int Ycoordinate)
        {
            Yellow_Bar.Margin = new Thickness(853, 323 + Ycoordinate, 0, 0);
        }

        public void DownOrangeBar(int Ycoordinate)
        {
            Orange_Bar.Margin = new Thickness(1104, 323 + Ycoordinate, 0, 0);
        }

        public void DownRedBar(int Ycoordinate)
        {
            Red_Bar.Margin = new Thickness(1365, 323 + Ycoordinate, 0, 0);
        }


        static int Ycoordinate = 0;
        public async void DownAllBar()
        {
            while (true)
            {
                if (!Effect)
                {
                    if (Ycoordinate > 501)
                    {
                        GameOver();
                        break;
                    }
                    if(Ycoordinate > 400)
                    {
                       ChangeOpacity();
                    }
                    DownBlueBar(Ycoordinate);
                    DownGreenBar(Ycoordinate);
                    DownYellowBar(Ycoordinate);
                    DownOrangeBar(Ycoordinate);
                    DownRedBar(Ycoordinate);
                    DownBomb(Ycoordinate);
                    Ycoordinate += 1;
                    CollectReader();
                    WrongReader();
                    if (GameStatusReader())
                    {
                        GameOver();
                        break;
                    }
                    await Task.Delay(BarSpeed);
                }
                else
                {
                    break;
                }
            }
        }
        double DangerOpacity = 0;
        int n = 0;

        public async void ChangeOpacity()
        {
            
            Danger.Opacity = DangerOpacity;
            if (DangerOpacity < 0.6 && n == 0)
            {
                DangerOpacity += 0.2;
            }
            else if(DangerOpacity == 0.6)
            {
                n++;
                if(n == 15)
                {
                    DangerOpacity -= 0.2;
                }
            }
            else if(n==15)
            {
                DangerOpacity -= 0.2;
                if(DangerOpacity == 0)
                {
                    n = 0;
                }
            }
        }

        public void DownBomb(int Ycoordinate)
        {
            Bomb.Margin = new Thickness(BombXCoordinate, 170 + Ycoordinate, 0, 0);
        }

        static int BombXCoordinate = 370;

        public void GameOver()
        {
            audio1.Stop();
            System.Windows.Application.Current.Properties["Point"] = GamePoint;
            System.Windows.Application.Current.Properties["TeamName"] = TeamName;
            ResetAll();
            var Destination = new Pages.ResultPage();
            NavigationService.Navigate(Destination);
        }

        public void ResetAll()
        {
            BombXCoordinate = 370;
            Ycoordinate = 0;
            GamePoint = 0;
            Effect = false;
            Explode = false;
            Blue_Bar.Margin = new Thickness(323, 0, 0, 0);
            Green_Bar.Margin = new Thickness(586, 0, 0, 0);
            Yellow_Bar.Margin = new Thickness(853, 0, 0, 0);
            Orange_Bar.Margin = new Thickness(1104, 0, 0, 0);
            Red_Bar.Margin = new Thickness(1365, 0, 0, 0);
            Bomb.Margin = new Thickness(BombXCoordinate, -153, 0, 0);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            InitGame();
            GameStart();
        }


        private void Collect()
        {
            PlayCollectSound();
            if (PlayerNumber == 5)
            {
                MoveBomb5Players();
            }
            else if (PlayerNumber == 4)
            {
                MoveBomb4Players();
            }
            else if (PlayerNumber == 3)
            {
                MoveBomb3Players();
            }
            GamePoint++;
            PointBox.Text = TeamName + ":" + GamePoint + "Points";

        }

        static int PlayerNumber;

        private async void MoveBomb5Players()
        {
            if (GamePoint % 10 == 0)//余りが0、つまり、爆弾が青にある
            {
                MoveBombSeemless(640);//GreenBarの上に配置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Blue_Bar.Width;
                    double imageHeight = Blue_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Blue_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 10 == 1)
            {
                MoveBombSeemless(900);//YellowBarの上に設置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Green_Bar.Width;
                    double imageHeight = Green_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Green_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 10 == 2)
            {
                MoveBombSeemless(1160);//Orange((ry
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Yellow_Bar.Width;
                    double imageHeight = Yellow_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Yellow_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 10 == 3)
            {
                MoveBombSeemless(1420);//Red((ry
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Orange_Bar.Width;
                    double imageHeight = Orange_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Orange_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 10 == 4 || GamePoint % 10 == 9)
            {
                ReStartEffect();
            }
            else if (GamePoint % 10 == 5)
            {
                MoveBombSeemless(1160);//Orange((ry            
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Red_Bar.Width;
                    double imageHeight = Red_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Red_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 10 == 6)
            {
                MoveBombSeemless(900);//YellowBarの上に設置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Orange_Bar.Width;
                    double imageHeight = Orange_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Orange_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 10 == 7)
            {
                MoveBombSeemless(640);//GreenBarの上に配置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Yellow_Bar.Width;
                    double imageHeight = Yellow_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Yellow_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 10 == 8)//余りが0、つまり、爆弾が青にある
            {
                MoveBombSeemless(380);//BlueBarの上に設置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Green_Bar.Width;
                    double imageHeight = Green_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Green_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
        }

        private async void MoveBomb4Players()
        {
            if (GamePoint % 8 == 0)//余りが0、つまり、爆弾が青にある
            {
                MoveBombSeemless(640);//GreenBarの上に配置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Blue_Bar.Width;
                    double imageHeight = Blue_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Blue_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 8 == 1)//緑にある
            {
                MoveBombSeemless(900);//YellowBarの上に設置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Green_Bar.Width;
                    double imageHeight = Green_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Green_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 8 == 2)//黄色にある
            {
                MoveBombSeemless(1160);//Orange((ry
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Yellow_Bar.Width;
                    double imageHeight = Yellow_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Yellow_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 8 == 3)//オレンジにある(往路)
            {
                ReStartEffect();
            }
            else if (GamePoint % 8 == 4)//オレンジにある(復路)
            {
                MoveBombSeemless(900);//YellowBarの上に設置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Orange_Bar.Width;
                    double imageHeight = Orange_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Orange_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 8 == 5)
            {
                MoveBombSeemless(640);//GreenBarの上に配置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Yellow_Bar.Width;
                    double imageHeight = Yellow_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Yellow_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 8 == 6)
            {
                MoveBombSeemless(380);//BlueBarの上に設置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Green_Bar.Width;
                    double imageHeight = Green_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Green_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 8 == 7)
            {
                ReStartEffect();
            }
        }

        private async void MoveBomb3Players()
        {
            if (GamePoint % 6 == 0)//余りが0、つまり、爆弾が青にある
            {
                MoveBombSeemless(640);//GreenBarの上に配置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Blue_Bar.Width;
                    double imageHeight = Blue_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Blue_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 6 == 1)//緑にある
            {
                MoveBombSeemless(900);//YellowBarの上に設置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Green_Bar.Width;
                    double imageHeight = Green_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Green_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 6 == 2)//黄色にある
            {
                ReStartEffect();
            }
            else if (GamePoint % 6 == 3)//オレンジにある(往路)
            {
                MoveBombSeemless(640);//GreenBarの上に配置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Yellow_Bar.Width;
                    double imageHeight = Yellow_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Yellow_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 6 == 4)//オレンジにある(復路)
            {
                MoveBombSeemless(380);//BlueBarの上に設置
                double rotationAngle = 0;
                do
                {
                    // 画像のサイズを取得
                    double imageWidth = Green_Bar.Width;
                    double imageHeight = Green_Bar.Height;

                    // 画像を中心を基準に回転させるためのTransformを作成
                    RotateTransform rotateTransform = new RotateTransform(rotationAngle, imageWidth / 2, imageHeight / 2);
                    Green_Bar.RenderTransform = rotateTransform;
                    rotationAngle += 10;
                    await Task.Delay(1);
                } while (rotationAngle != 190);
            }
            else if (GamePoint % 6 == 5)
            {
                ReStartEffect();
            }
        }

        private async void MoveBombSeemless(int BombXCoordinateDestination)
        {
            if (BombXCoordinate < BombXCoordinateDestination)
            {
                while (true)
                {
                    BombXCoordinate = BombXCoordinate + 10;
                    if (BombXCoordinate == BombXCoordinateDestination)
                    {
                        break;
                    }
                    else
                    {
                        await Task.Delay(1);
                    }
                }
            }
            else if (BombXCoordinate > BombXCoordinateDestination)
            {
                while (true)
                {
                    BombXCoordinate = BombXCoordinate - 10;
                    if (BombXCoordinate == BombXCoordinateDestination)
                    {
                        break;
                    }
                    else
                    {
                        await Task.Delay(1);
                    }
                }
            }

        }

        private void Wrong()
        {
            PlayWrongSound();
        }

        private void PlayCollectSound()
        {
            var audio2 = new Microsoft.DirectX.AudioVideoPlayback.Audio("Collect.wav");
            audio2.Play();
        }

        private void PlayWrongSound()
        {
            var audio3 = new Microsoft.DirectX.AudioVideoPlayback.Audio("Wrong.wav");
            audio3.Play();
        }

        private void IncrementCount()
        {
            GamePoint++;
        }


        static bool Effect = false;

        static int EffectCount = 0;

        private async void ReStartEffect()
        {
            audio1.Stop();
            var audio4 = new Microsoft.DirectX.AudioVideoPlayback.Audio("Switch.wav");
            audio4.Play();
            Effect = true;
            //備忘録。バーと爆弾をガン上げした後、もとに戻す。]
            var NowYCoordinate = Ycoordinate;
            //演出開始
            while (true)
            {
                Blue_Bar.Margin = new Thickness(323, 323 + Ycoordinate, 0, 0);
                Green_Bar.Margin = new Thickness(586, 323 + Ycoordinate, 0, 0);
                Yellow_Bar.Margin = new Thickness(853, 323 + Ycoordinate, 0, 0);
                Orange_Bar.Margin = new Thickness(1104, 323 + Ycoordinate, 0, 0);
                Red_Bar.Margin = new Thickness(1365, 323 + Ycoordinate, 0, 0);
                Bomb.Margin = new Thickness(BombXCoordinate, 170 + Ycoordinate, 0, 0);
                int DownSpeed = NowYCoordinate / 10;
                Ycoordinate = Ycoordinate - DownSpeed;
                if (Ycoordinate < -400)
                {
                    break;
                }
                else
                {
                    await Task.Delay(1);
                }
            }
            while (true)
            {
                Blue_Bar.Margin = new Thickness(323, 323 + Ycoordinate, 0, 0);
                Green_Bar.Margin = new Thickness(586, 323 + Ycoordinate, 0, 0);
                Yellow_Bar.Margin = new Thickness(853, 323 + Ycoordinate, 0, 0);
                Orange_Bar.Margin = new Thickness(1104, 323 + Ycoordinate, 0, 0);
                Red_Bar.Margin = new Thickness(1365, 323 + Ycoordinate, 0, 0);
                Bomb.Margin = new Thickness(BombXCoordinate, 170 + Ycoordinate, 0, 0);
                int DownSpeed = NowYCoordinate / 10;
                Ycoordinate = Ycoordinate + DownSpeed;
                if (Ycoordinate == NowYCoordinate)
                {
                    break;
                }
                else
                {
                    await Task.Delay(1);
                }
            }
            //演出終了
            await Task.Delay(1000);
            audio4.Stop();
            audio4.Stop();
            audio1.Play();
            Effect = false;
            DownAllBar();
        }


        public async void CollectReader()
        {
            var program = new GamePage();
            using (var reader = program.OpenReadFile("RemoteControl/Collect.txt"))
            {
                string readString = reader.ReadLine();
                if (readString == "1")
                {
                    Collect();
                    var program2 = new GamePage();
                    using (var fileStream = new FileStream("RemoteControl/Collect.txt", FileMode.Open))
                    {
                        fileStream.SetLength(0);
                    }
                }
            }
        }

        static bool GameStatusReader()
        {
            var program = new GamePage();
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
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async void WrongReader()
        {
            var program = new GamePage();
            using (var reader = program.OpenReadFile("RemoteControl/Wrong.txt"))
            {
                string readString = reader.ReadLine();
                if (readString == "1")
                {
                    Wrong();
                    var program2 = new GamePage();
                    using (var fileStream = new FileStream("RemoteControl/Wrong.txt", FileMode.Open))
                    {
                        fileStream.SetLength(0);
                    }
                }
            }
        }


        public void readtextfile()
        {

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

    }
}





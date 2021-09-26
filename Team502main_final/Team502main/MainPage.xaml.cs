using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Team502main.Serial;
using Team502main.Serial.Message;
using Team502main.Util;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace Team502main
{


    /// Private variables
    public sealed partial class MainPage : Page, ISerialReadNotification
    {
        List<Target> targetlist = new List<Target>();
        Dictionary<string, long> lastUpdate = new Dictionary<string, long>();
        Dictionary<string, Image> uiElements = new Dictionary<string, Image>();

        List<Image> imagelist = new List<Image>();
        BitmapImage image;//class image

        SerialManager serialManager;
        SerialParser serialParser = new SerialParser();

        private static System.Timers.Timer aTimer;
        private static readonly double cycleTime = 2000;

        private string messageQueue = "";
        private bool isMessageStarted = false;
        private int messageIndex = 0;

        User u;

        MediaPlayer mediaPlayer = new MediaPlayer();

        public MainPage()
        {
            InitializeComponent();
            u = new User(0.0, 0.0, 0.0);
            image = new BitmapImage
            {
                UriSource = new Uri("ms-appx:///Assets/stick-man-full.png") //image
            };
            serialManager = new SerialManager(this);
            aTimer = new System.Timers.Timer(cycleTime); // 이벤트 핸들러 연결
            aTimer.Elapsed += new ElapsedEventHandler(Check_Update); // Timer에서 Elapsed 이벤트를 반복해서 발생
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/ding.wav"));
            mediaPlayer.Volume = 1;
            mediaPlayer.IsLoopingEnabled = true;
        }

        private async void Check_Update(object sender, ElapsedEventArgs e)
        {
            var currentMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            var toRemove = new HashSet<string>();
            try
            {
                foreach (var item in lastUpdate.Keys)
                {
                    if (lastUpdate[item] < currentMillis - 2000 && uiElements.ContainsKey(item))
                    {
                        try
                        {
                            Target target = new Target();
                            foreach (var t in targetlist)
                            {
                                if (t.Uid == item)
                                    target = t;
                            }
                            targetlist.Remove(target);
                            toRemove.Add(item);
                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                try
                                {
                                    MainGrid.Children.Remove(uiElements[item]);
                                    imagelist.Remove(uiElements[item]);
                                    uiElements.Remove(item);
                                }
                                catch (Exception ex) { }
                            });
                        }
                        catch (Exception ex) { }
                    }
                }
                foreach (var item in toRemove)
                    lastUpdate.Remove(item);
            }
            catch (Exception exc) { }
        }

        public void OnMessageReceive(string message)
        {
            //Debug.Write(message);
            //Handle message at here
            //1문자 단위로 메시지를 수신한다.
            if (isMessageStarted)
            {
                //메시지 수신 중이었음 -> append to message queue.
                //\n 출현 시 메시지 수신을 완료
                //단, \r\n은 queue에 append하지 않는다.
                //메시지가 완료된 경우 parseMessage를 호출한다.
                //만약 메시지 수신 중, 완료되지 않은 경우(idx 0~44) 갑작스럽게 \r\n이 등장한 경우 corrupted 되었다고 판정, 현재 Flag를 초기화하고 queue를 비운다.
                //또는 수신 중 허용되지 않은 타입이 등장한 경우, Corrupted 처리
                if (message.StartsWith("\r") || message.StartsWith("\n"))
                {
                    if (messageIndex >= 43)
                    {
                        //아무 일도 없었다.
                        if(message.StartsWith("\n"))
                        {
                            //메시지는 종료됨
                            isMessageStarted = false;
                            Debug.WriteLine("Parse Msg : " + messageQueue);
                            ComputeMessage(messageQueue);
                            messageQueue = "";
                            messageIndex = 0;
                        }
                    }
                    else
                    {
                        //Message Index overflow
                        //Message Corrupted!
                        messageQueue = "";
                        isMessageStarted = false;
                        messageIndex = 0;
                        Debug.WriteLine("Message Corrupted");
                    }
                }
                else
                {
                    if(messageIndex > 46)
                    {
                        //Message Index overflow
                        //Message Corrupted!
                        messageQueue = "";
                        isMessageStarted = false;
                        messageIndex = 0;
                        Debug.WriteLine("Message Corrupted");
                    }
                    else
                    {
                        messageQueue += message;
                        messageIndex++;
                    }
                }
            }
            else
            {
                //메시지가 완료된 상태임: y/p/g만 수신되어야 함. 나머지 문자는 무시
                if(message.StartsWith("y") || message.StartsWith("p") || message.StartsWith("g"))
                {
                    isMessageStarted = true;
                    messageQueue += message;
                }
            }
        }

        private void ComputeMessage(string msg)
        {
            var parsedMessage = serialParser.ParseMessage(msg);
            if (parsedMessage != null)
            {
                if (parsedMessage.GetType() == typeof(PedestrianMessage))
                {
                    bool isAlreadyExist = false;
                    var pedestrianMsg = parsedMessage as PedestrianMessage;
                    double mLat = pedestrianMsg.User.Lat;
                    double mLng = pedestrianMsg.User.Lng;
                    foreach (var target in targetlist)
                    {
                        if (target.Uid.Equals(pedestrianMsg.User.Uid))
                        {
                            target.Uid = pedestrianMsg.User.Uid;
                            target.Lat = mLat;
                            target.Lng = mLng;
                            target.dbm = pedestrianMsg.User.dbm;
                            isAlreadyExist = true;
                        }
                    }

                    if (lastUpdate.ContainsKey(pedestrianMsg.User.Uid))
                        lastUpdate[pedestrianMsg.User.Uid] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    else
                        lastUpdate.Add(pedestrianMsg.User.Uid, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

                    if (isAlreadyExist == false)
                    {
                        var target = new Target(pedestrianMsg.User.Uid, pedestrianMsg.User.dbm, pedestrianMsg.User.Lat, pedestrianMsg.User.Lng);
                        targetlist.Add(target);
                    }
                }
                else if (parsedMessage.GetType() == typeof(GyroMessage))
                {
                    var GyroMsg = parsedMessage as GyroMessage;
                    u.Theta = GyroMsg.Yaw;
                }
                else if (parsedMessage.GetType() == typeof(GPSMessage))
                {
                    var GPSMsg = parsedMessage as GPSMessage;
                    u.Lat = GPSMsg.Latitude;
                    u.Lng = GPSMsg.Longitude;
                }

                check_Danger();
                refresh_UI();
            }
            else
            {
                Debug.WriteLine("Message NULL");
            }
        }

        private void changeUI(Target t, User u)
        {
            var distance = LocationManager.GetDistance(t, u);
            var degree = LocationManager.GetBearing(t, u); // 방위상의 각도
            var uiscale = 90;

            Image imageTemp;

            var temp = degree - 90;
            if (temp < 0) { temp = temp + 360; }
            double xydegree = 360 - temp;
            xydegree = xydegree + u.Theta;

            int uiweight;
            if (distance > 1 && distance < 25.0) { uiweight = 60; }
            else if (distance >= 25.0 && distance < 50.0) { uiweight = 90; }
            else if (distance >= 50.0 && distance < 1000.0) { uiweight = 130; }
            else { return; }

            var ui_x = (5 * uiweight) * Math.Cos(LocationManager.Deg2rad(xydegree));
            var ui_y = (2 * uiweight) * Math.Sin(LocationManager.Deg2rad(xydegree));
            if (ui_y > 0)
            {
                uiscale = (int)(uiscale - (ui_y / 6.5));
            }

            bool isAlraeadyExist = false;

            foreach (var img in imagelist)
            {
                if (img.Name.Equals(t.Uid))
                {
                    imageTemp = img;
                    isAlraeadyExist = true;
                    img.Width = uiscale;
                    img.Height = uiscale;
                    img.Margin = new Thickness(ui_x, 0, 0, ui_y);//X축,O,O,Y축 sidemax +-700 bottom max -420 top max 280?  front ~140 25M line side ~350 25m line
                }
            }
            if (isAlraeadyExist == false)
            {
                imagelist.Add(imageTemp = new Image()
                {
                    Source = image,
                    Width = uiscale,
                    Height = uiscale,
                    Margin = new Thickness(ui_x, 0, 0, ui_y),//X축,O,O,Y축 sidemax +-700 bottom max -420 top max 280?  front ~140 25M line side ~350 25m line
                    Name = t.Uid//put UserId

                });
                MainGrid.Children.Add(imageTemp);
                uiElements.Add(t.Uid, imageTemp);
            }

        }//change target mark location and size
        private void refresh_UI()
        {
            if (targetlist.Count > 0)
            {
                foreach (var target in targetlist)
                {
                    if (target != null)
                        changeUI(target, u);
                }
            }
        }
        private void check_Danger()
        {
            bool isDanger = false;
            var temp_targetList = targetlist.ConvertAll(t => new Target(t.Uid, t.dbm, t.Lat, t.Lng, t.theta));
            foreach (Target target in temp_targetList)
            {
                if (target.dbm > 11.0 || target.distance < 13.0)
                {
                    isDanger = true;
                    //if(mediaPlayer.PlaybackSession.CanPause)
                    //    mediaPlayer.Pause();
                    mediaPlayer.PlaybackSession.PlaybackRate = 1.3;
                    if (!(mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing))
                        mediaPlayer.Play();
                } else if (target.dbm > 9.0 || target.distance < 38.0)
                {
                    isDanger = true;
                    //if (mediaPlayer.PlaybackSession.CanPause)
                    //    mediaPlayer.Pause();
                    mediaPlayer.PlaybackSession.PlaybackRate = 1.0;
                    if (!(mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing))
                        mediaPlayer.Play();
                }
            }
            if (isDanger == false)
            {
                if (mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                    mediaPlayer.Pause();
            }
        }

    }
}

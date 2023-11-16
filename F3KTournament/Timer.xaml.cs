using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using F3KTournament.Helpers;
using F3KTournament.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace F3KTournament.Forms
{



    /// <summary>
    /// Interaction logic for Timer.xaml
    /// </summary>
    public partial class Timer : Window
    {
        DispatcherTimer _timer = new DispatcherTimer();
        TimerViewModel vm;
        public Timer(string playListPath)
        {
            vm = new TimerViewModel(playListPath);
            this.DataContext = vm;
            InitializeComponent();

            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(ticktock);

            timeSlider.AddHandler(MouseLeftButtonUpEvent,
                      new MouseButtonEventHandler(timeSlider_MouseLeftButtonUp),
                      true);
            vm.PlayRequested += (sender, e) =>
            {
                this.MediaPlayer.Play();
                timeSlider.IsEnabled = true;
                _timer.Start();
                MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            };

            vm.StopRequested += (sender, e) =>
            {
                timeSlider.IsEnabled = false;
                MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
                this.MediaPlayer.Stop();
                _timer.Stop();

            };

            

            RoundTextBlock.ContextMenu = new ContextMenu();
            GroupTextBlock.ContextMenu = new ContextMenu();

            for (int i = 0; i < vm.MaxRound; i++)
            {
                MenuItem item = new MenuItem();
                item.Header = i + 1;
                item.Click += RoundItem_Click;
                RoundTextBlock.ContextMenu.Items.Add(item);
            }
            UpdateGroupContextMenu();
        }

        private void UpdateGroupContextMenu()
        {
            GroupTextBlock.ContextMenu.Items.Clear();
            for (int i = 0; i < vm.MaxGroup; i++)
            {
                MenuItem item = new MenuItem();
                item.Header = i + 1;
                item.Click += GroupItem_Click;
                GroupTextBlock.ContextMenu.Items.Add(item);
            }
        }

        private void GroupItem_Click(object sender, RoutedEventArgs e)
        {
            var t = sender as MenuItem;
            vm.Group = int.Parse(t.Header.ToString());
        }

        private void RoundItem_Click(object sender, RoutedEventArgs e)
        {
            var t = sender as MenuItem;
            vm.Round = int.Parse(t.Header.ToString());
            UpdateGroupContextMenu();
        }

        void ticktock(object sender, EventArgs e)
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                timeSlider.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;

                var t = MediaPlayer.NaturalDuration.TimeSpan - MediaPlayer.Position;

                time_text.Text = string.Format("{0:D2}:{1:D2}",t.Minutes, t.Seconds);
                timeSlider.Value = MediaPlayer.Position.TotalSeconds;
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
            _timer.Stop();

            if (vm.NextFile())
            {
                this.MediaPlayer.Play();
                
                _timer.Start();
                MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            }


        }

        private void timeSlider_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(MediaPlayer.NaturalDuration.HasTimeSpan && MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                MediaPlayer.Position = TimeSpan.FromSeconds(timeSlider.Value);
            }
        }
    }
}

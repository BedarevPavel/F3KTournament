using F3KTournament.Code;
using F3KTournament.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace F3KTournament.ViewModel
{
    public class TimerViewModel : ViewModelBase
    {
        #region properties
        private int _round;
        public int Round
        {
            get
            {
                return _round;
            }
            set
            {
                _round = value;
                OnPropertyChanged("Round");
            }
        }

        private int _group;
        public int Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
                OnPropertyChanged("Group");
            }
        }

        private string _currentFile;
        public string CurrentFile
        {
            get
            {
                return _currentFile;
            }
            set
            {
                _currentFile = value;
                OnPropertyChanged("CurrentFile");
            }
        }

        private string _timeLeft;
        public string TimeLeft
        {
            get
            {
                return _timeLeft;
            }
            set
            {
                _timeLeft = value;
                OnPropertyChanged("TimeLeft");
            }
        }

        private string _playPauseBtnText;
        public string PlayPauseBtnText
        {
            get
            {
                return _playPauseBtnText;
            }
            set
            {
                _playPauseBtnText = value;
                OnPropertyChanged("PlayPauseBtnText");
            }
        }

        private bool _isPlay;
        public bool IsPlay
        {
            get
            {
                return _isPlay;
            }
            set
            {
                _isPlay = value;
                if (value)
                {
                    PlayPauseBtnText = "Stop";
                }
                else
                {
                    PlayPauseBtnText = "Start";
                }
                OnPropertyChanged("IsPlay");
            }
        }
        #endregion

        PlayListWrap PlayList = new PlayListWrap();
        public event EventHandler PlayRequested;
        public event EventHandler StopRequested;

        public TimerViewModel(string playListPath = "")
        {
            var missingAudioFiles = new List<string>();
            _playPauseBtnText = "Start";
            var audioDir = Common.AppDataAudioPath;

            if (string.IsNullOrWhiteSpace(playListPath))
            {
                playListPath = Path.Combine(audioDir, "playlist.m3u");
            }

            string[] lines = File.ReadAllLines(playListPath);

            int group = 0;
            int round = 0;

            foreach (var line in lines)
            {
                

                if (line.Contains("ROUND") && line.Contains("ROUND"))
                {
                    var pair = line.Split(',');

                    if (pair.Length < 2)
                    {
                        throw new Exception("Ошибка чтения плейлиста (pair.Length < 2)");
                    }

                    var r = pair[0].Split(' ');
                    int.TryParse(r[r.Length - 1], out round);

                    var g = pair[1].Split(' ');
                    int.TryParse(g[g.Length - 1], out group);
                    continue;
                }

                if (line.StartsWith("#")) continue;

                var fPath = Path.Combine(audioDir, line);
                if (File.Exists(fPath))
                {
                    if (round == 0 || group == 0)
                    {
                        throw new Exception("Ошибка чтения плейлиста (round == 0 || group == 0)");
                    }
                    PlayList.AddFile(group, round, fPath);
                }
                else if (fPath.EndsWith(".mp3"))
                {
                    if (!missingAudioFiles.Contains(fPath))
                    {
                        missingAudioFiles.Add(fPath);
                    }
                }
            }

            Group = PlayList.Items.Min(t => t.Group);
            Round = PlayList.Items.Min(t => t.Round);

            if (missingAudioFiles.Count > 0)
            {
                MessageBox.Show($"Не удалось найти следующие файлы из плей листа:\n{String.Join("\n", missingAudioFiles.ToArray()) }", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private PlayListWrap.PlayListItem CurrentGroupRound
        {
            get
            {
                return PlayList.Items.SingleOrDefault(t => t.Group == Group && t.Round == Round);
            }
        }

        public int MaxGroup
        {
            get
            {
               return PlayList.Items.Where(g => g.Round == Round).Max(g => g.Group);
            }
        }

        public int MaxRound
        {
            get
            {
                return PlayList.Items.Max(g => g.Round);
            }
        }

        private bool NextGroup()
        {
            var maxGroupNum = PlayList.Items.Where(g => g.Round == Round).Max(g => g.Group);
            return Group < maxGroupNum;
        }

        private bool NextRound()
        {
            var maxRoundNum = PlayList.Items.Max(g => g.Round);
            return Round < maxRoundNum;
        }

        public bool NextFile()
        {
            if (CurrentFileInx >= CurrentGroupRound.Files.Count)
            {
                CurrentFileInx = 0;
                if (NextGroup())
                {
                    Group++;
                }
                else if (NextRound())
                {
                    Group = 1;
                    Round++;
                }
                else
                {
                    return false;
                }

                CurrentFile = CurrentGroupRound.Files[CurrentFileInx];
                CurrentFileInx++;
            }
            else
            {
                CurrentFile = CurrentGroupRound.Files[CurrentFileInx];
                CurrentFileInx++;
            }

            return true;
        }

        private int CurrentFileInx = 0;
        #region commands
        public void PlayPause()
        {
            //CurrentFile = CurrentGroupRound.Files[CurrentFileInx];
            if (string.IsNullOrEmpty(CurrentFile))
            {
                NextFile();
            }

            if (IsPlay)
            {
                if (this.StopRequested != null)
                {
                    IsPlay = false;
                    this.StopRequested(this, EventArgs.Empty);
                    CurrentFileInx = 0;
                    CurrentFile = string.Empty;
                }
            }
            else
            {
                if (this.PlayRequested != null)
                {
                    IsPlay = true;
                    this.PlayRequested(this, EventArgs.Empty);
                }
            }

        }


        private RelayCommand _playPauseCommand;
        public RelayCommand PlayPauseCommand
        {
            get
            {
                return _playPauseCommand ?? (_playPauseCommand = new RelayCommand(obj => PlayPause()));
            }
        }
        #endregion
    }

    public class PlayListWrap
    {
        public List<PlayListItem> Items = new List<PlayListItem>();

        public void AddFile(int group, int round, string filePath)
        {
            var item = Items.SingleOrDefault(p => p.Group == group && p.Round == round);
            if (item == null)
            {
                item = new PlayListItem(round, group);
                Items.Add(item);
            }

            item.Files.Add(filePath);
        }

        public class PlayListItem
        {
            public List<string> Files = new List<string>();
            public int Round { get; }
            public int Group { get; }

            public PlayListItem(int round, int group)
            {
                Round = round;
                Group = group;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using F3KTournament.ViewModel;
using F3KTournament.DataModel;
using F3KTournament.Helpers;

namespace F3KTournament
{
    [Serializable]
    public class Task : ViewModelBase, ICloneable
    {
        public TasksEnum Index { get; set; }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }
        public string Rules { get; set; }
        private string _description;
        /// <summary>
        /// Описание упражнения
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private int _roud;
        public int Round
        {
            get { return _roud; }
            set
            {
                _roud = value;
                OnPropertyChanged("Round");
            }
        }

        public string TitleEn { get; set; }
        /// <summary>
        /// Описание упражнения
        /// </summary>
        public string DescriptionEn { get; set; }

        private string _shortTitle;
        public string ShortTitle
        {
            get
            {
                return _shortTitle;
            }
            set
            {
                _shortTitle = value;
                OnPropertyChanged("ShortTitle");
            }
        }

        public string ColumnTitle
        {
            get
            {
                return $"({this.Round}) {this.ShortTitle}";
            }
        }

        public List<SubTask> SubTasks { get; set; }

        public List<AudioInfo> Audio { get; set; }

        public int RowCount1 { get; set; }
        public int ColCount1 { get; set; }

        public int RowCount2 { get; set; }
        public int ColCount2 { get; set; }
        public string Name { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class SubTask : ViewModelBase
    {
        public string Title
        {
            get
            {
                return MaxTime > 0 ? $"{Text} (Max {MainHelper.ConvertSecToTimeString(MaxTime.ToString())})" : $"{Text}";
            }
        }

        public int MaxTime { get; set; }
        public string Text { get; set; }
    }

    public class AudioInfo
    {
        public string FileName { get; set; }
    }
}

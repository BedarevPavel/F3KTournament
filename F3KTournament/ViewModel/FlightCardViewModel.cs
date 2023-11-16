namespace F3KTournament.ViewModel
{
    public class FlightCardViewModel : ViewModelBase
    {
        public FlightCardViewModel()
        {

        }

        public string JudgeFIO { get; set; }
        public int JudgeNum { get; set; }
        public int RowCount1 { get; set; }
        public int ColCount1 { get; set; }

        public int RowCount2 { get; set; }
        public int ColCount2 { get; set; }

        public string Pilot { get; set; }

        public int Group { get; set; }

        public int Tour { get; set; }
        public string TourGroup
        {
            get
            {
                return $"{Tour} / {Group}";
            }
        }

        public string TaskText { get; set; }
        private string _taskName;
        public string TaskName
        {
            get
            {
                return _taskName;
            }
            set
            {
                _taskName = value;
                OnPropertyChanged("TaskName");
            }
        }

        public string Index { get; set; }
        public string Rules { get; set; }
    }
}

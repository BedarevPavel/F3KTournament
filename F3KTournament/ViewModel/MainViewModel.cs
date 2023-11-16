using F3KTournament.Code;
using F3KTournament.DataModel;
using F3KTournament.Forms;
using F3KTournament.Helpers;
using F3KTournament.PrintElements;
using F3KTournament.Reports;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace F3KTournament.ViewModel
{
    [Serializable]
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public string CurrendDataFile { get; set; }
        #region TasksTab
        private bool _taskIsUpEnable;
        public bool TaskIsUpEnable
        {
            get
            {
                return _taskIsUpEnable;
            }
            set
            {
                _taskIsUpEnable = value;
                OnPropertyChanged("TaskIsUpEnable");
            }
        }

        private bool _taskIsDownEnable;
        public bool TaskIsDownEnable
        {
            get
            {
                return _taskIsDownEnable;
            }
            set
            {
                _taskIsDownEnable = value;
                OnPropertyChanged("TaskIsDownEnable");
            }
        }

        private bool _taskIsDelEnable;
        public bool TaskIsDelEnable
        {
            get
            {
                return _taskIsDelEnable;
            }
            set
            {
                _taskIsDelEnable = value;
                OnPropertyChanged("TaskIsDelEnable");
            }
        }

        private Task _selectedTaskItem;
        public Task SelectedTaskItem
        {
            get
            {
                return _selectedTaskItem;
            }
            set
            {
                _selectedTaskItem = value;

                var inx = TasksList.IndexOf(_selectedTaskItem);
                TaskIsDownEnable = inx >= 0 && inx < TasksList.Count - 1;
                TaskIsUpEnable = inx > 0;
                TaskIsDelEnable = _selectedTaskItem != null;
                OnPropertyChanged("SelectedTaskItem");
            }
        }
        #endregion
        private List<Tour> _currentMatrix;
        private int _currentRound { get; set; }
        public int CurrentRound
        {
            get { return _currentRound; }
            set
            {
                _currentRound = value;
                OnPropertyChanged("CurrentRound");
            }
        }

        //matrix tab
        private int _groupsCount { get; set; }
        public int GroupsCount
        {
            get { return _groupsCount; }
            set
            {
                _groupsCount = value;
                OnPropertyChanged("GroupsCount");
            }
        }

        //task scores tab
        private Task _selectedTask { get; set; }
        public Task SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }


        private bool _calcIsOutOfCreditPilots = true;
        /// <summary>
        /// Учитывать результаты пилотов вне зачёте
        /// </summary>
        public bool CalcIsOutOfCreditPilots
        {
            get { return _calcIsOutOfCreditPilots; }
            set
            {
                _calcIsOutOfCreditPilots = value;
                OnPropertyChanged("CalcIsOutOfCreditPilots");
                RecalcAllScores(_calcIsOutOfCreditPilots);
            }
        }

        private ObservableCollection<DataTable> _tasksScoresList;
        public ObservableCollection<DataTable> TasksScoresList
        {
            get
            {
                if (_tasksScoresList == null)
                {
                    _tasksScoresList = new ObservableCollection<DataTable>();
                }
                return _tasksScoresList;
            }
            set
            { TasksScoresList = value; }
        }

        private List<TeamScore> TeamScoresData;
        private CollectionViewSource _teamScores;
        public CollectionViewSource TeamScores
        {
            get
            {
                if (_teamScores == null)
                {
                    _teamScores = new CollectionViewSource();

                    //Init table
                    DataTable table = new DataTable("TeamScores");

                    table.Columns.Add(Constants.Columns.Group, typeof(string));
                    //table.Columns.Add(Constants.Columns.Team, typeof(string));
                    DataColumn idColumn = table.Columns.Add(Constants.Columns.Pilot, typeof(string));
                    table.Columns.Add(Constants.Columns.Scores, typeof(string));
                    //table.Columns.Add(Constants.Columns.TeamScores, typeof(string));
                    table.PrimaryKey = new DataColumn[] { idColumn };
                    table.AcceptChanges();
                    _teamScores.Source = table;

                    _teamScores.GroupDescriptions.Add(new PropertyGroupDescription(Constants.Columns.Group));
                    //_teamScores.GroupDescriptions.Add(new PropertyGroupDescription(Constants.Columns.Team));

                }
                return _teamScores;
            }
        }

        private CollectionViewSource _taskScores;
        public CollectionViewSource TaskScores
        {
            get
            {
                if (_taskScores == null)
                {
                    _taskScores = new CollectionViewSource();
                    _taskScores.GroupDescriptions.Add(new PropertyGroupDescription(Constants.Columns.Group));
                }
                return _taskScores;
            }
            set => _taskScores = value;
        }

        private ObservableCollection<Pilot> _pilotList;
        public ObservableCollection<Pilot> PilotList
        {
            get
            {
                if (_pilotList == null)
                {
                    _pilotList = new ObservableCollection<Pilot>();
                }
                return _pilotList;
            }
            set
            { PilotList = value; }
        }

        private ObservableCollection<Judge> _judgeList;
        public ObservableCollection<Judge> JudgeList
        {
            get
            {
                if (_judgeList == null)
                {
                    _judgeList = new ObservableCollection<Judge>();
                }
                return _judgeList;
            }
            set
            { JudgeList = value; }
        }

        private ObservableCollection<Task> _tasksList;
        public ObservableCollection<Task> TasksList
        {
            get
            {
                if (_tasksList == null)
                {
                    _tasksList = new ObservableCollection<Task>();
                }
                return _tasksList;
            }
            set
            { TasksList = value; }
        }

        public List<Task> AllTasksList { get; set; }

        public static readonly DataTable _dt = new DataTable();
        private DataTable _fligthMatrix;
        public DataTable FligthMatrix
        {
            get { return _fligthMatrix; }
            set
            {
                _fligthMatrix = value;
                base.OnPropertyChanged("FligthMatrix");
            }
        }

        private DataTable _totalScores;
        public DataTable TotalScores
        {
            get
            {
                if (_totalScores == null)
                {
                    _totalScores = InitTotalScoresDataTable();
                }
                return _totalScores;
            }
            set
            {
                _totalScores = value;
                base.OnPropertyChanged("TotalScores");
            }
        }

        private List<TaskHeaders> _tasksHeaders;
        public List<TaskHeaders> TasksHeaders
        {
            get
            {
                if (_tasksHeaders == null)
                {
                    _tasksHeaders = new List<TaskHeaders>();
                    _tasksHeaders.Add(new TaskHeaders());
                }
                return _tasksHeaders;
            }
        }

        private List<TaskHeaders> _subTasksHeaders;
        public List<TaskHeaders> SubTasksHeaders
        {
            get
            {
                if (_subTasksHeaders == null)
                {
                    _subTasksHeaders = new List<TaskHeaders>();
                }
                return _subTasksHeaders;
            }
            set
            { SubTasksHeaders = value; }
        }

        #endregion

        #region ctor
        public MainViewModel()
        {
            JudgeList.CollectionChanged += JudgeList_CollectionChanged;
            Application.Current.MainWindow.Closing += MainWindow_Closing;

            CurrentRound = 1;
            GroupsCount = 1;

            var allTasksXmlPath = Path.Combine(Common.AppDataCatalogPath, Constants.AppFiles.AllTasks);
            if (File.Exists(allTasksXmlPath))
            {
                AllTasksList = SavedDataHelper.LoadFromXML<List<Task>>(allTasksXmlPath);
            }
            else
            {
                MessageBox.Show($"Не удалось загрузить список всех заданий - файл не найден {allTasksXmlPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //т.к. список тасков пуст при первом запуске - загружаем из списка всех тасков
            foreach (var task in AllTasksList)
            {
                TasksList.Add(task);
            }

            //PilotList = new ObservableCollection<Pilot>();
            //PilotList = LoadFromXML<ObservableCollection<Pilot>>(Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.Pilots));

            //JudgeList = new ObservableCollection<Judge>();
            //JudgeList = LoadFromXML<ObservableCollection<Judge>>(Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.Judges));

            //TasksList = new ObservableCollection<Task>();
            //LoadTasksLists();

            //TasksScoresList = new ObservableCollection<DataTable>();
            //LoadFromXML<ObservableCollection<DataTable>>(Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.TasksScores)); 

            //SubTasksHeaders = new List<TaskHeaders>();

            FligthMatrix = InitDataTable("FligthMatrix");
            TotalScores = InitTotalScoresDataTable();
            for (int i = 0; i < TasksList.Count; i++)
            {
                //номер тура
                TasksList[i].Round = i + 1;

                TasksScoresListAddTable(TasksList[i]);
                FligthMatrix.Columns.Add(TasksList[i].ColumnTitle, typeof(string));
                TasksHeaders[0].Headers.Add(TasksList[i].ColumnTitle, TasksList[i].ColumnTitle);
            }
            AddColumnsToTotalScores();


            //TasksForAdd = AllTasksList.Where(f => !TasksList.Any(t => t.Index == f.Index)).ToList();
            //RecalcAllScores(CalcIsOutOfCreditPilots);
        }

        /// <summary>
        /// Добавляет номер при создание нового судьи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JudgeList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var newJudge = e.NewItems[0] as Judge;
                if (newJudge != null && newJudge.Number == 0)
                {
                    var maxNumb = this.JudgeList.Max(t => t.Number);
                    if (maxNumb == 0)
                    {
                        maxNumb = 1;
                    }
                    else
                    {
                        maxNumb++;
                    }

                    newJudge.Number = maxNumb;
                }
            }
        }
        #endregion

        #region event handlers

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dRes = MessageBox.Show($"Сохранить данные?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (dRes == MessageBoxResult.Yes)
            {
                SaveData();
            }
            else if (dRes == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }

            //TODO - данные изменились? предложить сохранить
            //CloseProgramm(true);
        }
        /// <summary>
        /// Обновляем результаты текущего тура и общего счёта при изменение значения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScore_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change)
            {
                CalcScores(e.Row);
                UpdateTeamScores();
            }
        }

        private void TotalScores_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Change)
            {
                var totalScoresTable = e.Row.Table;
                totalScoresTable.RowChanged -= TotalScores_RowChanged;
                var columns = e.Row.Table.Columns;

                var allScores = new Dictionary<int, string>();
                int prevForseScoreCol = 0;

                //update total scores for current pilot
                int scoresSum = 0;
                for (int i = 0; i < columns.Count; i++)
                {
                    if (columns[i].Namespace == Constants.Columns.TasksNameSpace)
                    {
                        var val = e.Row[i].ToString();
                        //удаляем скобочки
                        if (val.Contains('('))
                        {
                            val = val.Substring(1, val.Length - 2);
                            prevForseScoreCol = i;
                            e.Row[i] = val;
                        }
                        if (!string.IsNullOrEmpty(val))
                        {
                            allScores.Add(i, val);
                        }
                        int taskScore = 0;
                        int.TryParse(val, out taskScore);
                        scoresSum += taskScore;
                    }
                }

                //calc penalty
                var penalty = e.Row[Constants.Columns.Penalty].ToString();
                if (!string.IsNullOrEmpty(penalty))
                {
                    var pair = penalty.Split('/');
                    var penaltyVal = 0;
                    if (int.TryParse(pair[0], out penaltyVal))
                    {
                        scoresSum = scoresSum - penaltyVal;
                    }
                }

                //вычитаем худший из 5 результатов
                int minScore = 1000;
                int minScoreCol = 0;

                bool substractScore = false;
                if (allScores.Count >= 5)
                {
                    foreach (var score in allScores)
                    {
                        int taskScore = 0;
                        int.TryParse(score.Value, out taskScore);

                        if (taskScore < minScore)
                        {
                            minScore = taskScore;
                            minScoreCol = score.Key;
                            substractScore = true;
                        }
                    }
                }

                if (substractScore)
                {
                    scoresSum = scoresSum - minScore;
                    e.Row[minScoreCol] = $"({minScore})";
                }

                //update sum scores
                e.Row[Constants.Columns.Scores] = scoresSum;

                //update pilots places
                var sortedTable = totalScoresTable.DefaultView.ToTable();
                for (int i = 0; i < sortedTable.Rows.Count; i++)
                {
                    var tsRow = totalScoresTable.Rows.Find(sortedTable.Rows[i][Constants.Columns.Pilot]);
                    tsRow[Constants.Columns.Place] = i + 1;
                }

                totalScoresTable.RowChanged += TotalScores_RowChanged;

            }
        }

        /// <summary>
        /// При изменение списка тасков - обновляем столбцы в полётной матрице
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TasksList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                var tempMatrxi = FligthMatrix;
                var tempTotalScores = TotalScores;
                FligthMatrix = _dt;
                TotalScores = _dt;
                if (e.Action == NotifyCollectionChangedAction.Move)
                {
                    for (int i = 0; i < TasksList.Count; i++)
                    {
                        var matrixCol = tempMatrxi.Columns[TasksList[i].ColumnTitle];
                        var totalScoresCol = tempTotalScores.Columns[TasksList[i].ColumnTitle];
                        totalScoresCol.SetOrdinal(i + 2);
                        //перемещаем столбец в матрице
                        matrixCol.SetOrdinal(i + 2);
                        //обновляем тур сначала в списке заданий, затем меняем имя столбца в матрице
                        TasksList[i].Round = i + 1;
                        matrixCol.ColumnName = TasksList[i].ColumnTitle;
                        totalScoresCol.ColumnName = TasksList[i].ColumnTitle;
                    }

                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var lastRound = TasksList.Max(t => t.Round);
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        Task task = e.NewItems[i] as Task;
                        task.Round = lastRound + (i + 1);
                        //Добавляем столбец в матрицу
                        tempMatrxi.Columns.Add(task.ColumnTitle);

                        //Добавляем таблицу в список результатов
                        TasksScoresListAddTable(task);

                        //Добавляем столбец перед столбцом штрафов в таблицу общих результатов
                        var penaltyCol = tempTotalScores.Columns[Constants.Columns.Penalty];
                        var newCol = tempTotalScores.Columns.Add(task.ColumnTitle, typeof(string));
                        newCol.SetOrdinal(penaltyCol.Ordinal);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        Task task = e.OldItems[i] as Task;
                        //удаляем из таблицы матрицы
                        tempMatrxi.Columns.Remove(task.ColumnTitle);

                        DataTable taskScoresTable = TasksScoresList.SingleOrDefault(t => t.TableName == task.Round.ToString());
                        //удаляем из списка результатов
                        TasksScoresList.Remove(taskScoresTable);

                        //удаляем столбец из общих результатов
                        var colForDel = tempTotalScores.Columns[task.ColumnTitle];
                        tempTotalScores.Columns.RemoveAt(colForDel.Ordinal);

                        //Обновляем порядок столбцов в матрице, в таблице результатов и имена таблиц в списке результатов
                        for (int j = 0; j < TasksList.Count; j++)
                        {
                            var colMatrix = tempMatrxi.Columns[TasksList[j].ColumnTitle];
                            var scoresCol = tempTotalScores.Columns[TasksList[j].ColumnTitle];
                            var scoresTable = TasksScoresList.SingleOrDefault(t => t.TableName == TasksList[j].Round.ToString());
                            TasksList[j].Round = j + 1;
                            colMatrix.ColumnName = TasksList[j].ColumnTitle;
                            scoresCol.ColumnName = TasksList[j].ColumnTitle;
                            scoresTable.TableName = TasksList[j].Round.ToString();
                        }
                    }
                }

                FligthMatrix = tempMatrxi;
                TotalScores = tempTotalScores;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Private metods
        private void AddColumnsToTotalScores()
        {
            for (int i = 0; i < TasksList.Count; i++)
            {
                var taskColumn = TotalScores.Columns.Add(TasksList[i].ColumnTitle.ToString(), typeof(string));
                taskColumn.DefaultValue = string.Empty;
                taskColumn.Namespace = Constants.Columns.TasksNameSpace;
            }

            TaskScores.Source = TasksScoresList[0];

            TotalScores.Columns.Add(Constants.Columns.Penalty, typeof(string));
            var scoresCol = TotalScores.Columns.Add(Constants.Columns.Scores, typeof(int));
            scoresCol.DefaultValue = 0;
            TotalScores.DefaultView.Sort = $"{Constants.Columns.Scores} DESC";

            //в выражение высчитываем процент от максимального результата и округляем до 2х знаков
            TotalScores.Columns.Add(Constants.Columns.Percent, typeof(string),
                $"Convert(IIF({Constants.Columns.Scores} > 0, ({Constants.Columns.Scores}/MAX({Constants.Columns.Scores})) * 100, 0)*100, System.Int64)/100");

            TotalScores.RowChanged += TotalScores_RowChanged;
            TasksList.CollectionChanged += TasksList_CollectionChanged;

        }
        #region save-load data
        //private void LoadTasksLists()
        //{
        //    var allTasksXmlPath = Path.Combine(Common.AppDataCatalogPath, Constants.AppFiles.AllTasks);
        //    if (File.Exists(allTasksXmlPath))
        //    {
        //        AllTasksList = LoadFromXML<List<Task>>(allTasksXmlPath);
        //    }
        //    else
        //    {
        //        MessageBox.Show($"Не удалось загрузить список всех заданий - файл не найден {allTasksXmlPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    var tasksXmlPath = Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.Tasks);
        //    if (File.Exists(tasksXmlPath))
        //    {
        //        TasksList = LoadFromXML<ObservableCollection<Task>>(tasksXmlPath);
        //    }
        //    else
        //    {
        //        TasksList = LoadFromXML<ObservableCollection<Task>>(allTasksXmlPath);
        //    }
        //}


        #endregion
        private void TasksScoresListAddTable(Task task)
        {
            //таблица результатов тура
            DataTable taskScore = InitTaskScoresDataTable(task.Round.ToString());
            taskScore.Namespace = task.Index.ToString();
            //список заголовков подзадач
            TaskHeaders subTaskHeaders = null;
            if (SubTasksHeaders.SingleOrDefault(s => s.TaskIndex == task.Index.ToString()) == null)
            {
                subTaskHeaders = new TaskHeaders { TaskIndex = task.Index.ToString() };
                SubTasksHeaders.Add(subTaskHeaders);
            }

            for (int j = 0; j < task.SubTasks.Count; j++)
            {
                var subTaskCol = $"{Constants.Columns.SubTask}{j + 1}";
                taskScore.Columns.Add(subTaskCol, typeof(string));
                subTaskHeaders?.Headers.Add(subTaskCol, task.SubTasks[j].Title);
            }

            taskScore.Columns.Add(Constants.Columns.TotalTime, typeof(string));
            taskScore.Columns.Add(Constants.Columns.Scores, typeof(int));
            taskScore.Columns.Add(Constants.Columns.Penalty, typeof(string));

            taskScore.RowChanged += TaskScore_RowChanged;

            TasksScoresList.Add(taskScore);
        }
        /// <summary>
        /// Загружаем сохранённые таски и полный список таксов
        /// </summary>
        /// <returns></returns>


        /// <summary>
        /// Пересчёт результатов с учётом/без учёта пилотов вне зачёта
        /// </summary>
        /// <param name="calcIsOutOfChredit"></param>
        private void RecalcAllScores(bool calcIsOutOfChredit)
        {
            foreach (DataTable scoresTable in TasksScoresList)
            {
                foreach (DataRow row in scoresTable.Rows)
                {
                    CalcScores(row);
                }
            }
            UpdateTeamScores();
        }

        private void CalcScores(DataRow row)
        {
            DataTable tasksTable = row.Table;
            tasksTable.RowChanged -= TaskScore_RowChanged;
            try
            {
                var task = TasksList.SingleOrDefault(t => t.Round.ToString() == tasksTable.TableName);

                //update total time
                var totalTime = 0;
                int subTaskNum = 0;
                for (int i = 0; i < tasksTable.Columns.Count; i++)
                {
                    var colName = tasksTable.Columns[i].ColumnName;
                    if (colName.IndexOf(Constants.Columns.SubTask) > -1)
                    {
                        var valStr = row[tasksTable.Columns[i]].ToString();
                        int val = 0;
                        var parseRes = int.TryParse(valStr, out val);

                        var subTask = task.SubTasks[subTaskNum];
                        subTaskNum++;

                        //проверяем максимальное возможное значение для подзадачи
                        if (val >= subTask.MaxTime && subTask.MaxTime != 0)
                        {
                            val = subTask.MaxTime;
                            row[tasksTable.Columns[i]] = subTask.MaxTime;
                        }

                        totalTime += val;

                        //fix bug - если в данных не число (случайное попадание левых символов)
                        if (valStr.Length > 0 && !parseRes)
                        {
                            row[tasksTable.Columns[i]] = string.Empty;
                        }
                    }
                }

                //update current row total time
                row[Constants.Columns.TotalTime] = totalTime;

                var group = row[Constants.Columns.Group].ToString();
                double max = 0;

                //find max total time
                foreach (DataRow item in tasksTable.Rows)
                {
                    if (item[Constants.Columns.Group].ToString() == group)
                    {
                        if (!IsOutOfCreditPilot(item[Constants.Columns.Pilot].ToString()) || CalcIsOutOfCreditPilots)
                        {
                            var itemTotalTime = 0;
                            int.TryParse(item[Constants.Columns.TotalTime].ToString(), out itemTotalTime);
                            if (max < itemTotalTime)
                            {
                                max = itemTotalTime;
                            }
                        }

                    }
                }

                //update scores
                foreach (DataRow item in tasksTable.Rows)
                {
                    if (item[Constants.Columns.Group].ToString() == group)
                    {
                        var tsRow = TotalScores.Rows.Find(item[Constants.Columns.Pilot]);

                        var taskCol = task.ColumnTitle;

                        if (!IsOutOfCreditPilot(item[Constants.Columns.Pilot].ToString()) || CalcIsOutOfCreditPilots)
                        {
                            var itemTotalTime = 0;
                            int.TryParse(item[Constants.Columns.TotalTime].ToString(), out itemTotalTime);
                            int scores = 0;
                            if (max > 0)
                            {
                                scores = (int)Math.Round((itemTotalTime / max) * 1000, 0);
                            }
                            item[Constants.Columns.Scores] = scores;

                            //update total scores table
                            var totalScoresvVal = 0;
                            int.TryParse(tsRow[taskCol].ToString(), out totalScoresvVal);
                            if (totalScoresvVal != scores || (totalScoresvVal == 0 && scores == 0 && task.Round == CurrentRound))
                            {
                                tsRow[taskCol] = scores;
                            }
                        }
                        else
                        {
                            item[Constants.Columns.Scores] = 0;
                            tsRow[taskCol] = string.Empty;
                        }
                    }
                }

                //update penalty
                var totalScoresRow = TotalScores.Rows.Find(row[Constants.Columns.Pilot]);
                var penaltyList = new List<string>();

                foreach (DataTable taskScoresTable in TasksScoresList)
                {
                    var currentTask = TasksList.SingleOrDefault(t => t.Round.ToString() == taskScoresTable.TableName);
                    var round = currentTask.Round;

                    foreach (DataRow item in taskScoresTable.Rows)
                    {
                        if (item[Constants.Columns.Pilot].ToString() != totalScoresRow[Constants.Columns.Pilot].ToString()) continue;

                        var penalty = item[Constants.Columns.Penalty].ToString();
                        penaltyList.Add($"{penalty}/{round}");
                    }
                }

                int penaltySum = 0;
                List<string> penaltyRoundsList = new List<string>();
                foreach (var penaltyPair in penaltyList)
                {
                    //calc all penalty
                    var val = 0;
                    var pair = penaltyPair.Split('/');
                    int.TryParse(pair[0], out val);

                    if (val != 0)
                    {
                        penaltySum += val;
                        if (penaltyRoundsList.IndexOf(pair[1]) == -1)
                        {
                            penaltyRoundsList.Add(pair[1]);
                        }
                    }

                }

                if (penaltyRoundsList.Count > 0 && penaltySum > 0)
                {
                    var penaltyRounds = string.Join(",", penaltyRoundsList);
                    totalScoresRow[Constants.Columns.Penalty] = $"{penaltySum}/{penaltyRounds}";
                }
                else
                {
                    totalScoresRow[Constants.Columns.Penalty] = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                tasksTable.RowChanged += TaskScore_RowChanged;
            }
        }

        private void UpdateTeamScores()
        {
            var teamGrouped = PilotList.GroupBy(t => t.Team).ToList();

            var dt = TeamScores.Source as DataTable;
            dt.Rows.Clear();
            TeamScoresData = new List<TeamScore>();
            foreach (var team in teamGrouped)
            {
                if (team.Key == null)
                {
                    continue;
                }

                var teamScore = new TeamScore(team.Key);

                foreach (var pilot in team.ToList())
                {
                    //get pilot scores
                    var scoreRow = TotalScores.Rows.Find(pilot.FullName);
                    var scoreVal = scoreRow[Constants.Columns.Scores].ToString();
                    int score = 0;
                    int.TryParse(scoreVal, out score);
                    teamScore.Pilots.Add(new PilotScore { TotalScores = score, Pilot = pilot });
                }

                //find 3 best pilots
                var orderedPilots = teamScore.Pilots.OrderByDescending(s => s.TotalScores).ToArray();
                teamScore.Pilots.Clear();
                for (int i = 0; i < 3; i++)
                {
                    if (orderedPilots.Length > i)
                    {
                        teamScore.TotalScores += orderedPilots[i].TotalScores;

                        teamScore.Pilots.Add(orderedPilots[i]);
                    }
                }

                TeamScoresData.Add(teamScore);
            }

            TeamScoresData = TeamScoresData.OrderByDescending(t => t.TotalScores).ToList();

            for (int i = 0; i < TeamScoresData.Count; i++)
            {
                TeamScoresData[i].Place = i + 1;
                var rowVals = new object[3];
                rowVals[0] = $"{i + 1} - {TeamScoresData[i].Team} ({TeamScoresData[i].TotalScores})";
                rowVals[1] = TeamScoresData[i].Pilots[0].Pilot.FullName;
                rowVals[2] = TeamScoresData[i].Pilots[0].TotalScores;
                //rowVals[3] = TeamScoresData[i].TotalScores;
                dt.Rows.Add(rowVals);

                for (int j = 1; j < TeamScoresData[i].Pilots.Count; j++)
                {
                    rowVals[0] = $"{i + 1} - {TeamScoresData[i].Team} ({TeamScoresData[i].TotalScores})";
                    rowVals[1] = TeamScoresData[i].Pilots[j].Pilot.FullName;
                    rowVals[2] = TeamScoresData[i].Pilots[j].TotalScores;
                    //rowVals[3] = string.Empty;
                    dt.Rows.Add(rowVals);
                }
            }

            TeamScores.Source = dt;
        }

        /// <summary>
        /// Проверяем находится ли пилот в зачёте
        /// </summary>
        /// <param name="pilotFIO"></param>
        /// <returns></returns>
        private bool IsOutOfCreditPilot(string pilotFIO)
        {
            bool res = false;
            Pilot pilot = PilotList.SingleOrDefault(p => p.FullName == pilotFIO);
            if (pilot != null)
            {
                return pilot.OutOfCredit;
            }

            return res;
        }

        private DataTable InitDataTable(string tableName)
        {
            DataTable table = new DataTable(tableName);
            table.Columns.Add(Constants.Columns.Num, typeof(int));
            DataColumn idColumn = table.Columns.Add(Constants.Columns.Pilot, typeof(string));
            table.PrimaryKey = new DataColumn[] { idColumn };
            table.AcceptChanges();

            return table;
        }

        private DataTable InitTotalScoresDataTable()
        {
            DataTable table = new DataTable("TotalScores");

            var placeCol = table.Columns.Add(Constants.Columns.Place, typeof(int));
            DataColumn idColumn = table.Columns.Add(Constants.Columns.Pilot, typeof(string));
            table.PrimaryKey = new DataColumn[] { idColumn };
            table.AcceptChanges();

            return table;
        }

        private DataTable InitTaskScoresDataTable(string name)
        {
            DataTable table = new DataTable(name);

            DataColumn idColumn = table.Columns.Add(Constants.Columns.Pilot, typeof(string));
            table.PrimaryKey = new DataColumn[] { idColumn };
            table.Columns.Add(Constants.Columns.Group, typeof(string));
            table.Columns.Add(Constants.Columns.Judge, typeof(string));
            table.AcceptChanges();

            return table;
        }

        #region export/import
        private void ExportDataTableToXLSX(string sheetName, DataTable table)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = sheetName;
            saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                var fi = new FileInfo(saveFileDialog.FileName);
                using (ExcelPackage pck = new ExcelPackage(fi))
                {
                    if (pck.Workbook.Worksheets[sheetName] != null)
                    {
                        pck.Workbook.Worksheets.Delete(pck.Workbook.Worksheets[sheetName]);
                    }
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);
                    ws.Cells["A1"].LoadFromDataTable(table, true);
                    pck.Save();
                }
            }
        }
        private void ImportFlightMatrix()
        {
            var res = MessageBox.Show("Импортировать только ранее экспортированный файл из данной программы!\nВ импортируемом файле должны соответсвовать списки пилотов и заданий, указанные в программе!\nПосле импорта полётная матрица и результаты туров затрутся!\nПродолжить выполнение?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel (*.xlsx)|*.xlsx";
                if (openFileDialog.ShowDialog() == true)
                {
                    DataTable importMatrix = InitDataTable("FligthMatrix");
                    var filePath = openFileDialog.FileName;
                    if (Path.GetExtension(filePath) != ".xlsx")
                    {
                        MessageBox.Show("Неверный формат файла!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        using (var pck = new ExcelPackage())
                        {
                            try
                            {
                                using (var stream = File.OpenRead(filePath))
                                {
                                    pck.Load(stream);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка открытия файла, возмонжно файл открыт в другой программе: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            try
                            {
                                TasksList.Clear();

                                //Читаем файл
                                var ws = pck.Workbook.Worksheets.First();
                                Dictionary<int, string> taskCols = new Dictionary<int, string>();
                                int roundNum = 1;

                                TasksList.CollectionChanged -= TasksList_CollectionChanged;
                                //Определяем список тасков
                                for (int i = 1; i <= ws.Dimension.Columns; i++)
                                {
                                    var taskShortName = ws.Cells[1, i].Value?.ToString();
                                    if (!string.IsNullOrEmpty(taskShortName) && taskShortName != Constants.Columns.Num && taskShortName != Constants.Columns.Pilot)
                                    {
                                        var inx = taskShortName.IndexOf(')');
                                        taskShortName = taskShortName.Substring(inx + 1).Trim();

                                        var task = AllTasksList.SingleOrDefault(t => t.ShortTitle == taskShortName);
                                        if (task != null)
                                        {
                                            task.Round = roundNum;
                                            TasksList.Add(task);
                                            roundNum++;
                                            //сохраняем номера столбцов и соответсвующие имена тасков
                                            taskCols.Add(i, task.ColumnTitle);
                                            //Добавляем столбцы в таблице
                                            importMatrix.Columns.Add(task.ColumnTitle, typeof(string));
                                        }
                                    }
                                }
                                TasksList.CollectionChanged += TasksList_CollectionChanged;

                                int maxGroup = 0;
                                for (int j = 2; j <= ws.Dimension.Rows; j++)
                                {
                                    var pilotFullName = ws.Cells[j, 2].Value?.ToString();
                                    if (!string.IsNullOrEmpty(pilotFullName))
                                    {
                                        DataRow row = importMatrix.NewRow();
                                        row[Constants.Columns.Num] = j - 1;
                                        row[Constants.Columns.Pilot] = pilotFullName;
                                        for (int i = 3; i <= ws.Dimension.Columns; i++)
                                        {
                                            var valStr = ws.Cells[j, i].Value?.ToString();
                                            int val = 0;
                                            int.TryParse(valStr, out val);
                                            row[taskCols[i]] = val;
                                            if (val > maxGroup)
                                            {
                                                maxGroup = val;
                                            }
                                        }

                                        importMatrix.Rows.Add(row);
                                    }
                                }

                                this.GroupsCount = maxGroup;
                                GenerateFligthMatrix(importMatrix);
                                MessageBox.Show($"Полётная матрица успешно импортирована", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка импорта данных: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }

                }
            }
        }

        private void ExportFlightMatrix()
        {
            ExportDataTableToXLSX("Полётная матрица", FligthMatrix);
        }

        private void ExportTotalScoresMatrix()
        {
            var orderedDt = TotalScores.AsEnumerable().OrderBy(c => c[Constants.Columns.Place]).CopyToDataTable();
            ExportDataTableToXLSX("Общий счёт", orderedDt);
        }


        private void ImportPilotList()
        {
            var execute = false;
            if (FligthMatrix.Rows.Count > 0)
            {
                var res = MessageBox.Show("После импорта полётная матрица и результаты туров затрутся! Продолжить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    FligthMatrix.Clear();
                    execute = true;
                }
            }
            else
            {
                execute = true;
            }

            if (execute)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel (*.xlsx)|*.xlsx";
                if (openFileDialog.ShowDialog() == true)
                {
                    var filePath = openFileDialog.FileName;
                    if (Path.GetExtension(filePath) != ".xlsx")
                    {
                        MessageBox.Show("Неверный формат файла!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        using (var pck = new ExcelPackage())
                        {
                            try
                            {
                                using (var stream = File.OpenRead(filePath))
                                {
                                    pck.Load(stream);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка открытия файла, возмонжно файл открыт в другой программе: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            //Очищаем существующие таблицы - матрица, результаты туров
                            PilotList.Clear();
                            FligthMatrix.Clear();
                            TotalScores.Clear();
                            foreach (DataTable taskScores in TasksScoresList)
                            {
                                taskScores.Clear();
                            }

                            //читаем список пилотов
                            var ws = pck.Workbook.Worksheets.First();
                            for (int i = 0; i < ws.Dimension.Rows; i++)
                            {
                                if (string.IsNullOrEmpty(ws.Cells[i + 2, 1].Value?.ToString()))
                                {
                                    continue;
                                }
                                var pilot = new Pilot();
                                pilot.Surname = ws.Cells[i + 2, 1].Value?.ToString();
                                pilot.Name = ws.Cells[i + 2, 2].Value?.ToString();
                                pilot.MiddleName = ws.Cells[i + 2, 3].Value?.ToString();
                                pilot.Rank = ws.Cells[i + 2, 4].Value?.ToString();
                                pilot.Team = ws.Cells[i + 2, 5].Value?.ToString();
                                pilot.LicenseFAI = ws.Cells[i + 2, 6].Value?.ToString();
                                pilot.OutOfCredit = ws.Cells[i + 2, 1].Value?.ToString() == "1" ? true : false;
                                PilotList.Add(pilot);
                            }
                        }
                    }
                }
            }
        }

        private void ExportPilotList()
        {
            var pilotDt = new DataTable();

            pilotDt.Columns.Add("Фамилия");
            pilotDt.Columns.Add("Имя");
            pilotDt.Columns.Add("Отчество");
            pilotDt.Columns.Add("Разряд");
            pilotDt.Columns.Add("Команда");
            pilotDt.Columns.Add("Лицензия FAI");
            pilotDt.Columns.Add("Вне зачёта");

            foreach (var pilot in PilotList)
            {
                string[] values = new string[7];
                values[0] = pilot.Surname;
                values[1] = pilot.Name;
                values[2] = pilot.MiddleName;
                values[3] = pilot.Rank;
                values[4] = pilot.Team;
                values[5] = pilot.LicenseFAI;
                values[6] = pilot.OutOfCredit ? "1" : "0";
                pilotDt.Rows.Add(values);
            }

            ExportDataTableToXLSX("Список пилотов", pilotDt);
        }

        #endregion
        private void ReportJudges()
        {
            if (_currentMatrix == null)
            {
                MessageBox.Show("Полётная матрица не сгенерирована!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add(Constants.Columns.Pilot, typeof(string));
            table.PrimaryKey = new DataColumn[] { idColumn };

            foreach (var task in TasksList)
            {
                table.Columns.Add(task.ColumnTitle, typeof(int));
            }

            table.AcceptChanges();

            foreach (var pilot in PilotList)
            {
                object[] values = new object[TasksList.Count + 1];
                values[0] = pilot.FullName;
                int i = 1;
                foreach (Task task in TasksList)
                {
                    Group group = _currentMatrix[task.Round - 1].Groups.SingleOrDefault(g => g.Pilots.SingleOrDefault(p => p.FullName == pilot.FullName) != null);
                    int judgeNum = group.JudgesDict[pilot];
                    values[i] = judgeNum;
                    i++;
                }
                table.Rows.Add(values);
            }

            var reportWindow = new JudgeReport(table);

            reportWindow.ShowDialog();

        }

        #region print
        private void PrintFlightMatrix()
        {
            PrintDataTabe pt = new PrintDataTabe(FligthMatrix, "Полётная матрица - печать");
            pt.ShowDialog();
            //if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
            //{
            //Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
            //// sizing of the element.
            //dataGrid.Measure(pageSize);
            //dataGrid.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
            //Printdlg.PrintVisual(dataGrid, "Полётная матрица");
            //}
        }

        private void PrintTaskScores()
        {
            if (TasksScoresList.Count > 0 && TasksScoresList[0].Rows.Count == 0)
            {
                MessageBox.Show("Список результатов туров не инициализирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PrintTaskScores pt = new PrintTaskScores(TasksScoresList, CurrentRound, TasksList);
            pt.Show();
        }

        private void PrintTotalScores()
        {
            if (TotalScores.Rows.Count == 0)
            {
                MessageBox.Show("Список итоговых результатов не инициализирован!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var orderedDt = TotalScores.AsEnumerable().OrderBy(c => c[Constants.Columns.Place]).CopyToDataTable();
            PrintDataTabe pt = new PrintDataTabe(orderedDt, "Общий счёт - печать", TasksHeaders[0], TasksList.Count > 10);
            pt.Show();
        }

        private void PrintTeamScores()
        {

            var pt = new PrintTeamScores(TeamScoresData);
            pt.Show();
        }

        //Печать полётных карточек
        private void PrintFlightLists()
        {
            if (FligthMatrix.Rows.Count == 0)
            {
                MessageBox.Show("Полётная матрица не сгенерирована!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var fSettings = new FlightListSettings(TasksList, PilotList, FligthMatrix, TasksScoresList, JudgeList);

            fSettings.ShowDialog();
        }
        #endregion

        #region task tab commands
        private void EditTask()
        {
            var selectedTask = AllTasksList.SingleOrDefault(t => t.Index == SelectedTaskItem.Index);
            var taskForm = new TaskForm() { DataContext = selectedTask };
            taskForm.ShowDialog();
        }
        private void TaskUp()
        {
            if (SelectedTaskItem != null)
            {
                var indx = TasksList.IndexOf(SelectedTaskItem);
                if (indx > 0)
                {
                    //если два одинаковых задания оказываются рядом
                    if (TasksList[indx].Title != TasksList[indx - 1].Title)
                    {
                        MoveTaskToPosition(indx, indx - 1);
                        TaskIsUpEnable = indx - 1 >= 1;
                        TaskIsDownEnable = indx - 1 < TasksList.Count - 1;
                    }
                    else
                    {
                        SelectedTaskItem = TasksList[indx - 1];
                    }
                }
            }
        }

        private void MoveTaskToPosition(int oldIndex, int newIndex)
        {
            if (TasksList[oldIndex].Title != TasksList[newIndex].Title)
            {
                TasksList.Move(oldIndex, newIndex);
                TasksScoresList.Move(oldIndex, newIndex);

                // Переименовываем таблицы в списке согласно индексу
                for (var i = 0; i < TasksScoresList.Count; i++)
                {
                    TasksScoresList[i].TableName = (i + 1).ToString();
                }
            }
        }

        private void TaskDown()
        {
            if (SelectedTaskItem != null)
            {
                var indx = TasksList.IndexOf(SelectedTaskItem);
                if (indx < TasksList.Count - 1)
                {
                    //если два одинаковых задания оказываются рядом
                    if (TasksList[indx].Title != TasksList[indx + 1].Title)
                    {
                        MoveTaskToPosition(indx, indx + 1);
                        TaskIsDownEnable = indx + 1 < TasksList.Count - 1;
                        TaskIsUpEnable = indx + 1 >= 1;
                    }
                    else
                    {
                        SelectedTaskItem = TasksList[indx + 1];
                    }
                }
            }
        }
        private void TaskDel()
        {
            if (SelectedTaskItem == null)
            {
                MessageBox.Show($"Задание для удаления не выбрано!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"Удалить задание '{SelectedTaskItem.Index}'-'{SelectedTaskItem.Title}' ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                TasksList.Remove(SelectedTaskItem);
                //TasksForAdd = AllTasksList.Where(f => !TasksList.Any(t => t.Index == f.Index)).ToList();
            }
        }
        private void TaskAdd()
        {
            var addTaskForm = new AddTaskForm(AllTasksList);
            if (addTaskForm.ShowDialog().Value)
            {
                foreach (var task in addTaskForm.SelectedTasks)
                {
                    var newTask = task.Clone() as Task;
                    newTask.Round = 0;
                    TasksList.Add(newTask);

                    if (SelectedTaskItem != null)
                    {
                        var oldIndx = TasksList.IndexOf(newTask);
                        var newIndx = TasksList.IndexOf(SelectedTaskItem) + 1;

                        if (oldIndx != newIndx)
                        {
                            MoveTaskToPosition(oldIndx, newIndx);
                        }

                        SelectedTaskItem = newTask;
                    }
                }

                //TasksForAdd = AllTasksList.Where(f => !TasksList.Any(t => t.Index == f.Index)).ToList();
            }
        }
        #endregion
        #region file menu commands
        private void OpenData()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "XML (*.xml)|*.xml";
            if (fd.ShowDialog() == true)
            {
                if (File.Exists(fd.FileName))
                {
                    var data = SavedDataHelper.Load(fd.FileName);
                    bool res = false;
                    try
                    {
                        // проверяем имена таблиц в списке результатов (индекс должен быть равен имени таблицы - 1)
                        // для поддержки сохранённых данных до исправления бага
                        for (var t = 0; t < data.TasksScoresList.Count; t++)
                        {
                            data.TasksScoresList[t].TableName = (t + 1).ToString();
                        }

                        _tasksList = new ObservableCollection<Task>(data.TasksList);
                        OnPropertyChanged("TasksList");
                        _judgeList = new ObservableCollection<Judge>(data.JudgeList);
                        OnPropertyChanged("JudgeList");
                        _pilotList = new ObservableCollection<Pilot>(data.PilotList);
                        OnPropertyChanged("PilotList");
                        _tasksScoresList = new ObservableCollection<DataTable>(data.TasksScoresList);
                        OnPropertyChanged("TasksScoresList");

                        foreach (var taskScore in _tasksScoresList)
                        {
                            taskScore.RowChanged += TaskScore_RowChanged;
                        }

                        GroupsCount = data.GroupsCount;

                        _fligthMatrix = data.FligthMatrix;
                        _currentMatrix = MainHelper.GenMatrixList(PilotList, JudgeList, TasksList, GroupsCount, 1, data.FligthMatrix);
                        OnPropertyChanged("FligthMatrix");

                        //if (data.TotalScores != null && data.TotalScores.Columns.Count == 0)
                        //{
                        //    data.TotalScores = InitTotalScoresDataTable();
                        //}
                        //_totalScores = data.TotalScores;
                        TotalScores = InitTotalScoresDataTable();
                        int i = 1;
                        foreach (var pilot in PilotList)
                        {
                            TotalScores.Rows.Add(i, pilot.FullName);
                            i++;
                        }

                        AddColumnsToTotalScores();
                        OnPropertyChanged("TotalScores");

                        CurrentRound = data.CurrentRound;
                        UpdateRoundScoresCollection();

                        _calcIsOutOfCreditPilots = data.CalcIsOutOfCreditPilots;
                        OnPropertyChanged("CalcIsOutOfCreditPilots");

                        RecalcAllScores(_calcIsOutOfCreditPilots);

                        CurrendDataFile = fd.FileName;
                        res = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки данных:\n{ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    if (res)
                    {
                        MessageBox.Show($"Данные успешно загружены", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"Файл не существует или к нему нет доступа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SaveData()
        {
            var saveDataHelper = new SavedDataHelper(this);
            if (saveDataHelper.Save())
            {
                MessageBox.Show($"Данные успешно сохранены", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            //var tasksXmlPath = Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.Tasks);
            //SaveToXML(tasksXmlPath, TasksList);

            //var allTasksXmlPath = Path.Combine(Common.AppDataCatalogPath, Constants.AppFiles.AllTasks);
            //SaveToXML(allTasksXmlPath, AllTasksList);

            //var pilotsXmlPath = Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.Pilots);
            //SaveToXML(pilotsXmlPath, PilotList);

            //var judgeXmlPath = Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.Judges);
            //SaveToXML(judgeXmlPath, JudgeList);

            //var taskScoresXmlPath = Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.TasksScores);
            //SaveToXML(taskScoresXmlPath, TasksScoresList);

            //var tolalTaskScoresXmlPath = Path.Combine(Common.AppDataSavedPath, Constants.AppFiles.TasksScores);
            //SaveToXML(tolalTaskScoresXmlPath, TotalScores);
        }
        private void SaveAsData()
        {
            var savedData = new SavedDataHelper(this);
            if (savedData.SaveAs())
            {
                MessageBox.Show($"Данные успешно сохранены", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void CloseProgramm(bool fromEventHandler = false)
        {
            var dRes = MessageBox.Show($"Сохранить данные?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (dRes == MessageBoxResult.Yes)
            {
                SaveData();
            }
            if (!fromEventHandler)
            {
                Application.Current.MainWindow.Close();
            }
        }
        #endregion
        private void RoundUp()
        {
            if (CurrentRound < TasksList.Count)
            {
                CurrentRound++;
                UpdateRoundScoresCollection();
            }
        }
        private void RoundDown()
        {
            if (CurrentRound > 1)
            {
                CurrentRound--;
                UpdateRoundScoresCollection();
            }
        }

        private void RunTimer()
        {
            if (!MainHelper.IsWindowOpen<Window>("Timer"))
            {
                if (GroupsCount > 4 || TasksList.Count > 16)
                {
                    MessageBox.Show("Для озвучки доступно максимум 4 группы и 16 туров");
                    return;
                }

                GeneratePlayList();
                var p = Path.Combine(Common.AppDataAudioPath, "playlist.m3u");
                var t = new Timer(p);
                t.Show();
            }

        }

        private Dictionary<int, string> RoundAudioFiles = new Dictionary<int, string>
        {
            { 1, "round_1.mp3" },
            { 2, "round_2.mp3" },
            { 3, "round_3.mp3" },
            { 4, "round_4.mp3" },
            { 5, "round_5.mp3" },
            { 6, "round_6.mp3" },
            { 7, "round_7.mp3" },
            { 8, "round_8.mp3" },
            { 9, "round_9.mp3" },
            { 10, "round_10.mp3" },
            { 11, "round_11.mp3" },
            { 12, "round_12.mp3" },
            { 13, "round_13.mp3" },
            { 14, "round_14.mp3" },
            { 15, "round_15.mp3" },
            { 16, "round_16.mp3" }
        };
        private Dictionary<int, string> GroupAudioFiles = new Dictionary<int, string>
        {
            { 1, "heat_A.mp3" },
            { 2, "heat_B.mp3" },
            { 3, "heat_C.mp3" },
            { 4, "heat_D.mp3" },
        };

        private void GeneratePlayList()
        {
            //create file
            var path = Path.Combine(Common.AppDataAudioPath, "playlist.m3u");
            if (File.Exists(path))
            {
                File.Delete(path);
            }


            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i < TasksList.Count; i++)
                {
                    for (int j = 0; j < GroupsCount; j++)
                    {
                        sw.WriteLine("#");
                        sw.WriteLine($"# ROUND {i + 1}, GROUP {j + 1}");
                        sw.WriteLine("#");
                        sw.WriteLine(RoundAudioFiles[i + 1]);
                        sw.WriteLine(GroupAudioFiles[j + 1]);

                        var task = AllTasksList.SingleOrDefault(t => t.Index == TasksList[i].Index);

                        foreach (var ai in task?.Audio)
                        {
                            sw.WriteLine(ai.FileName);
                        }

                        sw.WriteLine("#");
                    }
                }
            }


        }

        /// <summary>
        /// Генерирует полётную матрицу, если передан параметр - восстанавливает матрицу из таблицы
        /// </summary>
        /// <param name="importMatrix">Таблица для импорта</param>
        private void GenerateFligthMatrix(DataTable importMatrix = null)
        {
            var groupSize = (PilotList.Count / GroupsCount);

            if (GroupsCount > 1 && PilotList.Count % GroupsCount != 0)
            {
                groupSize++;
            }

            // добавляем судей, если не хватате
            if (groupSize > JudgeList.Count)
            {
                var j = JudgeList.Count;
                for (var t = j; t < groupSize; t++)
                {
                    var num = JudgeList.Count + 1;
                    JudgeList.Add(new Judge
                    {
                        Number = num,
                        FIO = $"Судья №{num}"
                    });
                }

                //var res = MessageBox.Show($"Количество судей({JudgeList.Count}) меньше размера группы({groupSize})", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //return;
            }

            var traveledTourInx = Common.GetTraveledTourIndex(TotalScores);

            List<Tour> matrix = null;
            // если генерится матрица в процессе соревнований
            // определяем индекс пройденого тура, и генерим матрицу с этого упражнения
            if (traveledTourInx > 0 && importMatrix == null)
            {
                var d = MessageBox.Show(
                    $"Результаты туров не пусты,\nбудет сгенерирована новая матрица " +
                    $"с {traveledTourInx} тура\n\nПродолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (d != MessageBoxResult.Yes)
                {
                    return;
                }

                var matrixTail = MainHelper.GenMatrixList(PilotList, JudgeList, TasksList, GroupsCount, traveledTourInx, importMatrix);

                // добавляем с текущей матрицы первые туры
                for (var i = traveledTourInx - 1; i > 0; i--)
                {
                    var traveledTour = _currentMatrix[i - 1];
                    matrixTail.Insert(0, traveledTour);
                }

                matrix = matrixTail;

                foreach (var pilot in PilotList)
                {
                    foreach (Task task in TasksList)
                    {
                        Group group = matrix[task.Round - 1].Groups.SingleOrDefault(g =>
                                g.Pilots.SingleOrDefault(p => p.FullName == pilot.FullName) != null);

                        // если группа не найдена - добавляем пилота в менее загруженную
                        if (group == null)
                        {
                            var smallGroup = matrix[task.Round - 1].Groups
                                .Aggregate((curMin, x) => (curMin == null || x.Pilots.Count < curMin.Pilots.Count ? x : curMin));
                            smallGroup.Pilots.Add(pilot);
                            group = smallGroup;

                            // добавляем в вьюху матрицы
                            if (FligthMatrix.Rows.Find(pilot.FullName) == null)
                            {
                                DataRow row = FligthMatrix.NewRow();
                                row[Constants.Columns.Num] = PilotList.Count;
                                row[Constants.Columns.Pilot] = pilot.FullName;
                                FligthMatrix.Rows.Add(row);
                            }
                        }

                        // обновляем список результатов туров
                        DataTable taskScores = TasksScoresList.SingleOrDefault(t => t.TableName == task.Round.ToString());
                        var ts = taskScores.Rows.Find(pilot.FullName);

                        taskScores.RowChanged -= TaskScore_RowChanged;

                        if (ts == null)
                        {

                            ts = taskScores.Rows.Add(pilot.FullName);
                            //ts[Constants.Columns.Pilot] = pilot.FullName;
                            ts[Constants.Columns.Group] = group.Number;

                            // выбираем судью который не занят, если все заняту - добавляем нового (с предупреждением)
                            var freeJudge = JudgeList.FirstOrDefault(t => !group.JudgesDict.ContainsValue(t.Number));

                            if (freeJudge == null && JudgeList.Count != group.JudgesDict.Count)
                            {
                                MessageBox.Show($"Для пилота '{pilot.FullName}' нет судьи," +
                                     $"\nбудет добавлен новый судья!", "Внимание", MessageBoxButton.OK);
                                var judgeNum = JudgeList.Max(t => t.Number) + 1;
                                freeJudge = new Judge
                                {
                                    FIO = $"Судья №{judgeNum}",
                                    Number = judgeNum
                                };

                                JudgeList.Add(freeJudge);
                            }

                            var freeJudgeNum = 0;
                            if (!group.JudgesDict.ContainsKey(pilot))
                            {
                                group.JudgesDict.Add(pilot, freeJudge.Number);
                                freeJudgeNum = freeJudge.Number;
                            }else
                            {
                                group.JudgesDict.TryGetValue(pilot, out freeJudgeNum);
                            }

                            ts[Constants.Columns.Judge] = freeJudgeNum;

                            for (int j = 0; j < task.SubTasks.Count; j++)
                            {
                                var subTaskCol = $"{Constants.Columns.SubTask}{j + 1}";
                                ts[subTaskCol] = "0";
                            }
                        }

                        // устанавливаем номер группы в таблице результатов
                        ts[Constants.Columns.Group] = group.Number.ToString();
                        ts.AcceptChanges();

                        taskScores.RowChanged += TaskScore_RowChanged;

                        //UpdateRoundScoresCollection();

                        // обновляем вьюху матрицы
                        foreach (DataRow dataRow in FligthMatrix.Rows)
                        {
                            // устанавливаем номер группы в матрице
                            dataRow[task.ColumnTitle] = group.Number;
                        }

                        // список общих результатов
                        if (TotalScores.Rows.Find(pilot.FullName) == null)
                        {
                            var row = TotalScores.NewRow();
                            row[Constants.Columns.Pilot] = pilot.FullName;
                            row[Constants.Columns.Place] = PilotList.Count;

                            TotalScores.Rows.Add(row);
                        }
                    }
                }
            }
            else
            {
                matrix = MainHelper.GenMatrixList(PilotList, JudgeList, TasksList, GroupsCount, 1, importMatrix);

                FligthMatrix.Rows.Clear();

                TotalScores.Rows.Clear();

                foreach (var taskScores in TasksScoresList)
                {
                    taskScores.Rows.Clear();
                }

                var i = 1;

                foreach (var pilot in PilotList)
                {
                    TotalScores.Rows.Add(i, pilot.FullName);

                    //object[] values = new object[TotalScores.Columns.Count];
                    //values[0] = i;
                    //values[1] = pilot.FullName;
                    DataRow row = FligthMatrix.NewRow();
                    row[Constants.Columns.Num] = i;
                    row[Constants.Columns.Pilot] = pilot.FullName;

                    foreach (Task task in TasksList)
                    {
                        Group group = matrix[task.Round - 1].Groups.SingleOrDefault(g => g.Pilots.SingleOrDefault(p => p.FullName == pilot.FullName) != null);

                        DataTable taskScores = TasksScoresList.SingleOrDefault(t => t.TableName == task.Round.ToString());

                        int judgeNum = group.JudgesDict[pilot];
                        taskScores.Rows.Add(pilot.FullName, @group.Number, judgeNum);

                        row[task.ColumnTitle] = group.Number;
                        //values[j] = 0;
                    }

                    FligthMatrix.Rows.Add(row);
                    i++;
                }
            }

            UpdateRoundScoresCollection();

            _currentMatrix = matrix;
        }

        private void UpdateRoundScoresCollection()
        {
            //Update tasks scores source
            Task currentTask = TasksList.SingleOrDefault(t => t.Round == CurrentRound);
            var taskScores = TasksScoresList.SingleOrDefault(t => t.TableName == currentTask.Round.ToString());
            taskScores.DefaultView.Sort = "Group";
            TaskScores.Source = taskScores;

            //update current task
            SelectedTask = currentTask;
        }

        private void AddPilotsFromDB()
        {
            var pilotsDBWindow = new PilotsDB.PilotsDB();
            if (pilotsDBWindow.ShowDialog().Value)
            {
                var newPilots = pilotsDBWindow.vm.PilotsList.Where(p => p.Checked);

                foreach (var pilot in newPilots)
                {
                    this.PilotList.Add(new Pilot
                    {
                        Name = pilot.Name,
                        MiddleName = pilot.MiddleName,
                        Surname = pilot.Surname,
                        LicenseFAI = pilot.LicNum,
                        Team = pilot.City,
                    });
                }
            }

        }
        #endregion

        #region Commands
        private RelayCommand _addPilotsFromDB;
        public RelayCommand AddPilotsFromDBCommand
        {
            get
            {
                return _addPilotsFromDB ?? (_addPilotsFromDB = new RelayCommand(obj => AddPilotsFromDB()));
            }
        }
        private RelayCommand _generateFligthMatrixCommand;
        public RelayCommand GenerateFligthMatrixCommand
        {
            get
            {
                return _generateFligthMatrixCommand ?? (_generateFligthMatrixCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        GenerateFligthMatrix();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Ошибка:{e}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }));
            }
        }

        private RelayCommand _runTimerCommand;
        public RelayCommand RunTimerCommand
        {
            get
            {
                return _runTimerCommand ?? (_runTimerCommand = new RelayCommand(obj => RunTimer()));
            }
        }

        #region task tab
        private RelayCommand _taskDelCommand;
        public RelayCommand TaskDelCommand
        {
            get
            {
                return _taskDelCommand ?? (_taskDelCommand = new RelayCommand(obj => TaskDel()));
            }
        }

        private RelayCommand _taskAddCommand;
        public RelayCommand TaskAddCommand
        {
            get
            {
                return _taskAddCommand ?? (_taskAddCommand = new RelayCommand(obj => TaskAdd()));
            }
        }

        private RelayCommand _editTaskCommand;
        public RelayCommand EditTaskCommand
        {
            get
            {
                return _editTaskCommand ?? (_editTaskCommand = new RelayCommand(obj => EditTask()));
            }
        }
        private RelayCommand _taskUpCommand;
        public RelayCommand TaskUpCommand
        {
            get
            {
                return _taskUpCommand ?? (_taskUpCommand = new RelayCommand(obj => TaskUp()));
            }
        }

        private RelayCommand _taskDownCommand;
        public RelayCommand TaskDownCommand
        {
            get
            {
                return _taskDownCommand ?? (_taskDownCommand = new RelayCommand(obj => TaskDown()));
            }
        }
        #endregion

        #region tasks scores tab
        private RelayCommand _roundUpCommand;
        public RelayCommand RoundUpCommand
        {
            get
            {
                return _roundUpCommand ?? (_roundUpCommand = new RelayCommand(obj => RoundUp()));
            }
        }

        private RelayCommand _roundDownCommand;
        public RelayCommand RoundDownCommand
        {
            get
            {
                return _roundDownCommand ?? (_roundDownCommand = new RelayCommand(obj => RoundDown()));
            }
        }
        #endregion

        #region file menu
        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = new RelayCommand(obj => OpenData()));
            }
        }
        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(obj => SaveData()));
            }
        }
        private RelayCommand _saveAsCommand;
        public RelayCommand SaveAsCommand
        {
            get
            {
                return _saveAsCommand ?? (_saveAsCommand = new RelayCommand(obj => SaveAsData()));
            }
        }
        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new RelayCommand(obj => CloseProgramm()));
            }
        }
        #endregion

        #region print menu
        private RelayCommand _printTaskScoresCommand;
        public RelayCommand PrintTaskScoresCommand
        {
            get
            {
                return _printTaskScoresCommand ?? (_printTaskScoresCommand = new RelayCommand(obj => PrintTaskScores()));
            }
        }

        private RelayCommand _printTotalScoresCommand;
        public RelayCommand PrintTotalScoresCommand
        {
            get
            {
                return _printTotalScoresCommand ?? (_printTotalScoresCommand = new RelayCommand(obj => PrintTotalScores()));
            }
        }

        private RelayCommand _printFlightMatrixCommand;
        public RelayCommand PrintFlightMatrixCommand
        {
            get
            {
                return _printFlightMatrixCommand ?? (_printFlightMatrixCommand = new RelayCommand(obj => PrintFlightMatrix()));
            }
        }
        private RelayCommand _printFlightListsCommand;
        public RelayCommand PrintFlightListsCommand
        {
            get
            {
                return _printFlightListsCommand ?? (_printFlightListsCommand = new RelayCommand(obj => PrintFlightLists()));
            }
        }
        private RelayCommand _printTeamScoresCommand;
        public RelayCommand PrintTeamScoresCommand
        {
            get
            {
                return _printTeamScoresCommand ?? (_printTeamScoresCommand = new RelayCommand(obj => PrintTeamScores()));
            }
        }
        #endregion

        #region report menu
        private RelayCommand _reportJudgesCommand;
        public RelayCommand ReportJudgesCommand
        {
            get
            {
                //return _reportJudgesCommand ?? (_reportJudgesCommand = new CommandHandler(() => ReportJudges(), true));
                return _reportJudgesCommand ?? (_reportJudgesCommand = new RelayCommand(obj => ReportJudges()));
            }
        }
        #endregion

        #region export-import menu
        private RelayCommand _exportFlightMatrixCommand;
        public RelayCommand ExportFlightMatrixCommand
        {
            get
            {
                return _exportFlightMatrixCommand ?? (_exportFlightMatrixCommand = new RelayCommand(obj => ExportFlightMatrix()));
            }
        }

        private RelayCommand _exportTotalScoresCommand;
        public RelayCommand ExportTotalScoresCommand
        {
            get
            {
                return _exportTotalScoresCommand ?? (_exportTotalScoresCommand = new RelayCommand(obj => ExportTotalScoresMatrix()));
            }
        }

        private RelayCommand _importFlightMatrixCommand;
        public RelayCommand ImportFlightMatrixCommand
        {
            get
            {
                return _importFlightMatrixCommand ?? (_importFlightMatrixCommand = new RelayCommand(obj => ImportFlightMatrix()));
            }
        }

        private RelayCommand _exportPilotListCommand;
        public RelayCommand ExportPilotListCommand
        {
            get
            {
                return _exportPilotListCommand ?? (_exportPilotListCommand = new RelayCommand(obj => ExportPilotList()));
            }
        }

        private RelayCommand _importPilotListCommand;
        public RelayCommand ImportPilotListCommand
        {
            get
            {
                return _importPilotListCommand ?? (_importPilotListCommand = new RelayCommand(obj => ImportPilotList()));
            }
        }
        #endregion
        #endregion
    }
}

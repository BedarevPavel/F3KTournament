using F3KTournament.DataModel;
using F3KTournament.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace F3KTournament.Code
{
    public class SavedDataHelper
    {
        public F3KTournamentSavedData Data;
        private MainViewModel _model;
        public SavedDataHelper(MainViewModel model)
        {
            _model = model;
            Data = new F3KTournamentSavedData()
            {
                TasksList = model.TasksList,
                JudgeList = model.JudgeList,
                PilotList = model.PilotList,
                FligthMatrix = model.FligthMatrix,
                TasksScoresList = model.TasksScoresList,
                //TotalScores = model.TotalScores,
                CalcIsOutOfCreditPilots = model.CalcIsOutOfCreditPilots,
                CurrentRound = model.CurrentRound,
                GroupsCount = model.GroupsCount
            };
        }

        public bool Save()
        {
            if (!string.IsNullOrEmpty(_model.CurrendDataFile))
            {
                return SaveToXML(_model.CurrendDataFile, Data);
            }
            else
            {
                return SaveAs();
            }
        }

        public bool SaveAs()
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Filter = "XML (*.xml)|*.xml";
            if (fd.ShowDialog() == true)
            {
                return SaveToXML(fd.FileName, Data);
            }
            return false;
        }

        public static F3KTournamentSavedData Load(string path)
        {
            return LoadFromXML<F3KTournamentSavedData>(path);
        }

        public static bool SaveToXML(string path, object obj)
        {
            bool res = false;
            
            try
            {
                using (TextWriter tw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    XmlSerializer sr = new XmlSerializer(obj.GetType());
                    sr.Serialize(tw, obj);
                    res = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return res;
        }
        public static T LoadFromXML<T>(string path)
        {
            if (File.Exists(path))
            {
                TextReader tr = new StreamReader(path, Encoding.UTF8);
                try
                {
                    XmlSerializer sr = new XmlSerializer(typeof(T));
                    return (T)Convert.ChangeType(sr.Deserialize(tr), typeof(T));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки файла:{path}, Msg:{ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    tr.Close();
                }
            }
            return default(T);
        }


    }
    public class F3KTournamentSavedData
    {
        public ObservableCollection<Task> TasksList { get; set; }
        public ObservableCollection<Judge> JudgeList { get; set; }
        public ObservableCollection<Pilot> PilotList { get; set; }
        public ObservableCollection<DataTable> TasksScoresList { get; set; }
        public DataTable FligthMatrix { get; set; }
        //public DataTable TotalScores { get; set; }
        public bool CalcIsOutOfCreditPilots { get; set; }
        public int CurrentRound { get; set; }
        public int GroupsCount { get; set; }
    }

}

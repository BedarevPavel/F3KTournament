using F3KTournament.Code;
using F3KTournament.Helpers;
using F3KTournament.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace F3KTournament.PilotsDB
{
    public class PilotsDBViewModel : ViewModelBase
    {
        private List<PilotEntity> _pilotsList;
        public List<PilotEntity> PilotsList
        {
            get
            {
                return _pilotsList;
            }
            set
            {
                _pilotsList = value;
                OnPropertyChanged("PilotsList");
            }
        }

        public List<PilotEntity> PilotsSource { get; set; }

        public bool IsDataChanged { get; set; }

        public PilotsDBViewModel()
        {
        }

        public void LoadPilotList()
        {
            PilotsList = new List<PilotEntity>();

            var pilotsXMLPath = Path.Combine(Common.AppDataCatalogPath, Constants.AppFiles.AllPilots);

            if (File.Exists(pilotsXMLPath))
            {
                PilotsSource = PilotsList = SavedDataHelper.LoadFromXML<List<PilotEntity>>(pilotsXMLPath);
            }
            else
            {
                MessageBox.Show($"Данные не найдены, импортируйте из файла", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private RelayCommand _importCommand;
        public RelayCommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new RelayCommand(obj => ImportData()));
            }
        }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(obj => AddPilots()));
            }
        }

        private void AddPilots()
        {
        }

        private void ImportData()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "CSV (*.csv)|*.csv";
            if (fd.ShowDialog() == true)
            {
                if (File.Exists(fd.FileName))
                {
                    try
                    {
                        int addedCount = 0;
                        int updatedCount = 0;
                        var _pilotsList = new List<PilotEntity>();
                        Helper.ParseCsv(fd.FileName, ref _pilotsList, ref addedCount, ref updatedCount);

                        MessageBox.Show($"Добавлено {addedCount} записей, обновлено {updatedCount} записей", "", MessageBoxButton.OK, MessageBoxImage.Information);

                        PilotsSource = PilotsList = _pilotsList;
                        IsDataChanged = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка импорта файла:{ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Ошибка открытия файла - не верно указан путь", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}

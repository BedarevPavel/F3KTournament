using F3KTournament.DataModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace F3KTournament.Helpers
{
    //public class ExcelHelper
    //{
    //    private ObservableCollection<Task> _tasksList;
    //    public ObservableCollection<Task> GetTasksList()
    //    {
    //        if (_tasksList != null)
    //        {
    //            return _tasksList;
    //        }
    //        else
    //        {
    //            _tasksList = new ObservableCollection<Task>();
    //            //parse tasklist
    //            try
    //            {
    //                var tasksListPath = Path.Combine(Common.AppDataCatalogPath, Constants.AppFiles.TasksList);

    //                if (!File.Exists(tasksListPath))
    //                {
    //                    return _tasksList;
    //                    //throw new IOException($"Справочник упражнений не найден:{tasksListPath}");
    //                }

    //                FileInfo newFile = new FileInfo(tasksListPath);

    //                using (ExcelPackage pck = new ExcelPackage(newFile))
    //                {
    //                    //читаем данные в первой вкладке
    //                    var sheet = pck.Workbook.Worksheets[1];
    //                    for (int i = 2; i <= sheet.Dimension.Rows; i++) //читаем со второй строки т.к. в первой шапка
    //                    {
    //                        if (sheet.Cells[i, 1].Value == null || string.IsNullOrWhiteSpace(sheet.Cells[i, 1].Value.ToString()))
    //                        {
    //                            continue;
    //                        }
    //                        var task = new Task();
    //                        // 1й столбец буква (идентификатор задания)
    //                        var taskIndex = sheet.Cells[i, 1].Value?.ToString();
    //                        task.Index = (TasksEnum)Enum.Parse(typeof(TasksEnum), taskIndex);
    //                        // 2й столбец название
    //                        task.Title = sheet.Cells[i, 2].Value?.ToString();
    //                        // 3й столбец описание
    //                        task.Description = sheet.Cells[i, 3].Value?.ToString();
    //                        // 4й столбец англ. название 
    //                        task.TitleEn = sheet.Cells[i, 4].Value?.ToString();
    //                        // 5й столбец англ. описание
    //                        task.DescriptionEn = sheet.Cells[i, 5].Value?.ToString();
    //                        // 6й столбец краткое наименование
    //                        task.ShortTitle = sheet.Cells[i, 6].Value?.ToString();

    //                        task.SubTasks = ParseSubTasks(i, 7, sheet);
    //                        _tasksList.Add(task);
    //                    }
    //                }

    //            }
    //            catch (IOException ex)
    //            {
    //                //TODO
    //                MessageBox.Show($"Msg:{ex.Message}|Stack:{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    //            }
    //            catch (Exception ex)
    //            {
    //                //TODO
    //                MessageBox.Show($"Msg:{ex.Message}|Stack:{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    //            }

    //        }
    //        return _tasksList;
    //    }

    //    /// <summary>
    //    /// Парсит подзадачи в строке
    //    /// </summary>
    //    /// <param name="i">Номер строки</param>
    //    /// <param name="col">Номер столбца</param>
    //    /// <param name="sheet">Лист</param>
    //    /// <returns></returns>
    //    private List<SubTask> ParseSubTasks(int row, int col, ExcelWorksheet sheet)
    //    {
    //        var res = new List<SubTask>();
    //        int inx = 1;
    //        for (int i = col; i < sheet.Dimension.Columns; i++)
    //        {
    //            var val = sheet.Cells[row, i].Value?.ToString();
    //            if (string.IsNullOrWhiteSpace(val))
    //            {
    //                return res;
    //            }
    //            if (val == "*")
    //            {
    //                val = string.Empty;
    //            }
    //            else
    //            {
    //                val = $" ({val})";
    //            }
    //            res.Add(new SubTask { Title = $"T-{inx}{val}" });
    //            inx++;
    //        }

    //        return res;
    //    }

 
    //}
}

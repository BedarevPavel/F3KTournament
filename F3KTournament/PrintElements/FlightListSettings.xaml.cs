using F3KTournament.DataModel;
using F3KTournament.Helpers;
using F3KTournament.ViewModel;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace F3KTournament.PrintElements
{
    /// <summary>
    /// Логика взаимодействия для FlightListSettings.xaml
    /// </summary>
    public partial class FlightListSettings : Window
    {
        private ObservableCollection<Task> TasksList;
        private ObservableCollection<Pilot> PilotList;
        private DataTable FligthMatrix;
        private ObservableCollection<DataTable> TasksScoresList;
        private ObservableCollection<Judge> JudgeList;
        public FlightListSettings(ObservableCollection<Task> tasksList,
            ObservableCollection<Pilot> pilotList,
            DataTable fligthMatrix,
            ObservableCollection<DataTable> tasksScoresList,
            ObservableCollection<Judge> judgeList)
        {
            TasksList = tasksList;
            PilotList = pilotList;
            FligthMatrix = fligthMatrix;
            TasksScoresList = tasksScoresList;
            JudgeList = judgeList;

            InitializeComponent();

            TourListDdl.Items.Add("Все");

            foreach (var task in tasksList)
            {
                TourListDdl.Items.Add(task.Round.ToString());
            }

            TourListDdl.SelectedItem = "Все";

            PilotListDdl.Items.Add("Все");

            foreach (var pilot in pilotList)
            {
                PilotListDdl.Items.Add(pilot.FullName);
            }
            PilotListDdl.SelectedItem = "Все";
        }

        private int GetGroup(Task task, Pilot pilot)
        {
            int res = 0;
            var row = FligthMatrix.Rows.Find(pilot.FullName);
            if (row != null)
            {
                var val = row[task.ColumnTitle];
                int.TryParse(val.ToString(), out res);
            }

            return res;
        }

        private Judge GetJudge(Pilot pilot, Task task)
        {
            DataTable taskScores = TasksScoresList.SingleOrDefault(t => t.TableName == task.Round.ToString());
            var row = taskScores.Rows.Find(pilot.FullName);
            int judgeNum = int.Parse(row[Constants.Columns.Judge].ToString());
            var judge = JudgeList.SingleOrDefault(j => j.Number == judgeNum);
            return judge;
        }

        private void PrintDocument(string html)
        {
            var wb = new System.Windows.Forms.WebBrowser();
            wb.Size = new System.Drawing.Size(500, 500);
            wb.DocumentCompleted += PageLoaded;
            wb.DocumentText = html;
        }

        private void PageLoaded(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //PrintPreviewDialog dlg = new PrintPreviewDialog();
            //((Form)dlg).WindowState = FormWindowState.Maximized;
            var wb = ((System.Windows.Forms.WebBrowser)sender);
            //wb.Bounds = new System.Drawing.Rectangle(100, 100, 500, 800);
            //wb.ClientSize = new System.Drawing.Size(600, 800);
            //wb.Anchor = AnchorStyles.Right;
            wb.ShowPrintPreviewDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserHelper.FixBrowserVersion(8888);

            /// подгрузить html из шаблона
            var flightCardTemplateFilePath = System.IO.Path.Combine(Common.AppDataTemplatePath, Constants.AppFiles.Templates.FlightCard);

            var doc = new HtmlDocument();
            doc.Load(flightCardTemplateFilePath);

            var cardItemTmpl = doc.DocumentNode.SelectNodes("//div[@class='card-item']")[0];

            var book = doc.GetElementbyId("book");
            book.RemoveAllChildren();

            //var cardCount = 0;
            
            var cardItems = new List<HtmlNode>();

            foreach (var pilot in PilotList)
            {
                if (PilotListDdl.SelectedValue.ToString() == "Все" || pilot.FullName == PilotListDdl.SelectedValue.ToString())
                {
                    foreach (var task in TasksList)
                    {
                        if (TourListDdl.SelectedValue.ToString() == "Все" || task.Round.ToString() == TourListDdl.SelectedValue.ToString())
                        {

                            var cardItem = cardItemTmpl.CloneNode(true);

                            var taskIndx = Regex.Replace(task.Index.ToString(), @"[\d-]", string.Empty);
                            cardItem.UpdateChildNode("task-inx", $"Упражнение '{taskIndx}'");
                            cardItem.UpdateChildNode("task-desc", $"({task.Name})");
                            cardItem.UpdateChildNode("tour-group", $"{task.Round} / {GetGroup(task, pilot)}");

                            var judge = GetJudge(pilot, task);
                            cardItem.UpdateChildNode("judge-number", $"Судья №{judge.Number.ToString()}");
                            cardItem.UpdateChildNode("judge-fio", $"ФИО: {judge.FIO}");

                            cardItem.UpdateChildNode("pilot-fio", $"ФИО: {pilot.FullName}");
                            cardItem.UpdateChildNode("task-info-desc", task.Rules);

                            var tasksTable1Wrap = cardItem.Descendants(0).FirstOrDefault(d => d.HasClass("tasks-table"));

                            tasksTable1Wrap.RemoveAllChildren();
                            tasksTable1Wrap.AppendChild(MainHelper.CreateHtmlTasksTable(task.ColCount1, task.RowCount1));

                            var tasksTable2Wrap = cardItem.Descendants(0).LastOrDefault(d => d.HasClass("tasks-table"));
                            tasksTable2Wrap.RemoveAllChildren();
                            tasksTable2Wrap.AppendChild(MainHelper.CreateHtmlTasksTable(task.ColCount2, task.RowCount2));

                            cardItems.Add(cardItem);

                            //if (cardCount > 0 && cardCount % 2 == 0)
                            //{
                            //    book.AppendChild(page);
                            //    page = HtmlNode.CreateNode("<div class='page'></div>");
                            //    cardCount = 0;
                            //}
                            //else if (page.ChildNodes.Count == 1)
                            //{
                            //    page.AppendChild(HtmlNode.CreateNode("<div class='card-seporator'></div>"));
                            //}

                            //page.AppendChild(cardItem);
                            //cardCount++;

                        }
                    }
                }
            }

            var page = HtmlNode.CreateNode("<div class='page'></div>");
            for (int i=0; i < cardItems.Count; i++)
            {
                page.AppendChild(cardItems[i]);

                if (page.ChildNodes.Count == 1)
                {
                    page.AppendChild(HtmlNode.CreateNode("<div class='card-seporator'></div>"));
                }

                if (page.ChildNodes.Count == 3 || cardItems.Count == i + 1)
                {
                    book.AppendChild(page);
                    page = HtmlNode.CreateNode("<div class='page'></div>");
                }

            }

            //PrintDocument(doc.DocumentNode.OuterHtml);

            string myTempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"f3k_tasks_list_{DateTime.Now:yyyyMMdd_hhmm}.html");
            using (StreamWriter sw = new StreamWriter(myTempFile))
            {
                sw.WriteLine(doc.DocumentNode.OuterHtml);
            }

            System.Diagnostics.Process.Start(myTempFile);

            //var cardList = new List<FlightCardViewModel>();
            //foreach (var task in TasksList)
            //{
            //    if (TourListDdl.SelectedValue.ToString() == "Все" || task.Round.ToString() == TourListDdl.SelectedValue.ToString())
            //    {
            //        foreach (var pilot in PilotList)
            //        {
            //            if (PilotListDdl.SelectedValue.ToString() == "Все" || pilot.FullName == PilotListDdl.SelectedValue.ToString())
            //            {
            //                var taskIndx =  Regex.Replace(task.Index.ToString(), @"[\d-]", string.Empty);


            //                var card = new FlightCardViewModel();
            //                card.Pilot = pilot.FullName;
            //                card.TaskName = task.Name;
            //                card.Index = taskIndx;
            //                card.Rules = task.Rules;
            //                card.Tour = task.Round;
            //                card.Group = GetGroup(task, pilot);
            //                var judge = GetJudge(pilot, task);
            //                card.JudgeFIO = judge.FIO;
            //                card.JudgeNum = judge.Number;
            //                //TODO
            //                card.RowCount1 = task.RowCount1;
            //                card.RowCount2 = task.RowCount2;
            //                card.ColCount1 = task.ColCount1;
            //                card.ColCount2 = task.ColCount2;
            //                cardList.Add(card);
            //            }
            //        }
            //    }
            //}

            //cardList = cardList.OrderBy(p => p.Pilot).ToList();
            //FlightLists fl = new FlightLists(cardList);
            //fl.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Application curApp = Application.Current;
            //Window mainWindow = curApp.MainWindow;
            //this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            //this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}

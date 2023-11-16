using F3KTournament.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using HtmlAgilityPack;

namespace F3KTournament.Helpers
{
    public static class MainHelper
    {
        public static HtmlNode CreateHtmlTasksTable(int col, int row)
        {
            var tasksTable = HtmlNode.CreateNode("<table></table>");

            for (int i = 0; i < row; i++)
            {
                var tr1 = string.Empty;
                var tr2 = string.Empty;

                for (int j = 0; j < col; j++)
                {
                    tr1 += $"<td>{j + 1}</td>";
                    tr2 += $"<td>&nbsp;</td>";
                }

                tasksTable.AppendChild(HtmlNode.CreateNode($"<tr>{tr1}</tr>"));
                tasksTable.AppendChild(HtmlNode.CreateNode($"<tr>{tr2}</tr>"));
            }

            return tasksTable;
        }

        public static void UpdateChildNode(this HtmlNode node, string className, string val)
        {
            var nodes = node.Descendants(0).Where(d => d.HasClass(className));
            foreach (var n in nodes)
            {
                n.InnerHtml = val;
            }
        }

        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Title.Equals(name));
        }

        public static string ConvertSecToTimeString(string value)
        {
            int secs = 0;
            int.TryParse(value, out secs);

            return convertSecToTimeString(secs);
        }

        private static string convertSecToTimeString(int secs)
        {
            TimeSpan t = TimeSpan.FromSeconds(secs);
            return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }

        public static string ConvertSecToTimeString(int value)
        {
            return convertSecToTimeString(value);
        }

        public static Size MeasureString(string candidate, bool isBold = false)
        {
            var fontWeigth = FontWeights.Normal;
            if (isBold)
            {
                fontWeigth = FontWeights.Bold;
            }

            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Verdana"), FontStyles.Normal, fontWeigth, FontStretches.Normal),
                12,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public static List<Tour> GenMatrixList(ObservableCollection<Pilot> pilots, ObservableCollection<Judge> judgeList, 
            ObservableCollection<Task> tasksList, int groups, int fromTask = 1, DataTable importMatrix = null)
        {
            var tours = new List<Tour>();
            var rnd = new Random();

            if (importMatrix == null)
            {
                int tasksCount = tasksList.Count;

                var groupSize = pilots.Count / groups;
                for (int task = fromTask; task <= tasksCount; task++)
                {
                    var groupsList = new List<Group>();
                    for (int i = 1; i <= groups; i++)
                    {
                        groupsList.Add(new Group() { Number = i, TaskNum = task, Size = groupSize + 1 });
                    }

                    //для первого задания генерируем случайным образом, для последующих проверяем пересекаемость
                    if (task == 1)
                    {
                        tours.Add(new Tour() { Groups = groupsList, Number = task });
                        var pilotsCopy = new List<Pilot>(pilots);
                        while (pilotsCopy.Count != 0)
                        {
                            var rndPilot = rnd.Next(0, pilotsCopy.Count);
                            var curPilot = pilotsCopy[rndPilot];

                            //берём группу где нет этого пилота
                            var groupsWithoutPilot = groupsList.Where(g => !g.Pilots.Contains(curPilot)).ToArray();

                            var rndGroup = rnd.Next(0, groupsWithoutPilot.Length);
                            var group = groupsWithoutPilot[rndGroup];

                            if (group.AddPilot(ref curPilot, pilotsCopy))
                            {
                                pilotsCopy.Remove(curPilot);
                            }
                        }

                        //проверяем равномерность распределения в группах
                        foreach (var group in groupsList)
                        {
                            //если в группе оказалось меньше пилотов, чем средний размер группы, перемещаем пилота из группы где больше всего пилотов в эту группу
                            if (group.Pilots.Count < groupSize)
                            {
                                var orderedGroups = groupsList.OrderBy(p => p.Pilots.Count).ToList();
                                var maxPilotsGroup = orderedGroups[groups - 1];
                                group.Pilots.Add(maxPilotsGroup.Pilots[0]);
                                maxPilotsGroup.Pilots.RemoveAt(0);
                            }
                        }
                    }
                    else
                    {
                        var pilotsCopy = new List<Pilot>(pilots);
                        while (pilotsCopy.Count != 0)
                        {

                            //смотрим в какой группе был пилот, генерим случайную, но чтобы не крайние и не дальше половины кол-ва групп
                            //var prevGroup = tours[task - 2].Groups.Single(f => f.Pilots.Contains(curPilot));
                            //var prevGroupNum = prevGroup.Number;
                            //var groupsArr = groups.ToString().Select(o => Convert.ToInt32(o)).ToList();
                            //groupsArr.Remove(prevGroupNum);
                            //int newGroupArrIndx = rnd.Next(1, groupsArr.Count);
                            //int newGroupNum = groupsArr[newGroupArrIndx];

                            var rndGroup = rnd.Next(1, groups + 1);
                            var group = groupsList.Single(t => t.Number == rndGroup);
                            var curPilot = pilotsCopy[0];

                            if (group.AddPilot(ref curPilot, pilotsCopy))
                            {
                                pilotsCopy.Remove(curPilot);

                                //добавляем в эту группу следующего пилота, с которым он не разу не пересекался в предыдущих турах

                                List<Pilot> intersectPilots = new List<Pilot>();
                                foreach (var tour in tours)
                                {
                                    var curGroup = tour.Groups.Single(f => f.Pilots.Contains(curPilot));
                                    intersectPilots = intersectPilots.Union(curGroup.Pilots).ToList();
                                }

                                var condidates = pilotsCopy.Except(intersectPilots).ToList();

                                if (condidates.Count > 0)
                                {

                                    bool groupFull = false;
                                    while (!groupFull)
                                    {
                                        var rndCand = rnd.Next(0, condidates.Count - 1);
                                        Pilot cond = condidates[rndCand];

                                        if (group.AddPilot(ref cond, condidates))
                                        {
                                            pilotsCopy.Remove(cond);
                                        }

                                        if (group.Pilots.Count == group.Size || condidates.Count < group.Size)
                                        {
                                            groupFull = true;
                                        }
                                    }
                                }
                            }
                        }

                        //проверяем равномерность распределения в группах
                        foreach (var group in groupsList)
                        {
                            //если в группе оказалось меньше пилотов, чем средний размер группы, перемещаем пилота из группы где больше всего пилотов в эту группу
                            if (group.Pilots.Count < groupSize)
                            {
                                var orderedGroups = groupsList.OrderBy(p => p.Pilots.Count).ToList();
                                var maxPilotsGroup = orderedGroups[groups - 1];
                                group.Pilots.Add(maxPilotsGroup.Pilots[0]);
                                maxPilotsGroup.Pilots.RemoveAt(0);
                            }


                        }
                        tours.Add(new Tour() { Groups = groupsList, Number = task });
                    }

                }
            }
            else
            {
                var tasksGroupsList = new Dictionary<string, List<Group>>();

                foreach (DataRow row in importMatrix.Rows)
                {
                    string pilotFullName = row[Constants.Columns.Pilot].ToString();

                    var pilot = pilots.Single(p => p.FullName == pilotFullName);

                    foreach (DataColumn col in importMatrix.Columns)
                    {
                        if (col.ColumnName == Constants.Columns.Num || col.ColumnName == Constants.Columns.Pilot)
                        {
                            continue;
                        }

                        if (!tasksGroupsList.ContainsKey(col.ColumnName))
                        {
                            var groupsList = new List<Group>();
                            for (int i = 1; i <= groups; i++)
                            {
                                groupsList.Add(new Group() { Number = i });
                            }

                            tasksGroupsList.Add(col.ColumnName, groupsList);
                        }

                        var groupNumStr = row[col].ToString();
                        int groupNum = int.Parse(groupNumStr);

                        var group = tasksGroupsList[col.ColumnName].Single(g => g.Number == groupNum);
                        group.Pilots.Add(pilot);
                        var task = tasksList.Single(t => t.ColumnTitle == col.ColumnName);

                        var tour = tours.SingleOrDefault(t => t.Number == task.Round);
                        if (tour == null)
                        {
                            tour = new Tour() { Number = task.Round };
                            tours.Add(tour);
                        }

                        tour.Groups = tasksGroupsList[col.ColumnName];
                    }
                }
            }


            //назначаем судей
            var judgeArr = judgeList.Select(t => t.Number).ToArray();
            judgeArr = ShuffledArray(judgeArr);

            var pilotsJudges = new Dictionary<Pilot, int[]>();
            int tourNum = 0;
            foreach (Tour tour in tours)
            {
                foreach (Group group in tour.Groups)
                {
                    int i = 0;
                    int[] judgeArrTmp = (int[])judgeArr.Clone();
                    foreach (Pilot pilot in group.Pilots)
                    {
                        if (!pilotsJudges.ContainsKey(pilot))
                        {
                            pilotsJudges.Add(pilot, new int[tours.Count]);
                        }
                        int[] pilotJudges = pilotsJudges[pilot];
                        bool flag1 = true;
                        int judgeNum = 0;
                        while (flag1)
                        {
                            var judgeRndInx = rnd.Next(0, judgeArrTmp.Length);
                            judgeNum = judgeArrTmp[judgeRndInx];

                            //смотрим предыдущего судью
                            if (tourNum == 0 || judgeNum != pilotJudges[tourNum - 1] || judgeArrTmp.Length == 1)
                            {
                                flag1 = false;
                            }
                        }

                        judgeArrTmp = judgeArrTmp.Where(n => n != judgeNum).ToArray();
                        pilotJudges[tourNum] = judgeNum;
                        group.JudgesDict.Add(pilot, judgeNum);

                        i++;
                    }
                }
                tourNum++;
            }

            return tours;
            //проверяем, чтобы каждый пилот пересёкся с каждым пилотом в течение всего соревнования
            //foreach (var pilot in pilots)
            //{
            //    //пилоты с которыми должен пересечся в одном из туров
            //    var subPilots = new List<Pilot>(pilots);
            //    subPilots.Remove(pilot);
            //    foreach (var subPilot in subPilots)
            //    {
            //        bool flag = false;
            //        foreach (var tour in tours)
            //        {
            //            var groupForCurrentPilot = tour.Groups.Single(g => g.Pilots.Contains(pilot));
            //            if (groupForCurrentPilot.Pilots.Contains(subPilot))
            //            {
            //                flag = true;
            //                continue;
            //            }
            //        }

            //        if (!flag)
            //        {
            //            Console.WriteLine($"{pilot.FullName} not intersect with {subPilot.FullName}");
            //        }
            //    }
            //}
        }

        private static int[] ShuffledArray(int[] array)
        {
            Random r = new Random();
            for (int i = array.Length; i > 0; i--)
            {
                int j = r.Next(i);
                int k = array[j];
                array[j] = array[i - 1];
                array[i - 1] = k;
            }
            return array;
        }
    }
}

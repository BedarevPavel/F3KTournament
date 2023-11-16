using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using F3KTournament;
using F3KTournament.PilotsDB;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {

            var groupsCount = 4;
            var pilotsCount = 44;
            var tasksCount = 11;

            var pilotsList = new List<Pilot>();
            for (int i = 0; i < pilotsCount; i++)
            {
                pilotsList.Add(new Pilot() { Name = "Pilot" + (i + 1).ToString() });
            }


            var tasksList = new List<Task>();
            for (int i = 0; i < tasksCount; i++)
            {
                tasksList.Add(new Task() { Round = i + 1, Title = $"Task{i + 1}" });
            }

            GenMatrixList(pilotsList, tasksList, groupsCount);

            //foreach (var pilot in pilotsList)
            //{
            //    Console.WriteLine(pilot.FullName);
            //}

            //var tours = new List<Tour>();


            Console.ReadLine();
        }

        public static int[] ShuffleArr(int groups, int tasksCount)
        {
            int[] arr = new int[tasksCount];
            int gr = groups;
            for (int j = 0; j < tasksCount; j++)
            {
                if (gr == 0)
                {
                    gr = groups;
                }
                arr[j] = gr;
                gr--;

            }

            var rnd = new Random();
            for (int t = 0; t < arr.Length; t++)
            {
                int tmp = arr[t];
                int r = rnd.Next(t, arr.Length);
                arr[t] = arr[r];
                arr[r] = tmp;
            }

            return arr;
        }
        public static void GenMatrixList(List<Pilot> pilots, List<Task> tasks, int groups)
        {
            var rnd = new Random();

            //for (int i = 0; i < groups; i++)
            //{
            //    var group = new Group() { Number = i + 1 };

            //    foreach (var pilot in pilots)
            //    {

            //    }
            //}


            //int[,] matrix = new int[pilots.Count, tasks.Count];

            //for (int i = 0; i < pilots.Count; i++)
            //{
            //    for (int j = 0; j < tasks.Count; j++)
            //    {
            //        var group = rnd.Next(1, groups+1);
            //        matrix[i, j] = group;
            //        Console.Write($"{group}|");
            //    }
            //    Console.WriteLine();
            //}



            //var listRows = new List<FlightMatrixRow>();

            //foreach (var pilot in pilots)
            //{
            //    var fRow = new FlightMatrixRow() { PilotName = pilot.FullName };
            //    foreach (var task in tasks)
            //    {
            //        var randGroup = rnd.Next(1, groups + 1);
            //        fRow.TasksGroups.Add(task.Number, randGroup);
            //    }
            //    listRows.Add(fRow);
            //    Console.WriteLine(fRow.ToString());
            //}


            var groupSize = pilots.Count / groups;

            //var listRows = new List<FlightMatrixRow>();

            var tours = new List<Tour>();

            //var res = new List<int[]>();
            ////v2
            //for (int i = 0; i < pilots.Count; i++)
            //{
            //    //для каждого пилота генерируем на каждый тур группу
            //    //желательно чтобы между турами разница между группами была минимальная
            //    //для первого пилота генерируем группы случайным образом

            //    int[] tsks = ShuffleArr(groups, tasks.Count);

            //    //все последующие массивы проверяем с предыдущими, чтобы хотябы один элемент пересекался
            //    if (i > 0)
            //    {
            //        int[] prev = res[i - 1];
            //        var intersect = prev.Intersect(tsks);

            //    }

            //    res.Add(tsks);

            //    for (int k = 0; k < tsks.Length; k++)
            //    {
            //        Console.Write(tsks[k] + "|");
            //    }

            //    Console.WriteLine();
            //    Console.ReadKey();

            //}



            //return;

            //v1
            for (int task = 1; task <= tasks.Count; task++)
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
                        for (int i = 0; i < pilotsCopy.Count; i++)
                        {
                            var rndGroup = rnd.Next(1, groups + 1);
                            var group = groupsList.Single(t => t.Number == rndGroup);
                            if (group.AddPilot(pilotsCopy[i]))
                            {
                                pilotsCopy.Remove(pilotsCopy[i]);
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
                }
                else
                {
                    var pilotsCopy = new List<Pilot>(pilots);
                    while (pilotsCopy.Count != 0)
                    {
                        var curPilot = pilotsCopy[0];
                        //смотрим в какой группе был пилот, генерим случайную, но чтобы не крайние и не дальше половины кол-ва групп
                        var prevGroup = tours[task - 2].Groups.Single(f => f.Pilots.Contains(curPilot));
                        var prevGroupNum = prevGroup.Number;
                        //var groupsArr = groups.ToString().Select(o => Convert.ToInt32(o)).ToList();
                        //groupsArr.Remove(prevGroupNum);
                        //int newGroupArrIndx = rnd.Next(1, groupsArr.Count);
                        //int newGroupNum = groupsArr[newGroupArrIndx];

                        var rndGroup = rnd.Next(1, groups + 1);
                        var group = groupsList.Single(t => t.Number == rndGroup);
                        if (group.AddPilot(curPilot))
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

                                    if (group.AddPilot(cond))
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

            //проверяем, чтобы каждый пилот пересёкся с каждым пилотом в течение всего соревнования

            foreach (var pilot in pilots)
            {
                //пилоты с которыми должен пересечся в одном из туров
                var subPilots = new List<Pilot>(pilots);
                subPilots.Remove(pilot);
                foreach (var subPilot in subPilots)
                {
                    bool flag = false;
                    foreach (var tour in tours)
                    {
                        var groupForCurrentPilot = tour.Groups.Single(g => g.Pilots.Contains(pilot));
                        if (groupForCurrentPilot.Pilots.Contains(subPilot))
                        {
                            flag = true;
                            continue;
                        }
                    }

                    if (!flag)
                    {
                        Console.WriteLine($"{pilot.FullName} not intersect with {subPilot.FullName}");
                    }
                }
            }

            foreach (var pilot in pilots)
            {
                string row = pilot.FullName;

                foreach (var tour in tours)
                {
                    var group = tour.Groups.Single(g => g.Pilots.Contains(pilot));
                    row += $"|{group.Number}";
                }

                Console.WriteLine(row);
            }

            //foreach (var tour in tours)
            //{
            //    Console.WriteLine($"Tour №{tour.Number}");
            //    foreach (var group in tour.Groups.OrderBy(g=>g.Number))
            //    {
            //        Console.WriteLine($"Group №{group.Number}");
            //        foreach (var pilot in group.Pilots)
            //        {
            //            Console.WriteLine(pilot.FullName);
            //        }
            //    }
            //}

        }
    }

    class Tour
    {
        public int Number { get; set; }

        public List<Group> Groups { get; set; }

    }

    class Group
    {
        public int TaskNum { get; set; }

        public int Size { get; set; }
        public int Number { get; set; }
        public List<Pilot> Pilots { get; }
        public Group()
        {
            Pilots = new List<Pilot>();
        }

        public bool AddPilot(Pilot pilot)
        {
            if (Pilots.Count < Size && !Pilots.Contains(pilot))
            {
                Pilots.Add(pilot);
                return true;
            }

            return false;
        }
    }


    class FlightMatrixRow
    {
        public string PilotName { get; set; }
        /// <summary>
        /// Key - task num, val - group  num
        /// </summary>
        public Dictionary<int, int> TasksGroups { get; }

        public FlightMatrixRow()
        {
            this.TasksGroups = new Dictionary<int, int>();
        }

        public override string ToString()
        {
            string res = $"{PilotName}";
            foreach (var tg in TasksGroups)
            {
                res += $"|{tg.Value}";
            }
            return res;
        }
    }
}

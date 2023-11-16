using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F3KTournament
{
    public static class Constants
    {
        public static class AppDirectories
        {
            public const string Catalog = "Catalog";
            public const string Templates = "Templates";
            public const string Audio = "Audio";
            public const string Data = "Data";
            public const string SavedData = "SavedData";
        }

        public static class AppFiles
        {
            public const string Tasks = "Tasks.xml";
            public const string AllTasks = "AllTasks.xml";
            public const string AllPilots = "AllPilots.xml";
            public const string Pilots = "Pilots.xml";
            public const string Judges = "Judges.xml";
            public const string TasksScores = "TasksScores.xml";

            public static class Templates
            {
                public const string FlightCard = "FlightCard.html";
            }
        }

        public static class Columns
        {
            public const string Team = "Команда";
            public const string Group = "Group";
            public const string Judge = "Судья";
            public const string TotalTime = "Суммарное время";
            public const string Scores = "Очки";
            public const string TeamScores = "Очки команды";
            public const string Penalty = "Штраф";
            public const string SubTask = "Время-";
            public const string Pilot = "Пилот";
            public const string Num = "№";
            public const string Place = "Место";
            public const string Percent = "Процент";

            public const string TasksNameSpace = "Tasks";
        }

    }
}

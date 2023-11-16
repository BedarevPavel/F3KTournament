using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F3KTournament.DataModel
{
    public class Group
    {
        public int TaskNum { get; set; }

        public int Size { get; set; }
        public int Number { get; set; }
        public List<Pilot> Pilots { get; }
        public Dictionary<Pilot, int> JudgesDict { get; set; }

        public Group()
        {
            Pilots = new List<Pilot>();
            JudgesDict = new Dictionary<Pilot, int>();
        }

        public bool AddPilot(ref Pilot pilot, List<Pilot> freePilots)
        {
            var refTeam = pilot.Team;
            //проверяем если есть возможность распределить пилотов, чтобы не пересикались из одной команды
            int teamPilots = Pilots.Count(p => p.Team == refTeam);
            if (teamPilots > 0)
            {
                var freeTeams = freePilots.Select(t => t.Team).Distinct().ToArray();
                var currentTeams = Pilots.Select(t => t.Team).ToArray();

                var exceptTeams = freeTeams.Except(currentTeams).ToArray();
                if (exceptTeams.Length > 0)
                {
                    pilot = freePilots.Where(p => p.Team == exceptTeams[0]).ToArray()[0];
                }
            }

            if (Pilots.Count < Size && !Pilots.Contains(pilot))
            {
                Pilots.Add(pilot);
                return true;
            }

            return false;
        }
    }
}

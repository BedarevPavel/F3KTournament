using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F3KTournament.DataModel
{
    public class TeamScore
    {
        public string Team { get; set; }
        public int Place { get; set; }
        public int TotalScores { get; set; }
        public List<PilotScore> Pilots { get; set; }

        public TeamScore(string teamName)
        {
            TotalScores = 0;
            Team = teamName;
            Pilots = new List<PilotScore>();
        }
    }

    public class PilotScore 
    {
        public Pilot Pilot { get; set; }
        public int TotalScores { get; set; }
    }
}

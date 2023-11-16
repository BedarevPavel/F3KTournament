using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F3KTournament
{
    public class Pilot
    {
        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName
        {
            get
            {
                return $"{Surname} {Name} {MiddleName}";
            }
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Rank { get; set; }
        public bool OutOfCredit { get; set; }
        public string Team { get; set; }
        public string LicenseFAI { get; set; }
    }
}

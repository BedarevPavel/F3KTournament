using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace F3KTournament.PilotsDB
{
    public class PilotEntity
    {
        public string FIO { get; set; }

        public string City { get; set; }

        public string ID_FAI { get; set; }

        public string LicNum { get; set; }

        [XmlIgnore]
        public bool Checked { get; set; }

        [XmlIgnore]
        public string Name
        {
            get
            {
                return GetFIOPath(1);
            }
        }

        [XmlIgnore]
        public string Surname
        {
            get
            {
                return GetFIOPath(0);
            }
        }

        [XmlIgnore]
        public string MiddleName
        {
            get
            {
                return GetFIOPath(2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">0 - фамилия, 1 - имя, 2 - отчество</param>
        /// <returns></returns>
        private string GetFIOPath(int path)
        {
            var f = FIO.Split();
            if (f.Length > path)
            {
                return f[path];
            }

            return string.Empty;
        }
    }
}

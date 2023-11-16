using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F3KTournament.PilotsDB
{
    public static class Helper
    {
        public static void ParseCsv(string fPath, ref List<PilotEntity> pilotsList, ref int addedCount, ref int updatedCount)
        {
            using (TextFieldParser parser = new TextFieldParser(fPath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    var pilot = pilotsList.SingleOrDefault(p => p.FIO == fields[0]);

                    if (pilot == null)
                    {
                        pilot = new PilotEntity();
                        pilotsList.Add(pilot);
                        addedCount++;
                    }
                    else
                    {
                        updatedCount++;
                    }


                    pilot.FIO = fields[0];
                    pilot.City = fields[1];
                    pilot.ID_FAI = fields[2];
                    pilot.LicNum = fields[3];
                }
            }
        }
    }
}

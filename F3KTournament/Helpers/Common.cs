using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace F3KTournament.Helpers
{
    public static class Common
    {
        /// <summary>
        /// Возвращает номер тура до которого заполнены результаты
        /// </summary>
        /// <returns></returns>
        public static int GetTraveledTourIndex(DataTable totalScores)
        {
            foreach (DataRow row in totalScores.Rows)
            {
                for (var i = 2; i < row.ItemArray.Length - 3; i++)
                {
                    var val = row.ItemArray[i]?.ToString();
                    if (!string.IsNullOrEmpty(val) && val !="0")
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        public static string AppDataTemplatePath
        {
            get
            {
                string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                return Path.Combine(path, Constants.AppDirectories.Data, Constants.AppDirectories.Templates);
            }
        }

        public static string AppDataCatalogPath
        {
            get
            {
                string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                return Path.Combine(path, Constants.AppDirectories.Data, Constants.AppDirectories.Catalog);
            }
        }

        public static string AppDataSavedPath
        {
            get
            {
                string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                return Path.Combine(path, Constants.AppDirectories.Data, Constants.AppDirectories.SavedData);
            }
        }

        public static string AppDataAudioPath
        {
            get
            {
                string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                return Path.Combine(path, Constants.AppDirectories.Data, Constants.AppDirectories.Audio);
            }
        }

        public static string AppDataPath
        {
            get
            {
                string path = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                return Path.Combine(path, Constants.AppDirectories.Data);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F3KTournament.Code
{
    /// <summary>
    /// Заголовки для таблиц
    /// </summary>
    public class TaskHeaders
    {
        public string TaskIndex { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public TaskHeaders()
        {
            Headers = new Dictionary<string, string>();
        }
    }
}

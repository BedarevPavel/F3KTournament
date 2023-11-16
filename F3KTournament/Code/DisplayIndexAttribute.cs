using System;

namespace F3KTournament.DataModel
{
    internal class DisplayIndexAttribute : Attribute
    {
        public DisplayIndexAttribute(int index)
        {
            DispalyIndex = index;
        }

        public int DispalyIndex { get; set; }
    }
}
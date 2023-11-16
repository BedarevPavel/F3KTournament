using F3KTournament.PrintElements.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F3KTournament.Code
{
    class Page
    {
        List<FlightCard> _cardList = new List<FlightCard>();

        public List<FlightCard> CardList
        {
            get { return _cardList; }
        }
    }
}

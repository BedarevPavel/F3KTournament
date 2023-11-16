using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F3KTournament.Code
{
    class Document
    {
        int currentPageIndex = 0;

        public int CurrentPageIndex
        {
            get { return currentPageIndex; }
            set
            {
                if (value >= 0 && value < pageList.Count)
                {
                    currentPageIndex = value;
                }
            }
        }
        List<Page> pageList = new List<Page>();

        public Page CurrentPage
        {
            get { return pageList[currentPageIndex]; }
        }

        public int PageCount
        {
            get { return pageList.Count; }
        }

        public void AddPage(int location)
        {
            Page newPage = new Page();
            pageList.Insert(location, newPage);
            currentPageIndex = location;
        }
    }
}

using F3KTournament.PrintElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace F3KTournament.Code
{
    public class Paginator : DocumentPaginator
    {
        Document document;
        Canvas pageCanvas;
        private Window _window;
        public Paginator(Window window)
        {
            _window = window;
            FlightLists mainWindow = (FlightLists)_window;
            document = mainWindow.Document;
            pageCanvas = mainWindow.PageCanvas;
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            FlightLists mainWindow = (FlightLists)_window;
            Canvas printCanvas = new Canvas();
            mainWindow.Document.CurrentPageIndex = pageNumber;
            mainWindow.DrawPage(printCanvas);
            printCanvas.Measure(PageSize);
            printCanvas.Arrange(new Rect(new Point(), PageSize));
            printCanvas.UpdateLayout();
            return new DocumentPage(printCanvas);
        }

        public override bool IsPageCountValid
        {
            get { return true; }
        }

        public override int PageCount
        {
            get { return document.PageCount; }
        }

        public override System.Windows.Size PageSize
        {
            get
            {
                return new Size(pageCanvas.Width, pageCanvas.Height);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }
    }
}

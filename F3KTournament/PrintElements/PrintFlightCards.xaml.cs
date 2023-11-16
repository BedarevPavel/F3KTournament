using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace F3KTournament.PrintElements
{
    /// <summary>
    /// Interaction logic for PrintFlightCards.xaml
    /// </summary>
    public partial class PrintFlightCards : Window
    {
        public PrintFlightCards(string content)
        {
            InitializeComponent();

            var wb = new System.Windows.Forms.WebBrowser();


            //webBrowser.NavigateToString(content);
        }




        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
        internal interface IServiceProvider
        {
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object QueryService(ref Guid guidService, ref Guid riid);
        }
        static readonly Guid SID_SWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");

        void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            IServiceProvider serviceProvider = null;
            if (webBrowser.Document != null)
            {
                serviceProvider = (IServiceProvider)webBrowser.Document;
            }

            Guid serviceGuid = SID_SWebBrowserApp;
            Guid iid = typeof(SHDocVw.IWebBrowser2).GUID;

            object NullValue = null;

            SHDocVw.IWebBrowser2 target = (SHDocVw.IWebBrowser2)serviceProvider.QueryService(ref serviceGuid, ref iid);
            target.ExecWB(SHDocVw.OLECMDID.OLECMDID_PRINTPREVIEW, SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref NullValue, ref NullValue);

        }
    }
}

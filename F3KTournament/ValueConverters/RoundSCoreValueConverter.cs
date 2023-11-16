using F3KTournament.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace F3KTournament.ValueConverters
{
    public class RoundSCoreValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            //int secs = 0;
            //int.TryParse(value.ToString(), out secs);
            
            //TimeSpan t = TimeSpan.FromSeconds(secs);
            //string res = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

            return MainHelper.ConvertSecToTimeString(value.ToString()); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int sec = 0;
                int min = 0;

                var val = value.ToString();
                if (val.IndexOf(':') > 0)
                {
                    var pair = val.Split(':');
                    min = int.Parse(pair[0]);
                    sec = int.Parse(pair[1]);
                }
                else if (val.Length > 2) // если ввели без двоеточния минуты и секунды
                {
                    sec = int.Parse(val.Substring(val.Length - 2));
                    min = int.Parse(val.Substring(0, val.Length - 2));
                }
                else
                {
                    sec = int.Parse(val);
                }

                var t = min * 60 + sec;
                return t;
            }
            catch
            {
                return null; // TODO
            }
        }
    }
}

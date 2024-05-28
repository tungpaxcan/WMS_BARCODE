using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS
{
    public class QueryDateVsTime
    {
        public static string Date(DateTime date)
        {
            try
            {
                var rsl = "";
                if (date != null)
                {
                    rsl = $"{date.Day}/{date.Month}/{date.Year} {date.Hour}:{date.Minute}:{date.Second} {GetNameTimeSysTemLaTinh(int.Parse(date.Hour.ToString()))}";
                }
                return rsl;
            }
            catch(Exception e)
            {
                return $"Lỗi: {e.Message}";
            } 
        }
        public static string GetNameTimeSysTemLaTinh(int data)
        {
            try
            {
                var rsl = "";
                if (data != null)
                {
                    rsl = data < 12 ? "AM" : "PM";
                }
                return rsl;
            }
            catch (Exception e)
            {
                return $"Lỗi: {e.Message}";
            }
        }
    }
}
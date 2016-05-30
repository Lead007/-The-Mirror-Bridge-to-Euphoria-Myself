using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExceptionHelper
{
    public static class ExceptionLogger
    {
        /// <summary>记录发生的异常</summary>
        /// <param name="ex">发生的异常</param>
        public static void Log(this Exception ex)
        {
            var path = Directory.GetCurrentDirectory() + "\\Exception" + DateTime.Now.DateString() + ".log";
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(ex.ToString());
            }
            var result = MessageBox.Show(string.Format("发生异常，已记录至{0},\n是否立即查看？", path), "发生异常",
                MessageBoxButton.OKCancel, MessageBoxImage.Error);
            if (result == MessageBoxResult.OK)
            {
                Process.Start(path);
            }
        }

        private static string DateString(this DateTime time)
        {
            var builder = new StringBuilder(17);
            builder.Append(time.Year);
            builder.Append(time.Month.ToTwoCharString());
            builder.Append(time.Day.ToTwoCharString());
            builder.Append("_");
            builder.Append(time.Hour);
            builder.Append("_");
            builder.Append(time.Minute);
            builder.Append("_");
            builder.Append(time.Second);
            return builder.ToString();
        }

        private static string ToTwoCharString(this int num)
        {
            return num < 10 ? "0" + num : num.ToString();
        }
    }
}

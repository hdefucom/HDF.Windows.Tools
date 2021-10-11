using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HDF.Windows.Tools.Common
{
    public class Log
    {
        static Log()
        {
            LogPath = Application.StartupPath + "\\log.txt";
        }

        readonly static string LogPath;

        public static void Write(string msg)
        {
            File.AppendAllText(LogPath, msg + Environment.NewLine);
            Console.WriteLine(msg);
        }
    }











}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OnTheFlyLog
{
    /// <summary>
    /// Log Writer
    /// </summary>
    public class FlyLog
    {
        /// <summary>
        /// Log Level Type
        /// </summary>
        public enum LogTypeEnum
        {
            /// <summary>
            /// Only Error Log
            /// </summary>
            Error = 1,
            /// <summary>
            /// Important Log and Error
            /// </summary>
            Warning = 2,
            /// <summary>
            /// Debug Mode Log Level All Possible Log 
            /// </summary>
            All = 3,
        }


        private static FlyLogConfiguration config = null;
        private static List<LogClass> codaLog = new List<LogClass>();
        private static object lockObj = new object();

        /// <summary>
        /// Write a Log Information into File
        /// </summary>
        /// <param name="Control">Object fired Log</param>
        /// <param name="LogType">Log Level Type</param>
        /// <param name="LogText">Log Text with a String.Format options</param>
        /// <param name="LogOptionsText">Optional String format addings information</param>
        public static void WriteLog(object Control, LogTypeEnum LogType, string LogText, params object[] LogOptionsText)
        {
            lock (codaLog)
            {
                codaLog.Add(new LogClass()
                {
                    _Control = Control,
                    _LogType = LogType,
                    _LogText = LogText,
                    _LogOptionsText = LogOptionsText
                });
                if (codaLog.Count > 1)
                    return;
            }


            lock (lockObj)
            {
                List<LogClass> coda;
                lock (codaLog)
                {
                    coda = codaLog.Select(item => (LogClass)item.Clone()).ToList();
                    codaLog.Clear();
                }
                Write(coda);
            }
        }
      
        private static void Write(List<LogClass> coda)
        {

            try
            {
                if (config == null)
                    config = new FlyLogConfiguration();

                using (StreamWriter wri = new StreamWriter(config.LogFile.FullName, true))
                {
                    for (int i = 0; i < coda.Count; i++)
                    {
                        LogClass log = coda[i];
                        if (!CheckWrite(log._LogType))
                            return;

                        string LogSource = "";
                        #region Log Source
                        try
                        {
                            if (log._Control is string)
                                LogSource = log._Control as string;
                            else if (log._Control is Type)
                                LogSource = (log._Control as Type).FullName;
                            else
                                LogSource = log._Control.GetType().FullName;
                        }
                        catch (Exception)
                        {
                            LogSource = "";
                        }
                        #endregion

                        wri.WriteLine(string.Format("{0} -> {1} -> {2} -> {3}",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").PadRight(21),
                            LogSource.PadRight(30),
                            log._LogType.ToString().PadRight(7),
                            string.Format(log._LogText, log._LogOptionsText)));
                    }

                }
            }
            catch (Exception)
            {
                //No log Write
            }

        }

        private static bool CheckWrite(LogTypeEnum LogType)
        {
            if (config.DisabledLog)
                return false;
            return ((int)LogType <= (int)config.LogLevel);
        }
    }
}

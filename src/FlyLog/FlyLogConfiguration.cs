using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace OnTheFlyLog
{
    class FlyLogConfiguration
    {
        public FlyLogConfiguration()
        {
            ReadConfig();
        }

        public bool DisabledLog { get; set; }
        public FlyLog.LogTypeEnum LogLevel { get; set; }
        public DirectoryInfo LogPath { get; set; }

        public FileInfo LogFile
        {
            get
            {
                return new FileInfo(string.Format(@"{0}\OnTheFlyLog_{1:yyyyMMddHH}.log", LogPath, DateTime.Now)); 
            }
        }

        public void ReadConfig()
        {
            try
            {
                DisabledLog = false;

                FileInfo Xmlfi =new FileInfo( AppDomain.CurrentDomain.RelativeSearchPath + "\\ServiceConfiguration.xml");
                if (!Xmlfi.Exists) throw new Exception("ServiceConfiguration not Exist");

                 string path = "";
                string level = "";
                using (StreamReader reaXml = new StreamReader(Xmlfi.FullName))
                {
                    XElement XmlConfig = XElement.Parse(reaXml.ReadToEnd());
                    List<XElement> res = (from a in XmlConfig.Descendants()
                              where a.Name.LocalName.ToLower() == "add" && a.Attribute("key") != null
                              && new List<string>() { "LogLocation", "LogLevel" }.Contains(a.Attribute("key").Value)
                              select a).ToList<XElement>();
                    foreach (XElement item in res)
                    {
                        if (item.Attribute("key") != null && item.Attribute("value") != null)
                        {
                            if (item.Attribute("key").Value == "LogLocation")
                                path = item.Attribute("value").Value;
                            if (item.Attribute("key").Value == "LogLevel")
                                level = item.Attribute("value").Value;
                        }
                    }
                }

               

                if (level.ToUpper() == "NONE")
                {
                    DisabledLog = true;
                    return;
                }

                if (path.Contains("%Temp%"))
                    path = path.Replace("%Temp%", Path.GetTempPath());
                else if (path.StartsWith("/") || path.StartsWith("\\"))
                    path = AppDomain.CurrentDomain.RelativeSearchPath + path;
                
                LogPath = new DirectoryInfo(path);
                if (!LogPath.Exists)
                    LogPath.Create();

                LogLevel = (FlyLog.LogTypeEnum)Enum.Parse(typeof(FlyLog.LogTypeEnum), level,true);
            }
            catch (Exception)
            {
                DisabledLog = true;

            }
        }
    }
}

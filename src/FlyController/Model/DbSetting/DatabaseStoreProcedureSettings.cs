using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyController.Model.DbSetting
{
    /// <summary>
    /// Model Class included all names of Store Procedure to call
    /// </summary>
    public class DatabaseStoreProcedureSettings
    {
        /// <summary>
        /// Operations Type ... name will then be converted to run from store procedure
        /// </summary>
        public DBOperationEnum Operation { get; set; }
        /// <summary>
        /// Store Procedure Name
        /// </summary>
        public string StoreProcedureName { get; set; }

        /// <summary>
        /// Get Store Procedure Names from File Config
        /// </summary>
        /// <param name="settingparent">Node whit Store Procedure name information</param>
        /// <returns>a list of DatabaseStoreProcedureSettings <see cref="DatabaseStoreProcedureSettings"/></returns>
        public static List<DatabaseStoreProcedureSettings> GetDbStoreSetting(XmlNode settingparent)
        {
            List<DatabaseStoreProcedureSettings> Settings = new List<DatabaseStoreProcedureSettings>();
            foreach (XmlNode item in settingparent.ChildNodes)
            {
                if (item.Name != "store" || item.Attributes == null || item.Attributes["key"] == null || item.Attributes["name"] == null)
                        continue;
                DBOperationEnum op;
                if (Enum.TryParse <DBOperationEnum>(item.Attributes["key"].Value,true,out op))
                {
                    Settings.Add(new DatabaseStoreProcedureSettings()
                    {
                        Operation = op,
                        StoreProcedureName = item.Attributes["name"].Value
                    });
                }
            }
            return Settings;
        }
    }
}

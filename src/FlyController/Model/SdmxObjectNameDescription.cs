using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyController.Model
{
    /// <summary>
    /// SdmxObjectNameDescription representing the Description Names of all overrided SdmxObjects
    /// </summary>
    public class SdmxObjectNameDescription
    {
        /// <summary>
        /// Languages if LocaleIsoCode
        /// </summary>
        public string Lingua { get; set; }
        /// <summary>
        /// Name description
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Static Function that return this instance from XmlNode
        /// </summary>
        /// <param name="_object">XmlNode to get a Names</param>
        /// <returns>founded List of SdmxObjectNameDescription</returns>
        public static List<SdmxObjectNameDescription> GetNameDescriptions(XmlNode _object)
        {
            List<SdmxObjectNameDescription> descrName = new List<SdmxObjectNameDescription>();
            foreach (XmlNode Nomi in _object.ChildNodes)
            {
                if ((Nomi.Name == "Name" || Nomi.Name == "Description") && Nomi.Attributes != null && Nomi.Attributes["LocaleIsoCode"] != null)
                    descrName.Add(new SdmxObjectNameDescription()
                    {
                        Lingua = Nomi.Attributes["LocaleIsoCode"].Value,
                        Name = Nomi.InnerText
                    });
            }
            return descrName;
        }
    }
}

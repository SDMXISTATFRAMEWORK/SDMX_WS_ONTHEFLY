using FlyController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyDotStat_implementation.Manager
{
    /// <summary>
    /// Object Model contains information of association beetwen Dataset and Category
    /// </summary>
    public class InternalDatasetObject
    {
        /// <summary>
        /// Dataset identify
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Dataflow Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Dataflow Agency
        /// </summary>
        public string Agency { get; set; }
        /// <summary>
        /// Dataflow Version
        /// </summary>
        public string Version { get; set; }




        /// <summary>
        /// Category Code
        /// </summary>
        public string ThemeId { get; set; }
        /// <summary>
        /// Dataset Description Names
        /// </summary>
        public List<SdmxObjectNameDescription> Nomi { get; set; }
        /// <summary>
        /// List of Parent Category Code. From Root Parent to last child
        /// </summary>
        public List<string> CategoryParent { get; set; }

        /// <summary>
        /// String representation of InternalDatasetObject
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Code.ToString();
        }

    }

    /// <summary>
    /// Object Model contains information of Category
    /// </summary>
    public class InternalCategoryObject
    {
        /// <summary>
        /// Category Code
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Category Names description
        /// </summary>
        public List<SdmxObjectNameDescription> Nomi { get; set; }
        /// <summary>
        /// List of Sub Category (childs)
        /// </summary>
        public List<InternalCategoryObject> SubCategory { get; set; }
        /// <summary>
        /// Category Parent Code
        /// </summary>
        public string PadreId { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Nomi != null && Nomi.Count > 0 ? Nomi[0].Name : Id;
        }
    }
}

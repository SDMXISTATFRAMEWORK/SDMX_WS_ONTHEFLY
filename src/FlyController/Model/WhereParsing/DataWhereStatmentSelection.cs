using FlyController.Model.WhereParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Object that representing a DataWhere Statment Selection
    /// </summary>
    public class DataWhereStatmentSelection : IDataWhereStatmentSelection
    {
        /// <summary>
        /// Component Id (Dimension Code or Attribute Code)
        /// </summary>
        public string ComponentId { get; set; }
        /// <summary>
        /// List of string possible Value (in OR)
        /// </summary>
        public List<string> Values { get; set; }
    }

}

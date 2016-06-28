using System;
using System.Collections.Generic;
namespace FlyController.Model.WhereParsing
{
    /// <summary>
    /// Object that representing a DataWhere Statment Selection
    /// </summary>
    public interface IDataWhereStatmentSelection
    {
        /// <summary>
        /// Component Id (Dimension Code or Attribute Code)
        /// </summary>
        string ComponentId { get; set; }
        /// <summary>
        /// List of string possible Value (in OR)
        /// </summary>
        List<string> Values { get; set; }
    }
}

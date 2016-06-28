using System;
namespace FlyController.Model.WhereParsing
{
    /// <summary>
    /// interface for Parse a SDMX Data Query Selection Groups
    /// </summary>
    public interface IDataWhereStatmentSelectionGroup
    {
        /// <summary>
        /// Date From in SdmxDate If in query contains Tipe Period Filter 
        /// </summary>
        Org.Sdmxsource.Sdmx.Api.Model.Base.ISdmxDate DateFrom { get; set; }
        /// <summary>
        /// Date To in SdmxDate If in query contains Tipe Period Filter 
        /// </summary>
        Org.Sdmxsource.Sdmx.Api.Model.Base.ISdmxDate DateTo { get; set; }
        /// <summary>
        /// list of  Selection Where Statment
        /// </summary>
        System.Collections.Generic.List<IDataWhereStatmentSelection> Selections { get; set; }
    }
}

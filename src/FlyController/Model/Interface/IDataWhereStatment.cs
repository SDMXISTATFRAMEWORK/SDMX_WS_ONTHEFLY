using Org.Sdmxsource.Sdmx.Api.Manager.Retrieval;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using System;
using System.Collections.Generic;
namespace FlyController.Model.WhereParsing
{
    /// <summary>
    /// DataWhereStatment Parse a SDMX Data Query
    /// </summary>
    public interface IDataWhereStatment
    {
        /// <summary>
        /// Parse a Query SDMX REST and create SelectionGroups
        /// </summary>
        /// <param name="SdmxRetrievalManager"></param>
        void BuildRestQuery(ISdmxObjectRetrievalManager SdmxRetrievalManager);
        /// <summary>
        /// DimensionAtObservation (Null if not exist in query)
        /// </summary>
        string DimensionAtObservation { get; set; }
        /// <summary>
        /// Flag that determinate if is Rest call
        /// </summary>
        bool IsRest { get; set; }
        /// <summary>
        /// The rest call must be parsed after populate the retreivalManger.
        /// Save the IRestDataQuery in this field for PostParsing
        /// </summary>
        IRestDataQuery Query { get; set; }
        /// <summary>
        /// List of Data Where Statment SelectionGroup Parsed
        /// </summary>
        List<IDataWhereStatmentSelectionGroup> SelectionGroups { get; set; }
    }
}

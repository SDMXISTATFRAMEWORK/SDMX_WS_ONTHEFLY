using FlyController.Model;
using FlyController.Model.Delegate;
using FlyController.Model.WhereParsing;
using FlyMapping.Build;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using System;
using System.Collections.Generic;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for DataMessage Manager
    /// DataMessageManager is class that retrieves data from the database to the requests of DataMessage
    /// </summary>
    public interface IDataMessageManager : IBaseManager
    {
        /// <summary>
        /// Get structured Data Message from Database
        /// </summary>
        /// <param name="idDataset">Dataset Code</param>
        /// <param name="whereStatement">Where condition</param>
        /// <param name="BuilderCallback">delegate to call for write data response</param>
        /// <param name="TimeStamp">LastUpdate parameter request only observation from this date onwards</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        IFlyWriterBody GetTableMessage(string idDataset, IDataWhereStatment whereStatement, WriteResponseDelegate BuilderCallback, string TimeStamp);

        /// <summary>
        /// List of Concept
        /// </summary>
        List<IConceptObjectImpl> Concepts { get; set; }
    }
}

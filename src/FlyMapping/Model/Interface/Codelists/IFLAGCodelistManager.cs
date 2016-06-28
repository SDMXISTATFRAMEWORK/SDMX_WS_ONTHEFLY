using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for FLAG Manager
    /// FLAGManager retrieves the data for FLAG Codelist
    /// </summary>
    public interface IFLAGCodelistManager : IBaseManager
    {
        /// <summary>
        /// retrieves the codelist of FlagDimension (OBS_STATUS) from database
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <param name="flag">Instance of Attribute "AttributeConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        List<ICodeMutableObject> GetFlagCodelist(string DataflowCode, IAttributeConcept flag);
       
    }
}

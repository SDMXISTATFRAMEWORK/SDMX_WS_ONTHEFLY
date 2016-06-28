using FlyEngine.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyMapping.Model
{
     /// <summary>
    /// Interface for Codelists Manager
    /// CodelistsManager retrieves the data for build  Codelist
    /// </summary>
    public interface ICodelistManager : IBaseManager
    {
        /// <summary>
        /// retrieves the codelist Contrain  from database
        /// </summary>
        /// <returns>list of Codelist for SDMXObject</returns>
        List<ICodelistMutableObject> GetCodelistConstrain();

        /// <summary>
        /// retrieves the codelist Contrain from database
        /// </summary>
        /// <returns>list of Codelist for SDMXObject</returns>
        List<ICodelistMutableObject> GetCodelistNoConstrain();


        /// <summary>
        /// retrieves the codelist Contrain from database
        /// </summary>
        /// <param name="refObj">Referenced Objects</param>
        /// <returns>list of Codelist for SDMXObject</returns>
        List<ICodelistMutableObject> GetCodelistReferences(IReferencesObject refObj);
    }
}

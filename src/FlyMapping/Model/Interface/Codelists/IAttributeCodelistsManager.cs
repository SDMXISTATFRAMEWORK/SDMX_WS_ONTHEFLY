using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for Attribute Codelists Manager
    /// AttributeCodelistsManager retrieves the data for build Attribute Codelist
    /// </summary>
    public interface IAttributeCodelistsManager : IBaseManager
    {
         /// <summary>
        /// retrieves the codelist of an attribute from SP Attribute codelist Constrain or from the file "AttributeConcept.xml"
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <param name="attribute">Instance of Attribute "AttributeConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        List<ICodeMutableObject> GetAttributeCodelistConstrain(string DataflowCode, IAttributeConcept attribute);
             /// <summary>
        /// retrieves the codelist of an attribute from SP Attribute codelist NoConstrain or from the file "AttributeConcept.xml"
        /// </summary>
        /// <param name="attribute">Instance of Attribute "AttributeConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        List<ICodeMutableObject> GetAttributeCodelistNoConstrain(IAttributeConcept attribute);
      
    }
}

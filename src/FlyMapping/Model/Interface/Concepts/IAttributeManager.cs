using FlyController.Model;
using System;
using System.Collections.Generic;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for Attribute Manager
    /// Get Attribute Concept from file AttributeConcepts.xml
    /// </summary>
    public interface IAttributeManager : IBaseManager
    {
        /// <summary>
        /// Get Attribute Concept from file AttributeConcepts.xml
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <returns>list of Attribute</returns>
        List<IAttributeConcept> GetAttribute(string DataflowCode);

        /// <summary>
        /// return OBS_VALUE concept
        /// </summary>
        /// <returns></returns>
        IAttributeConcept GetObsValue();
    }
}

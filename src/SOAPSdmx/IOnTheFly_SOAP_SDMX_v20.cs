using FlyController.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using SOAPSdmx.Builder;
using SOAPSdmx.Model;

namespace SOAPSdmx
{
    /// <summary>
    /// The SOAP SDMX v2.0 interface.
    /// On The Fly endpoint for data and metadata SOAP Sdmx 2.0
    /// </summary>
    [ServiceContract(Namespace = Constant.ServiceNamespace, Name = Constant.ContractNamespaceSdmx20, ConfigurationName = Constant.ContractNamespaceSdmx20, SessionMode = SessionMode.NotAllowed)]
    [DispatchByBodyElementBehavior]
    public interface IOnTheFly_SOAP_SDMX_v20
    {
        /// <summary>
        /// Request a CompactData (SDMX 2.0)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// retrun Compact Data message
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetCompactData", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetCompactData(Message query);


        ///// <summary>
        ///// Request a CrossSectionalData (SDMX 2.0)
        ///// </summary>
        ///// <param name="query">Query Message</param>
        ///// <returns>
        ///// Soap SDMX result
        ///// retrun CrossSectional Data message
        ///// </returns>
        //[OperationContract]
        //[FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ContractNamespaceSdmx20)]
        // Message GetCrossSectionalData(Message query);

        /// <summary>
        /// Request a GenericData (SDMX 2.0)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// retrun Generic Data message
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetGenericData", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetGenericData(Message query);

        /// <summary>
        /// Request a QueryStructure accept all RegistryInterface for get metadatas (SDMX 2.0)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// return Sdmx Metadata (agencyScheme, Dataflows, CategoryScheme, ConceptScheme, Codelist, KeyFamily)
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("QueryStructure", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message QueryStructure(Message query);

        /// <summary>
        /// Web Method that is used to retrieve <c>SDMX</c> structure based on a <c>SDMX</c> query
        /// </summary>
        /// <param name="request">
        /// The <c>SDMX</c> query
        /// </param>
        /// <returns>
        /// The queried structure in <c>SDMX</c> SDMX-ML v2.1  format
        /// </returns>
        [OperationContract(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message DefaultHandler(Message request);

    }

}

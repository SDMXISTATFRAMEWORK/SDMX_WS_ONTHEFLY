using FlyController.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using SOAPSdmx.Builder;
using SOAPSdmx.Model;

namespace SOAPSdmx
{
    /// <summary>
    /// The SOAP SDMX v2.1 interface.
    /// On The Fly endpoint for data and metadata SOAP Sdmx 2.1
    /// </summary>
    [ServiceContract(Namespace = Constant.ServiceNamespace, Name = Constant.ContractNamespaceSdmx21, ConfigurationName = Constant.ContractNamespaceSdmx21, SessionMode = SessionMode.NotAllowed)]
    [DispatchByBodyElementBehavior]
    public interface IOnTheFly_SOAP_SDMX_v21
    {

        /// <summary>
        /// Request a Structure GenericData TimeSeries Data (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Structure GenericData TimeSeries Data
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetGenericTimeSeriesData", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetGenericTimeSeriesData(Message query);

        /// <summary>
        /// Request a Structure GenericData Data (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Structure GenericData Data
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetGenericData", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetGenericData(Message query);

        /// <summary>
        /// Request a Structure StructureSpecific Data (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Structure StructureSpecific Data
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetStructureSpecificData", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetStructureSpecificData(Message query);

        /// <summary>
        /// Request a Structure StructureSpecific TimeSeries Data (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Structure StructureSpecific TimeSeries Data
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetStructureSpecificTimeSeriesData", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetStructureSpecificTimeSeriesData(Message query);

        /// <summary>
        /// Request a Data Structure (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// DataStructure Metadata
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetStructures", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetStructures(Message query);

        /// <summary>
        /// Request a Data Structure (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// DataStructure Metadata
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetDataStructure", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetDataStructure(Message query);

        /// <summary>
        /// Request a Categorisation (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Categorisation Metadata
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetCategorisation", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetCategorisation(Message query);

        /// <summary>
        /// Request a Dataflows (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Dataflows Metadata
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetDataflow", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetDataflow(Message query);

        /// <summary>
        /// Request a CategoryScheme (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// CategoryScheme Metadata
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetCategoryScheme", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetCategoryScheme(Message query);

        /// <summary>
        /// Request a ConceptScheme (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// ConceptScheme Metadata
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetConceptScheme", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetConceptScheme(Message query);

        /// <summary>
        /// Request a Codelist (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Codelist Metadata
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetCodelist", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetCodelist(Message query);

        ///// <summary>
        ///// Request a HierarchicalCodelist (SDMX 2.1)
        ///// </summary>
        ///// <param name="query">Query Message</param>
        ///// <returns>
        ///// Soap SDMX result
        ///// HierarchicalCodelist Metadata
        ///// </returns>
        //[OperationContract]
        //[FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ContractNamespaceSdmx21)]
        //Message GetHierarchicalCodelist(Message query);

        /// <summary>
        /// Request a OrganisationScheme (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// OrganisationScheme Metadata
        /// </returns>
        [OperationContract(Action = "", ReplyAction = "*")]
        [DispatchBodyElement("GetOrganisationScheme", Constant.ServiceNamespace)]
        [FaultContract(typeof(SdmxFaultError), Name = "Error", Namespace = Constant.ServiceNamespace)]
        Message GetOrganisationScheme(Message query);


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

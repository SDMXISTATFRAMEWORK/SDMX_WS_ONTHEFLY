using FlyController;
using FlyController.Model.Error;
using FlyController.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using SOAPSdmx.Builder;
using SOAPSdmx.Model;

namespace SOAPSdmx
{
    /// <summary>
    /// Impementation of EntryPoint On The Fly Application for SOAP SDMX 2.1
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class OnTheFly_SOAP_SDMX_v21 : IOnTheFly_SOAP_SDMX_v21
    {
        /// <summary>
        /// Create a instance of WS_SDMX_SOAP20 and read a Configuration file
        /// </summary>
        public OnTheFly_SOAP_SDMX_v21()
        {
            FlyConfiguration.InitConfig(AppDomain.CurrentDomain.RelativeSearchPath);
        }

        /// <summary>
        /// Web Method that is used to retrieve <c>SDMX</c> structure based on a <c>SDMX</c> query
        /// </summary>
        /// <param name="request">
        /// The <c>SDMX</c> query
        /// </param>
        /// <returns>
        /// The queried structure in <c>SDMX</c> SDMX-ML v2.1  format
        /// </returns>
        public Message DefaultHandler(Message request)
        {
            throw SdmxFaultError.BuildException(new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.NotImplemented, new Exception("Default Handler")));
        }

        /// <summary>
        /// Request a Structure GenericData TimeSeries Data (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Structure GenericData TimeSeries Data
        /// </returns>
        public Message GetGenericTimeSeriesData(Message query)
        {

            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetGenericTimeSeriesData);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a Structure GenericData Data (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Structure GenericData Data
        /// </returns>
        public Message GetGenericData(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetGenericData);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a Structure StructureSpecific Data (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Structure StructureSpecific Data
        /// </returns>
        public Message GetStructureSpecificData(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetStructureSpecificData);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a Structure StructureSpecific TimeSeries Data (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Structure StructureSpecific TimeSeries Data
        /// </returns>
        public Message GetStructureSpecificTimeSeriesData(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetStructureSpecificTimeSeriesData);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a Data Structure (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// DataStructure Metadata
        /// </returns>
        public Message GetStructures(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetStructures);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a Data Structure (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// DataStructure Metadata
        /// </returns>
        public Message GetDataStructure(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetStructures);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a Dataflows (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Dataflows Metadata
        /// </returns>
        public Message GetDataflow(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetDataflow);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a CategoryScheme (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// CategoryScheme Metadata
        /// </returns>
        public Message GetCategoryScheme(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetCategoryScheme);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a Categorisation (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Categorisation Metadata
        /// </returns>
        public Message GetCategorisation(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetCategorisation);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a ConceptScheme (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// ConceptScheme Metadata
        /// </returns>
        public Message GetConceptScheme(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetConceptScheme);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a Codelist (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// Codelist Metadata
        /// </returns>
        public Message GetCodelist(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetCodelist);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        ///// <summary>
        ///// Request a HierarchicalCodelist (SDMX 2.1)
        ///// </summary>
        ///// <param name="query">Query Message</param>
        ///// <returns>
        ///// Soap SDMX result
        ///// HierarchicalCodelist Metadata
        ///// </returns>
        //public Message GetHierarchicalCodelist(Message query)
        //{
        //    try
        //    {
        //        QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetHierarchicalCodelist);
        //        return qp.Parse(query);
        //    }
        //    catch (SdmxException e)
        //    {
        //        throw SdmxFaultError.BuildException(e);
        //    }
        //}

        /// <summary>
        /// Request a OrganisationScheme (SDMX 2.1)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// OrganisationScheme Metadata
        /// </returns>
        public Message GetOrganisationScheme(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser21(QueryOperation.MessageTypeEnum.GetOrganisationScheme);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

    }
}

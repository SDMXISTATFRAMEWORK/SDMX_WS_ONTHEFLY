using FlyController.Utils;
using FlyController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using SOAPSdmx.Builder;
using SOAPSdmx.Model;
using FlyController.Model;
using FlyController.Model.Error;
using System.ServiceModel.Activation;

namespace SOAPSdmx
{
    /// <summary>
    /// Impementation of EntryPoint On The Fly Application for SOAP SDMX 2.0
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class OnTheFly_SOAP_SDMX_v20 : IOnTheFly_SOAP_SDMX_v20
    {
        /// <summary>
        /// Create a instance of WS_SDMX_SOAP20 and read a Configuration file
        /// </summary>
        public OnTheFly_SOAP_SDMX_v20()
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
            throw SdmxFaultError.BuildException(new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.NotImplemented,new Exception("Default Handler")));
        }

        /// <summary>
        /// Request a CompactData (SDMX 2.0)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// retrun Compact Data message
        /// </returns>
        public Message GetCompactData(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser20(QueryOperation.MessageTypeEnum.GetCompactData);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        ///// <summary>
        ///// Request a CrossSectionalData (SDMX 2.0)
        ///// </summary>
        ///// <param name="query">Query Message</param>
        ///// <returns>
        ///// Soap SDMX result
        ///// retrun CrossSectional Data message
        ///// </returns>
        //public Message GetCrossSectionalData(Message query)
        //{
        //    try
        //    {
        //        QueryParser qp = new QueryParser20(QueryOperation.MessageTypeEnum.GetCrossSectionalData);
        //        return qp.Parse(query);
        //    }
        //    catch (SdmxException e)
        //    {
        //        throw SdmxFaultError.BuildException(e);
        //    }
        //}

        /// <summary>
        /// Request a GenericData (SDMX 2.0)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// retrun Generic Data message
        /// </returns>
        public Message GetGenericData(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser20(QueryOperation.MessageTypeEnum.GetGenericData);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }
        }

        /// <summary>
        /// Request a QueryStructure accept all RegistryInterface for get metadatas (SDMX 2.0)
        /// </summary>
        /// <param name="query">Query Message</param>
        /// <returns>
        /// Soap SDMX result
        /// return Sdmx Metadata (agencyScheme, Dataflows, CategoryScheme, ConceptScheme, Codelist, KeyFamily)
        /// </returns>
        public Message QueryStructure(Message query)
        {
            try
            {
                QueryParser qp = new QueryParser20(QueryOperation.MessageTypeEnum.QueryStructure);
                return qp.Parse(query);
            }
            catch (SdmxException e)
            {
                throw SdmxFaultError.BuildException(e);
            }

        }
    }


}

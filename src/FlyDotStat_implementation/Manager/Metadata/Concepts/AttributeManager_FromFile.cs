using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyDotStat_implementation.Manager.Metadata
{
    /// <summary>
    /// Get Attribute Concept from file AttributeConcepts.xml
    /// </summary>
    public class AttributeManager_FromFile : BaseManager, IAttributeManager
    {
         /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public AttributeManager_FromFile(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            :base(_parsingObject, _versionTypeResp){}

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }
       
        /// <summary>
        /// Get Attribute Concept from file AttributeConcepts.xml
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <returns>list of Attribute</returns>
        public List<IAttributeConcept> GetAttribute(string DataflowCode)
        {
            try
            {
                List<IAttributeConcept> Attributes = new List<IAttributeConcept>();
                try
                {
                    XmlDocument configuration = new XmlDocument();
                    configuration.Load(FlyConfiguration.AttributeFile);
                    foreach (XmlNode item in configuration.ChildNodes[0].ChildNodes)
                    {
                        if (item.Name != "Attribute" && item.Attributes == null || item.Attributes["Code"] == null || item.Attributes["attachmentLevel"] == null || item.Attributes["assignmentStatus"] == null)
                            continue;

                        IAttributeConcept AttributeConcept = new AttributeConcept(item.Attributes["Code"].Value, SdmxObjectNameDescription.GetNameDescriptions(item));
                        AttributeConcept.AttributeAttachmentLevelType = (AttributeAttachmentLevel)Enum.Parse(typeof(AttributeAttachmentLevel), item.Attributes["attachmentLevel"].Value, true);
                        AttributeConcept.AssignmentStatusType = (AssignmentStatusTypeEnum)Enum.Parse(typeof(AssignmentStatusTypeEnum), item.Attributes["assignmentStatus"].Value, true);
                        Attributes.Add(AttributeConcept);
                    }

                }
                catch (SdmxException) { throw; }
                catch (Exception ex)
                {
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetAttributeError, ex);
                }

                return Attributes;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetListConcepts, ex);
            }
        }

        /// <summary>
        /// return OBS_VALUE concept
        /// </summary>
        /// <returns></returns>
        public IAttributeConcept GetObsValue()
        {
            IAttributeConcept ObsValue = new AttributeConcept("OBS_VALUE", 
                new List<SdmxObjectNameDescription> 
                {
                    new SdmxObjectNameDescription(){Lingua="en",Name="Observation Value, primary measure"}
                
                })
            {
                IsValueAttribute = true,
                AssignmentStatusType = AssignmentStatusTypeEnum.Mandatory,
                AttributeAttachmentLevelType = Org.Sdmxsource.Sdmx.Api.Constants.AttributeAttachmentLevel.Observation
            };
            return ObsValue;
        }
    }
}

using FlyController;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyDotStat_implementation.Manager.Metadata
{
    /// <summary>
    /// Get Attribute Concept from Store Procedure
    /// </summary>
    public class AttributeManagerSP : BaseManager, IAttributeManager
    {
         /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public AttributeManagerSP(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
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

            List<IAttributeConcept> Attributes = new List<IAttributeConcept>();
            try
            {
                List<IParameterValue> parametri = new List<IParameterValue>() { new ParameterValue() { Item = "Code", Value = DataflowCode } };
                if (!string.IsNullOrEmpty(this.parsingObject.TimeStamp))
                    parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.parsingObject.TimeStamp, SqlType = SqlDbType.DateTime });

                //EFFETTUO LA RICHIESTA AL DB
                List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.GetAttributes, parametri);

                //PARSO LA RISPOSTA E CREO L'OGGETTO
                if (risposta.Count == 1 && risposta[0].Name == "DataSet")
                {
                    foreach (XmlNode Attribute in risposta[0].ChildNodes)
                    {
                        if (Attribute.Attributes == null || Attribute.Attributes["Code"] == null)
                            continue;

                        IAttributeConcept newconcept = new AttributeConcept(Attribute.Attributes["Code"].Value, SdmxObjectNameDescription.GetNameDescriptions(Attribute));
                        if (Attribute.Attributes["assignmentStatus"] != null )
                            newconcept.AssignmentStatusType = (AssignmentStatusTypeEnum)Enum.Parse(typeof(AssignmentStatusTypeEnum), Attribute.Attributes["assignmentStatus"].Value, true);
                        if (Attribute.Attributes["attachmentLevel"] != null)
                            newconcept.AttributeAttachmentLevelType = (AttributeAttachmentLevel)Enum.Parse(typeof(AttributeAttachmentLevel), Attribute.Attributes["attachmentLevel"].Value, true);
                        Attributes.Add(newconcept);
                    }
                }

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetAttributeError, ex);
            }

            return Attributes;

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

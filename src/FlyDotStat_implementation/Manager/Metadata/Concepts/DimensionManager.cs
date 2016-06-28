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
    /// DimensionManager retrieves the data for build conceptScheme
    /// </summary>
    public class DimensionManager : BaseManager, IDimensionManager
    {
         /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public DimensionManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            :base(_parsingObject, _versionTypeResp){}

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// retrieves all Concept Dimension from database
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <param name="Names">return Dataflow NameDescription</param>
        /// <returns>list of Dimension</returns>
        public List<IDimensionConcept> GetDimensionConceptObjects(string DataflowCode, out List<SdmxObjectNameDescription> Names)
        {
            try
            {
                Names = new List<SdmxObjectNameDescription>();
                List<IDimensionConcept> dimConcepts = new List<IDimensionConcept>();

                List<IParameterValue> parametri = new List<IParameterValue>() { new ParameterValue() { Item = "Code", Value = DataflowCode } };
                if (!string.IsNullOrEmpty(this.parsingObject.TimeStamp))
                    parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.parsingObject.TimeStamp, SqlType = SqlDbType.DateTime });
                
                //EFFETTUO LA RICHIESTA AL DB
                List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.GetDimensions, parametri);

                //PARSO LA RISPOSTA E CREO L'OGGETTO
                if (risposta.Count == 1 && risposta[0].Name == "DataSet")
                {

                    Names = SdmxObjectNameDescription.GetNameDescriptions(risposta[0]);
                    foreach (XmlNode Dimension in risposta[0].ChildNodes)
                    {
                        if (Dimension.Attributes == null || Dimension.Attributes["Code"] == null)
                            continue;

                        IDimensionConcept newconcept = new DimensionConcept(Dimension.Attributes["Code"].Value, SdmxObjectNameDescription.GetNameDescriptions(Dimension));
                        if (Dimension.Attributes["IsTimeSeriesDim"] != null && Dimension.Attributes["IsTimeSeriesDim"].Value.Trim().ToLower() == "true")
                            newconcept.DimensionType = DimensionTypeEnum.Time;
                        dimConcepts.Add(newconcept);
                    }
                }

                if (!dimConcepts.Exists(co => co.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)co).DimensionType == DimensionTypeEnum.Frequency))
                {

                    IDimensionConcept Freq = new DimensionConcept("FREQ", GetFakeFreqNames()) { IsFakeFrequency = true };
                    dimConcepts.Add(Freq);

                }

                return dimConcepts;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetDimensionConceptObjects, ex);
            }

        }

        /// <summary>
        /// If not Exist Frequency Dimension in database, the application adds it because it is mandatory and takes the Names Description from the file "FrequencyCodelist.xml"
        /// </summary>
        /// <returns></returns>
        private List<SdmxObjectNameDescription> GetFakeFreqNames()
        {
            try
            {
                XmlDocument configuration = new XmlDocument();
                configuration.Load(FlyConfiguration.FrequencyCodelistFile);
                return SdmxObjectNameDescription.GetNameDescriptions(configuration.ChildNodes[0]);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetFrequencyCodelist, ex);
            }

        }
    }
}

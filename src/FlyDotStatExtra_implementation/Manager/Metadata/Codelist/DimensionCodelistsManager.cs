using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.Codelist;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Objects.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlyDotStatExtra_implementation.Manager.Metadata
{
    /// <summary>
    /// CodelistsManager retrieves the data for build  Codelist
    /// </summary>
    public class DimensionCodelistsManager : BaseManager//, IDimensionCodelistsManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public DimensionCodelistsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// retrieves the codelist Contrain of Dimension from database
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <param name="dimension">Instance of Dimension "DimensionConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodeMutableObject> GetDimensionCodelistContrain(string DataflowCode, IDimensionConcept dimension)
        {
            try
            {
                if (dimension.DimensionType == DimensionTypeEnum.Time)
                    return new List<ICodeMutableObject>();
                if (dimension.DimensionType == DimensionTypeEnum.Frequency)
                {
                    if (dimension.IsFakeFrequency)
                    {
                        SpecialCodelistsManager sp = new SpecialCodelistsManager(this.parsingObject, this.versionTypeResp);
                        return sp.GetFrequencyCodelist(DataflowCode);
                    }
                    else
                        return InternalGetDimensionCodelistConstrain(DataflowCode, dimension.RealNameFreq);
                }

                return InternalGetDimensionCodelistConstrain(DataflowCode, dimension.ConceptObjectCode);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICodeMutableObject, ex);
            }
        }

        /// <summary>
        /// retrieves the codelist Contrain of Dimension from database
        /// </summary>
        /// <param name="dimension">Instance of Dimension "DimensionConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodeMutableObject> GetDimensionCodelistNoContrain(IDimensionConcept dimension)
        {
            try
            {
                if (this.DbAccess.CheckExistStoreProcedure(DBOperationEnum.GetDimensionCodelistNOConstrain))
                {
                    if (dimension.DimensionType == DimensionTypeEnum.Time)
                        return new List<ICodeMutableObject>();
                    if (dimension.DimensionType == DimensionTypeEnum.Frequency)
                    {
                        if (dimension.IsFakeFrequency)
                        {
                            SpecialCodelistsManager sp = new SpecialCodelistsManager(this.parsingObject, this.versionTypeResp);
                            return sp.GetFrequencyCodelist();
                        }
                        else
                            return InternalGetDimensionCodelistNOConstrain(dimension.CodelistCode);
                    }

                    return InternalGetDimensionCodelistNOConstrain(dimension.CodelistCode);
                }
                else
                {
                    if (FlyConfiguration.CodelistWhitoutConstrain)
                    {
                        IDataflowsManager dfMan = new MetadataFactory().InstanceDataflowsManager(this.parsingObject.CloneForReferences(), this.versionTypeResp);
                        List<ICodeMutableObject> listCode = new List<ICodeMutableObject>();
                        var _foundedDataflow = dfMan.GetDataFlows();
                        List<SdmxObjectNameDescription> nomiDf = new List<SdmxObjectNameDescription>();
                        foreach (var df in _foundedDataflow)
                        {
                            string dfCode = df.Id;
                            DimensionManager dim = new DimensionManager(this.parsingObject, this.versionTypeResp);
                            List<IDimensionConcept> conc = dim.GetDimensionConceptObjects(dfCode, out nomiDf);
                            if (conc.Exists(c => c.Id == dimension.Id))
                            {
                                foreach (ICodeMutableObject item in GetDimensionCodelistContrain(dfCode, conc.Find(c => c.Id == dimension.Id)))
                                {
                                    if (!listCode.Exists(cl => cl.Id == item.Id))
                                        listCode.Add(item);
                                }
                            }

                        }
                        return listCode;
                    }
                    else
                        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CodelistContrainRequired);
                }

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICodeMutableObject, ex);
            }
        }




        internal List<ICodeMutableObject> InternalGetDimensionCodelistConstrain(string DataflowCode, string ConceptId)
        {
            try
            {
                List<ICodeMutableObject> _CodesObjects = new List<ICodeMutableObject>();
                //prima capisco se non è un attributo o un flag
                List<IParameterValue> parametri = new List<IParameterValue>() 
                { 
                    new ParameterValue() { Item = "DatasetCode", Value = DataflowCode } ,
                    new ParameterValue() { Item = "DimCode", Value = ConceptId} ,
                    new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                    new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
                };
                if (!string.IsNullOrEmpty(this.parsingObject.TimeStamp))
                    parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.parsingObject.TimeStamp, SqlType = SqlDbType.DateTime });
                //EFFETTUO LA RICHIESTA AL DB

                List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.GetDimensionCodelistConstrain, parametri);


                ////PARSO LA RISPOSTA E CREO L'OGGETTO
                if (risposta.Count == 1 && risposta[0].Name == "Codelist" && risposta[0].ChildNodes.Count > 0)
                {
                    _CodesObjects.AddRange(GetRecurviveDimension(risposta[0].ChildNodes, null));
                }

                return _CodesObjects;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICodeMutableObject, ex);
            }
        }
        internal List<ICodeMutableObject> InternalGetDimensionCodelistNOConstrain(string CodelistTable)
        {
            try
            {
                List<ICodeMutableObject> _CodesObjects = new List<ICodeMutableObject>();
                //prima capisco se non è un attributo o un flag
                List<IParameterValue> parametri = new List<IParameterValue>() 
                { 
                    new ParameterValue() { Item = "DimTableName", Value = CodelistTable} ,
                    new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                    new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
                };
                if (!string.IsNullOrEmpty(this.parsingObject.TimeStamp))
                    parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.parsingObject.TimeStamp, SqlType = SqlDbType.DateTime });
                //EFFETTUO LA RICHIESTA AL DB

                List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.GetDimensionCodelistNOConstrain, parametri);


                ////PARSO LA RISPOSTA E CREO L'OGGETTO
                if (risposta.Count == 1 && risposta[0].Name == "Codelist" && risposta[0].ChildNodes.Count > 0)
                {
                    _CodesObjects.AddRange(GetRecurviveDimension(risposta[0].ChildNodes, null));
                }

                return _CodesObjects;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICodeMutableObject, ex);
            }
        }

        /// <summary>
        /// Get hierachical codelist
        /// </summary>
        /// <param name="xmlNodeList">list of XmlNode (child nodes)</param>
        /// <param name="ParentCode">dimension Parent Code</param>
        /// <returns>list of Mutable Code Object</returns>
        private List<ICodeMutableObject> GetRecurviveDimension(XmlNodeList xmlNodeList, string ParentCode)
        {
            try
            {
                List<ICodeMutableObject> dim = new List<ICodeMutableObject>();

                foreach (XmlNode Member in xmlNodeList)
                {
                    if (((Member.Name != "Codelist" && Member.Name != "ChildMember" && Member.Name != "Member")) || Member.Attributes == null || Member.Attributes["Code"] == null)
                        continue;
                    string code = Member.Attributes["Code"].Value;

                    dim.Add(CodelistItemBuilder.BuildCodeObjects(Member.Attributes["Code"].Value, SdmxObjectNameDescription.GetNameDescriptions(Member), ParentCode));

                    dim.AddRange(GetRecurviveDimension(Member.ChildNodes, code));
                }
                return dim;
            }
            catch (SdmxException) { throw; }
            catch (Exception)
            {
                return new List<ICodeMutableObject>();
            }
        }

    }
}

using FlyMapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyController.Model;
using FlyMapping.Build;
using FlyController;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;
using System.Xml;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using System.Data;
using Org.Sdmxsource.Sdmx.Api.Constants;
using FlyEngine.Model;
using FlyController.Builder;

namespace FlyDotStat_implementation.Manager.Metadata
{
     /// <summary>
    /// FLAGManager retrieves the data for Concept FLAG
    /// </summary>
    public class FLAGCodelistManager : BaseManager//, IFLAGCodelistManager
    {
        
         /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public FLAGCodelistManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            :base(_parsingObject, _versionTypeResp){}

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// retrieves the codelist of FlagDimension (OBS_STATUS) from database
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <param name="flag">Instance of Attribute "AttributeConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodeMutableObject> GetFlagCodelist(string DataflowCode, IAttributeConcept flag)
        {
            try
            {
                if (!this.DbAccess.CheckExistStoreProcedure(DBOperationEnum.GetFlags))
                    return new List<ICodeMutableObject>();
                List<ICodeMutableObject> _CodesObjects = new List<ICodeMutableObject>();

                //prima capisco se non è un attributo o un flag
                List<IParameterValue> parametri = new List<IParameterValue>() 
                { 
                    new ParameterValue() { Item = "Code", Value = DataflowCode} ,
                };
                if (!string.IsNullOrEmpty(this.parsingObject.TimeStamp))
                    parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.parsingObject.TimeStamp, SqlType = SqlDbType.DateTime });

                //EFFETTUO LA RICHIESTA AL DB
                List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.GetFlags, parametri);


                ////PARSO LA RISPOSTA E CREO L'OGGETTO
                if (risposta.Count == 1 && risposta[0].Name == "DataSet" && risposta[0].ChildNodes.Count > 0)
                {
                    foreach (XmlNode Member in risposta[0])
                    {
                        if (Member.Name != "Flag" || Member.Attributes == null || Member.Attributes["Code"] == null)
                            continue;
                        string code = Member.Attributes["Code"].Value;
                        _CodesObjects.Add(CodelistItemBuilder.BuildCodeObjects(Member.Attributes["Code"].Value, SdmxObjectNameDescription.GetNameDescriptions(Member), null));
                    }
                }
                return _CodesObjects;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICodeMutableObject, ex);
            }
        }

    }
}

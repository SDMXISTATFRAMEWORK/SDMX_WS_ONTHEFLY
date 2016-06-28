using FlyController;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyMapping.Build;
using FlyMapping.Model;
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
using Org.Sdmxsource.Sdmx.Api.Constants;
using FlyEngine.Model;
using FlyController.Builder;

namespace FlyDotStatExtra_implementation.Manager.Metadata
{
    /// <summary>
    /// CodelistsManager retrieves the data for build  Codelist
    /// </summary>
    public class AttributeCodelistsManager : BaseManager//, IAttributeCodelistsManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public AttributeCodelistsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            : base(_parsingObject, _versionTypeResp) { }

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// retrieves the codelist of an attribute from SP Attribute codelist Constrain or from the file "AttributeConcept.xml"
        /// </summary>
        /// <param name="DataflowCode">Dataflow Code</param>
        /// <param name="attribute">Instance of Attribute "AttributeConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodeMutableObject> GetAttributeCodelistConstrain(string DataflowCode, IAttributeConcept attribute)
        {
            //prima capisco se non è un attributo o un flag
            List<IParameterValue> parametri = new List<IParameterValue>() 
                { 
                    new ParameterValue() { Item = "DatasetCode", Value = DataflowCode } ,
                    new ParameterValue() { Item = "AttributeCode", Value = attribute.Id} ,
                    new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                    new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
                };
            if (!string.IsNullOrEmpty(this.parsingObject.TimeStamp))
                parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.parsingObject.TimeStamp, SqlType = SqlDbType.DateTime });
            //EFFETTUO LA RICHIESTA AL DB

            List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.GetAttributeCodelistConstrain, parametri);
            return InternalGetAttributeCodelist(risposta);

        }

        /// <summary>
        /// retrieves the codelist of an attribute from SP Attribute codelist NoConstrain or from the file "AttributeConcept.xml"
        /// </summary>
        /// <param name="CodelistTable">CodelistCode of Attribute "AttributeConcept"</param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodeMutableObject> GetAttributeCodelistNoConstrain(string CodelistTable)
        {

            //prima capisco se non è un attributo o un flag
            List<IParameterValue> parametri = new List<IParameterValue>() 
                { 
                    new ParameterValue() { Item = "CodeListTable", Value = CodelistTable} ,
                    new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                    new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
                };
            if (!string.IsNullOrEmpty(this.parsingObject.TimeStamp))
                parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.parsingObject.TimeStamp, SqlType = SqlDbType.DateTime });
            //EFFETTUO LA RICHIESTA AL DB

            List<XmlNode> risposta = this.DbAccess.Execute(DBOperationEnum.GetAttributeCodelistNOConstrain, parametri);
            return InternalGetAttributeCodelist(risposta);

        }


        private List<ICodeMutableObject> InternalGetAttributeCodelist(List<XmlNode> risposta)
        {
            try
            {

                List<ICodeMutableObject> _CodesObjects = new List<ICodeMutableObject>();

                ////PARSO LA RISPOSTA E CREO L'OGGETTO
                if (risposta.Count == 1 && risposta[0].Name == "Codelist" && risposta[0].ChildNodes.Count > 0)
                {
                    _CodesObjects.AddRange(GetRecurviveAttribute(risposta[0].ChildNodes, null));
                }

                return _CodesObjects;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateICodeMutableObject, ex);
            }
        }
        private List<ICodeMutableObject> GetRecurviveAttribute(XmlNodeList xmlNodeList, string ParentCode)
        {
            try
            {
                List<ICodeMutableObject> dim = new List<ICodeMutableObject>();

                foreach (XmlNode Member in xmlNodeList)
                {
                    if (((Member.Name != "Attribute" && Member.Name != "Member")) || Member.Attributes == null || Member.Attributes["Code"] == null)
                        continue;
                    string code = Member.Attributes["Code"].Value;

                    dim.Add(CodelistItemBuilder.BuildCodeObjects(Member.Attributes["Code"].Value, SdmxObjectNameDescription.GetNameDescriptions(Member), ParentCode));

                    dim.AddRange(GetRecurviveAttribute(Member.ChildNodes, code));
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

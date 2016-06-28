using FlyController;
using FlyController.Builder;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyDotStat_implementation.Build;
using FlyDotStat_implementation.Manager.Data;
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
    /// SpecialCodelistsManager retrieves the data for build  Codelist:
    /// Frequency where not Dimension is present (From file), CL_COUNT, CL_TIME_PERIOD...
    /// </summary>
    public class SpecialCodelistsManager : BaseManager//, ISpecialCodelistsManager
    {

         /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_parsingObject">the <see cref="ISdmxParsingObject"/></param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public SpecialCodelistsManager(ISdmxParsingObject _parsingObject, SdmxSchemaEnumType _versionTypeResp)
            :base(_parsingObject, _versionTypeResp){}

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// retrieves the codelist of Frequency dimension from the file "FrequencyCodelist.xml"
        /// </summary>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodeMutableObject> GetFrequencyCodelist()
        {
            List<ICodeMutableObject> FrequencyCodesObjects = new List<ICodeMutableObject>();
            try
            {
                XmlDocument configuration = new XmlDocument();
                configuration.Load(FlyConfiguration.FrequencyCodelistFile);
                foreach (XmlNode item in configuration.ChildNodes[0].ChildNodes)
                {
                    if (item.Name != "Code" && item.Attributes == null || item.Attributes["Code"] == null)
                        continue;
                    FrequencyCodesObjects.Add(CodelistItemBuilder.BuildCodeObjects(item.Attributes["Code"].Value, SdmxObjectNameDescription.GetNameDescriptions(item), null));
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetFrequencyCodelist, ex);
            }

            return FrequencyCodesObjects;

        }

        /// <summary>
        /// retrieves the codelist of Frequency dimension from AllData in DB
        /// </summary>
        /// <param name="_dataFlowCode">Dataflow Code</param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodeMutableObject> GetFrequencyCodelist(string _dataFlowCode)
        {
            ISpecialConcept specialConcept = new SpecialConcept("FREQ", SpecialTypeEnum.CL_CONTRAINED);
            return SpecialCodelist_CL_CONTRAINED(_dataFlowCode, specialConcept);

        }
        /// <summary>
        /// retrieves the codelist constrained
        /// </summary>
        /// <param name="_dataFlowCode">Dataflow Code</param>
        /// <param name="specialConcept">the <see cref="ISpecialConcept"/></param>
        /// <returns>list of Mutable Code Object</returns>
        public List<ICodeMutableObject> GetSpecialCodelist(string _dataFlowCode, ISpecialConcept specialConcept)
        {
            List<IParameterValue> parametri = new List<IParameterValue>()
            {
                new ParameterValue() {Item="DataSetCode",Value= _dataFlowCode},
                new ParameterValue() {Item="WhereStatement",Value="1=1"},
                new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
            };
            List<ICodeMutableObject> codes = new List<ICodeMutableObject>();
            switch (specialConcept.SpecialType)
            {
                case SpecialTypeEnum.CL_TIME_PERIOD:
                    codes = SpecialCodelist_CL_TIME_PERIOD(_dataFlowCode);
                    break;
                case SpecialTypeEnum.CL_COUNT:
                    codes = SpecialCodelist_CL_COUNT(_dataFlowCode);
                    break;
                case SpecialTypeEnum.CL_CONTRAINED:
                    codes = SpecialCodelist_CL_CONTRAINED(_dataFlowCode, specialConcept);
                    break;
            }
            return codes;
        }



        private List<ICodeMutableObject> SpecialCodelist_CL_TIME_PERIOD(string _dataFlowCode)
        {
            List<IParameterValue> parametri = new List<IParameterValue>()
            {
                new ParameterValue() {Item="DataSetCode",Value= _dataFlowCode},
                new ParameterValue() {Item="WhereStatement",Value="1=1"},
                new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
            };
            List<ICodeMutableObject> codes = new List<ICodeMutableObject>();
            string StartDate = null;
            string EndDate = null;

            ((DWHAccess)DbAccess).GetCL_TIME_PERIOD(parametri, out StartDate, out EndDate);

            codes.Add(CodelistItemBuilder.BuildCodeObjects(string.Format("{0}-01-01", StartDate), new List<SdmxObjectNameDescription>()
            {
                new SdmxObjectNameDescription()
                {
                    Lingua="en",
                    Name="Start Time period",
                }
            }, null));
            codes.Add(CodelistItemBuilder.BuildCodeObjects(string.Format("{0}-12-31", EndDate), new List<SdmxObjectNameDescription>()
            {
                new SdmxObjectNameDescription()
                {
                    Lingua="en",
                    Name="End Time period",
                }
            }, null));
            return codes;
        }
        private List<ICodeMutableObject> SpecialCodelist_CL_COUNT(string _dataFlowCode)
        {
            List<IParameterValue> parametri = new List<IParameterValue>()
            {
                new ParameterValue() {Item="DataSetCode",Value= _dataFlowCode},
                new ParameterValue() {Item="WhereStatement",Value="1=1"},
                new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
            };
            List<ICodeMutableObject> codes = new List<ICodeMutableObject>();

            codes.Add(CodelistItemBuilder.BuildCodeObjects(((DWHAccess)DbAccess).GetCL_COUNT(parametri).ToString(), new List<SdmxObjectNameDescription>()
                    {
                        new SdmxObjectNameDescription()
                        {
                            Lingua="en",
                            Name="Data count",
                        }
                    }, null));
            return codes;
        }
        private List<ICodeMutableObject> SpecialCodelist_CL_CONTRAINED(string _dataFlowCode, ISpecialConcept specialConcept)
        {
            string WhereStatement = "1=1";
            string Time = "1=1";
            List<string> _ConceptWhere = new List<string>();
            if (specialConcept.ContrainConceptREF != null && specialConcept.ContrainConceptREF.Keys.Count > 0)
            {

                foreach (string otherconept in specialConcept.ContrainConceptREF.Keys.ToList())
                {
                    if (specialConcept.ContrainConceptREF[otherconept] == null) continue;
                    if (new List<string>() { "TIME", "TIME_PERIOD" }.Contains(otherconept.Trim().ToUpper()))
                    {
                        if (specialConcept.ContrainConceptREF[otherconept].Count > 0 && specialConcept.TimeDimensionRef != null)
                        {
                            string timedim = string.Format(DataMessageManager.FormatWhereValue, ((IDimensionConcept)specialConcept.TimeDimensionRef).GetColumTimeName());
                            ISdmxDate StartDate = new SdmxDateCore(specialConcept.ContrainConceptREF[otherconept][0].Value);
                            string STime = TimePeriodDBFormat.GetTimeWhereStatment(timedim, TimePeriodDBFormat.TypeDateOperation.Major, StartDate.TimeFormatOfDate.EnumType, StartDate);
                            Time = String.Format("({0})", STime);
                            if (specialConcept.ContrainConceptREF[otherconept].Count == 2)
                            {
                                ISdmxDate EndDate = new SdmxDateCore(specialConcept.ContrainConceptREF[otherconept][1].Value);
                                string ETime = TimePeriodDBFormat.GetTimeWhereStatment(timedim, TimePeriodDBFormat.TypeDateOperation.Minor, EndDate.TimeFormatOfDate.EnumType, EndDate);
                                Time = String.Format("({0} AND {1})", STime, ETime);
                            }
                        }
                        continue;
                    }
                    if (otherconept.Trim().ToUpper() == "FREQ")
                    {
                        //Devo controllare se stò in modalità FakeFrequency se cosi 
                        continue;
                    }
                    List<string> ConceptInternalWhere = new List<string>();
                    foreach (var item in specialConcept.ContrainConceptREF[otherconept])
                        ConceptInternalWhere.Add(string.Format("{0}='{1}'", string.Format(DataMessageManager.FormatWhereValue, otherconept), item.Value));
                    _ConceptWhere.Add(string.Format("({0})", string.Join(" OR ", ConceptInternalWhere)));
                }
            }
            WhereStatement = string.Format("{0}", string.Join(" AND ", _ConceptWhere));
            if (string.IsNullOrEmpty(WhereStatement))
                WhereStatement = "1=1";

            List<IParameterValue> parametri = new List<IParameterValue>()
            {
                new ParameterValue() {Item="DataSetCode",Value= _dataFlowCode},
                new ParameterValue() {Item="WhereStatement",Value=WhereStatement},
                new ParameterValue() {Item="Time",Value=Time},
                new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
            };

            List<ICodeMutableObject> codes = null;
            if (specialConcept.Id.Trim().ToUpper() == "FREQ")
                codes = GetFrequencyCodelist();
            else
            {
                DimensionCodelistsManager dimcm = new DimensionCodelistsManager(this.parsingObject,this.versionTypeResp);

                codes = dimcm.InternalGetDimensionCodelistConstrain(_dataFlowCode, specialConcept.Id);
            }

            List<string> existCode = ((DWHAccess)DbAccess).GetCL_CONTRAINED(parametri, specialConcept.Id);

            List<ICodeMutableObject> Contrainedcodes = new List<ICodeMutableObject>();
            for (int i = codes.Count - 1; i >= 0; i--)
            {
                if (existCode.Contains(codes[i].Id))
                    Contrainedcodes.Add(codes[i]);
            }
            for (int i = 0; i < Contrainedcodes.Count; i++)
            {
                ICodeMutableObject observeCode = Contrainedcodes[i];

                while (true)
                {
                    if (!string.IsNullOrEmpty(observeCode.ParentCode) && !Contrainedcodes.Exists(c => c.Id == observeCode.ParentCode))
                    {
                        ICodeMutableObject ParentCode = codes.Find(c => c.Id == observeCode.ParentCode);
                        if (ParentCode != null)
                        {
                            Contrainedcodes.Add(ParentCode);
                            observeCode = ParentCode;
                            continue;
                        }
                    }
                    break;
                }
            }

            return Contrainedcodes;
        }

      



      

    }
}

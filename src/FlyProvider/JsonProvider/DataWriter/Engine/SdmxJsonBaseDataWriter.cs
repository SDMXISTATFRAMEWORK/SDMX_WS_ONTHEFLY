using System;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Newtonsoft.Json;
using Estat.Sri.TabularWriters.Engine;
using Org.Sdmxsource.Sdmx.Api.Model.Header;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model;
using System.Collections.Generic;
using Org.Sdmxsource.Sdmx.Api.Constants.InterfaceConstant;
using System.Collections;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using IstatExtension.DataWriter.Model;
using FlyController.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Mutable.Codelist;

namespace IstatExtension.SdmxJson.DataWriter.Engine
{
    public class SdmxJsonBaseDataWriter : IDataWriterEngine
    {
        private readonly JsonWriter _jsonDataWriter;

        private string _dimensionAtObservation;
        private bool _isTimePeriodAtObservation = false;


        public SDMXJsonStructure JsonStruct = new SDMXJsonStructure();

        public bool _startedObservations = false;

        public IDataStructureObject _dsd;

        public SdmxJsonBaseDataWriter(JsonWriter writer)
        {
            this._jsonDataWriter = writer;
            this._jsonDataWriter.Formatting = Formatting.None;

        }

        protected string DimensionAtObservation
        {
            get
            {
                return this._dimensionAtObservation;
            }
        }

        public bool StartedObservations
        {
            get
            {
                return this._startedObservations;
            }
        }

        protected bool IsTimePeriodAtObservation
        {
            get
            {
                return this._isTimePeriodAtObservation;
            }
        }

        protected virtual string GetDimensionAtObservation(IDatasetHeader header)
        {
            string dimensionAtObservation = DimensionObject.TimeDimensionFixedId;
            if (header != null && (header.DataStructureReference != null
                    && !string.IsNullOrWhiteSpace(header.DataStructureReference.DimensionAtObservation)))
            {
                dimensionAtObservation = header.DataStructureReference.DimensionAtObservation;
            }

            return dimensionAtObservation;
        }

        //protected bool IsTimePeriodAtObservation
        //{
        //    get
        //    {
        //        return this._isTimePeriodAtObservation;
        //    }
        //}

        public void Close(params IFooterMessage[] footer)
        {
            _jsonDataWriter.WriteEndObject();
        }

        public void StartDataset(IDataflowObject dataflow, IDataStructureObject dsd, IDatasetHeader header, params IAnnotation[] annotations)
        {
            this._dimensionAtObservation = this.GetDimensionAtObservation(header);
            _jsonDataWriter.Formatting = Formatting.None;
            _jsonDataWriter.WritePropertyName("dataSets");
            _jsonDataWriter.WriteStartArray();
            _jsonDataWriter.WriteStartObject();
            _jsonDataWriter.WritePropertyName("actions");
            _jsonDataWriter.WriteValue("Information");
        }

        public void CloseObject()
        {
            this._jsonDataWriter.WriteEndObject();
        }

        public void CloseArray()
        {
            this._jsonDataWriter.WriteEndArray();
        }

        public void StartElement(string name, bool isObj)
        {

            this._jsonDataWriter.WritePropertyName(name);

            if (isObj)
                this._jsonDataWriter.WriteStartObject();
            else
                this._jsonDataWriter.WriteStartArray();

        }

        public void StartSeries(params Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IAnnotation[] annotations)
        {
            StartElement("series", true);
        }

        public void WriteObservation(DateTime obsTime, string obsValue, Org.Sdmxsource.Sdmx.Api.Constants.TimeFormat sdmxSwTimeFormat, params Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IAnnotation[] annotations)
        {
            this._jsonDataWriter.WriteValue(obsTime);
            this._jsonDataWriter.WriteValue(obsValue);
        }

        public void WriteObservation(string obsConceptValue, string obsValue, params Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IAnnotation[] annotations)
        {
            this._jsonDataWriter.WritePropertyName(obsConceptValue);
            this._jsonDataWriter.WriteStartArray();
            this._jsonDataWriter.WriteValue(obsValue);
        }

        public void WriteSeriesKeyValue(string id, string value_ren)
        {
            StartElement(value_ren, true);
        }

        public void StartGroup(string groupId, params Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IAnnotation[] annotations)
        {
            throw new NotImplementedException();
        }

        public void WriteAttributeValue(string id, string valueRen)
        {
            this._jsonDataWriter.WriteValue(valueRen);
        }

        public void OpenObject()
        {
            this._jsonDataWriter.WriteStartObject();

        }

        public void WriteValue(string id, string valueRen)
        {
            this._jsonDataWriter.WritePropertyName(id);
            this._jsonDataWriter.WriteValue(valueRen);
        }

        public void WriteHeader(IHeader header)
        {
            this._jsonDataWriter.Formatting = Formatting.None;

            this._jsonDataWriter.WriteStartObject();
            this._jsonDataWriter.WritePropertyName("header");
            this._jsonDataWriter.WriteStartObject();
            this._jsonDataWriter.WritePropertyName("id");
            this._jsonDataWriter.WriteValue(header.Id);

            this._jsonDataWriter.WritePropertyName("test");
            this._jsonDataWriter.WriteValue(header.Test);

            this._jsonDataWriter.WritePropertyName("prepared");
            this._jsonDataWriter.WriteValue(header.Prepared);

            this._jsonDataWriter.WritePropertyName("sender");
            this._jsonDataWriter.WriteStartObject();

            this._jsonDataWriter.WritePropertyName("id");
            this._jsonDataWriter.WriteValue(header.Sender.Id);

            this._jsonDataWriter.WritePropertyName("name");
            this._jsonDataWriter.WriteValue(header.Sender.Name[0].Value);

            this._jsonDataWriter.WritePropertyName("contact");
            this._jsonDataWriter.WriteStartArray();
            this._jsonDataWriter.WriteStartObject();
            this._jsonDataWriter.WritePropertyName("name");
            this._jsonDataWriter.WriteValue(header.Sender.Contacts[0].Name[0].Value);
            this._jsonDataWriter.WritePropertyName("department");
            this._jsonDataWriter.WriteValue(header.Sender.Contacts[0].Departments[0].Value);
            this._jsonDataWriter.WritePropertyName("role");
            this._jsonDataWriter.WriteValue(header.Sender.Contacts[0].Role[0].Value);
            this._jsonDataWriter.WritePropertyName("telephone");
            this._jsonDataWriter.WriteValue(header.Sender.Contacts[0].Telephone[0]);
            this._jsonDataWriter.WritePropertyName("email");
            this._jsonDataWriter.WriteValue(header.Sender.Contacts[0].Email[0]);
            this._jsonDataWriter.WriteEndObject();
            this._jsonDataWriter.WriteEndArray();
            this._jsonDataWriter.WriteEndObject();
            this._jsonDataWriter.WriteEndObject();
        }

        public void Dispose()
        {
        }

        public string GetObsValPosition(string ObsVal)
        {
            return this.JsonStruct.Observation.GetValueIndex(ObsVal).ToString();
        }

        public string GetAttValPosition(string idAtt, string AttVal)
        {
            foreach (var Dim in this.JsonStruct.Attributes)
            {
                if (idAtt == Dim.Id)
                {
                    return Dim.GetValueIndex(AttVal).ToString();
                }
            }
            return "0";
        }

        public string GetDimValPosition(string idDim, string dimVal)
        {
            foreach (var Dim in this.JsonStruct.Series)
            {
                if (idDim == Dim.Id)
                {
                    return Dim.GetValueIndex(dimVal).ToString();
                }
            }
            return "0";
        }

        public void WriteSeriesKey(List<DataMessageObject> row)
        {
            string serieKey = "";
            foreach (var Dim in this._dsd.GetDimensions())
            {
                foreach (DataMessageObject seriesKey in row)
                {
                    if (seriesKey.ColImpl.ConceptObjectCode == Dim.Id)
                    {
                        serieKey += GetDimValPosition(seriesKey.ColImpl.ConceptObjectCode, seriesKey.Val.ToString()) + ":";
                    }
                }
            }

            serieKey = serieKey.Substring(0, serieKey.LastIndexOf(':'));
            WriteSeriesKeyValue("", serieKey);

            this.StartElement("annotations", false);
            this.CloseArray();



        }

        public void WriteAttributes(DataMessageObject attributes)
        {
            var value = this.GetAttValPosition(attributes.ColId.ToString(), attributes.Val.ToString());
            WriteAttributeValue(attributes.ColId.ToString(), value);
        }

        public delegate List<ICodelistObject> RetreiveCodelistDelegate(string agency, string codelistId, string version, IDataflowObject Dataflow, string ConceptId);

        public void WriteSDMXJsonStructure(IDataflowObject dataflow, RetreiveCodelistDelegate RetrieveCodelist)
        {
            //Set the Description of Dimensions

            SetJsonStructureDescriptions(dataflow, RetrieveCodelist);

            CloseObject();
            CloseObject();
            CloseObject();

            _startedObservations = false;
            this.StartElement("structure", true);

            this.WriteValue("name", dataflow.Name);
            if (dataflow.Description != null)
            {
                this.WriteValue("description", dataflow.Description);
            }
            if (dataflow.Uri != null && dataflow.Uri.AbsolutePath != null)
            {
                this.WriteValue("uri", dataflow.Uri.AbsolutePath);
            }
            this.StartElement("dimensions", true);

            this.StartElement("dataSet", false);
            this.CloseArray();

            this.StartElement("series", false);
            foreach (var Serie in this.JsonStruct.Series)
            {
                this.OpenObject();
                this.WriteValue("id", Serie.Id);
                this.WriteValue("keyPosition", Serie.KeyPosition);
                if (!(Serie.Role == null))
                {
                    this.WriteValue("role", Serie.Role);
                }
                this.StartElement("values", false);

                foreach (var Value in Serie.Values)
                {
                    this.OpenObject();
                    this.WriteValue("id", Value.Id);
                    this.WriteValue("descr", Value.Descr);
                    this.CloseObject();
                }


                this.CloseArray();
                this.CloseObject();

            }
            this.CloseArray();

            this.StartElement("observation", false);

            this.OpenObject();

            if (!(JsonStruct.Observation.Role == null))
            {
                this.WriteValue("role", JsonStruct.Observation.Role);
            }
            this.WriteValue("id", this.JsonStruct.Observation.ID);
            this.StartElement("values", false);

            foreach (var Value in this.JsonStruct.Observation.Values)
            {
                this.OpenObject();
                this.WriteValue("id", Value.Id);
                this.CloseObject();
            }

            this.CloseArray();
            this.CloseObject();
            this.CloseArray();

            this.CloseObject();

            //atributes
            this.StartElement("attributes", true);
            this.StartElement("dataSet", false);
            foreach (var Att in this.JsonStruct.Attributes)
            {
                if (Att.AttachmentLevel == "DataSet")
                {
                    this.OpenObject();
                    this.WriteValue("id", Att.Id);

                    this.StartElement("values", false);

                    foreach (var Value in Att.Values)
                    {
                        this.OpenObject();
                        this.WriteValue("id", Value.Id);
                        this.WriteValue("descr", Value.Descr);
                        this.CloseObject();
                    }


                    this.CloseArray();
                    this.CloseObject();
                }
            }
            this.CloseArray();

            this.StartElement("series", false);
            foreach (var Att in this.JsonStruct.Attributes)
            {
                if (Att.AttachmentLevel == "Group")
                {
                    this.OpenObject();
                    this.WriteValue("id", Att.Id);

                    this.StartElement("values", false);

                    foreach (var Value in Att.Values)
                    {
                        this.OpenObject();
                        this.WriteValue("id", Value.Id);
                        this.WriteValue("descr", Value.Descr);
                        this.CloseObject();
                    }


                    this.CloseArray();
                    this.CloseObject();
                }
            }
            this.CloseArray();

            this.StartElement("observation", false);
            foreach (var Att in this.JsonStruct.Attributes)
            {
                if (Att.AttachmentLevel == "Observation")
                {
                    this.OpenObject();
                    this.WriteValue("id", Att.Id);

                    this.StartElement("values", false);

                    foreach (var Value in Att.Values)
                    {
                        this.OpenObject();
                        this.WriteValue("id", Value.Id);
                        this.WriteValue("descr", Value.Descr);
                        this.CloseObject();
                    }


                    this.CloseArray();
                    this.CloseObject();
                }
            }
            this.CloseArray();

            this.CloseObject();
            this.StartElement("annotations", false);
            this.CloseArray();
            this.CloseObject();


            //close file
            this.CloseObject();
            this.CloseArray();
            this.CloseObject();
        }

        public void SetJsonStructureDescriptions(IDataflowObject dataflow, RetreiveCodelistDelegate RetrieveCodelist)
        {

            foreach (var x in _dsd.DimensionList.Dimensions)
            {
                string codelist = "";
                string agenzia = "";
                string versione = "";


                if (!(x.Representation == null))
                {
                    ISet<ICrossReference> isr = x.Representation.CrossReferences;
                    foreach (var y in isr)
                    {
                        codelist = y.MaintainableReference.MaintainableId;
                        agenzia = y.MaintainableReference.AgencyId;
                        versione = y.MaintainableReference.Version;
                    }

                    List<ICodelistObject> codelists = null;
                    if (string.IsNullOrEmpty(codelist) && x.TimeDimension)
                        codelists = RetrieveCodelist("MA", "CL_TIME_PERIOD", "1.0", dataflow, x.Id);
                    else
                        codelists = RetrieveCodelist(agenzia, codelist, versione, dataflow, x.Id);

                    foreach (var serie in this.JsonStruct.Series)
                    {
                        foreach (var value in serie.Values)
                        {
                            foreach (var codeBean in codelists)
                            {
                                foreach (var codeBeanItem in codeBean.Items)
                                {
                                    if (codeBeanItem.Id == value.Id)
                                    {
                                        foreach (var item in codeBeanItem.Names)
                                        {
                                            if (item.Locale == "en")
                                            {
                                                value.Descr = item.Value;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            foreach (var x in _dsd.DatasetAttributes)
            {
                string codelist = "";
                string agenzia = "";
                string versione = "";

                if (!(x.Representation == null))
                {
                    ISet<ICrossReference> isr = x.Representation.CrossReferences;
                    foreach (var y in isr)
                    {
                        codelist = y.MaintainableReference.MaintainableId;
                        agenzia = y.MaintainableReference.AgencyId;
                        versione = y.MaintainableReference.Version;
                    }

                    List<ICodelistObject> codelists = RetrieveCodelist(agenzia, codelist, versione, dataflow, x.Id);

                    foreach (var att in this.JsonStruct.Attributes)
                    {
                        if (att.AttachmentLevel == "DataSet")
                        {
                            foreach (var value in att.Values)
                            {
                                foreach (var codeBean in codelists)
                                {
                                    foreach (var codeBeanItem in codeBean.Items)
                                    {
                                        if (codeBeanItem.Id == value.Id)
                                        {
                                            foreach (var item in codeBeanItem.Names)
                                            {
                                                if (item.Locale == "en")
                                                {
                                                    value.Descr = item.Value;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (var x in _dsd.GroupAttributes)
            {
                string codelist = "";
                string agenzia = "";
                string versione = "";

                if (!(x.Representation == null))
                {
                    ISet<ICrossReference> isr = x.Representation.CrossReferences;
                    foreach (var y in isr)
                    {
                        codelist = y.MaintainableReference.MaintainableId;
                        agenzia = y.MaintainableReference.AgencyId;
                        versione = y.MaintainableReference.Version;
                    }

                    List<ICodelistObject> codelists = RetrieveCodelist(agenzia, codelist, versione, dataflow, x.Id);
                    foreach (var att in this.JsonStruct.Attributes)
                    {
                        if (att.AttachmentLevel == "Group")
                        {
                            foreach (var value in att.Values)
                            {
                                foreach (var codeBean in codelists)
                                {
                                    foreach (var codeBeanItem in codeBean.Items)
                                    {
                                        if (codeBeanItem.Id == value.Id)
                                        {
                                            foreach (var item in codeBeanItem.Names)
                                            {
                                                if (item.Locale == "en")
                                                {
                                                    value.Descr = item.Value;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var x in _dsd.ObservationAttributes)
            {
                string codelist = "";
                string agenzia = "";
                string versione = "";

                if (!(x.Representation == null))
                {
                    ISet<ICrossReference> isr = x.Representation.CrossReferences;
                    foreach (var y in isr)
                    {
                        codelist = y.MaintainableReference.MaintainableId;
                        agenzia = y.MaintainableReference.AgencyId;
                        versione = y.MaintainableReference.Version;
                    }

                    List<ICodelistObject> codelists = RetrieveCodelist(agenzia, codelist, versione, dataflow, x.Id);
                    foreach (var att in this.JsonStruct.Attributes)
                    {
                        if (att.AttachmentLevel == "Observation")
                        {
                            foreach (var value in att.Values)
                            {
                                foreach (var codeBean in codelists)
                                {
                                    foreach (var codeBeanItem in codeBean.Items)
                                    {
                                        if (codeBeanItem.Id == value.Id)
                                        {
                                            foreach (var item in codeBeanItem.Names)
                                            {
                                                if (item.Locale == "en")
                                                {
                                                    value.Descr = item.Value;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        public string GetKeyPosition(IDataStructureObject DsD, string keyDim)
        {
            foreach (var Dim in DsD.DimensionList.Dimensions)
            {
                if (Dim.Id == keyDim)
                {
                    return (Dim.Position - 1).ToString();
                }
            }
            return null;
        }


        public void AddSerie(string DimId, string Value, string KeyPosition, bool isFrequency)
        {
            bool Exists = false;
            foreach (var item in this.JsonStruct.Series)
            {
                if (item.Id == DimId)
                {
                    Exists = true;
                    SDMXJsonValue Val = new SDMXJsonValue();
                    Val.Id = Value;
                    if (!(item.ExistValue(Val)))
                    {
                        if (item.Values.Count > 0)
                        {
                            Val.Index = item.GetLastIndex(Val) + 1;
                        }
                        else
                        {
                            Val.Index = item.GetLastIndex(Val);
                        }

                        item.Values.Add(Val);
                        break;
                    }
                }
            }
            if (!Exists)
            {
                SDMXJsonDimension Serie = new SDMXJsonDimension();
                Serie.Id = DimId;
                Serie.KeyPosition = KeyPosition;
                if (isFrequency)
                {
                    Serie.Role = "frequency";
                }
                this.JsonStruct.Series.Add(Serie);
                AddSerie(DimId, Value, KeyPosition, isFrequency);
            }
        }

        public void AddAttribute(string AttId, string Value, string AttachmentLevel)
        {
            bool Exists = false;
            foreach (var item in this.JsonStruct.Attributes)
            {
                if (item.Id == AttId)
                {
                    Exists = true;
                    SDMXJsonValue Val = new SDMXJsonValue();
                    Val.Id = Value;
                    if (!(item.ExistValue(Val)))
                    {
                        item.Values.Add(Val);
                        break;
                    }
                }
            }
            if (!Exists)
            {
                SDMXJsonAttributes Att = new SDMXJsonAttributes();
                Att.Id = AttId;
                Att.AttachmentLevel = AttachmentLevel;
                this.JsonStruct.Attributes.Add(Att);
                AddAttribute(AttId, Value, AttachmentLevel);
            }
        }

        public void AddObsValue(string TimeValue)
        {
            bool Exists = false;
            if (!(this.JsonStruct.Observation.Values == null))
            {
                foreach (var item in this.JsonStruct.Observation.Values)
                {
                    if (item.Id == TimeValue)
                    {
                        Exists = true;
                        break;
                    }
                }
            }
            if (!Exists)
            {
                SDMXJsonValue Val = new SDMXJsonValue();
                Val.Id = TimeValue;
                if (!(this.JsonStruct.Observation.ExistValue(Val)))
                {

                    if (this.JsonStruct.Observation.Values.Count > 0)
                    {
                        Val.Index = this.JsonStruct.Observation.GetLastIndex(Val) + 1;
                    }
                    else
                    {
                        Val.Index = this.JsonStruct.Observation.GetLastIndex(Val);
                    }

                    this.JsonStruct.Observation.Values.Add(Val);
                }
            }
        }

        #region NotImplemented

        public void WriteGroupKeyValue(string id, string valueRen)
        {
            throw new NotImplementedException();
        }

        public void WriteObservation(string observationConceptId, string obsConceptValue, string obsValue, params Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IAnnotation[] annotations)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void StartColumns()
        {
            throw new NotImplementedException();
        }

        public void StartRecord()
        {
            throw new NotImplementedException();
        }

        public long TotalRecordsWritten
        {
            get { throw new NotImplementedException(); }
        }

        public void WriteCellAttributeValue(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteCellKeyValue(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteCellMeasureValue(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteColumnAttribute(string attribute)
        {
            throw new NotImplementedException();
        }

        public void WriteColumnKey(string key)
        {
            throw new NotImplementedException();
        }

        public void WriteColumnMeasure(string measure)
        {
            throw new NotImplementedException();
        }
        #endregion


        public void WriteHeaderForDownloadJson()
        {
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=OnTheFly_Response.json");
        }
    }
}


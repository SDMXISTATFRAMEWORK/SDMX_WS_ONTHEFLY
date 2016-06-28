using FlyController;
using FlyController.Model;
using FlyController.Model.Delegate;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyController.Model.WhereParsing;
using FlyDotStat_implementation.Manager.Metadata;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using FlyMapping.Model.Enum;
using OnTheFlyLog;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query.Complex;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FlyDotStat_implementation.Manager.Data
{
    /// <summary>
    /// DataMessageManager is class that retrieves data from the database to the requests of DataMessage
    /// </summary>
    public class DataMessageManager : BaseManager, IDataMessageManager
    {

        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public DataMessageManager(SdmxSchemaEnumType _versionTypeResp)
            :base(null, _versionTypeResp){}

        /// <summary>
        /// Referenced objects 
        /// </summary>
        public override IReferencesObject ReferencesObject { get; set; }

        /// <summary>
        /// List of Concept
        /// </summary>
        public List<IConceptObjectImpl> Concepts { get; set; }
        /// <summary>
        /// Constat Format for required field in Where Statment parameter
        /// </summary>
        public const string FormatWhereValue = "[${0}$].Code";

       

        /// <summary>
        /// Get structured Data Message from Database
        /// </summary>
        /// <param name="idDataset">Dataset Code</param>
        /// <param name="whereStatement">Where condition</param>
        /// <param name="BuilderCallback">delegate to call for write data response</param>
        /// <param name="TimeStamp">LastUpdate parameter request only observation from this date onwards</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public IFlyWriterBody GetTableMessage(string idDataset, IDataWhereStatment whereStatement, WriteResponseDelegate BuilderCallback, string TimeStamp)
        {
            try
            {
                string TimeWhere = "";

                List<IParameterValue> parametri = new List<IParameterValue>()
                {
                    new ParameterValue() {Item="DataSetCode",Value= idDataset},
                    new ParameterValue() {Item="WhereStatement",Value=ConvertIDataQueryToWhereStatement(whereStatement, out TimeWhere)},
                    new ParameterValue() {Item="Time",Value=TimeWhere},
                    new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                    new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
                };
                if (!string.IsNullOrEmpty(TimeStamp))
                    parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = TimeStamp,SqlType=SqlDbType.DateTime });
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"GetData parameter: {0}", string.Join("\n",parametri));

                //EFFETTUO LA RICHIESTA AL DB
                return this.DbAccess.ExecuteGetData(parametri, new GetDBResponseDelegate(GetDBResponse), BuilderCallback);

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetTableMessage, ex);
            }

        }

        /// <summary>
        /// Get Database response and call delegate WriteBuilder
        /// </summary>
        /// <param name="rea">SqlDataReader response</param>
        /// <param name="builder"> Delegate for callback of Write response</param>
        /// <param name="writer"> Contains the object of transport used for transmitting data in streaming</param>
        public void GetDBResponse(SqlDataReader rea, WriteResponseDelegate builder, IFlyWriter writer)
        {
            try
            {
                DataTable schemaTable = rea.GetSchemaTable();


                IDataTableMessageObject DTMessage = new DataTableMessageObject();
                List<string> columns = new List<string>();
                foreach (DataRow item in schemaTable.Rows)
                {
                    string itemColumnName = item["ColumnName"].ToString();
                    columns.Add(itemColumnName);
                    SpacialColumsNamesEnum tipoSpeciale;

                    IConceptObjectImpl conc = Concepts.Find(c =>
                        (c.Id.Trim().ToUpper() == itemColumnName.Trim().ToUpper())
                        || (c.Id.Trim().ToUpper() == itemColumnName.Trim().ToUpper().Substring(1))
                         || CheckRenamedFrequency(c, itemColumnName)
                        || CheckRenamedFrequency(c, itemColumnName.Substring(1)));

                    if (conc != null)
                        DTMessage.Colums.Add(itemColumnName, conc);
                    else if (Enum.TryParse<SpacialColumsNamesEnum>(itemColumnName.Trim().ToUpper(), true, out tipoSpeciale))
                    {
                        switch (tipoSpeciale)
                        {
                            case SpacialColumsNamesEnum._TIME_:
                            case SpacialColumsNamesEnum._TIME:
                                DTMessage.Colums.Add(itemColumnName, Concepts.Find(c => c.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)c).DimensionType == DimensionTypeEnum.Time));
                                break;
                            case SpacialColumsNamesEnum.VALUE:
                            case SpacialColumsNamesEnum._VALUE_:
                                if (Concepts.Exists(c => c.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)c).IsValueAttribute))
                                    DTMessage.Colums.Add(itemColumnName, Concepts.Find(c => c.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)c).IsValueAttribute));
                                break;
                            case SpacialColumsNamesEnum.FLAGS:
                                if (Concepts.Exists(c => c.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)c).IsFlagAttribute))
                                    DTMessage.Colums.Add(itemColumnName, Concepts.Find(c => c.ConceptType == ConceptTypeEnum.Attribute && ((IAttributeConcept)c).IsFlagAttribute));
                                break;
                        }

                    }
                }

                //La colonna Frequency è obbligatoria (Se non c'è l'aggiungo)
                if (string.IsNullOrEmpty(DTMessage.FrequencyCol))
                {
                    var _freqconcept = Concepts.Find(c => c.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)c).DimensionType == DimensionTypeEnum.Frequency && ((IDimensionConcept)c).IsFakeFrequency);
                    if (_freqconcept != null)
                        DTMessage.Colums.Add(_freqconcept.ConceptObjectCode, _freqconcept);
                }
                //Inseriamo anche la colonna TimeFormat a meno che non ci sia già nel db
                if (string.IsNullOrEmpty(DTMessage.TimeFormatCol))
                {
                    IConceptObjectImpl _timeFormatCol = Concepts.Find(co => co.ConceptType == ConceptTypeEnum.Attribute && co.Id == FlyConfiguration.Time_Format_Id);
                    if (_timeFormatCol != null)
                        DTMessage.Colums.Add(_timeFormatCol.ConceptObjectCode, _timeFormatCol);
                }

                DTMessage.DBDataReader = rea;
                DTMessage.isFakeTimeFormat = !columns.Contains(DTMessage.TimeFormatCol);

                builder(DTMessage, writer);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.GetTableMessage, ex);
            }
        }




        /// <summary>
        /// Check if filed Frequency must be renamed (if is a fake fiels or real)
        /// </summary>
        /// <param name="c">Concept</param>
        /// <param name="colname">Real Colum name</param>
        /// <returns>true if must be rename false than else</returns>
        private bool CheckRenamedFrequency(IConceptObjectImpl c, string colname)
        {
            try
            {
                return (c is IDimensionConcept && !string.IsNullOrEmpty(((IDimensionConcept)c).RealNameFreq) && ((IDimensionConcept)c).RealNameFreq.Trim().ToUpper() == colname.Trim().ToUpper());
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }


        #region Parsing Where
        /// <summary>
        /// Converting in WhereStatment string to pass at database parameter the DataWhereStatment parsed from query
        /// </summary>
        /// <param name="DataWhereStatment">DataWhereStatment parsed from query</param>
        /// <param name="TimeWhere">return a string representing a TimeWhere to pass a database parameter</param>
        /// <returns>a string to pass at database WhereStatment parameter</returns>
        private string ConvertIDataQueryToWhereStatement(IDataWhereStatment DataWhereStatment, out string TimeWhere)
        {
            try
            {
                TimeWhere = "";
                List<string> _TimeAndFreqWhere = new List<string>();
                List<string> ParseGroupWhere = new List<string>();
                foreach (IDataWhereStatmentSelectionGroup group in DataWhereStatment.SelectionGroups)
                {
                    List<string> ParseWhere = new List<string>();
                    string _freqWhere, _TimeWhere = null;
                    string _parseWhereGroup = parseWhereGroup(group, out _freqWhere);
                    if (!string.IsNullOrEmpty(_parseWhereGroup))
                        ParseWhere.Add(_parseWhereGroup);

                    //Controllo l'esistenza di una where per Time
                    if (group.DateFrom != null || group.DateTo != null)
                        _TimeWhere = ParseTimeValueWhereStatment(group.DateFrom, group.DateTo);

                    if (!string.IsNullOrEmpty(_freqWhere) && !string.IsNullOrEmpty(_TimeWhere))
                        _TimeAndFreqWhere.Add(string.Format("({0} AND {1})", _freqWhere, _TimeWhere));
                    else if (!string.IsNullOrEmpty(_freqWhere))
                        _TimeAndFreqWhere.Add(_freqWhere);
                    else if (!string.IsNullOrEmpty(_TimeWhere))
                        _TimeAndFreqWhere.Add(_TimeWhere);


                    if (ParseWhere.Count > 0)
                        ParseGroupWhere.Add(string.Format("({0})", string.Join(" AND ", ParseWhere)));

                }

                //Popolo il TimeWhere
                if (_TimeAndFreqWhere.Count > 0)
                    TimeWhere = string.Format("({0})", string.Join(" OR ", _TimeAndFreqWhere));

                if (ParseGroupWhere.Count == 0)
                    return "1=1";//per far tornare tutti i valori nel campo Where

                return string.Join(" OR ", ParseGroupWhere);
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }

        /// <summary>
        /// Parsing a Group of Selection query
        /// </summary>
        /// <param name="group">Selection Group</param>
        /// <param name="_freqWhere">return a string that representing a TimeWhere to pass a database parameter</param>
        /// <returns>string representing the group formatted</returns>
        private string parseWhereGroup(IDataWhereStatmentSelectionGroup group, out string _freqWhere)
        {
            try
            {
                List<string> ParseWhere = new List<string>();
                List<string> FreqWhere = new List<string>();

                foreach (IDataWhereStatmentSelection item in group.Selections)
                {
                    string _freqWhereSel = null;
                    string _parseWhereSelection = parseWhereSelection(item, out _freqWhereSel);
                    if (!string.IsNullOrEmpty(_parseWhereSelection))
                        ParseWhere.Add(_parseWhereSelection);
                    if (!string.IsNullOrEmpty(_freqWhereSel))
                        FreqWhere.Add(_freqWhereSel);
                }

                _freqWhere = null;
                if (FreqWhere.Count > 0)
                    _freqWhere = string.Format("({0})", string.Join(") AND (", FreqWhere));

                if (ParseWhere.Count == 0)
                    return null;
                return string.Format("({0})", string.Join(") AND (", ParseWhere));
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }

        /// <summary>
        ///  Parsing a Selections query
        /// </summary>
        /// <param name="querys">Selection</param>
        /// <param name="_freqWhere">return a string that representing a TimeWhere to pass a database parameter</param>
        /// <returns>string representing the selection formatted</returns>
        private string parseWhereSelection(IDataWhereStatmentSelection querys, out string _freqWhere)
        {
            try
            {
                _freqWhere = null;
                List<string> SingleValues = new List<string>();
                IConceptObjectImpl realConceptFound = Concepts.Find(c => (c.Id.Trim().ToUpper() == querys.ComponentId.Trim().ToUpper()) || CheckRenamedFrequency(c, querys.ComponentId));
                if (realConceptFound == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DimensionNotFound, new Exception(querys.ComponentId));

                IConceptObjectImpl realConceptTime = Concepts.Find(c => c.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)c).DimensionType == DimensionTypeEnum.Time);
                string timedim = string.Format(FormatWhereValue, ((IDimensionConcept)realConceptTime).GetColumTimeName());

                if (realConceptFound is IDimensionConcept && ((IDimensionConcept)realConceptFound).DimensionType == DimensionTypeEnum.Frequency)
                {
                    IDimensionConcept FreqConcept = (IDimensionConcept)realConceptFound;
                    if (!FreqConcept.IsFakeFrequency)
                    {
                        string dim = string.Format(FormatWhereValue, FreqConcept.RealNameFreq);
                        foreach (string val in querys.Values)
                            SingleValues.Add(string.Format("{0}='{1}'", dim, val));
                    }
                    else
                        _freqWhere = TimePeriodDBFormat.GetFreqValueWhereStatment(querys.Values, timedim);
                }
                else if (realConceptFound is IDimensionConcept)
                {
                    string dim = string.Format(FormatWhereValue, realConceptFound.ConceptObjectCode);
                    foreach (string val in querys.Values)
                        SingleValues.Add(string.Format("{0}='{1}'", dim, val));
                }
                else if (realConceptFound is IAttributeConcept)
                {//Gestione degli attributi 

                    if (realConceptFound.Id == FlyConfiguration.Time_Format_Id)
                        _freqWhere = TimePeriodDBFormat.GetTimeFormatValueWhereStatment(querys.Values, timedim);
                }


                if (SingleValues.Count == 0)
                    return null;
                return string.Format("({0})", string.Join(" OR ", SingleValues));
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }

        /// <summary>
        /// Parse a time where condition
        /// </summary>
        /// <param name="DateFrom">Date From in Sdmx Format</param>
        /// <param name="DateTo">Date To in Sdmx Format</param>
        /// <returns>string representing the date where formatted</returns>
        private string ParseTimeValueWhereStatment(ISdmxDate DateFrom, ISdmxDate DateTo)
        {
            try
            {
                IConceptObjectImpl realConceptTime = Concepts.Find(c => c.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)c).DimensionType == DimensionTypeEnum.Time);
                string timedim = string.Format(FormatWhereValue, ((IDimensionConcept)realConceptTime).GetColumTimeName());
                List<string> SingleValues = new List<string>();

                //dò per scontato che i timeFormat delle 2 date siano Uguali
                TimeFormatEnumType tf = TimeFormatEnumType.Null;
                if (DateFrom != null && DateFrom.Date.HasValue)
                {
                    tf = DateFrom.TimeFormatOfDate.EnumType;
                    string wherestr = TimePeriodDBFormat.GetTimeWhereStatment(timedim, TimePeriodDBFormat.TypeDateOperation.Major, tf, DateFrom);
                    if (!string.IsNullOrEmpty(wherestr))
                        SingleValues.Add(wherestr);
                }

                if (DateTo != null && DateTo.Date.HasValue)
                {
                    if (tf == TimeFormatEnumType.Null)
                        tf = DateTo.TimeFormatOfDate.EnumType;
                    string wherestr = TimePeriodDBFormat.GetTimeWhereStatment(timedim, FlyController.Model.TimePeriodDBFormat.TypeDateOperation.Minor, tf, DateTo);
                    if (!string.IsNullOrEmpty(wherestr))
                        SingleValues.Add(wherestr);
                }

                if (SingleValues.Count > 0)
                    return string.Format("({0})", string.Join(") AND (", SingleValues));

                return null;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }




        #endregion

    }
}

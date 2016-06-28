using FlyController.Model;
using FlyController.Model.Error;
using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Model object contains information of Time Format in database
    /// </summary>
    public class TimePeriodDBFormat
    {
        /// <summary>
        /// Database mapping TimePeriod format
        /// </summary>
        private static Dictionary<TimeFormat, string> MappingTimePeriodDbFormat = new Dictionary<TimeFormat, string>()
        {
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Year), ""},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Month), "M"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.QuarterOfYear), "Q"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.HalfOfYear), "S"},
            
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Week), "W"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Date), "D"},

            {TimeFormat.GetFromEnum(TimeFormatEnumType.DateTime), "-"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.Hour), "-"},
            {TimeFormat.GetFromEnum(TimeFormatEnumType.ThirdOfYear), "T"},
        };


        /// <summary>
        /// Tranform a database time value to Sdmx Time format value
        /// </summary>
        /// <param name="TimeDBVal">String representing a time value getted from database</param>
        /// <returns>Sdmx time value</returns>
        public static string ParseTimeVal(object TimeDBVal)
        {
            try
            {
                if (TimeDBVal == null)
                    return null;
                if (string.IsNullOrEmpty(TimeDBVal.ToString()))
                    return null;
                string TimeDb = TimeDBVal.ToString();
                TimeFormat tf = null;
                if (TimeDb.Length < 5)
                    tf = TimeFormat.GetFromEnum(TimeFormatEnumType.Year);

                if (tf == null)
                {
                    if (!MappingTimePeriodDbFormat.ContainsValue(TimeDb.Substring(4, 1).ToUpper()))
                        return TimeDb;//non ho capito il formato
                    tf = MappingTimePeriodDbFormat.FirstOrDefault(m => m.Value == TimeDb.Substring(4, 1).ToUpper()).Key;
                }

                switch (tf.EnumType)
                {
                    case TimeFormatEnumType.Month:
                        return string.Format("{0}-{1}", TimeDb.Substring(0, 4), TimeDb.Substring(5).PadLeft(2, '0'));
                    case TimeFormatEnumType.Date:
                    case TimeFormatEnumType.DateTime:
                    case TimeFormatEnumType.Hour:
                        return string.Format("{0}-{1}{2}", TimeDb.Substring(0, 4), tf.FrequencyCode, TimeDb.Substring(5));
                    case TimeFormatEnumType.HalfOfYear:
                    case TimeFormatEnumType.QuarterOfYear:
                    case TimeFormatEnumType.ThirdOfYear:
                    case TimeFormatEnumType.Week:
                        return string.Format("{0}-{1}{2}", TimeDb.Substring(0, 4), tf.FrequencyCode, TimeDb.Substring(5));
                    case TimeFormatEnumType.Year:
                        return TimeDb;
                    case TimeFormatEnumType.Null:
                    default:
                        break;
                }
                return TimeDb;//non ho capito la data
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(TimePeriodDBFormat), FlyExceptionObject.FlyExceptionTypeEnum.CastingTimeFormat, ex);
            }
        }

        /// <summary>
        /// Tranform a database time value to Sdmx Frequency format value
        /// </summary>
        /// <param name="TimeVal">String representing a time value getted from database</param>
        /// <returns>Sdmx Frequency value</returns>
        public static string GetFrequencyValueFromTime(string TimeVal)
        {
            try
            {
                string DBFreqCode = "";
                if (TimeVal.Trim().Length > 4)
                    DBFreqCode = TimeVal.Substring(4, 1);

                if (MappingTimePeriodDbFormat.ContainsValue(DBFreqCode))
                    return MappingTimePeriodDbFormat.FirstOrDefault(m => m.Value == DBFreqCode).Key.FrequencyCode;
                else
                    return ""; //not recognized frequency
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(TimePeriodDBFormat), FlyExceptionObject.FlyExceptionTypeEnum.CastingTimeFormat, ex);
            }
        }

        /// <summary>
        /// Tranform a database time value to Sdmx TimeFormat attribute format value
        /// </summary>
        /// <param name="TimeVal">String representing a time value getted from database</param>
        /// <returns>Sdmx TimeFormat attribute value</returns>
        public static string GetTimeFormatValueFromTime(string TimeVal)
        {
            try
            {
                string DBFreqCode = "";
                if (TimeVal.Trim().Length > 4)
                    DBFreqCode = TimeVal.Substring(4, 1);

                if (!MappingTimePeriodDbFormat.ContainsValue(DBFreqCode))
                    return ""; //Not Recognized TimePeriod

                switch (MappingTimePeriodDbFormat.FirstOrDefault(m => m.Value == DBFreqCode).Key.EnumType)
                {
                    case TimeFormatEnumType.Date:
                        return "P1D";
                    case TimeFormatEnumType.DateTime:
                        return "PT1M";
                    case TimeFormatEnumType.HalfOfYear:
                        return "P6M";
                    case TimeFormatEnumType.Month:
                        return "P1M";
                    case TimeFormatEnumType.QuarterOfYear:
                        return "P3M";
                    case TimeFormatEnumType.Week:
                        return "P7D";
                    case TimeFormatEnumType.Year:
                        return "P1Y";
                    case TimeFormatEnumType.ThirdOfYear:
                    case TimeFormatEnumType.Hour:
                    case TimeFormatEnumType.Null:
                    default:
                        return ""; //Not Recognized TimeFormat
                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(TimePeriodDBFormat), FlyExceptionObject.FlyExceptionTypeEnum.CastingTimeFormat, ex);
            }
        }


        /// <summary>
        /// Create a Where Condition filtering by Time from Time frequency value
        /// </summary>
        /// <param name="_values">time values</param>
        /// <param name="timedim">TimeFormat Colum name</param>
        /// <returns>Where Condition string</returns>
        public static string GetFreqValueWhereStatment(List<string> _values, string timedim)
        {
            try
            {
                List<string> SingleValues = new List<string>();

                foreach (string freqval in _values)
                {
                    TimeFormat tf = TimeFormat.GetTimeFormatFromCodeId(freqval);
                    if (tf == null)
                        continue;

                    //Trasformo il codice SDMX freq nel nostro formato time nel DB
                    if (!MappingTimePeriodDbFormat.ContainsKey(tf))
                        //nel Mapping devono essere presenti tutti i timeformat
                        continue;

                    string DBtimecode = MappingTimePeriodDbFormat[tf];
                    if (tf.EnumType == TimeFormatEnumType.Year)
                        SingleValues.Add(string.Format("DATALENGTH({0}) = 4", timedim));
                    else
                        SingleValues.Add(string.Format("SUBSTRING({0},5,1) ='{1}'", timedim, DBtimecode));
                }

                if (SingleValues.Count > 0)
                    return string.Format("({0})", string.Join(") OR (", SingleValues));

                return null;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(TimePeriodDBFormat), FlyExceptionObject.FlyExceptionTypeEnum.CastingTimeFormat, ex);
            }

        }

        /// <summary>
        /// Create a Where Condition filtering Time from Time format value
        /// </summary>
        /// <param name="_values">time values</param>
        /// <param name="timedim">TimeFormat Colum name</param>
        /// <returns>Where Condition string</returns>
        public static string GetTimeFormatValueWhereStatment(List<string> _values, string timedim)
        {
            try
            {
                List<string> SingleValues = new List<string>();

                foreach (string TFvalue in _values)
                {
                    TimeFormat tf = null;
                    switch (TFvalue.Trim().ToUpper())
                    {
                        case "P1Y":
                            tf = TimeFormat.GetFromEnum(TimeFormatEnumType.Year);
                            break;
                        case "P1M":
                            tf = TimeFormat.GetFromEnum(TimeFormatEnumType.Month);
                            break;
                        case "P1D":
                            tf = TimeFormat.GetFromEnum(TimeFormatEnumType.Date);
                            break;
                        case "P3M":
                            tf = TimeFormat.GetFromEnum(TimeFormatEnumType.QuarterOfYear);
                            break;
                        case "P6M":
                            tf = TimeFormat.GetFromEnum(TimeFormatEnumType.HalfOfYear);
                            break;
                        case "P7D":
                            tf = TimeFormat.GetFromEnum(TimeFormatEnumType.Week);
                            break;
                        case "PT1M":
                            tf = TimeFormat.GetFromEnum(TimeFormatEnumType.DateTime);
                            break;
                    }

                    if (tf == null)
                        continue;

                    //Trasformo il codice SDMX freq nel nostro formato time nel DB
                    if (!MappingTimePeriodDbFormat.ContainsKey(tf))
                        //nel Mapping devono essere presenti tutti i timeformat
                        continue;


                    string DBtimecode = MappingTimePeriodDbFormat[tf];
                    if (tf.EnumType == TimeFormatEnumType.Year)
                        SingleValues.Add(string.Format("DATALENGTH({0}) = 4", timedim));
                    else
                        SingleValues.Add(string.Format("SUBSTRING({0},5,1) ='{1}'", timedim, DBtimecode));
                }

                if (SingleValues.Count > 0)
                    return string.Format("({0})", string.Join(") OR (", SingleValues));

                return null;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(TimePeriodDBFormat), FlyExceptionObject.FlyExceptionTypeEnum.CastingTimeFormat, ex);
            }

        }

        /// <summary>
        /// Create a Where Condition filtering Time from Time period value
        /// </summary>
        /// <param name="timedim">Time period colum name</param>
        /// <param name="to">Date to (TypeDateOperation)</param>
        /// <param name="tf">Time format type</param>
        /// <param name="dt">Date in Sdmx Format</param>
        /// <returns></returns>
        public static string GetTimeWhereStatment(string timedim, TypeDateOperation to, TimeFormatEnumType tf, ISdmxDate dt)
        {
            try
            {
                string SymbolComp = to == TypeDateOperation.Minor ? "<" : ">";
                List<string> OrTimeFormatWhere = new List<string>();

                int MounthConsidered = dt.Date.Value.Month;
                if (tf == TimeFormatEnumType.Year && to == TypeDateOperation.Minor)
                    MounthConsidered = 12;

                //Tutte le possibili condizioni
                //==yyyy-MM-dd oppure yyyyMMdd
                OrTimeFormatWhere.Add(string.Format("({0} = '{2}')", timedim, SymbolComp, dt.Date.Value.ToString("yyyy-MM-dd")));
                OrTimeFormatWhere.Add(string.Format("({0} = '{2}')", timedim, SymbolComp, dt.Date.Value.ToString("yyyyMMdd")));
                #region Anno
                OrTimeFormatWhere.Add(string.Format("(DATALENGTH({0}) = 4 AND {0} {1}= '{2}')", timedim, SymbolComp, dt.Date.Value.Year));
                #endregion
                #region Mensile
                string DBtimecode = MappingTimePeriodDbFormat[TimeFormat.GetFromEnum(TimeFormatEnumType.Month)];
                OrTimeFormatWhere.Add(string.Format(@"
(
    DATALENGTH({0}) > 4 AND
    SUBSTRING({0},5,1) ='{1}' 
    AND 
    (
        SUBSTRING({0},0,5) {2} '{3}' 
        OR
        (
            SUBSTRING({0},0,5) = '{3}' 
            AND 
            CONVERT(int,SUBSTRING({0},6,DATALENGTH({0})-5)) {2}= {4}
        )
    )
)", timedim, DBtimecode, SymbolComp, dt.Date.Value.Year, MounthConsidered));
                #endregion
                #region Quartely
                DBtimecode = MappingTimePeriodDbFormat[TimeFormat.GetFromEnum(TimeFormatEnumType.QuarterOfYear)];
                int quarter = (MounthConsidered + 2) / 3;
                OrTimeFormatWhere.Add(string.Format(@"
(
    DATALENGTH({0}) > 4 AND
    SUBSTRING({0},5,1) ='{1}' 
    AND 
    (
        SUBSTRING({0},0,5) {2} '{3}' 
        OR
        (
            SUBSTRING({0},0,5) = '{3}' 
            AND 
            CONVERT(int,SUBSTRING({0},6,DATALENGTH({0})-5)) {2}= {4}
        )
    )
)", timedim, DBtimecode, SymbolComp, dt.Date.Value.Year, quarter));
                #endregion
                #region ThirdOfYear
                DBtimecode = MappingTimePeriodDbFormat[TimeFormat.GetFromEnum(TimeFormatEnumType.ThirdOfYear)];
                int ThirdOfYear = (MounthConsidered + 3) / 4;
                OrTimeFormatWhere.Add(string.Format(@"
(
    DATALENGTH({0}) > 4 AND
    SUBSTRING({0},5,1) ='{1}' 
    AND 
    (
        SUBSTRING({0},0,5) {2} '{3}' 
        OR
        (
            SUBSTRING({0},0,5) = '{3}' 
            AND 
            CONVERT(int,SUBSTRING({0},6,DATALENGTH({0})-5)) {2}= {4}
        )
    )
)", timedim, DBtimecode, SymbolComp, dt.Date.Value.Year, ThirdOfYear));
                #endregion
                #region HalfOfYear
                DBtimecode = MappingTimePeriodDbFormat[TimeFormat.GetFromEnum(TimeFormatEnumType.HalfOfYear)];
                int HalfOfYear = (MounthConsidered + 5) / 6;
                OrTimeFormatWhere.Add(string.Format(@"
(
    DATALENGTH({0}) > 4 AND
    SUBSTRING({0},5,1) ='{1}' 
    AND 
    (
        SUBSTRING({0},0,5) {2} '{3}' 
        OR
        (
            SUBSTRING({0},0,5) = '{3}' 
            AND 
            CONVERT(int,SUBSTRING({0},6,DATALENGTH({0})-5)) {2}= {4}
        )
    )
)", timedim, DBtimecode, SymbolComp, dt.Date.Value.Year, HalfOfYear));
                #endregion


                return string.Format("({0})", string.Join(" OR ", OrTimeFormatWhere));

                //                if (tf == TimeFormatEnumType.Year)
                //                {
                //                    return string.Format("(DATALENGTH({0}) = 4 AND {0} {1} '{2}')", timedim, SymbolComp, dt.Date.Value.Year);
                //                }
                //                else
                //                {
                //                    string queryValAfterFreq = dt.Date.Value.Month.ToString();
                //                    if (dt.DateInSdmxFormat.IndexOf(dt.TimeFormatOfDate.FrequencyCode) > 4)
                //                        queryValAfterFreq = dt.DateInSdmxFormat.Substring(dt.DateInSdmxFormat.IndexOf(dt.TimeFormatOfDate.FrequencyCode) + 1);

                //                    string DBtimecode = dt.TimeFormatOfDate.FrequencyCode;
                //                    //Trasformo il codice SDMX freq nel nostro formato time nel DB
                //                    if (MappingTimePeriodDbFormat.ContainsKey(dt.TimeFormatOfDate)) //nel Mapping devono essere presenti tutti i timeformat
                //                        DBtimecode = MappingTimePeriodDbFormat[dt.TimeFormatOfDate];

                //                    return string.Format(@"
                //(
                //    SUBSTRING({0},5,1) ='{1}' 
                //    AND 
                //    (
                //        SUBSTRING({0},0,5) {2} '{3}' 
                //        OR
                //        (
                //            SUBSTRING({0},0,5) = '{3}' 
                //            AND 
                //            CONVERT(int,SUBSTRING({0},6,DATALENGTH({0})-5)) {2}= {4}
                //        )
                //    )
                //)",
                //                    timedim, DBtimecode, SymbolComp, dt.Date.Value.Year, queryValAfterFreq);
                //                }
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(typeof(FlyController.Model.TimePeriodDBFormat), FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }
        /// <summary>
        /// Enumerator that representig the math logical operation
        /// </summary>
        public enum TypeDateOperation
        {
            /// <summary>
            /// Minor value
            /// </summary>
            Minor,
            /// <summary>
            /// Major value
            /// </summary>
            Major
        }
    }
}

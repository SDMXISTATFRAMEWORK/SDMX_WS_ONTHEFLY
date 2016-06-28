using FlyController;
using FlyController.Model;
using FlyController.Model.Error;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// DataTableMessageObject is object Model contains a mapping Database Data for insert in Sdmx response
    /// </summary>
    public class DataTableMessageObject : IDataTableMessageObject
    {
        /// <summary>
        /// Initialize instance of DataTableMessageObject
        /// </summary>
        public DataTableMessageObject()
        {
            Colums = new Dictionary<string, IConceptObjectImpl>();
        }

        /// <summary>
        /// Dictionary contains all Concepts
        /// </summary>
        public Dictionary<string, IConceptObjectImpl> Colums { get; set; }

        /// <summary>
        /// SqlDataReader conteins database response not readed (sequencial reading) 
        /// </summary>
        public System.Data.SqlClient.SqlDataReader DBDataReader { get; set; }

        /// <summary>
        /// Flag that representing Time format Name manually created
        /// </summary>
        public bool isFakeTimeFormat { get; set; }

        /// <summary>
        /// Counter of Record return from Database
        /// </summary>
        public int RowsCounter { get; set; }

        #region Gest Frequency

        /// <summary>
        /// Check is present Frequency Dimension and if is Fake or real
        /// </summary>
        public string FrequencyCol
        {
            get
            {
                IConceptObjectImpl _frequencyCol = Colums.Values.ToList().Find(co => co.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)co).DimensionType == DimensionTypeEnum.Frequency);
                if (_frequencyCol == null)
                    return null;
                else
                    return _frequencyCol.ConceptObjectCode;
            }
        }
        /// <summary>
        ///  Check the name of TimeDimension with a TimeDimendion in database field
        /// </summary>
        public string TimeFormatCol
        {
            get
            {
                IConceptObjectImpl _TimeFormatCol = Colums.Values.ToList().Find(co => co.ConceptType == ConceptTypeEnum.Attribute && co.Id == FlyConfiguration.Time_Format_Id);
                if (_TimeFormatCol == null)
                    return null;
                else
                    return _TimeFormatCol.ConceptObjectCode;
            }
        }
        #endregion

        /// <summary>
        /// Get the Next response values
        /// </summary>
        /// <returns></returns>
        public List<DataMessageObject> GetNext()
        {
            try
            {

                if (DBDataReader.Read())
                {
                    string ColTime = Colums.FirstOrDefault(col => col.Value.ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)col.Value).DimensionType == DimensionTypeEnum.Time).Key;
                    string TimeVal = DBDataReader[ColTime].ToString();

                    Dictionary<string, object> Vals = new Dictionary<string, object>();
                    foreach (string col in Colums.Keys)
                    {
                        if (Colums[col] != null && Colums[col].ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)Colums[col]).DimensionType == DimensionTypeEnum.Frequency && ((IDimensionConcept)Colums[col]).IsFakeFrequency)
                            Vals[col] = (TimePeriodDBFormat.GetFrequencyValueFromTime(TimeVal));
                        else if (Colums[col] != null && Colums[col].ConceptObjectCode == FlyConfiguration.Time_Format_Id && isFakeTimeFormat)
                            Vals[col] = (TimePeriodDBFormat.GetTimeFormatValueFromTime(TimeVal));
                        else if (Colums[col] != null && Colums[col].ConceptType == ConceptTypeEnum.Dimension && ((IDimensionConcept)Colums[col]).DimensionType == DimensionTypeEnum.Time)
                            Vals[col] = (TimePeriodDBFormat.ParseTimeVal(DBDataReader[col]));
                        else
                            Vals[col] = (DBDataReader[col]);
                    }

                    List<DataMessageObject> dmo = new List<DataMessageObject>();
                    Colums.Keys.ToList().ForEach(col => dmo.Add(new DataMessageObject()
                    {
                        ColId = col,
                        ColImpl = Colums[col],
                        Val = Vals[col]
                    }));

                    RowsCounter++;
                    return dmo;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, ex);
            }
        }

       
    }
    /// <summary>
    /// Model Object that representing association of concept/value
    /// </summary>
    public class DataMessageObject
    {
        /// <summary>
        /// Identificable
        /// </summary>
        public string ColId { get; set; }
        /// <summary>
        /// Concept
        /// </summary>
        public IConceptObjectImpl ColImpl { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public object Val { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", ColId, Val);
        }
    }


}

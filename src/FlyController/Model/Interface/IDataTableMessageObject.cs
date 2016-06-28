using System;
using System.Collections.Generic;
namespace FlyController.Model
{
    /// <summary>
    /// IDataTableMessageObject is interface Model contains a mapping Database Data for insert in Sdmx response
    /// </summary>
    public interface IDataTableMessageObject
    {
        /// <summary>
        /// SqlDataReader conteins database response not readed (sequencial reading) 
        /// </summary>
        System.Data.SqlClient.SqlDataReader DBDataReader { get; set; }
        /// <summary>
        /// Check is present Frequency Dimension and if is Fake or real
        /// </summary>
        string FrequencyCol { get; }
        /// <summary>
        /// Get the Next response values
        /// </summary>
        /// <returns></returns>
        List<DataMessageObject> GetNext();
       
        /// <summary>
        /// Flag that representing Time format Name manually created
        /// </summary>
        bool isFakeTimeFormat { get; set; }
        /// <summary>
        /// Counter of Record return from Database
        /// </summary>
        int RowsCounter { get; set; }
        /// <summary>
        ///  Check the name of TimeDimension with a TimeDimendion in database field
        /// </summary>
        string TimeFormatCol { get; }

        /// <summary>
        /// Dictionary contains all Concepts
        /// </summary>
        Dictionary<string, IConceptObjectImpl> Colums { get; set; }

    }
}

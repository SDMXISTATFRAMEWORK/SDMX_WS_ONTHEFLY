using System;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface of Parameter information to pass at database
    /// </summary>
    public interface IParameterValue
    {
        /// <summary>
        /// Name of Parameter
        /// </summary>
        string Item { get; set; }
        /// <summary>
        /// Parameter Type in SqlDbType format
        /// </summary>
      
        System.Data.SqlDbType SqlType { get; set; }

        /// <summary>
        /// Parameter Value
        /// </summary>
        object Value { get; set; }
    }
}

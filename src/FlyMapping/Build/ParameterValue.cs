using FlyController.Model.Error;
using FlyMapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyMapping.Build
{
    /// <summary>
    /// Object model Contains a Parameter information to pass at database
    /// </summary>
    public class ParameterValue : IParameterValue
    {
        private String _Item;
        private object _Value;
        private System.Data.SqlDbType _SqlType;

        /// <summary>
        /// Create a ParameterValue instance
        /// </summary>
        public ParameterValue()
        {
            _Item = null;
            _Value = null;
            _SqlType = System.Data.SqlDbType.VarChar;
        }

        /// <summary>
        /// Name of Parameter
        /// </summary>
        public String Item
        {
            get { return _Item; }
            set { _Item = value; }
        }

        /// <summary>
        /// Parameter Value
        /// </summary>
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        /// <summary>
        /// Parameter Type in SqlDbType format
        /// </summary>
        public System.Data.SqlDbType SqlType
        {
            get { return _SqlType; }
            set { _SqlType = value; }
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} = {1}", _Item, _Value != null ? _Value.ToString() : "");
        }

        /// <summary>
        /// Create a SqlParameter
        /// </summary>
        /// <returns>SqlParameter</returns>
        public System.Data.SqlClient.SqlParameter CreateParameter()
        {
            try
            {
                return new System.Data.SqlClient.SqlParameter("@" + this.Item, this.SqlType) { Value = this.Value };
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateDBParameter, ex);
            }
        }
    }
}

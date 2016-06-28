using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Delegate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
namespace FlyMapping.Model
{
    /// <summary>
    /// Interface for Access to Database
    /// </summary>
    public interface IDBAccess
    {
        /// <summary>
        /// Performs correct store procedures and it process results
        /// </summary>
        /// <param name="operation">Operations Type (indicates the store procedure to execute)</param>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <returns>Returns a list of xmlNode with which the builder that identifies the response</returns>
        List<XmlNode> Execute(DBOperationEnum operation, List<IParameterValue> parameter);

        /// <summary>
        /// Performs correct store procedures and it process results into Table
        /// </summary>
        /// <param name="operation">Operations Type (indicates the store procedure to execute)</param>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <returns>Returns a Table of response</returns>
        DataTable ExecutetoTable(DBOperationEnum operation, List<IParameterValue> parameter);

        /// <summary>
        /// Execute a store procedure for get a Data for DataMessage
        /// </summary>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <param name="parse">Delegate to call for parse result</param>
        /// <param name="builder">Delegate to pass at GetDBResponseDelegate for write result</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        IFlyWriterBody ExecuteGetData(List<IParameterValue> parameter, GetDBResponseDelegate parse, WriteResponseDelegate builder);

        /// <summary>
        /// Connection preliminary to determine if the credentials are correct and whether the DataBase is reached
        /// </summary>
        void TestConnection();

         /// <summary>
        /// Check exist StoreProcedure in Database 
        /// </summary>
        /// <param name="operation">Get a relative StoreProcedureName</param>
        /// <returns>Boolean result: if true exist</returns>
        bool CheckExistStoreProcedure(DBOperationEnum operation);
    }
}

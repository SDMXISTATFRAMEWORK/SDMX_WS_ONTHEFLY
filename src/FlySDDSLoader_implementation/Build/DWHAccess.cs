using FlyController;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Delegate;
using FlyController.Model.Error;
using FlyController.Model.Streaming;
using FlyMapping.Build;
using FlyMapping.Model;
using FlyMapping.Model.Enum;
using OnTheFlyLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;

namespace FlySDDSLoader_implementation.Build
{
    /// <summary>
    /// Class of Access to Database
    /// </summary>
    public class DWHAccess : IDBAccess
    {
        /// <summary>
        /// ConnectionString (Configured in File Config)
        /// </summary>
        private String ConnectionString { get; set; }
        /// <summary>
        /// Sql Connection
        /// </summary>
        private SqlConnection _conn;

        /// <summary>
        /// Flag determines whether TestConnection was made
        /// </summary>
        private static bool TestConn = false;

        /// <summary>
        /// Create a instance of DWHAccess for access to Datawharehouse 
        /// Initializes the local variables needed to connect and throws a TestConnection
        /// </summary>
        /// <param name="_ConnectionString"></param>
        public DWHAccess(String _ConnectionString)
        {
            this.ConnectionString = _ConnectionString;
            if (!TestConn)
            {
                TestConn = true;
                //Test della connessione al DB
                TestConnection();
            }

        }

        /// <summary>
        /// Connection preliminary to determine if the credentials are correct and whether the DataBase is reached
        /// </summary>
        public void TestConnection()
        {
            try
            {
                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();
                //Connessione riuscita
                _conn.Close();
                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Database Test Connection successfully");

            }
            catch (SdmxException) { throw; }
            catch (Exception e)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ErrorConnectionDB, e);
            }
        }


        /// <summary>
        /// Performs correct store procedures and it process results
        /// </summary>
        /// <param name="operation">Operations Type (indicates the store procedure to execute)</param>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <returns>Returns a list of xmlNode with which the builder that identifies the response</returns>
        public List<XmlNode> Execute(DBOperationEnum operation, List<IParameterValue> parameter)
        {
            try
            {

                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();

                string SqlQuery = null;
                switch (operation)
                {
                    case DBOperationEnum.MSGetCodelist:
                        SqlQuery = QuerySql.MSGetCodelist(ref parameter);
                        break;
                    case DBOperationEnum.MSGetConceptScheme:
                        SqlQuery = QuerySql.MSGetConceptScheme(ref parameter);
                        break;
                    case DBOperationEnum.MSGetDSD:
                        SqlQuery = QuerySql.MSGetDSD(ref parameter);
                        break;
                    case DBOperationEnum.MSGetDataflows:
                        SqlQuery = QuerySql.MSGetDataflows(ref parameter);
                        break;
                    case DBOperationEnum.MSGetCategoryAndCategorisation:
                        SqlQuery = QuerySql.MSGetCategoryAndCategorisation(ref parameter);
                        break;
                }

                if (string.IsNullOrEmpty(SqlQuery))
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.CreateDBParameter, new Exception(string.Format("Not recognized operation: {0}", operation.ToString())));

                SqlCommand cmd = new SqlCommand(SqlQuery, _conn);
                cmd.CommandTimeout = int.MaxValue;
                cmd.CommandType = CommandType.Text;

                //if (parameter != null)
                //    parameter.ForEach(p => cmd.Parameters.Add(((ParameterValue)p).CreateParameter()));

                StringBuilder dbres = new StringBuilder();
                using (SqlDataReader rea = cmd.ExecuteReader())
                {
                    while (rea.Read())
                        dbres.Append(rea.GetValue(0).ToString());
                }
                //  cmd.ExecuteScalar();//perche ritorna sempre un valore E'LIMITATO A 2033 CARATTERI
                _conn.Close();
                if (dbres == null || string.IsNullOrEmpty(dbres.ToString()))
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse);


                XmlDocument risposta = new XmlDocument();
                risposta.LoadXml(dbres.ToString());
                if (risposta.ChildNodes.Count > 0)
                {
                    StringBuilder Par = new StringBuilder();
                    foreach (var item in parameter)
                        Par.AppendLine(String.Format("{0} \t\t-> {1}", item.Item, item.Value));
                    FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, "Execution Store procedure {0} for {1} Successfully. Reading results {2}", cmd.CommandText, operation.ToString(), Par);
                    return new List<XmlNode>(risposta.ChildNodes[0].ChildNodes.Cast<XmlNode>());
                }
                else
                    FlyLog.WriteLog(this, FlyLog.LogTypeEnum.Error, "Execution Store procedure {0} for {1}. NO RESULT", cmd.CommandText, operation.ToString());

                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse);

            }
            catch (SdmxException) { throw; }
            catch (Exception e)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
            }
        }


        #region Non Implementate
        /// <summary>
        /// Performs correct store procedures and it process results into Table
        /// </summary>
        /// <param name="operation">Operations Type (indicates the store procedure to execute)</param>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <returns>Returns a Table of response</returns>
        public DataTable ExecutetoTable(DBOperationEnum operation, List<IParameterValue> parameter)
        {
            try
            {
                DataTable dt = new DataTable();
                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();

                string SqlQuery = null;

                switch (operation)
                {
                    case DBOperationEnum.GetCategorySchemes:
                        SqlQuery = QuerySql.MSGetCategoryScheme(ref parameter);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                SqlCommand cmd = new SqlCommand(SqlQuery, _conn);
                cmd.CommandTimeout = int.MaxValue;
                cmd.CommandType = CommandType.Text;

                using (SqlDataReader rea = cmd.ExecuteReader())
                {
                    dt.Load(rea);
                }

                _conn.Close();

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Not Implemented in this Project
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parse"></param>
        /// <param name="builder"></param>
        /// <returns> throw new NotImplementedException()</returns>
        public IFlyWriterBody ExecuteGetData(List<IParameterValue> parameter, GetDBResponseDelegate parse, WriteResponseDelegate builder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not Implemented in this Project
        /// </summary>
        /// <param name="operation"></param>
        /// <returns> throw new NotImplementedException()</returns>
        public bool CheckExistStoreProcedure(DBOperationEnum operation)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

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

namespace FlyDotStat_implementation.Build
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

                SqlCommand cmd = new SqlCommand(GetStoreRef(operation), _conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameter != null)
                    parameter.ForEach(p => cmd.Parameters.Add(((ParameterValue)p).CreateParameter()));

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



        /// <summary>
        /// Execute a store procedure for get a Data for DataMessage
        /// </summary>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <param name="parse">Delegate to call for parse result</param>
        /// <param name="builder">Delegate to pass at GetDBResponseDelegate for write result</param>
        /// <returns>Object for Write response in streaming <see cref="IFlyWriterBody"/></returns>
        public IFlyWriterBody ExecuteGetData(List<IParameterValue> parameter, GetDBResponseDelegate parse, WriteResponseDelegate builder)
        {
            try
            {
                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();

                SqlCommand cmd = new SqlCommand(GetStoreRef(DBOperationEnum.GetData), _conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameter != null)
                    parameter.ForEach(p => cmd.Parameters.Add(((ParameterValue)p).CreateParameter()));

                //if (true)//FAKE
                //{
                //    cmd = new SqlCommand("SELECT TOP 1000000 *  FROM [Datawarehouse].[dbo].[Dataset_6_ViewAllData]", _conn);
                //    cmd.CommandType = CommandType.Text;
                //    cmd.Parameters.Clear();
                //}

                cmd.CommandTimeout = int.MaxValue;
                SqlDataReader rea = cmd.ExecuteReader();

                StringBuilder Par = new StringBuilder();
                foreach (var item in parameter)
                    Par.AppendLine(String.Format("{0} \t\t-> {1}", item.Item, item.Value));

                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"Execution Store procedure {0} for  data request Successfully. Reading results {1}", cmd.CommandText, Par);

                return new FlyDataWriterBody()
                 {
                     Rea = rea,
                     Conn = _conn,
                     Builder = builder,
                     Parser = parse
                 };
            }
            catch (SdmxException) { throw; }
            catch (Exception e)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
            }
        }


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

                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();

                SqlCommand cmd = new SqlCommand(GetStoreRef(operation), _conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameter != null)
                    parameter.ForEach(p => cmd.Parameters.Add(((ParameterValue)p).CreateParameter()));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable RisTable = new DataTable();
                da.Fill(RisTable);
                _conn.Close();
                return RisTable;

            }
            catch (SdmxException) { throw; }
            catch (Exception e)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
            }
        }

        /// <summary>
        /// Check exist StoreProcedure in Database 
        /// </summary>
        /// <param name="operation">Get a relative StoreProcedureName</param>
        /// <returns>Boolean result: if true exist</returns>
        public bool CheckExistStoreProcedure(DBOperationEnum operation)
        {
            try
            {
                if (!FlyConfiguration.StoreProcedureSettings.Exists(s => s.Operation == operation))
                    return false;
                string onlynameSP = FlyConfiguration.StoreProcedureSettings.Find(s => s.Operation == operation).StoreProcedureName;
                if (string.IsNullOrEmpty(onlynameSP))
                    return false;
                if (onlynameSP.Contains(".") && onlynameSP.Split('.').Length > 1)
                    onlynameSP = onlynameSP.Split('.')[1];
                string checkSP = String.Format("select count(*) from sys.objects where type = 'p' and name='{0}'",
                    onlynameSP);


                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();
                SqlCommand cmd = new SqlCommand(checkSP, _conn);

                bool Exist = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                _conn.Close();
                return Exist;
            }
            catch (Exception)
            {
                return false;
            }

        }


        /// <summary>
        /// Mapping enum DBOperationEnum, with names in the store procedure to execute
        /// </summary>
        /// <param name="operation">DBOperationEnum value to convert in StoreProcedure name</param>
        /// <returns>Name of StoreProcedure</returns>
        private string GetStoreRef(DBOperationEnum operation)
        {
            try
            {
                if (FlyConfiguration.StoreProcedureSettings.Exists(s => s.Operation == operation))
                    return FlyConfiguration.StoreProcedureSettings.Find(s => s.Operation == operation).StoreProcedureName;
                else
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, new Exception("DB Operation not recognized"));
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.InternalError, ex);
            }
        }


        #region SpecialCodelist

        internal XmlNodeList TreeExtract()
        {
            try
            {
                SqlConnection _conn = new SqlConnection(FlyConfiguration.ConnectionStringCategory);
                _conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM ContentTree", _conn);
                SqlDataReader xmlTreeReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(xmlTreeReader);
                string xmlTree = "";
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    xmlTree = dt.Rows[i]["Data"].ToString();
                    if (!string.IsNullOrEmpty(FlyConfiguration.CategoryName) && FlyConfiguration.CategoryName.Trim().ToLower() == dt.Rows[i]["Name"].ToString().Trim().ToLower())
                        break;
                }
                _conn.Close();
                if (string.IsNullOrEmpty(xmlTree))
                    throw new Exception("No Category Found");
                XmlDocument treedoc = new XmlDocument();
                treedoc.LoadXml(xmlTree);
                if (treedoc.ChildNodes.Count > 0)
                {
                    foreach (XmlNode item in treedoc.ChildNodes[0].ChildNodes)
                    {
                        if (item.Name == "ThemeList")
                            return item.ChildNodes;
                    }
                }
                return null;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, ex);
                //non ritorno niente è sbagliato qualcosa all'interno del DB
            }
        }

        /// <summary>
        /// Return a Number of record of Data result of GetData StoreProcedure
        /// </summary>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <returns>Number of record of Observation</returns>
        public int GetCL_COUNT(List<IParameterValue> parameter)
        {
            try
            {
                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();
                string datasetCode = parameter.Find(p => p.Item == "DataSetCode").Value.ToString();
                string SqlQuery = @"
declare @DataSetCode varchar(100) = '" + datasetCode + @"'
declare @IDSet int= NULL
declare @req_FiltTableName nvarchar(255)


select TOP 1 @IDSet = IDSet from CatSet where Code=@DataSetCode
IF (@IDSet IS NULL)
BEGIN
	RAISERROR ('Dataset not found',15,1)
END

SET @req_FiltTableName= 'Dataset_'+ convert(varchar(50),@IDSet) +'_ViewAllData'

exec('select count(*) from ' + @req_FiltTableName)";

                int conta = 0;
                SqlCommand cmd = new SqlCommand(SqlQuery, _conn);
                cmd.CommandTimeout = int.MaxValue;
                cmd.CommandType = CommandType.Text;

                conta = Convert.ToInt32(cmd.ExecuteScalar());
                _conn.Close();

                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"Execution Store procedure {0} for  Get CL_COUNT request Successfully. Reading results", cmd.CommandText);
                return conta;
            }
            catch (SdmxException) { throw; }
            catch (Exception e)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
            }
        }

        /// <summary>
        /// Return a Minor and Max DateTime into recorSet data results of GetData StoreProcedure
        /// </summary>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <param name="FirstTime">OUT Minor DateTime into recorSet data results</param>
        /// <param name="EndTime">OUT Max DateTime into recorSet data results</param>
        public void GetCL_TIME_PERIOD(List<IParameterValue> parameter, out string FirstTime, out string EndTime)
        {
            try
            {
                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();
                string datasetCode = parameter.Find(p => p.Item == "DataSetCode").Value.ToString();
                string SqlQuery = @"
declare @DataSetCode varchar(100) = '" + datasetCode + @"'

declare @TimeCol varchar(100) = NULL
declare @IDSet int= NULL
declare @req_FiltTableName nvarchar(255)


select TOP 1 @IDSet = IDSet from CatSet where Code=@DataSetCode
IF (@IDSet IS NULL)
BEGIN
	RAISERROR ('Dataset not found',15,1)
END
select TOP 1 @TimeCol='_' + Code from CatDim where IDSet=@IDSet AND IsTimeSeriesDim=1
IF (@TimeCol IS NULL)
BEGIN
	RAISERROR ('Time dimension not found',15,1)
END

SET @req_FiltTableName= 'Dataset_'+ convert(varchar(50),@IDSet) +'_ViewAllData'
exec('select TOP 1 
(select DISTINCT top 1 '+@TimeCol+' as tempo from ' + @req_FiltTableName + ' order by tempo) as startPeriod,
(select DISTINCT top 1 '+@TimeCol+' as tempo from ' + @req_FiltTableName + ' order by tempo desc) as endPeriod 
from '+ @req_FiltTableName )";

                FirstTime = null;
                EndTime = null;
                SqlCommand cmd = new SqlCommand(SqlQuery, _conn);
                cmd.CommandTimeout = int.MaxValue;
                cmd.CommandType = CommandType.Text;

                using (SqlDataReader rea = cmd.ExecuteReader())
                {
                    while (rea.Read())
                    {
                        FirstTime = rea["startPeriod"].ToString();
                        EndTime = rea["endPeriod"].ToString();
                    }
                }
                _conn.Close();

                if (!string.IsNullOrEmpty(FirstTime))
                    FirstTime = FirstTime.Substring(0, 4);
                if (!string.IsNullOrEmpty(EndTime))
                    EndTime = EndTime.Substring(0, 4);

                FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"Execution Store procedure {0} for  Get CL_TIME_PERIOD request Successfully. Reading results", cmd.CommandText);

            }
            catch (SdmxException) { throw; }
            catch (Exception e)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
            }
        }
        #region OLD Function
        //public void GetCL_TIME_PERIOD(List<IParameterValue> parameter, out string FirstTime, out string EndTime)
        //{
        //    try
        //    {
        //        _conn = new SqlConnection();
        //        _conn.ConnectionString = this.ConnectionString;
        //        _conn.Open();

        //        SqlCommand cmd = new SqlCommand(GetStoreRef(DBOperationEnum.GetData), _conn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        if (parameter != null)
        //            parameter.ForEach(p => cmd.Parameters.Add(((ParameterValue)p).CreateParameter()));


        //        cmd.CommandTimeout = int.MaxValue;
        //        FirstTime = null;
        //        EndTime = null;
        //        string TimeColum = null;

        //        using (SqlDataReader rea = cmd.ExecuteReader())
        //        {

        //            if (string.IsNullOrEmpty(TimeColum))
        //            {
        //                DataTable schemaTable = rea.GetSchemaTable();
        //                foreach (DataRow item in schemaTable.Rows)
        //                {
        //                    string itemColumnName = item["ColumnName"].ToString();
        //                    if (itemColumnName.Trim().ToUpper() == SpacialColumsNamesEnum._TIME.ToString() ||
        //                    itemColumnName.Trim().ToUpper() == SpacialColumsNamesEnum._TIME_.ToString())
        //                    {
        //                        TimeColum = itemColumnName;
        //                        break;
        //                    }
        //                }

        //                if (string.IsNullOrEmpty(TimeColum))
        //                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, new Exception("Not Found Time Colums in DB"));
        //            }

        //            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"Execution Store procedure {0} for  Get CL_COUNT request Successfully. Reading results", cmd.CommandText);
        //            while (rea.Read())
        //            {
        //                string ActDate = rea[TimeColum].ToString();
        //                if (ActDate.Trim().Length < 4)
        //                    continue;
        //                if (string.IsNullOrEmpty(FirstTime))
        //                {
        //                    FirstTime = ActDate.Trim().Substring(0, 4);
        //                    EndTime = ActDate.Trim().Substring(0, 4);
        //                    continue;
        //                }
        //                if (ActDate.Trim().Substring(0, 4).CompareTo(FirstTime) < 0)
        //                    FirstTime = ActDate.Trim().Substring(0, 4);
        //                if (ActDate.Trim().Substring(0, 4).CompareTo(EndTime) > 0)
        //                    EndTime = ActDate.Trim().Substring(0, 4);
        //            }
        //        }
        //    }
        //    catch (SdmxException) { throw; }
        //    catch (Exception e)
        //    {
        //        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
        //    }
        //}
        //public int GetCL_COUNT(List<IParameterValue> parameter)
        //{
        //    try
        //    {
        //        _conn = new SqlConnection();
        //        _conn.ConnectionString = this.ConnectionString;
        //        _conn.Open();

        //        SqlCommand cmd = new SqlCommand(GetStoreRef(DBOperationEnum.GetData), _conn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        if (parameter != null)
        //            parameter.ForEach(p => cmd.Parameters.Add(((ParameterValue)p).CreateParameter()));


        //        cmd.CommandTimeout = int.MaxValue;
        //        int Counter = 0;
        //        using (SqlDataReader rea = cmd.ExecuteReader())
        //        {
        //            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"Execution Store procedure {0} for  Get CL_COUNT request Successfully. Reading results", cmd.CommandText);
        //            while (rea.Read())
        //                Counter++;
        //        }
        //        return Counter;
        //    }
        //    catch (SdmxException) { throw; }
        //    catch (Exception e)
        //    {
        //        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
        //    }
        //}
        //public List<string> GetCL_CONTRAINED(List<IParameterValue> parameter, string ConceptColum)
        //{
        //    try
        //    {
        //        _conn = new SqlConnection();
        //        _conn.ConnectionString = this.ConnectionString;
        //        _conn.Open();

        //        SqlCommand cmd = new SqlCommand(GetStoreRef(DBOperationEnum.GetData), _conn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        if (parameter != null)
        //            parameter.ForEach(p => cmd.Parameters.Add(((ParameterValue)p).CreateParameter()));


        //        cmd.CommandTimeout = int.MaxValue;
        //        List<string> CodeObject = new List<string>();
        //        using (SqlDataReader rea = cmd.ExecuteReader())
        //        {
        //            bool ExistColum = false;
        //            DataTable schemaTable = rea.GetSchemaTable();
        //            string TimeCol = "";
        //            foreach (DataRow item in schemaTable.Rows)
        //            {
        //                string itemColumnName = item["ColumnName"].ToString();
        //                if (itemColumnName.Trim().ToUpper() == ConceptColum.Trim().ToUpper())
        //                {
        //                    ConceptColum = itemColumnName;
        //                    ExistColum = true;
        //                }
        //                else if (itemColumnName.Trim().ToUpper() == SpacialColumsNamesEnum._TIME.ToString() ||
        //                    itemColumnName.Trim().ToUpper() == SpacialColumsNamesEnum._TIME_.ToString())
        //                {
        //                    TimeCol = itemColumnName;
        //                }
        //            }
        //            StringBuilder Par = new StringBuilder();
        //            foreach (var item in parameter)
        //                Par.AppendLine(String.Format("{0} \t\t-> {1}", item.Item, item.Value));

        //            FlyLog.WriteLog(this, FlyLog.LogTypeEnum.All, @"Execution Store procedure {0} for  Get CL_COUNT request Successfully. Reading results {1}", cmd.CommandText, Par);
        //            while (rea.Read())
        //            {
        //                if (!ExistColum)
        //                {
        //                    if (ConceptColum.Trim().ToUpper() == "FREQ")
        //                    {
        //                        string TVal = (TimePeriodDBFormat.GetFrequencyValueFromTime(rea[TimeCol].ToString()));
        //                        if (!CodeObject.Contains(TVal))
        //                            CodeObject.Add(TVal);
        //                    }
        //                    else
        //                        break;
        //                }
        //                else
        //                {
        //                    string Val = rea[ConceptColum].ToString();
        //                    if (!CodeObject.Contains(Val))
        //                        CodeObject.Add(Val);
        //                }
        //            }
        //        }
        //        return CodeObject;
        //    }
        //    catch (SdmxException) { throw; }
        //    catch (Exception e)
        //    {
        //        throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
        //    }
        //}
        #endregion

        /// <summary>
        /// Return a CodelistConstrained with other Dimension in Join
        /// </summary>
        /// <param name="parameter">Parameters to be passed to the store procedure</param>
        /// <param name="ConceptColum">List of Other Dimension separated by Comma</param>
        /// <returns></returns>
        public List<string> GetCL_CONTRAINED(List<IParameterValue> parameter, string ConceptColum)
        {
            try
            {
                _conn = new SqlConnection();
                _conn.ConnectionString = this.ConnectionString;
                _conn.Open();
                string datasetCode = parameter.Find(p => p.Item == "DataSetCode").Value.ToString();
                string WhereStatement = parameter.Find(p => p.Item == "WhereStatement").Value.ToString().Replace("'","''");
                string TimePar = parameter.Find(p => p.Item == "Time").Value.ToString().Replace("'", "''");

                string SqlQuery = string.Format(@"
declare @DataSetCode varchar(100) = '{0}'
declare @ColumDistinct varchar(100) = '{1}'
declare @WhereStatement varchar(max)='{2}'
declare @Time varchar(max) = '{3}'

declare @TableID nvarchar(255);
declare @TableName nvarchar(255);
declare @req_WherePart as varchar(max);

SET @TableID =
(
	SELECT CatSet.[IDSet] FROM  CatSet WHERE CatSet.Code=@DataSetCode
)
SET @TableName = 'Dataset_'+@TableID+'_ViewAllData';
SET @req_WherePart=@WhereStatement
if(RTRIM(@Time) <> '')
  SET @req_WherePart = @req_WherePart +' AND '+ @Time

Set @req_WherePart=@WhereStatement
if(RTRIM(@Time) <> '')
  SET @req_WherePart = @req_WherePart +' AND '+ @Time

SELECT
@req_WherePart=
(CASE WHEN CD.IsTimeSeriesDim=1  THEN replace(@req_WherePart,'[$TIME_PERIOD$].Code', '[_'+CD.CODE+']')
 ELSE replace(@req_WherePart,'[$'+CD.CODE+'$].Code', '[_'+CD.CODE+']')
 END
)
from CatDim as CD 
where  CD.IDSet=@TableID

declare @TimeCol as varchar(100)
select TOP 1 @TimeCol= Code from CatDim where IDSet=@TableID AND IsTimeSeriesDim=1

SET @ColumDistinct=REPLACE(@ColumDistinct,'TIME_PERIOD',@TimeCol);

exec('select DISTINCT _'+@ColumDistinct+'  from '+@TableName + ' where ' + @req_WherePart)",
datasetCode, (ConceptColum.Trim().ToUpper() == "FREQ" ? "TIME_PERIOD" : ConceptColum), WhereStatement, TimePar);


                SqlCommand cmd = new SqlCommand(SqlQuery, _conn);
                cmd.CommandTimeout = int.MaxValue;
                cmd.CommandType = CommandType.Text;

                List<string> CodeObject = new List<string>();
                using (SqlDataReader rea = cmd.ExecuteReader())
                {
                    while (rea.Read())
                    {
                        if (ConceptColum.Trim().ToUpper() == "FREQ")
                        {
                            string TVal = (TimePeriodDBFormat.GetFrequencyValueFromTime(rea[0].ToString()));
                            if (!CodeObject.Contains(TVal))
                                CodeObject.Add(TVal);
                        }
                        else
                        {
                            string Val = rea[0].ToString();
                            if (!CodeObject.Contains(Val))
                                CodeObject.Add(Val);
                        }
                    }
                }
                _conn.Close();

                return CodeObject;
            }
            catch (SdmxException) { throw; }
            catch (Exception e)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.DBErrorResponse, e);
            }
        }



        #endregion

    }
}

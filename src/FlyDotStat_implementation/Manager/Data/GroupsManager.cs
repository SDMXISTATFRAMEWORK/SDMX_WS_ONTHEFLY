using FlyController;
using FlyController.Model;
using FlyController.Model.DbSetting;
using FlyController.Model.Error;
using FlyEngine.Model;
using FlyMapping.Build;
using FlyMapping.Model;
using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FlyDotStat_implementation.Manager.Data
{
    /// <summary>
    /// Class for retreive Groups information from Database
    /// </summary>
    public class GroupsManager : BaseManager, IGroupsManager
    {
        /// <summary>
        /// Inizialize for BaseManager Class
        /// </summary>
        /// <param name="_TimeStamp">LastUpdate parameter request only data from this date onwards</param>
        /// <param name="_versionTypeResp">Sdmx Version</param>
        public GroupsManager(string _TimeStamp, SdmxSchemaEnumType _versionTypeResp)
            : base(null, _versionTypeResp) { }

        /// <summary>
        /// LastUpdate parameter request only data from this date onwards
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// Get Groups information from Database
        /// </summary>
        /// <param name="DataFlowID">Dataflow Code</param>
        /// <param name="GroupId">Group Code</param>
        /// <param name="DimensionRef">All Dimension in references of Group</param>
        /// <param name="AttributeRef">All Attribute that referenced this Group</param>
        /// <returns>List of <see cref="DataGroupObject"/></returns>
        public List<DataGroupObject> GetGroups(string DataFlowID, string GroupId, List<string> DimensionRef, List<string> AttributeRef)
        {
            try
            {
                string Columns = string.Join(", ", DimensionRef);
                if (AttributeRef!=null && AttributeRef.Count>0)
                    Columns= string.Format("{0}, {1}", Columns,string.Join(", ", AttributeRef));
                
                List<DataGroupObject> Gruppi = new List<DataGroupObject>();
                if (this.DbAccess.CheckExistStoreProcedure(DBOperationEnum.GetGroups))
                {
                    //prima capisco se non è un attributo o un flag
                    List<IParameterValue> parametri = new List<IParameterValue>() 
                { 
                    new ParameterValue() { Item = "DatasetCode", Value = DataFlowID } ,
                    new ParameterValue() { Item = "Columns", Value = Columns} ,
                    new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                    new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
                };
                    if (!string.IsNullOrEmpty(this.TimeStamp))
                        parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.TimeStamp, SqlType = SqlDbType.DateTime });
                    //EFFETTUO LA RICHIESTA AL DB

                    DataTable risposta = this.DbAccess.ExecutetoTable(DBOperationEnum.GetGroups, parametri);

                    if (risposta!=null && risposta.Rows.Count>0)
                    {
                        foreach (DataRow groupTable in risposta.Rows)
                        {
                            DataGroupObject group = new DataGroupObject(GroupId);
                            for (int i = 0; i < risposta.Columns.Count; i++)
                            {
                                if (DimensionRef.Contains(risposta.Columns[i].ColumnName))
                                {
                                    group.DimensionReferences.Add(new GroupReferenceObject()
                                    {
                                        ConceptCode = risposta.Columns[i].ColumnName,
                                        ConceptValue = groupTable[i]
                                    });
                                }
                                else
                                {
                                    group.AttributeReferences.Add(new GroupReferenceObject()
                                    {
                                        ConceptCode = risposta.Columns[i].ColumnName,
                                        ConceptValue = groupTable[i]
                                    });
                                }
                               
                            }
                            Gruppi.Add(group);
                        }
                    }
                }


                return Gruppi;

            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RetreiveGroups, ex);
            }

        }


        /// <summary>
        /// Referenced objects (Not Used for Groups)
        /// </summary>
        public override IReferencesObject ReferencesObject { get { return null; } set { } }
    }
}

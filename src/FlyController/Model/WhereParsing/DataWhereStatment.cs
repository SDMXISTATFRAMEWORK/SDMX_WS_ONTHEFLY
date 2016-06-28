using FlyController.Model.Error;
using FlyController.Model.WhereParsing;
using Org.Sdmxsource.Sdmx.Api.Model.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query.Complex;
using Org.Sdmxsource.Sdmx.Api.Model.Query;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Data.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// DataWhereStatment Parse a SDMX Data Query
    /// </summary>
    public class DataWhereStatment : IDataWhereStatment
    {
        /// <summary>
        /// Flag that determinate if is Rest call
        /// </summary>
        public bool IsRest { get; set; }
        /// <summary>
        /// The rest call must be parsed after populate the retreivalManger.
        /// Save the IRestDataQuery in this field for PostParsing
        /// </summary>
        public IRestDataQuery Query { get; set; }


        /// <summary>
        /// List of Data Where Statment SelectionGroup Parsed
        /// </summary>
        public List<IDataWhereStatmentSelectionGroup> SelectionGroups { get; set; }
        /// <summary>
        /// DimensionAtObservation (Null if not exist in query)
        /// </summary>
        public string DimensionAtObservation { get; set; }

        /// <summary>
        /// Create a instance of DataWhereStatment from RetrievalManager.BuildDataQuery result for SDMX 2.0 Version
        /// </summary>
        /// <param name="WhereStatement20">RetrievalManager.BuildDataQuery result (IList of IDataQuery ) </param>
        public DataWhereStatment(IList<IDataQuery> WhereStatement20)
        {
            ParseGroup20(WhereStatement20);
        }

        /// <summary>
        /// Create a instance of DataWhereStatment from RetrievalManager.BuildComplexDataQuery result for SDMX 2.1 Version
        /// </summary>
        /// <param name="WhereStatement21">RetrievalManager.BuildComplexDataQuery result (IList of IComplexDataQuery) </param>
        public DataWhereStatment(IList<IComplexDataQuery> WhereStatement21)
        {
            ParseGroup21(WhereStatement21);
        }

        /// <summary>
        /// Create a instance of DataWhereStatment from RetrievalManager.BuildComplexDataQuery result for SDMX REST Version
        /// </summary>
        /// <param name="WhereStatementREST">IRestDataQuery <see cref="IRestDataQuery"/></param>
        public DataWhereStatment(IRestDataQuery WhereStatementREST)
        {
            ParseGroupREST(WhereStatementREST);
        }



        /// <summary>
        /// Parsing a Query SDMX 2.0 and create SelectionGroups
        /// </summary>
        /// <param name="WhereStatement20">Query Parameter</param>
        private void ParseGroup20(IList<IDataQuery> WhereStatement20)
        {
            try
            {
                if (WhereStatement20.Count == 0)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RetrivalParsingError);

                this.SelectionGroups = new List<IDataWhereStatmentSelectionGroup>();
                foreach (var item in WhereStatement20[0].SelectionGroups)
                    this.SelectionGroups.Add(new DataWhereStatmentSelectionGroup(item));
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, ex);
            }
        }

        /// <summary>
        /// Parsing a Query SDMX 2.0 and create SelectionGroups
        /// </summary>
        /// <param name="WhereStatement21">Query Parameter</param>
        private void ParseGroup21(IList<IComplexDataQuery> WhereStatement21)
        {
            try
            {
                if (WhereStatement21.Count == 0)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RetrivalParsingError);

                this.SelectionGroups = new List<IDataWhereStatmentSelectionGroup>();
                foreach (var item in WhereStatement21[0].SelectionGroups)
                    this.SelectionGroups.Add(new DataWhereStatmentSelectionGroup(item));

                this.DimensionAtObservation = WhereStatement21[0].DimensionAtObservation;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, ex);
            }
        }

        /// <summary>
        /// Prepare a Parsing of Query SDMX REST 
        /// </summary>
        /// <param name="WhereStatementREST">Query Parameter</param>
        private void ParseGroupREST(IRestDataQuery WhereStatementREST)
        {
            try
            {
                if (WhereStatementREST == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RetrivalParsingError);

                IsRest = true;
                Query = WhereStatementREST;

                this.DimensionAtObservation = WhereStatementREST.DimensionAtObservation;
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, ex);
            }
        }

        /// <summary>
        /// Parse a Query SDMX REST and create SelectionGroups
        /// </summary>
        /// <param name="SdmxRetrievalManager"></param>
        public void BuildRestQuery(Org.Sdmxsource.Sdmx.Api.Manager.Retrieval.ISdmxObjectRetrievalManager SdmxRetrievalManager)
        {
            try
            {
                this.SelectionGroups = new List<IDataWhereStatmentSelectionGroup>();
                
                IDataQuery dataQuery = new DataQueryImpl(this.Query, SdmxRetrievalManager);
                if (dataQuery == null)
                    throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.RetrivalParsingError);

                foreach (var item in dataQuery.SelectionGroups)
                    this.SelectionGroups.Add(new DataWhereStatmentSelectionGroup(item));
            }
            catch (SdmxException) { throw; }
            catch (Exception ex)
            {
                throw new SdmxException(this, FlyExceptionObject.FlyExceptionTypeEnum.ParsingQueryError, ex);
            }
        }
    }
}

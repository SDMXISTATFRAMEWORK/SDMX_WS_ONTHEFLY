using FlyController.Model.WhereParsing;
using Org.Sdmxsource.Sdmx.Api.Model.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query;
using Org.Sdmxsource.Sdmx.Api.Model.Data.Query.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// DataWhereStatmentSelectionGroup Parse a SDMX Data Query Selection Groups
    /// </summary>
    public class DataWhereStatmentSelectionGroup : IDataWhereStatmentSelectionGroup
    {
        /// <summary>
        /// Selection Group SDMX 2.0
        /// </summary>
        private IDataQuerySelectionGroup Group20 = null;
        /// <summary>
        /// Selection Group SDMX 2.0
        /// </summary>
        private IComplexDataQuerySelectionGroup Group21 = null;

        /// <summary>
        /// Create a instance of DataWhereStatmentSelectionGroup for SDMX 2.0 Version
        /// </summary>
        /// <param name="_group20">from DataWhereStatment ParseGroup20</param>
        public DataWhereStatmentSelectionGroup(IDataQuerySelectionGroup _group20)
        {
            this.Group20 = _group20;
            this.Group21 = null;
            ParseGroup20();
        }
        /// <summary>
        /// Create a instance of DataWhereStatmentSelectionGroup for SDMX 2.1 Version
        /// </summary>
        /// <param name="_group21">from DataWhereStatment ParseGroup21</param>
        public DataWhereStatmentSelectionGroup(IComplexDataQuerySelectionGroup _group21)
        {
            this.Group20 = null;
            this.Group21 = _group21;
            ParseGroup21();
        }

        /// <summary>
        /// Parsing a SelectionGroup SDMX 2.0 and create Selections Where Statment
        /// </summary>
        private void ParseGroup20()
        {
            this.DateFrom = Group20.DateFrom;
            this.DateTo = Group20.DateTo;
            this.Selections = new List<IDataWhereStatmentSelection>();
            foreach (var item in Group20.Selections)
            {
                IDataWhereStatmentSelection dws = new DataWhereStatmentSelection() { ComponentId = item.ComponentId };
                if (item.HasMultipleValues)
                    dws.Values = item.Values.ToList<string>();
                else
                    dws.Values = new List<string>() { item.Value };

                this.Selections.Add(dws);
            }
        }
        /// <summary>
        /// Parsing a SelectionGroup SDMX 2.1 and create Selections Where Statment
        /// </summary>
        private void ParseGroup21()
        {
            this.DateFrom = Group21.DateFrom;
            this.DateTo = Group21.DateTo;
            this.Selections = new List<IDataWhereStatmentSelection>();
            foreach (var item in Group21.Selections)
            {
                IDataWhereStatmentSelection dws = new DataWhereStatmentSelection() { ComponentId = item.ComponentId };
                if (item.HasMultipleValues())
                {
                    //dws.Values = new List<string>();
                    //foreach (var val in item.Values)
                    //    dws.Values.Add(val.Value);

                    dws.Values = (from v in item.Values
                                  select v.Value).ToList();
                }
                else
                    dws.Values = new List<string>() { item.Value.Value };

                this.Selections.Add(dws);
            }
        }

        /// <summary>
        /// list of  Selection Where Statment
        /// </summary>
        public List<IDataWhereStatmentSelection> Selections { get; set; }
        /// <summary>
        /// Date From in SdmxDate If in query contains Tipe Period Filter 
        /// </summary>
        public ISdmxDate DateFrom { get; set; }
        /// <summary>
        /// Date To in SdmxDate If in query contains Tipe Period Filter 
        /// </summary>
        public ISdmxDate DateTo { get; set; }

    }
}

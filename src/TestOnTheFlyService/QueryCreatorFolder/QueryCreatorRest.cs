using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace TestOnTheFlyService
{
    public partial class QueryCreatorRest : UserControl, TestOnTheFlyService.IQueryCreator
    {
        public event QueryCreator.voidDelegate ResetEvent;
        public event QueryCreator.voidDelegate UpdateFlatEvent;
        public event QueryCreator.GetDataEventDelegate GetDataEvent;

        public TestFlyQueryCreation.SdmxVersionEnum SdmxVersion { get; set; }


        public QueryCreatorRest()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            switch (this.SdmxVersion)
            {
                case TestFlyQueryCreation.SdmxVersionEnum.Sdmx20:
                    cmbTypeOP.Items.Add("GenericData");
                    cmbTypeOP.Items.Add("CompactData");
                    cmbTypeOP.Items.Add("RDF");
                    cmbTypeOP.Items.Add("DSPL");
                    cmbTypeOP.Items.Add("JSON");
                    ChkFlat.Visible = false;
                    break;
                case TestFlyQueryCreation.SdmxVersionEnum.Sdmx21:
                    cmbTypeOP.Items.Add("GenericData");
                    cmbTypeOP.Items.Add("StructureSpecificData");
                    cmbTypeOP.Items.Add("RDF");
                    cmbTypeOP.Items.Add("DSPL");
                    cmbTypeOP.Items.Add("JSON");
                    ChkFlat.Visible = true;
                    break;
            }
            cmbTypeOP.SelectedIndex = 0;
            cmbTypeOP_SelectedIndexChanged(cmbTypeOP, new EventArgs());

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (ResetEvent != null)
                ResetEvent();
        }



        private void btnGetData_Click(object sender, EventArgs e)
        {
            //SendQueryStreaming.StructTypeEnum StructType = SendQueryStreaming.StructTypeEnum.GenericData;
            //switch (cmbTypeOP.SelectedItem.ToString())
            //{
            //    case "CompactData":
            //        StructType = SendQueryStreaming.StructTypeEnum.Compact;
            //        break;
            //    case "StructureSpecificData":
            //        StructType = SendQueryStreaming.StructTypeEnum.StructureSpecificData;
            //        break;
            //}
            if (GetDataEvent != null)
                GetDataEvent(txtQuery.Text, txtHeader.Text);
        }

        public void UpdateQuery(SdmxObject _ChoosesDf, Dictionary<string, List<SdmxQueryInput>> _DataQuery, List<OrderDim> _DimensionOrdered)
        {
            txtQuery.Text = "Loading...";
            Application.DoEvents();
            Thread tt = new Thread(new ParameterizedThreadStart(UpdateQueryThread));
            tt.Start(new objforQuery() { ChoosesDf = _ChoosesDf, DataQuery = _DataQuery, DimensionOrdered = _DimensionOrdered });
        }

        private string QueryTmp = null;
        public void UpdateQueryThread(object testobj)
        {
            QueryTmp = "";
            try
            {
                objforQuery obj = testobj as objforQuery;
                if (obj == null || obj.ChoosesDf == null)
                    return;

                StringBuilder RestDataQuery = new StringBuilder("data/");
                RestDataQuery.AppendFormat("{1},{0},{2}/", obj.ChoosesDf.Code, obj.ChoosesDf.DataFlowAgency, obj.ChoosesDf.DataFlowVersion);

                if (obj.DataQuery.Count == 0)
                    RestDataQuery.Append("ALL");
                else
                    RestDataQuery.Append(GetDimensionValues(obj.DataQuery, obj.DimensionOrdered));

                RestDataQuery.Append("/?detail=full");
                List<TimeWhere> TimePeriod = GetEffectiveTimePeriod(obj.DataQuery);
                if (TimePeriod != null && TimePeriod.Count > 0 && !string.IsNullOrEmpty(TimePeriod[0].StartTime))
                    RestDataQuery.AppendFormat("&startPeriod={0}", TimePeriod[0].StartTime);
                if (TimePeriod != null && TimePeriod.Count > 0 && !string.IsNullOrEmpty(TimePeriod[0].EndTime))
                    RestDataQuery.AppendFormat("&endPeriod={0}", TimePeriod[0].EndTime);
                if (ChkFlat.Visible)
                    RestDataQuery.AppendFormat("&dimensionAtObservation={0}", ChkFlat.Checked ? "AllDimensions" : "TIME_PERIOD");

                QueryTmp = RestDataQuery.ToString();
            }
            catch (Exception)
            {
                QueryTmp = "";
            }
            finally
            {
                try
                {
                    this.Invoke(new QueryCreator.voidDelegate(UpdateQueryText));
                }
                catch (Exception)
                {
                    //Sto in chiusura...
                }

            }
        }

        private string GetDimensionValues(Dictionary<string, List<SdmxQueryInput>> DataQuery, List<OrderDim> DimensionOrdered)
        {
            string[] dimvalues = new string[DimensionOrdered.Count];
            int index = 0;
            foreach (OrderDim orderdim in DimensionOrdered.OrderBy(o => o.Order))
            {
                dimvalues[index] = "";

                if (DataQuery.ContainsKey(orderdim.DimCode))
                    dimvalues[index] = string.Join("+", DataQuery[orderdim.DimCode].FindAll(x => x.Value).Select(x => x.Code).ToArray());

                index++;
            }
            string Key = string.Join(".", dimvalues);
            if (Key.Length == DimensionOrdered.Count - 1)
            {//è vuoto
                return "ALL";
            }
            return Key;
        }


        //        private string GetOrVal(int StartTab, Dictionary<string, List<SdmxQueryInput>> _queryval)
        //        {
        //            StringBuilder orval = new StringBuilder();

        //            foreach (string concept in _queryval.Keys.ToList().FindAll(con => _queryval[con].Count(vc => vc.Value && !vc.IsAttribute) > 1))
        //            {
        //                List<SdmxQueryInput> orstat = _queryval[concept].FindAll(dim => dim.Value && !dim.IsAttribute);
        //                orstat.ForEach(unico =>
        //                    {
        //                        orval.AppendLine(string.Format(@"{0}<DimensionValue>
        //{0}{0}<ID>{1}</ID>
        //{0}{0}<Value operator=""equal"">{2}</Value>
        //{0}</DimensionValue>", Tab(StartTab), concept, unico.Code));
        //                    });
        //            }
        //            return orval.ToString();
        //        }

        //        private string GetAndVal(int StartTab, Dictionary<string, List<SdmxQueryInput>> _queryval)
        //        {
        //            StringBuilder andval = new StringBuilder();

        //            foreach (string concept in _queryval.Keys.ToList().FindAll(con => _queryval[con].Count(vc => vc.Value && !vc.IsAttribute) == 1))
        //            {
        //                SdmxQueryInput unico = _queryval[concept].Find(dim => dim.Value && !dim.IsAttribute);
        //                if (unico == null) continue;

        //                andval.Append(string.Format(@"
        //{0}<DimensionValue>
        //{0}{0}<ID>{1}</ID>
        //{0}{0}<Value operator=""equal"">{2}</Value>
        //{0}</DimensionValue>", Tab(StartTab), concept, unico.Code));
        //            }
        //            return andval.ToString();
        //        }

        //        private string GetOrValAttribute(int StartTab, Dictionary<string, List<SdmxQueryInput>> _queryval)
        //        {
        //            StringBuilder orval = new StringBuilder();

        //            foreach (string concept in _queryval.Keys.ToList().FindAll(con => _queryval[con].Count(vc => vc.Value && vc.IsAttribute) >= 1))
        //            {
        //                List<SdmxQueryInput> orstat = _queryval[concept].FindAll(dim => dim.Value && dim.IsAttribute);
        //                orstat.ForEach(unico =>
        //                {
        //                    orval.AppendLine(string.Format(@"{0}<AttributeValue>
        //{0}{0}<ID>{1}</ID>
        //{0}{0}<Value operator=""equal"">{2}</Value>
        //{0}</AttributeValue>", Tab(StartTab), concept, unico.Code));
        //                });
        //            }
        //            return orval.ToString();
        //        }

        #region TimePeriod
        private List<TimeWhere> GetEffectiveTimePeriod(Dictionary<string, List<SdmxQueryInput>> dictionary)
        {
            List<TimeWhere> periods = (from timep in dictionary
                                       where timep.Value != null && timep.Value != null && timep.Value.Count == 1 && timep.Value[0].TimeWhereValue != null
                                       select timep.Value[0].TimeWhereValue).FirstOrDefault();
            if (periods != null)
            {
                for (int i = periods.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(periods[i].StartTime) && string.IsNullOrEmpty(periods[i].EndTime))
                        periods.RemoveAt(i);
                }

            }
            return periods;
        }



        #endregion


        private void UpdateQueryText()
        {
            txtQuery.Text = QueryTmp;
            Application.DoEvents();
        }

        private void ChkFlat_CheckedChanged(object sender, EventArgs e)
        {
            if (UpdateFlatEvent != null)
                UpdateFlatEvent();
        }

        private void cmbTypeOP_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtHeader.Text = string.Format("application/vnd.sdmx.{0}+xml;", cmbTypeOP.SelectedItem.ToString().ToLower());
            switch (this.SdmxVersion)
            {
                case TestFlyQueryCreation.SdmxVersionEnum.Sdmx20:
                    txtHeader.Text += "version=2.0";
                    break;
                case TestFlyQueryCreation.SdmxVersionEnum.Sdmx21:
                    txtHeader.Text += "version=2.1";
                    break;
            }
            if (cmbTypeOP.SelectedItem.ToString().Trim().ToUpper() == "RDF")
            {
                txtHeader.Text = string.Format("application/rdf+xml");
            }
            if (cmbTypeOP.SelectedItem.ToString().Trim().ToUpper() == "DSPL")
            {
                txtHeader.Text = string.Format("application/dspl");
            }
            if (cmbTypeOP.SelectedItem.ToString().Trim().ToUpper() == "JSON")
            {
                txtHeader.Text = string.Format("application/json");
            }
        }


    }

}

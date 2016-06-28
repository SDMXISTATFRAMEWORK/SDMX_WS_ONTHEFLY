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
    public partial class QueryCreator21 : UserControl, TestOnTheFlyService.IQueryCreator
    {
        public event QueryCreator.voidDelegate ResetEvent;
        public event QueryCreator.voidDelegate UpdateFlatEvent;
        public event QueryCreator.GetDataEventDelegate GetDataEvent;

        public SendQueryStreaming.StructTypeEnum DataStructType { get; set; }
        public QueryCreator21()
        {
            InitializeComponent();
            UpdateGraphicsStructType(SendQueryStreaming.StructTypeEnum.StructureSpecificData);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (ResetEvent != null)
                ResetEvent();
        }

        #region Gestione Tipo
        public void UpdateGraphicsStructType(SendQueryStreaming.StructTypeEnum _dataStructType)
        {
            DataStructType = _dataStructType;
            btnGenericData.BackColor = Color.White;
            BtnGenericTimeSeries.BackColor = Color.White;
            btnStructureSpecific.BackColor = Color.White;
            btnStructureSpecificTimeSeries.BackColor = Color.White;
            switch (DataStructType)
            {
                case SendQueryStreaming.StructTypeEnum.GenericData:
                    btnGenericData.BackColor = Color.Yellow;
                    BtnGenericTimeSeries.BackColor = Color.Yellow;
                    break;
                case SendQueryStreaming.StructTypeEnum.StructureSpecificData:
                     btnStructureSpecificTimeSeries.BackColor = Color.Yellow;
                    btnStructureSpecific.BackColor = Color.Yellow;
                    break;
            }
        }
        private void BtnGenericTimeSeries_Click(object sender, EventArgs e)
        {
            UpdateGraphicsStructType(SendQueryStreaming.StructTypeEnum.GenericData);
        }

        private void btnGenericData_Click(object sender, EventArgs e)
        {
            UpdateGraphicsStructType(SendQueryStreaming.StructTypeEnum.GenericData);
        }

        private void btnStructureSpecificTimeSeries_Click(object sender, EventArgs e)
        {
            UpdateGraphicsStructType(SendQueryStreaming.StructTypeEnum.StructureSpecificData);
        }

        private void btnStructureSpecific_Click(object sender, EventArgs e)
        {
            UpdateGraphicsStructType(SendQueryStreaming.StructTypeEnum.StructureSpecificData);
        }
        #endregion


        private void btnGetData_Click(object sender, EventArgs e)
        {
            if (GetDataEvent != null)
                GetDataEvent(txtQuery.Testo, DataStructType.ToString());
        }

        public void UpdateQuery(SdmxObject _ChoosesDf, Dictionary<string, List<SdmxQueryInput>> _DataQuery, List<OrderDim> DimensionOrdered)
        {
            txtQuery.SetSize(9);
            txtQuery.Testo = "Loading...";
            Application.DoEvents();
            Thread tt = new Thread(new ParameterizedThreadStart(UpdateQueryThread));
            tt.Start(new objforQuery() { ChoosesDf = _ChoosesDf, DataQuery = _DataQuery });
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


                int StartTab = 2;
                List<TimeWhere> TimePeriod = GetEffectiveTimePeriod(obj.DataQuery);

                //string DatasetQuery = string.Format(@"{0}<DataSetID operator=""equal"">{1}</DataSetID>
                string DatasetQuery = string.Format(@"{0}<Dataflow>
{0}{0}<Ref agencyID=""{2}"" id=""{1}"" version=""{3}"" xmlns="""" />
{0}</Dataflow>", Tab(StartTab + 1), obj.ChoosesDf.Code, obj.ChoosesDf.DataFlowAgency, obj.ChoosesDf.DataFlowVersion);
                string ValueAnd = DatasetQuery + GetAndVal(StartTab + 1, obj.DataQuery);
                if (TimePeriod != null && TimePeriod.Count > 0)
                    ValueAnd = DatasetQuery + GetAndVal(StartTab + 1, obj.DataQuery) + WriteSingleTimePeriod(StartTab + 1, TimePeriod[0]);
                string ValueOr = GetOrVal(StartTab + 3, obj.DataQuery);
                if (!string.IsNullOrEmpty(ValueOr))
                {
                    ValueAnd = string.Format(@"{2}
{0}<Or>
{1}{0}</Or>", Tab(StartTab + 2), ValueOr, ValueAnd);
                }
                string ValueOrAttribute = GetOrValAttribute(StartTab + 3, obj.DataQuery);
                if (!string.IsNullOrEmpty(ValueOrAttribute))
                {
                    ValueAnd = string.Format(@"{2}
{0}<Or>
{1}{0}</Or>", Tab(StartTab + 2), ValueOrAttribute, ValueAnd);
                }
                string ANDVal = string.Format(@"

{1}
", Tab(StartTab), ValueAnd);

                if (TimePeriod != null && TimePeriod.Count > 1)
                    ANDVal = WriteMultipleTimePeriod(StartTab, ANDVal, TimePeriod, DatasetQuery);


                string DimensionAtObservation = "TIME_PERIOD";
                if (ChkFlat.Checked)
                    DimensionAtObservation = "AllDimensions";

                string structureQuery = @"<Query>
<ReturnDetails detail=""Full""  observationAction=""Active"" xmlns=""http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query"">
  <query:Structure  structureID=""STR_{1}_1.0"" dimensionAtObservation=""{2}"" xmlns=""http://www.sdmx.org/resources/sdmxml/schemas/v2_1/common"">
    <Structure>
      <Ref agencyID=""{3}"" id=""{1}"" version=""{4}"" xmlns="""" />
    </Structure>
  </query:Structure>
</ReturnDetails>
    
<DataWhere xmlns=""http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query"">
{0}
</DataWhere>
</Query>";

                QueryTmp = string.Format(structureQuery, ANDVal, obj.ChoosesDf.Code, DimensionAtObservation, obj.ChoosesDf.DataFlowAgency, obj.ChoosesDf.DataFlowVersion);
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

        private string Tab(int StartTab)
        {
            return "".PadLeft(StartTab * 3);
        }

        private string GetOrVal(int StartTab, Dictionary<string, List<SdmxQueryInput>> _queryval)
        {
            StringBuilder orval = new StringBuilder();

            foreach (string concept in _queryval.Keys.ToList().FindAll(con => _queryval[con].Count(vc => vc.Value && !vc.IsAttribute) > 1))
            {
                List<SdmxQueryInput> orstat = _queryval[concept].FindAll(dim => dim.Value && !dim.IsAttribute);
                orstat.ForEach(unico =>
                    {
                        orval.AppendLine(string.Format(@"{0}<DimensionValue>
{0}{0}<ID>{1}</ID>
{0}{0}<Value operator=""equal"">{2}</Value>
{0}</DimensionValue>", Tab(StartTab), concept, unico.Code));
                    });
            }
            return orval.ToString();
        }

        private string GetAndVal(int StartTab, Dictionary<string, List<SdmxQueryInput>> _queryval)
        {
            StringBuilder andval = new StringBuilder();

            foreach (string concept in _queryval.Keys.ToList().FindAll(con => _queryval[con].Count(vc => vc.Value && !vc.IsAttribute) == 1))
            {
                SdmxQueryInput unico = _queryval[concept].Find(dim => dim.Value && !dim.IsAttribute);
                if (unico == null) continue;

                andval.Append(string.Format(@"
{0}<DimensionValue>
{0}{0}<ID>{1}</ID>
{0}{0}<Value operator=""equal"">{2}</Value>
{0}</DimensionValue>", Tab(StartTab), concept, unico.Code));
            }
            return andval.ToString();
        }

        private string GetOrValAttribute(int StartTab, Dictionary<string, List<SdmxQueryInput>> _queryval)
        {
            StringBuilder orval = new StringBuilder();

            foreach (string concept in _queryval.Keys.ToList().FindAll(con => _queryval[con].Count(vc => vc.Value && vc.IsAttribute) >= 1))
            {
                List<SdmxQueryInput> orstat = _queryval[concept].FindAll(dim => dim.Value && dim.IsAttribute);
                orstat.ForEach(unico =>
                {
                    orval.AppendLine(string.Format(@"{0}<AttributeValue>
{0}{0}<ID>{1}</ID>
{0}{0}<Value operator=""equal"">{2}</Value>
{0}</AttributeValue>", Tab(StartTab), concept, unico.Code));
                });
            }
            return orval.ToString();
        }

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

        private string WriteMultipleTimePeriod(int StartTab, string ANDVal, List<TimeWhere> TimePeriod, string DatasetQuery)
        {
            string OtherTimePeriod = "";
            for (int i = 1; i < TimePeriod.Count; i++)
            {
                OtherTimePeriod += string.Format(@"
{0}<And>
{1}
{2}
{0}</And>", Tab(StartTab), DatasetQuery, WriteSingleTimePeriod(StartTab + 1, TimePeriod[i]));

            }
            return string.Format(@"
{0}<Or>
{1}
{2}
{0}</Or>", Tab(StartTab), ANDVal, OtherTimePeriod);

        }
        private string WriteSingleTimePeriod(int StartTab, TimeWhere TimePeriod)
        {
            string StartTime = "";
            string EndTime = "";
            if (!string.IsNullOrEmpty(TimePeriod.StartTime))
                StartTime = string.Format(@"{0}{0}<TimeValue operator=""equal"">{1}</TimeValue>", Tab(StartTab), TimePeriod.StartTime);
            if (!string.IsNullOrEmpty(TimePeriod.EndTime))
                EndTime = string.Format(@"{0}{0}<TimeValue operator=""equal"">{1}</TimeValue>", Tab(StartTab), TimePeriod.EndTime);
            return string.Format(@"
{0}<TimeDimensionValue>
{1}
{2}
{0}</TimeDimensionValue>", Tab(StartTab), StartTime, EndTime);
        }

        #endregion


        private void UpdateQueryText()
        {
            txtQuery.Testo = QueryTmp;
            Application.DoEvents();
        }

        private void ChkFlat_CheckedChanged(object sender, EventArgs e)
        {
            if (UpdateFlatEvent!=null)
                UpdateFlatEvent();
        }

    }

}

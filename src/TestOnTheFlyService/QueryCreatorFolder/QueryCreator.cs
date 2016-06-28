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
    public partial class QueryCreator : UserControl, TestOnTheFlyService.IQueryCreator
    {
        public delegate void voidDelegate();
        public event QueryCreator.voidDelegate ResetEvent;
        public event QueryCreator.voidDelegate UpdateFlatEvent;
        public delegate void GetDataEventDelegate(string Text, string TypeOperation);
        public event QueryCreator.GetDataEventDelegate GetDataEvent;


        public QueryCreator()
        {
            InitializeComponent();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (ResetEvent != null)
                ResetEvent();
            if (UpdateFlatEvent != null)
                UpdateFlatEvent();
        }



        private void BtnGeneric_Click(object sender, EventArgs e)
        {
            if (GetDataEvent != null)
                GetDataEvent(txtQuery.Testo, TestOnTheFlyService.SendQueryStreaming.StructTypeEnum.Generic.ToString());
        }

        private void btnCompact_Click(object sender, EventArgs e)
        {
            if (GetDataEvent != null)
                GetDataEvent(txtQuery.Testo, TestOnTheFlyService.SendQueryStreaming.StructTypeEnum.Compact.ToString());
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

                string DatasetQuery = string.Format(@"
{0}<Dataflow>{1}</Dataflow>", Tab(StartTab + 1), obj.ChoosesDf.Code);
                string ValueAnd = GetAndVal(StartTab + 1, obj.DataQuery) + DatasetQuery;
                if (TimePeriod != null && TimePeriod.Count > 0)
                    ValueAnd = GetAndVal(StartTab + 1, obj.DataQuery) + WriteSingleTimePeriod(StartTab + 1, TimePeriod[0]) + DatasetQuery;
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
{0}<And>
{1}
{0}</And>", Tab(StartTab), ValueAnd);

                if (TimePeriod != null && TimePeriod.Count > 1)
                    ANDVal = WriteMultipleTimePeriod(StartTab, ANDVal, TimePeriod, DatasetQuery);

                string structureQuery = @"<Query>
    <DataWhere xmlns=""http://www.SDMX.org/resources/SDMXML/schemas/v2_0/query"">{0}
    </DataWhere>
</Query>";

                QueryTmp = string.Format(structureQuery, ANDVal);
            }
            catch (Exception)
            {
                QueryTmp = "";
            }
            finally
            {
                try
                {
                    this.Invoke(new voidDelegate(UpdateQueryText));
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
                        orval.AppendLine(string.Format(@"{0}<Dimension id=""{1}"">{2}</Dimension>", Tab(StartTab), concept, unico.Code));
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
{0}<Dimension id=""{1}"">{2}</Dimension>", Tab(StartTab), concept, unico.Code));
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
                    orval.AppendLine(string.Format(@"{0}<Attribute id=""{1}"">{2}</Attribute>", Tab(StartTab), concept, unico.Code));
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
{2}
{1}
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
                StartTime = string.Format(@"{0}{0}<StartTime>{1}</StartTime>", Tab(StartTab), TimePeriod.StartTime);
            if (!string.IsNullOrEmpty(TimePeriod.EndTime))
                EndTime = string.Format(@"{0}{0}<EndTime>{1}</EndTime>", Tab(StartTab), TimePeriod.EndTime);
            return string.Format(@"
{0}<Time>
{1}
{2}
{0}</Time>", Tab(StartTab), StartTime, EndTime);
        }

        #endregion


        private void UpdateQueryText()
        {
            txtQuery.Testo = QueryTmp;
            Application.DoEvents();
        }
    }
   
}

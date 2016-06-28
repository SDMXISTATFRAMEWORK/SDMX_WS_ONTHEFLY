using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TestOnTheFlyService
{
    public class ChangeLanguages
    {
        public static string Lang { get; set; }
        public static string GetCorrectLanguages(string Code, List<SdmxObjectName> Names)
        {
            if (Names != null && Names.Count > 0)
            {
                if (Names.Exists(n => n.Lingua.Trim().ToUpper() == Lang.Trim().ToUpper()))
                    return Names.Find(n => n.Lingua.Trim().ToUpper() == Lang.Trim().ToUpper()).Nome;
            }
            return Names != null && Names.Count > 0 ? Names[0].Nome : Code;
        }
    }
    public class SdmxObject : TreeNode
    {
        public SdmxObject(string _code, List<SdmxObjectName> _names, bool _isDataset)
        {
            this.Code = _code;
            this.Names = _names;
            this.IsDataset = _isDataset;

            base.Name = Code;
            base.Text = ChangeLanguages.GetCorrectLanguages(this.Code, this.Names);
            base.ImageIndex = (this.IsDataset ? 1 : 0);
            base.SelectedImageIndex = (this.IsDataset ? 2 : 0);

        }
        public string Code { get; set; }
        public List<SdmxObjectName> Names { get; set; }

        public bool IsDataset { get; set; }
        public bool isTimeConcept { get; set; }

        public override string ToString()
        {
            return base.Text;
        }

        internal void SetNames(List<SdmxObjectName> _names)
        {
            this.Names = _names;
            base.Text = ChangeLanguages.GetCorrectLanguages(this.Code, this.Names);
        }

       

        #region Dataflow
        public string DataFlowAgency { get; set; }
        public string DataFlowVersion { get; set; }

        public void SetDataflowInfo(XElement df)
        {
            //<Dataflow agencyID="IT1" id="250_3" urn="urn:sdmx:org.sdmx.infomodel.datastructure.Dataflow=IT1:250_3(1.0)" version="1.0" isFinal="true" 
            if (df.Attributes().Count(a => a.Name.LocalName.Trim().ToLower() == "agencyID".Trim().ToLower()) > 0)
                DataFlowAgency = df.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "agencyID".Trim().ToLower()).Value;
            if (df.Attributes().Count(a => a.Name.LocalName.Trim().ToLower() == "version".Trim().ToLower()) > 0)
                DataFlowVersion = df.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "version".Trim().ToLower()).Value;
        }


        #endregion

        #region KeyFamily
        public string KeyFamilyId { get; set; }
        public string KeyFamilyAgency { get; set; }
        public string KeyFamilyVersion { get; set; }
        internal void SetKeyFamilyRef(System.Xml.Linq.XElement dataflowrefNodo)
        {
            XElement el = (from kr in dataflowrefNodo.Elements()
                           where kr.Name.LocalName.Trim().ToLower() == "KeyFamilyRef".Trim().ToLower()
                           select kr).First();
            foreach (var kr in el.Elements())
            {
                if (kr.Name.LocalName.Trim().ToLower() == "KeyFamilyID".Trim().ToLower())
                    KeyFamilyId = kr.Value;
                else if (kr.Name.LocalName.Trim().ToLower() == "KeyFamilyAgencyID".Trim().ToLower())
                    KeyFamilyAgency = kr.Value;
                else if (kr.Name.LocalName.Trim().ToLower() == "Version".Trim().ToLower())
                    KeyFamilyVersion = kr.Value;
            }
        }

        internal void SetStructureRef(XElement dataflowrefNodo)
        {
            //<Structure>
            //   <Ref id="Pre-primary_school_158" version="1.0" agencyID="IT1" package="datastructure" class="DataStructure" xmlns="" />
            // </Structure>
            XElement el = (from kr in dataflowrefNodo.Elements()
                           where kr.Name.LocalName.Trim().ToLower() == "Structure".Trim().ToLower()
                           select kr).First();
            var Sr = el.Elements().First(k => k.Name.LocalName.Trim().ToLower() == "Ref".Trim().ToLower());
            if (Sr != null)
            {
                KeyFamilyId = Sr.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "id").Value;
                KeyFamilyAgency = Sr.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "agencyID".Trim().ToLower()).Value;
                KeyFamilyVersion = Sr.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "version").Value;
            }
        }
        #endregion

        #region Codelist
        public string CodelistId { get; set; }
        public string CodelistAgency { get; set; }
        public string CodelistVersion { get; set; }

        internal void SetCodelist20(XElement Concept)
        {
            try
            {
                if (Concept.Attributes().Count(a => a.Name.LocalName.Trim().ToLower() == "codelist".Trim().ToLower()) > 0)
                    CodelistId = Concept.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "codelist".Trim().ToLower()).Value;
                else
                {
                    CodelistId = null;
                    return;
                }
            }
            catch (Exception)
            {
                CodelistId = null;
                return;
            }
            try
            {
                CodelistAgency = Concept.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "codelistAgency".Trim().ToLower()).Value;
            }
            catch (Exception)
            {
                CodelistAgency = "ALL";
            }
            try
            {
                CodelistVersion = Concept.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "codelistVersion".Trim().ToLower()).Value;
            }
            catch (Exception)
            {
                CodelistVersion = "*";
            }
        }

        internal void SetCodelist21(XElement Concept)
        {
             XElement localrepresentation = Concept.Elements().FirstOrDefault(el => el.Name.LocalName.Trim().ToLower() == "LocalRepresentation".ToLower());
            if (localrepresentation == null) return;
            XElement Enumeration = localrepresentation.Elements().FirstOrDefault(el => el.Name.LocalName.Trim().ToLower() == "Enumeration".ToLower());
            if (Enumeration == null) return;
            XElement ConceptRef = Enumeration.Elements().FirstOrDefault(el => el.Name.LocalName.Trim().ToLower() == "Ref".ToLower());
            if (ConceptRef == null) return;
            try
            {
                if (ConceptRef.Attributes().Count(a => a.Name.LocalName.Trim().ToLower() == "id".Trim().ToLower()) > 0)
                    CodelistId = ConceptRef.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "id".Trim().ToLower()).Value;
                else
                {
                    CodelistId = null;
                    return;
                }
            }
            catch (Exception)
            {
                CodelistId = null;
                return;
            }
            try
            {
                CodelistAgency = ConceptRef.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "agencyID".Trim().ToLower()).Value;
            }
            catch (Exception)
            {
                CodelistAgency = "ALL";
            }
            try
            {
                CodelistVersion = ConceptRef.Attributes().FirstOrDefault(a => a.Name.LocalName.Trim().ToLower() == "version".Trim().ToLower()).Value;
            }
            catch (Exception)
            {
                CodelistVersion = "*";
            }
        }
        #endregion
    }
    public class SdmxObjectName
    {
        public string Lingua { get; set; }
        public string Nome { get; set; }
    }
    public class SdmxQueryInput : TreeNode
    {
        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                base.Text = ChangeLanguages.GetCorrectLanguages(this.Code, this.Names); ;
            }
        }
        private List<SdmxObjectName> _names = null;
        public List<SdmxObjectName> Names
        {
            get { return _names; }
            set
            {
                _names = value;
                base.Text = ChangeLanguages.GetCorrectLanguages(this.Code, this.Names); ;
            }
        }


        public bool Value
        {
            get { return base.Checked; }
            set { base.Checked = value; }
        }

        public List<TimeWhere> TimeWhereValue { get; set; }
        public bool IsAttribute { get; set; }
        public override string ToString()
        {
            return Text;
        }

        public string ParentCode { get; set; }

       
    }
    public class TimeWhere
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}

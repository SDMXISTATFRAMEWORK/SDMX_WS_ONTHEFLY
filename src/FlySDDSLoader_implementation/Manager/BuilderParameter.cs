using FlyController;
using FlyMapping.Build;
using FlyMapping.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FlySDDSLoader_implementation.Manager
{
    /// <summary>
    /// Parameter for StoreProcedure in MS Database 
    /// </summary>
    public class BuilderParameter
    {
       
        internal bool IsStub { get; set; }
        internal string TimeStamp { get; set; }

        internal string DSDCode { get; set; }
        internal string DSDAgencyId { get; set; }
        internal string DSDVersion { get; set; }

        internal string ConceptSchemeCode { get; set; }
        internal string ConceptSchemeAgencyId { get; set; }
        internal string ConceptSchemeVersion { get; set; }

        internal string CodelistCode { get; set; }
        internal string CodelistAgencyId { get; set; }
        internal string CodelistVersion { get; set; }

        internal int? DFId { get; set; }


      

        internal List<IParameterValue> GetParameter()
        {
            List<IParameterValue> parametri = new List<IParameterValue>()
                {
                    new ParameterValue() {Item="UserName",Value=FlyConfiguration.UserName},
                    new ParameterValue() {Item="Domain",Value=FlyConfiguration.Domain},
                };

            if (!string.IsNullOrEmpty(this.TimeStamp))
                parametri.Add(new ParameterValue() { Item = "TimeStamp", Value = this.TimeStamp, SqlType = SqlDbType.DateTime });
          

            if (!string.IsNullOrEmpty(this.DSDCode))
                parametri.Add(new ParameterValue() { Item = "DSDCode", Value = this.DSDCode });
            if (!string.IsNullOrEmpty(this.DSDAgencyId))
                parametri.Add(new ParameterValue() { Item = "DSDAgencyId", Value = this.DSDAgencyId });
            if (!string.IsNullOrEmpty(this.DSDVersion))
                parametri.Add(new ParameterValue() { Item = "DSDVersion", Value = this.DSDVersion });

            if (!string.IsNullOrEmpty(this.ConceptSchemeCode))
                parametri.Add(new ParameterValue() { Item = "ConceptSchemeCode", Value = this.ConceptSchemeCode });
            if (!string.IsNullOrEmpty(this.ConceptSchemeAgencyId))
                parametri.Add(new ParameterValue() { Item = "ConceptSchemeAgencyId", Value = this.ConceptSchemeAgencyId });
            if (!string.IsNullOrEmpty(this.ConceptSchemeVersion))
                parametri.Add(new ParameterValue() { Item = "ConceptSchemeVersion", Value = this.ConceptSchemeVersion });

            if (!string.IsNullOrEmpty(this.CodelistCode))
                parametri.Add(new ParameterValue() { Item = "CodelistCode", Value = this.CodelistCode });
            if (!string.IsNullOrEmpty(this.CodelistAgencyId))
                parametri.Add(new ParameterValue() { Item = "CodelistAgencyId", Value = this.CodelistAgencyId });
            if (!string.IsNullOrEmpty(this.CodelistVersion))
                parametri.Add(new ParameterValue() { Item = "CodelistVersion", Value = this.CodelistVersion });

            if (DFId.HasValue)
                parametri.Add(new ParameterValue() { Item = "DFId", Value = this.DFId.Value, SqlType=SqlDbType.Int });

            if (this.IsStub)
                parametri.Add(new ParameterValue() { Item = "IsStub", Value = 1, SqlType = SqlDbType.Bit });

            return parametri;

        }

    }
}

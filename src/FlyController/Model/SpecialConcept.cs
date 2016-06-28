
using Org.Sdmx.Resources.SdmxMl.Schemas.V20.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Special Concept for request a Codelist Constrained of type Special 
    /// </summary>
    public class SpecialConcept : ConceptObjectImpl, FlyController.Model.ISpecialConcept
    {
        /// <summary>
        /// Special Type Codelist <see cref="SpecialTypeEnum"/>
        /// </summary>
        public SpecialTypeEnum SpecialType { get; set; }

      


        /// <summary>
        /// create a instace of DimensionConcept
        /// </summary>
        /// <param name="code">Dimension Code</param>
        /// <param name="_specialType">Special Type Codelist <see cref="SpecialTypeEnum"/></param>
        public SpecialConcept(string code, SpecialTypeEnum _specialType)
            : base(code, GetNames(_specialType), ConceptTypeEnum.Special)
        {
            this.SpecialType = _specialType;

        }

        private static List<SdmxObjectNameDescription> GetNames(SpecialTypeEnum _specialType)
        {
            switch (_specialType)
            {
                case SpecialTypeEnum.CL_TIME_PERIOD:
                    return new List<SdmxObjectNameDescription>()
                    {
                        new SdmxObjectNameDescription()
                        {
                        Lingua="en",
                        Name="Time Dimension Start and End periods",
                        }
                    };
                case SpecialTypeEnum.CL_COUNT:
                    return new List<SdmxObjectNameDescription>()
                    {
                        new SdmxObjectNameDescription()
                        {
                        Lingua="en",
                        Name="Special dataflow count codelist",
                        }
                    };
            }
            return new List<SdmxObjectNameDescription>()
            {
                new SdmxObjectNameDescription()
                {
                Lingua="en",
                Name=_specialType.ToString(),
                }
            };
        }



        /// <summary>
        /// Add Names at Codelist Constrained
        /// </summary>
        /// <param name="names">the names <see cref="SdmxObjectNameDescription"/></param>
        public void SetNames(List<SdmxObjectNameDescription> names)
        {
            this.ConceptObjectNames = names;
            #region Popolo i dati di base
            foreach (SdmxObjectNameDescription item in names)
                base.AddName(item.Lingua, item.Name);
            #endregion
        }

        /// <summary>
        /// List of Member for Codelist Constrained (Other concept value in constrain, call a Special Concept CL_CONSTRAIN)
        /// </summary>
        public Dictionary<string, IList<MemberValueType>> ContrainConceptREF { get; set; }

        /// <summary>
        /// Save a TimeDimension Ref for return a Constrained Codelist whitout recall a Concepts list
        /// </summary>
        public IConceptObjectImpl TimeDimensionRef { get; set; }
    }
}

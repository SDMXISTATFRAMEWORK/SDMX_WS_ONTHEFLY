
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// DimensionConcept is Implementation of ConceptObjectImpl that representing a Dimension
    /// </summary>
    public class DimensionConcept : ConceptObjectImpl, IDimensionConcept
    {
        /// <summary>
        /// Dimension Type 
        /// </summary>
        public DimensionTypeEnum DimensionType { get; set; }

        /// <summary>
        /// Flag that representing if this is a Frequency not real
        /// </summary>
        public bool IsFakeFrequency { get; set; }
        /// <summary>
        /// Database Frequency Name (in case different of FREQ)
        /// </summary>
        public string RealNameFreq { get; set; }
        /// <summary>
        /// Database Time Name (in case different of TIME_PERIOD)
        /// </summary>
        public string RealNameTime { get; set; }

        /// <summary>
        /// create a instace of DimensionConcept
        /// </summary>
        /// <param name="code">Dimension Code</param>
        /// <param name="names">Dimension Descriptions Names</param>
        public DimensionConcept(string code, List<SdmxObjectNameDescription> names)
            : base(code, names, ConceptTypeEnum.Dimension)
        {
            this.DimensionType = DimensionTypeEnum.Dimension;
            IsFakeFrequency = false;
            if (code.Trim().ToUpper() == "FREQUENCY" || code.Trim().ToUpper() == "FREQ")
            {
                this.DimensionType = DimensionTypeEnum.Frequency;
                this.RealNameFreq = code.Trim().ToUpper();
                base.Id = "FREQ";
                this.ConceptObjectCode = base.Id;
            }
        }

        /// <summary>
        /// Get a ColumName of Time dimension 
        /// </summary>
        /// <returns></returns>
        public string GetColumTimeName()
        {
            if (string.IsNullOrEmpty(RealNameTime))
                return base.ConceptObjectCode;
            else
                return RealNameTime;
        }

    }
}

using System;
namespace FlyController.Model
{
    /// <summary>
    /// DimensionConcept is Interface of ConceptObjectImpl that representing a Dimension
    /// </summary>
    public interface IDimensionConcept : IConceptObjectImpl
    {
        /// <summary>
        /// Dimension Type 
        /// </summary>
        DimensionTypeEnum DimensionType { get; set; }
        /// <summary>
        /// Get a ColumName of Time dimension 
        /// </summary>
        /// <returns></returns>
        string GetColumTimeName();
        /// <summary>
        /// Flag that representing if this is a Frequency not real
        /// </summary>
        bool IsFakeFrequency { get; set; }
        /// <summary>
        /// Database Frequency Name (in case different of FREQ)
        /// </summary>
        string RealNameFreq { get; set; }
        /// <summary>
        /// Database Time Name (in case different of TIME_PERIOD)
        /// </summary>
        string RealNameTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Dimension Type Enum (Dimension, Time, Frequency)
    /// </summary>
    public enum DimensionTypeEnum
    {
        /// <summary>
        /// Normal Dimension
        /// </summary>
        Dimension,
        /// <summary>
        /// TimeDimension
        /// </summary>
        Time,
        /// <summary>
        /// FrequencyDimension
        /// </summary>
        Frequency
    }
}

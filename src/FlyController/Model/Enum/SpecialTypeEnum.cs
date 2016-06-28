using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Special Type list
    /// </summary>
    public enum SpecialTypeEnum
    {
        /// <summary>
        /// Request CL_TIME_PERIOD return a first and last Date Observed
        /// </summary>
        CL_TIME_PERIOD,
        /// <summary>
        /// Request CL_COUNT return a count of rows in Observation request
        /// </summary>
        CL_COUNT,
        /// <summary>
        /// Request CL_CONTRAINED return a codelist request constrained according to another concept
        /// </summary>
        CL_CONTRAINED
    }
}

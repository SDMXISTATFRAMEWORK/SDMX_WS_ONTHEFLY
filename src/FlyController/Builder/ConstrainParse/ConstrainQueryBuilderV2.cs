using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using Org.Sdmxsource.Sdmx.Structureparser.Builder.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Builder.ConstrainParse
{
    /// <summary>
    /// Implementation of <see cref="QueryBuilderV2"/> intercept in query non-standard a contrain object 
    /// </summary>
    public class ConstrainQueryBuilderV2 : QueryBuilderV2
    {
        /// <summary>
        /// Create an instance of <see cref="ConstrainQueryBuilderV2"/>
        /// </summary>
        public ConstrainQueryBuilderV2():base()
        { }

        /// <summary>
        /// override of BuildDataflowQuery, populate a ConstraintObject if is present
        /// </summary>
        /// <param name="dataflowRefType">the <see cref="Org.Sdmx.Resources.SdmxMl.Schemas.V20.registry.DataflowRefType"/></param>
        /// <returns></returns>
        protected override IStructureReference BuildDataflowQuery(Org.Sdmx.Resources.SdmxMl.Schemas.V20.registry.DataflowRefType dataflowRefType)
        {
            return new ConstrainableStructureReference(base.BuildDataflowQuery(dataflowRefType)) 
            {
                ConstraintObject = dataflowRefType.Constraint
            };
        }

    }
}

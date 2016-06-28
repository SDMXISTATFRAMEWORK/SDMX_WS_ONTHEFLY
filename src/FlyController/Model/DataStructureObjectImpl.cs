
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model.Mutable.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Class implementation DataStructureMutableCore.
    /// Extended with the addition of the list of IConceptObjectImpl
    /// </summary>
    public class DataStructureObjectImpl : DataStructureMutableCore
    {

       
        private IDataStructureObject immutated = null;
        /// <summary>
        /// Get a ImmutableIstance of object saved in Build Method
        /// </summary>
        public IDataStructureObject Immutated
        {
            get
            {
                return immutated;
            }
            set
            {
                immutated = value;
            }
        }

        /// <summary>
        /// List of <see cref="IConceptObjectImpl"/>
        /// </summary>
        public List<IConceptObjectImpl> _Concepts { get; set; }
    }
}

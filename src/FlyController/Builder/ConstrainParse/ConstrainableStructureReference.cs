using Org.Sdmxsource.Sdmx.Api.Model.Objects.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Builder.ConstrainParse
{
    /// <summary>
    ///     A IStructureReference is used to reference an identifiable artifact using a combination of reference parameters.
    ///     whit a Contrains
    ///     <p />
    ///     If all reference parameters are present then the reference is for a single identifiable item, if any are missing, then this represents
    ///     a wildcard parameter / or ALL.
    ///     <p />
    ///     SWSDMX_STRUCTURE_TYPE is a mandatory reference parameter, all others are optional
    /// </summary>
    public class ConstrainableStructureReference : IStructureReference
    {
        private IStructureReference StructureReference = null;
        /// <summary>
        /// inizialize a ConstrainableStructureReference
        /// </summary>
        /// <param name="_structureReference"></param>
        public ConstrainableStructureReference(IStructureReference _structureReference)
        {
            StructureReference = _structureReference;
        }

        /// <summary>
        ///     Gets the child artifact that is being referenced, returns null if there is no child artifact
        /// </summary>
        /// <value> </value>
        public IIdentifiableRefObject ChildReference
        {
            get { return StructureReference.ChildReference; }
        }

        /// <summary>
        ///     Creates a copy of this @object
        /// </summary>
        /// <returns>
        ///     The <see cref="IStructureReference" /> .
        /// </returns>
        public IStructureReference CreateCopy()
        {
           return StructureReference.CreateCopy();
        }

        /// <summary>
        /// Gets the full id of the identifiable reference.  This is a dot '.' separated id which consists of the parent identifiable ids and the target.
        /// If the referenced structure is a Sub-Agency then this will include the parent Agency ids, and the id of the target agency.  If this reference
        /// is referencing a maintainable then null will be returned.  If there is only one child identifiable, then the id of that identifable will be returned, with no '.' seperators.
        /// </summary>
        /// <value> </value>
        public string FullId
        {
            get { return StructureReference.FullId; }
        }

        /// <summary>
        /// Gets the matched Identifiable Object from this reference, returns the original Maintainable if this is a Maintainable reference that matches the maintainable.  Gets null if no match
        ///     is found
        /// </summary>
        /// <param name="maintainableObject"> The maintainable object. </param>
        /// <returns>
        /// The <see cref="Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IIdentifiableObject"/> .
        /// </returns>
        public Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IIdentifiableObject GetMatch(Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IMaintainableObject maintainableObject)
        {
            return StructureReference.GetMatch(maintainableObject);
        }

        /// <summary>
        ///     Gets a value indicating whether the getChildReference returns a value
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" /> .
        /// </returns>
        public bool HasChildReference()
        {
            return StructureReference.HasChildReference();
        }

        /// <summary>
        ///     Gets a value indicating whether the getMaintainableUrn returns a value
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" /> .
        /// </returns>
        public bool HasMaintainableUrn()
        {
            return StructureReference.HasMaintainableUrn();
        }

        /// <summary>
        ///     Gets a value indicating whether the getTargetUrn returns a value
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" /> .
        /// </returns>
        public bool HasTargetUrn()
        {
            return StructureReference.HasTargetUrn();
        }

        /// <summary>
        ///     Gets a string array of any child ids (obtained from getChildReference()), returns null if getChildReference() is null
        /// </summary>
        /// <value> </value>
        public IList<string> IdentifiableIds
        {
            get { return StructureReference.IdentifiableIds; }
        }

        /// <summary>
        /// Gets a value indicating whether the reference matches the IMaintainableObject, or one of its indentifiable composites.
        ///     <p/>
        ///     This @object does not require all reference parameters to be set, this method will return true if all the parameters that are set match
        ///     the @object passed in.  If this reference is referencing an Identifiable composite, then the maintainable's identifiable composites will also be
        ///     checked to determine if this is a match.
        /// </summary>
        /// <param name="reference"> The reference object. </param>
        /// <returns>
        /// The <see cref="bool"/> .
        /// </returns>
        public bool IsMatch(Org.Sdmxsource.Sdmx.Api.Model.Objects.Base.IMaintainableObject reference)
        {
            return StructureReference.IsMatch(reference);
        }

        /// <summary>
        ///     Gets the reference parameters as a maintainable reference
        /// </summary>
        /// <value> </value>
        public IMaintainableRefObject MaintainableReference
        {
            get { return StructureReference.MaintainableReference; }
        }

        /// <summary>
        ///     Gets the SDMX Structure that is being referenced at the top level (maintainable level) by this reference @object.
        ///     <p />
        ///     Any child references will further refine the structure type that is being referenced.
        /// </summary>
        /// <value> </value>
        public Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureType MaintainableStructureEnumType
        {
            get { return StructureReference.MaintainableStructureEnumType; }
        }

        /// <summary>
        ///     Gets the URN of the maintainable structure that is being referenced, returns null if there is no URN available
        /// </summary>
        public Uri MaintainableUrn
        {
            get { return StructureReference.MaintainableUrn; }
        }

        /// <summary>
        ///     Gets the SDMX Structure that is being referenced by this reference @object.
        /// </summary>
        /// <value> </value>
        public Org.Sdmxsource.Sdmx.Api.Constants.SdmxStructureType TargetReference
        {
            get { return StructureReference.TargetReference; }
        }

        /// <summary>
        ///     Gets the URN of the target structure that is being referenced, returns null if there is no URN available
        /// </summary>
        public Uri TargetUrn
        {
            get { return StructureReference.TargetUrn; }
        }

        /// <summary>
        ///     Gets the maintainable id attribute
        /// </summary>
        /// <value> </value>
        public string AgencyId
        {
            get { return StructureReference.AgencyId; }
        }

        /// <summary>
        ///     Gets a value indicating whether the there is an agency Id set
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" /> .
        /// </returns>
        public bool HasAgencyId()
        {
            return StructureReference.HasAgencyId();
        }

        /// <summary>
        ///     Gets a value indicating whether the there is a maintainable id set
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" /> .
        /// </returns>
        public bool HasMaintainableId()
        {
            return StructureReference.HasMaintainableId();
        }

        /// <summary>
        ///     Gets a value indicating whether the there is a value for version set
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" /> .
        /// </returns>
        public bool HasVersion()
        {
            return StructureReference.HasVersion();
        }

        /// <summary>
        ///     Gets the maintainable id attribute
        /// </summary>
        /// <value> </value>
        public string MaintainableId
        {
            get { return this.StructureReference.MaintainableId; }
        }

        /// <summary>
        ///     Gets the version attribute
        /// </summary>
        /// <value> </value>
        public string Version
        {
            get { return StructureReference.Version; }
        }

        /// <summary>
        ///     Gets Constrain Associated
        /// </summary>
        /// <value> </value>
        public Org.Sdmx.Resources.SdmxMl.Schemas.V20.common.ConstraintType ConstraintObject { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}

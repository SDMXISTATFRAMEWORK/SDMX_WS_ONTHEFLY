using System;
namespace FlyEngine.Model
{
    /// <summary>
    /// Determines which and how many are the references which we must return
    /// </summary>
    public interface IRetrievalReferences
    {
        /// <summary>
        /// <see cref="ISDMXObjectBuilder"/> where to add a reference Metadata
        /// </summary>
        ISDMXObjectBuilder _SDMXObjectBuilder { get; set; }
        /// <summary>
        /// adds a reference from a structure
        /// </summary>
        /// <param name="_structType">starting structure</param>
        void AddReferences(FlyController.Model.MetadataReferences.ReferenceTreeEnum _structType);

        /// <summary>
        /// Add Categorisation References
        /// </summary>
        void AddCategorisation();
        /// <summary>
        /// Add CategoryScheme References
        /// </summary>
        void AddCategoryScheme();
        /// <summary>
        /// Add Codelist References
        /// </summary>
        void AddCodelist();
        /// <summary>
        /// Add ConceptScheme References
        /// </summary>
        void AddConceptScheme();
        /// <summary>
        /// Add Dataflows References
        /// </summary>
        void AddDataflow();
        /// <summary>
        /// Add DataStructure References
        /// </summary>
        void AddDataStructure();
       
    }
}

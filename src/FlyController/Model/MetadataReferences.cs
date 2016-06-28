using Org.Sdmxsource.Sdmx.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyController.Model
{
    /// <summary>
    /// Class that retreival a References of metadata
    /// </summary>
    public class MetadataReferences
    {
        /// <summary>
        /// list of Metadata type referenced
        /// </summary>
        public enum ReferenceTreeEnum
        {
            /// <summary>
            /// Categorisation Reference
            /// </summary>
            Categorisation=0,

            /// <summary>
            /// CategoryScheme Reference
            /// </summary>
            CategoryScheme=10,
            /// <summary>
            /// Dataflow Reference
            /// </summary>
            Dataflow=11,

            /// <summary>
            /// DataStructure or KeyFamily Reference
            /// </summary>
            Dsd=20,

            /// <summary>
            /// ConceptScheme  Reference
            /// </summary>
            Concept=30,
            /// <summary>
            /// Codelist Reference
            /// </summary>
            Codelist = 31,
        }
        /// <summary>
        /// list of Metadata type referenced
        /// a tree of references
        ///   *         Categorisation
        ///   * CategoryScheme       Dataflow
        ///   *                        Dsd
        ///   *                Codelist    Concept
        /// </summary>
        /// <param name="startMetaType">Metadata type to found reference</param>
        /// <param name="refType">StructureReferenceDetailEnumType</param>
        /// <param name="specificRef">List of Structure required for references</param>
        /// <returns>list of Metadata type referenced</returns>
        public static List<ReferenceTreeEnum> GetReferences(ReferenceTreeEnum startMetaType, StructureReferenceDetailEnumType refType, List<SdmxStructureType> specificRef)
        {
            List<ReferenceTreeEnum> references = new List<ReferenceTreeEnum>();

            switch (refType)
            {
                case StructureReferenceDetailEnumType.All:
                    GetDescendants(startMetaType, ref references);
                    references.AddRange(GetParentsAndSibling(startMetaType));
                    break;
                case StructureReferenceDetailEnumType.Children:
                    references= GetChildren(startMetaType);
                    break;
                case StructureReferenceDetailEnumType.Descendants:
                    GetDescendants(startMetaType, ref references);
                    break;
                case StructureReferenceDetailEnumType.Parents:
                    references= GetParent(startMetaType);
                    break;
                case StructureReferenceDetailEnumType.ParentsSiblings:
                    references= GetParentsAndSibling(startMetaType);
                    break;
                case StructureReferenceDetailEnumType.Specific:
                    references = GetSpecificRef(startMetaType, specificRef);
                    break;
                case StructureReferenceDetailEnumType.None:
                case StructureReferenceDetailEnumType.Null:
                    break;
            }

            references = DeleteDuplexAndOrder(references);

            return references;
        }

      
        /// <summary>
        /// Get a Structure reference in ResolveReference Query request
        /// </summary>
        /// <param name="startMetaType">requested Metadata type</param>
        /// <returns></returns>
        public static List<ReferenceTreeEnum> ResolveReferences(ReferenceTreeEnum startMetaType)
        {
            //Children+Siblings
            List<ReferenceTreeEnum> padre = GetParent(startMetaType);
            List<ReferenceTreeEnum> references = new List<ReferenceTreeEnum>();
            padre.ForEach(p => references.AddRange(GetChildren(p)));
            references.AddRange(GetChildren(startMetaType));

          

            references = DeleteDuplexAndOrder(references);
            return references;
        }

        private static List<ReferenceTreeEnum> DeleteDuplexAndOrder(List<ReferenceTreeEnum> references)
        {
            List<ReferenceTreeEnum> duplex = new List<ReferenceTreeEnum>();
            for (int i = references.Count-1; i >=0; i--)
            {
                if (duplex.Contains(references[i]))
                    references.RemoveAt(i);
                else
                    duplex.Add(references[i]);
            }

            return references.OrderBy(r => (int)r).ToList();
        }
        private static List<ReferenceTreeEnum> GetParentsAndSibling(ReferenceTreeEnum MetaType)
        {
            List<ReferenceTreeEnum> pa = GetParent(MetaType);
            List<ReferenceTreeEnum> sibling = new List<ReferenceTreeEnum>();
            pa.ForEach(p => sibling.AddRange(GetChildren(p)));
            pa.AddRange(sibling);
            return pa;
        }
        private static void GetDescendants(ReferenceTreeEnum MetaType, ref List<ReferenceTreeEnum> lst)
        {
            List<ReferenceTreeEnum> desc = GetChildren(MetaType);
            lst.AddRange(desc);
            foreach (var d in desc)
                GetDescendants(d, ref lst);
        }
        private static List<ReferenceTreeEnum> GetParent(ReferenceTreeEnum MetaType)
        {
            switch (MetaType)
            {
                case ReferenceTreeEnum.CategoryScheme:
                case ReferenceTreeEnum.Dataflow:
                    return new List<ReferenceTreeEnum>(){ReferenceTreeEnum.Categorisation};
                case ReferenceTreeEnum.Dsd:
                    return new List<ReferenceTreeEnum>(){ReferenceTreeEnum.Dataflow};
                case ReferenceTreeEnum.Codelist:
                case ReferenceTreeEnum.Concept:
                    return new List<ReferenceTreeEnum>(){ReferenceTreeEnum.Dsd};
                case ReferenceTreeEnum.Categorisation:
                default:
                    return new List<ReferenceTreeEnum>();
            }
        }
        private static List<ReferenceTreeEnum> GetChildren(ReferenceTreeEnum MetaType)
        {
            switch (MetaType)
            {
                case ReferenceTreeEnum.Categorisation:
                    return new List<ReferenceTreeEnum>() { ReferenceTreeEnum.CategoryScheme, ReferenceTreeEnum.Dataflow };
                case ReferenceTreeEnum.Dataflow:
                    return new List<ReferenceTreeEnum>() { ReferenceTreeEnum.Dsd };
                case ReferenceTreeEnum.Dsd:
                    return new List<ReferenceTreeEnum>() { ReferenceTreeEnum.Codelist, ReferenceTreeEnum.Concept };
                case ReferenceTreeEnum.CategoryScheme:
                case ReferenceTreeEnum.Codelist:
                case ReferenceTreeEnum.Concept:
                default:
                    return new List<ReferenceTreeEnum>();
            }
        }
        private static List<ReferenceTreeEnum> GetSpecificRef(ReferenceTreeEnum startMetaType, List<SdmxStructureType> specificRef)
        {
            List<ReferenceTreeEnum> references = new List<ReferenceTreeEnum>();
            if (specificRef == null) return references;
            foreach (var item in specificRef)
            {
                switch (item.EnumType)
                {
                  
                    case SdmxStructureEnumType.Categorisation:
                        references.Add(ReferenceTreeEnum.Categorisation);
                        break;
                    case SdmxStructureEnumType.CategoryScheme:
                        references.Add(ReferenceTreeEnum.CategoryScheme);
                        break;
                    case SdmxStructureEnumType.CodeList:
                        references.Add(ReferenceTreeEnum.Codelist);
                        break;
                    case SdmxStructureEnumType.ConceptScheme:
                        references.Add(ReferenceTreeEnum.Concept);
                        break;
                    case SdmxStructureEnumType.Dataflow:
                        references.Add(ReferenceTreeEnum.Dataflow);
                        break;
                    case SdmxStructureEnumType.Dsd:
                        references.Add(ReferenceTreeEnum.Dsd);
                        break;
                    default:
                        break;
                }
            }
            return references;
        }
        
    }
}

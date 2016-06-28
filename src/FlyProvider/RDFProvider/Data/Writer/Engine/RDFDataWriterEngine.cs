// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RdfDataWriterEngine.cs" company="Eurostat">
//   Date Created : 2013-04-01
//   //   Copyright (c) 2013 by the European   Commission, represented by Eurostat.   All rights reserved.
//   // 
//   //   Licensed under the European Union Public License (EUPL) version 1.1. 
//   //   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RDFProvider.Writer.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    
    using RDFProvider.Constants;

    using Estat.Sri.SdmxXmlConstants;

    using Org.Sdmxsource.Sdmx.Api.Constants;
    using Org.Sdmxsource.Sdmx.Api.Constants.InterfaceConstant;
    using Org.Sdmxsource.Sdmx.Api.Model;
    using Org.Sdmxsource.Sdmx.Api.Model.Header;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    using Org.Sdmxsource.Sdmx.Util.Objects;
    using Org.Sdmxsource.Sdmx.DataParser.Engine;
    using System.Globalization;



    public class RDFDataWriterEngine : Writer, IRDFDataWriterEngine
    {
        #region Fields


        private readonly XmlWriter _writer;

        private readonly RDFNamespaces _namespaces;

        private string _dimensionAtObservation;

        private Action<string> _writeObservationMethod;

        private bool _startedDataSet;

        private bool _startedSeries;

        private readonly List<KeyValuePair<string, string>> _componentVals = new List<KeyValuePair<string, string>>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDataWriterEngine"/> class.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="schema">
        /// The schema.
        /// </param>
        public RDFDataWriterEngine(XmlWriter writer)
            : base(writer, CreateDataNamespaces())
        {
            this._writer = writer;
            this._writeObservationMethod = this.RDFWriteObservation21;
            this._namespaces = CreateDataNamespaces();
        }


        #endregion

        #region Public Properties


        #endregion

        #region Properties

        protected string DimensionAtObservation
        {
            get
            {
                return this._dimensionAtObservation;
            }
        }

        protected string MessageElement
        {
            get
            {
                return RDFNameTableCache.GetElementName(RDFElementNameTable.RDF);
            }
        }

       

        protected RDFNamespaces Namespaces
        {
            get
            {
                return this._namespaces;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Close(params IFooterMessage[] footerMessages)
        {
            this.EndSeries();
            this.EndDataSet();

            this.CloseMessageTag();
        }

        public void StartDataset(string DimensionAtObservation, string DataSetId, string DataStructureId)
        {

            this.WriteFormatDataSet(DimensionAtObservation, DataSetId, DataStructureId);
            this._startedDataSet = true;
        }

     
        public void StartSeries(string values, string Dataset)
        {
            string[] val;
            val = (values.Split('/'));

            this.EndSeries();

            this.WriteStartElement(this.Namespaces.RDF, RDFElementNameTable.Description);
            this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.about, values);
            this.WriteStartElement(this.Namespaces.RDF, RDFElementNameTable.type);
            this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.resource, RDFConstants.Rdftype);
            this.WriteEndElement();
            this.WriteStartElement(this.Namespaces.QB, RDFElementNameTable.dataset);
            this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.resource, RDFConstants.RdfDataset  + Dataset);
            this.WriteEndElement();

            this._startedSeries = true;
        }

        public void RDFWriterStrucInfo(string dataset, string struc)
        {
            this.WriteStartElement(this.Namespaces.RDF, RDFElementNameTable.Description);
            this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.about, RDFConstants.Rdfqb + dataset);
            this.WriteStartElement(this.Namespaces.RDF, RDFElementNameTable.type);
            this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.resource, RDFConstants.RdfqbDataSet);
            this.WriteEndElement();
            this.WriteStartElement(this.Namespaces.QB, RDFElementNameTable.structure.ToString());
            this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.resource, RDFConstants.RdfStructure + struc);
            this.WriteEndElement();
            this.WriteStartElement(this.Namespaces.DocTerms, RDFElementNameTable.identifier);
            this.RDFWriteString(dataset);
            this.WriteEndElement();
            this.WriteEndElement();
        }

      

        public void RDFWriteObservation(string observationConceptId, string obsConceptValue, string primaryMeasureValue)
        {
            if (observationConceptId == null)
            {
                throw new ArgumentNullException("observationConceptId");
            }

            string obsValue = string.IsNullOrEmpty(primaryMeasureValue) ? this.DefaultObs : primaryMeasureValue;
            this._writeObservationMethod(obsConceptValue);

            if (!string.IsNullOrEmpty(obsValue))
            {
                this.WriteStartElement(this.Namespaces.Property, RDFElementNameTable.OBS_VALUE);
                this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.datatype, RDFConstants.RdfObsValue);
                this.RDFWriteString(obsValue);
                this.WriteEndElement();
                this.WriteStartElement(this.Namespaces.Property, RDFElementNameTable.OBS_STATUS);
                this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.resource, RDFConstants.RdfObsStatus);
                this.WriteEndElement();
            }

        }

        public void RDFWriteString(string obsValue)
        {
            this.WriteString(obsValue);
        }

        public void WriteSeriesKeyValue(string key, string value, string version, string id)
        {
            this.WriteConceptValue(key, value, version, id);
        }

        #endregion

        #region Methods

        private static RDFNamespaces CreateDataNamespaces()
        {
            var namespaces = new RDFNamespaces();
            namespaces.Xsi = new NamespacePrefixPair(XmlConstants.XmlSchemaNS, XmlConstants.XmlSchemaPrefix);
            namespaces.RDF = new NamespacePrefixPair(RDFConstants.RdfNs21, RDFPrefixConstants.RDF);
            namespaces.Property = new NamespacePrefixPair(RDFConstants.RdfProperty, RDFPrefixConstants.Property);
            namespaces.QB = new NamespacePrefixPair(RDFConstants.Rdfqb, RDFPrefixConstants.QB);
            namespaces.DocTerms = new NamespacePrefixPair(RDFConstants.RdfDcTerms, RDFPrefixConstants.DocTerms);
            namespaces.RDFs = new NamespacePrefixPair(RDFConstants.RdfS, RDFPrefixConstants.RDFs);
            namespaces.Owl = new NamespacePrefixPair(RDFConstants.RdfOwl, RDFPrefixConstants.Owl);
            namespaces.Skos = new NamespacePrefixPair(RDFConstants.RdfSkos, RDFPrefixConstants.Skos);
            namespaces.Xkos = new NamespacePrefixPair(RDFConstants.Rdfxkos, RDFPrefixConstants.Xkos);
            namespaces.XML = new NamespacePrefixPair(RDFConstants.XmlNs, RDFPrefixConstants.XML);
            namespaces.SdmxConcept = new NamespacePrefixPair(RDFConstants.RdfSdmxConcept, RDFPrefixConstants.SdmxConcept);
            namespaces.SchemaLocation = string.Format(
                CultureInfo.InvariantCulture, "{0} SDMXMessage.xsd", SdmxConstants.MessageNs21);

            return namespaces;
        }

        protected override void WriteFormatDataSet(string DimensionAtObservation, string DataSetId, string DataStructureId)
        {
            if (this._startedDataSet)
            {
                return;
            }
            this._dimensionAtObservation = DimensionAtObservation;

            this.RDFWriteMessageTag(MessageElement, this.Namespaces);
            this.WriteStartElement(this.Namespaces.RDF, RDFElementNameTable.RDF);
            this.RDFWriterStrucInfo(DataSetId, DataStructureId);

            this._startedDataSet = true;

        }

        protected void RDFWriteMessageTag(string element, RDFNamespaces namespaces)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            // <?xml version="1.0" encoding="UTF-8"?>
            if (!this.Wrapped)
            {

                // <RDFData
                this.WriteStartElement(this.Namespaces.RDF, element);

                // xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                this.WriteNamespaceDecl(this.Namespaces.Xsi);

                // rdf:Property
                this.WriteNamespaceDecl(this.Namespaces.Property);
                this.WriteNamespaceDecl(this.Namespaces.QB);
                this.WriteNamespaceDecl(this.Namespaces.Skos);
                this.WriteNamespaceDecl(this.Namespaces.Xkos);
                this.WriteNamespaceDecl(this.Namespaces.Owl);
                this.WriteNamespaceDecl(this.Namespaces.RDFs);
                this.WriteNamespaceDecl(this.Namespaces.DocTerms);
                this.WriteNamespaceDecl(this.Namespaces.SdmxConcept);

                string schemaLocation = this.Namespaces.SchemaLocation ?? string.Empty;
                string structureSpecific = string.Empty;

                if (!string.IsNullOrWhiteSpace(schemaLocation))
                {
                    this.WriteAttributeString(this.Namespaces.Xsi, XmlConstants.SchemaLocation, schemaLocation);
                }


            }
        }

        private void EndSeries()
        {
            if (this._startedSeries)
            {
                this.WriteEndElement();
                this._startedSeries = false;
            }
        }

        private void WriteConceptValue(string concept, string value, string version, string id)
        {
            this.WriteStartElement(this.Namespaces.Property, concept);
            this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.resource, RDFConstants.RdfCode + version + "/" + id + "/" + value);
            this.WriteEndElement();
        }

        private void RDFWriteObservation21(string obsConcept)
        {
            this.WriteStartElement(this.Namespaces.Property, this.DimensionAtObservation);
            this.WriteAttributeString(this.Namespaces.RDF, RDFAttributeNameTable.resource, RDFConstants.RdfTimePeriod + obsConcept);
            this.WriteEndElement();
        }

       

        #endregion



       
    }
}
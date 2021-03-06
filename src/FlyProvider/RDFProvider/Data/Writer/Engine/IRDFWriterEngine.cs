﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataWriterEngine.cs" company="Eurostat">
//   Date Created : 2013-04-01
//   //   Copyright (c) 2013 by the European   Commission, represented by Eurostat.   All rights reserved.
//   // 
//   //   Licensed under the European Union Public License (EUPL) version 1.1. 
//   //   If you do not accept this license, you are not allowed to make any use of this file.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RDFProvider.Writer.Engine
{
    #region Using directives

    using System;

    using Org.Sdmxsource.Sdmx.Api.Constants;
    using Org.Sdmxsource.Sdmx.Api.Model;
    using Org.Sdmxsource.Sdmx.Api.Model.Header;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
    using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
    

    #endregion

    public interface IRDFDataWriterEngine
    {
        #region Public Methods and Operators

        void Close(params IFooterMessage[] footer);

        void StartDataset(string DimensionAtObservation, string DataSetId, string DataStructureId);

        void RDFWriteObservation(string observationConceptId, string obsConceptValue, string primaryMeasureValue);

        void WriteSeriesKeyValue(string id, string value_ren, string value_version, string value_Id);

        void StartSeries(string about, string Dataset);

        void RDFWriterStrucInfo(string dataset, string struc);

        #endregion

    }
}
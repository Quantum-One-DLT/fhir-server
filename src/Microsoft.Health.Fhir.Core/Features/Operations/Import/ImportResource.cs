﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.Health.Fhir.Core.Features.Persistence;

namespace Microsoft.Health.Fhir.Core.Features.Operations.Import
{
    public class ImportResource
    {
        public ImportResource(long index, long offset, int length, bool keepVersion, ResourceWrapper resourceWrapper)
        {
            Index = index;
            Offset = offset;
            Length = length;
            KeepVersion = keepVersion;
            ResourceWrapper = resourceWrapper;
        }

        public ImportResource(ResourceWrapper resource)
            : this(0, 0, 0, false, resource)
        {
        }

        public ImportResource(long index, long offset, string importError)
        {
            Index = index;
            Offset = offset;
            Length = 0;
            KeepVersion = false;
            ImportError = importError;
        }

        /// <summary>
        /// Resource index in the resource file
        /// </summary>
        public long Index { get; set; }

        /// <summary>
        /// Read stream offset in bytes
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Json length including EOL
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Flag indicating whether version was provided on input
        /// </summary>
        public bool KeepVersion { get; set; }

        /// <summary>
        /// Resource wrapper from raw content
        /// </summary>
        public ResourceWrapper ResourceWrapper { get; set; }

        /// <summary>
        /// Processing error
        /// </summary>
        public string ImportError { get; set; }
    }
}

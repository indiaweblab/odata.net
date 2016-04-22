//---------------------------------------------------------------------
// <copyright file="ResourceSetWithoutExpectedTypeValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper class to verify that all items of a collection are of the same kind and type.
    /// </summary>
    /// <remarks>This class is only used if no expected item type is specified for the collection; 
    /// otherwise all items are already validated against the expected item type.</remarks>
    internal sealed class ResourceSetWithoutExpectedTypeValidator
    {
        /// <summary>
        /// The base type for all entries in the resource set.
        /// </summary>
        private IEdmEntityType itemType;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ResourceSetWithoutExpectedTypeValidator()
        {
        }

        /// <summary>
        /// Validates the type of a resource in a top-level resource set.
        /// </summary>
        /// <param name="resourceType">The type of the resource.</param>
        internal void ValidateResource(IEdmEntityType resourceType)
        {
            Debug.Assert(resourceType != null, "entityType != null");

            // If we don't have a type, store the type of the first item.
            if (this.itemType == null)
            {
                this.itemType = resourceType;
            }

            // Validate the expected and actual types.
            if (this.itemType.IsEquivalentTo(resourceType))
            {
                return;
            }

            // If the types are not equivalent, make sure they have a common base type.
            IEdmType commonBaseType = EdmLibraryExtensions.GetCommonBaseType(this.itemType, resourceType);
            if (commonBaseType == null)
            {
                throw new ODataException(Strings.FeedWithoutExpectedTypeValidator_IncompatibleTypes(resourceType.FullTypeName(), this.itemType.FullTypeName()));
            }

            this.itemType = (IEdmEntityType)commonBaseType;
        }
    }
}
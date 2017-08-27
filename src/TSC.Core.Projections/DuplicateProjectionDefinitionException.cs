// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    using System;

    /// <summary>
    /// The exception is thrown when a two projection definitions are defined for the same read model and passed to the projection factory.
    /// </summary>
    public class DuplicateProjectionDefinitionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateProjectionDefinitionException"/> class.
        /// </summary>
        /// <param name="projectionDefinitionType">The <see cref="Type"/> of the projection definition that is a duplicate.</param>
        public DuplicateProjectionDefinitionException(Type projectionDefinitionType)
            : base($"A projection definition for [{projectionDefinitionType.Name}] already exists.")
        {
            this.ProjectionDefinition = projectionDefinitionType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of projection definition that did not define itself.
        /// </summary>
        public Type ProjectionDefinition { get; }
    }
}

// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    using System;

    /// <summary>
    /// The exception is thrown when a class implementing <see cref="IProjectionDefinition"/> does not define a projection.
    /// </summary>
    public class ProjectionNotDefinedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionNotDefinedException"/> class.
        /// </summary>
        /// <param name="projectionDefinitionType">The <see cref="Type"/> of the projection definition that doesn't define itself.</param>
        public ProjectionNotDefinedException(Type projectionDefinitionType)
            : base($"Projection [{projectionDefinitionType.Name}] does not define itself.")
        {
            this.ProjectionDefinition = projectionDefinitionType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of projection definition that did not define itself.
        /// </summary>
        public Type ProjectionDefinition { get; }
    }
}

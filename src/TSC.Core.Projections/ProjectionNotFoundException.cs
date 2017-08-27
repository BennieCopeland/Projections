// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    using System;

    /// <summary>
    /// The exception is thrown when a projection can not be found for the requested read model projection.
    /// </summary>
    public class ProjectionNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionNotFoundException"/> class.
        /// </summary>
        /// <param name="readmodel">The <see cref="Type"/> of the read model the projection was requested for.</param>
        public ProjectionNotFoundException(Type readmodel)
        {
            this.ReadModel = readmodel;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the read model the projection was requested for.
        /// </summary>
        public Type ReadModel { get; }
    }
}

// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    using System;

    /// <summary>
    /// The exception is thrown when a two event handlers for the same event are defined on a projection definition.
    /// </summary>
    public class DuplicateEventHandlerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEventHandlerException"/> class.
        /// </summary>
        /// <param name="projectionDefinitionType">The <see cref="Type"/> of the projection definition the duplicate event exists on.</param>
        /// <param name="event">The <see cref="Type"/> of the event that is a duplicate.</param>
        public DuplicateEventHandlerException(Type projectionDefinitionType, Type @event)
            : base($"An event handler for event type: [{@event.Name}] already exists for projection definition: [{projectionDefinitionType.Name}].")
        {
            this.ProjectionDefinition = projectionDefinitionType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of projection definition that did not define itself.
        /// </summary>
        public Type ProjectionDefinition { get; }
    }
}

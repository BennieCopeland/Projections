// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    using System.Collections.Generic;

    /// <summary>
    /// A delegate representing whether a projection is allowed to save its state or not.
    /// </summary>
    /// <returns>true if the projection is allowed to save, false otherwise</returns>
    public delegate bool CanSaveProjection();

    /// <summary>
    /// Defines a projection interface for handling the updating of read models.
    /// </summary>
    public interface IProjection
    {
        /// <summary>
        /// Gets the sequence number that this projection processed previously. -1 for new projections.
        /// </summary>
        long PreviousSequenceNumber { get; }

        /// <summary>
        /// Gets the current read model state.
        /// </summary>
        object ReadModel { get; }

        /// <summary>
        /// Gets or sets a delegate method indicating whether the projection is allowed to save the read model when an event is handled. Default delegate returns true.
        /// </summary>
        /// <remarks>
        /// This is useful to allow an event publisher to prevent costly database activity
        /// on projections that have a large number of events to process until it reaches the end of the stream.
        /// </remarks>
        CanSaveProjection CanSaveProjection { get; set; }

        /// <summary>
        /// Handles an event and updates the read model
        /// </summary>
        /// <param name="sequenceNumber">The sequence number of this event in the stream.</param>
        /// <param name="event">The event data.</param>
        /// <param name="metadata">The event meta data.</param>
        void HandleEvent(long sequenceNumber, object @event, IDictionary<string, object> metadata);
    }
}

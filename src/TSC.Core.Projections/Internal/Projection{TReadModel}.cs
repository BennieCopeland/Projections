// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class that implements <see cref="IProjection"/> and contains the core projection functionality.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of read model this projection updates.</typeparam>
    internal class Projection<TReadModel> : IProjection
    {
        private readonly IDictionary<Type, EventHandlerDelegate<TReadModel>> eventHandlers;
        private readonly IProjectionRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Projection{TReadModel}"/> class.
        /// </summary>
        /// <param name="createInitialState">The delegate used to create the initial read model state when there is not an existing read model in the repository.</param>
        /// <param name="eventHandlers">A collection of event handlers used to update the read model state.</param>
        /// <param name="repository">A repository to retrieve and save the read model state.</param>
        public Projection(Func<TReadModel> createInitialState, IDictionary<Type, EventHandlerDelegate<TReadModel>> eventHandlers, IProjectionRepository repository)
        {
            var result = repository.Get<ProjectionState<TReadModel>>();

            if (result.HasValue)
            {
                this.ReadModel = result.State.ReadModel;
                this.PreviousSequenceNumber = result.State.SequenceNumber;
            }
            else
            {
                this.ReadModel = createInitialState();
            }

            this.eventHandlers = eventHandlers;
            this.repository = repository;
        }

        /// <summary>
        /// Gets the sequence number that this projection processed previously. -1 for new projections.
        /// </summary>
        public long PreviousSequenceNumber { get; private set; } = -1;

        /// <summary>
        /// Gets the current read model state.
        /// </summary>
        public object ReadModel { get; }

        /// <summary>
        /// Gets or sets a delegate method indicating whether the projection is allowed to save the read model when an event is handled. Default delegate returns true.
        /// </summary>
        /// <remarks>
        /// This is useful to allow an event publisher to prevent costly database activity
        /// on projections that have a large number of events to process until it reaches the end of the stream.
        /// </remarks>
        public CanSaveProjection CanSaveProjection { get; set; } = () => true;

        /// <summary>
        /// Handles an event and updates the read model
        /// </summary>
        /// <param name="sequenceNumber">The sequence number of this event in the stream.</param>
        /// <param name="event">The event data.</param>
        /// <param name="metadata">The event meta data.</param>
        public void HandleEvent(long sequenceNumber, object @event, IDictionary<string, object> metadata)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (metadata == null)
            {
                metadata = new Dictionary<string, object>();
            }

            if (this.AlreadySeenThis(sequenceNumber))
            {
                return;
            }
            else if (this.UnexpectedSequenceNumber(sequenceNumber))
            {
                throw new ArgumentOutOfRangeException(nameof(sequenceNumber));
            }

            this.PreviousSequenceNumber = sequenceNumber;

            if (this.eventHandlers.ContainsKey(@event.GetType()))
            {
                this.eventHandlers[@event.GetType()](@event, metadata, (TReadModel)this.ReadModel);

                if (this.CanSaveProjection())
                {
                    this.repository.Save(new ProjectionState<TReadModel>(this.PreviousSequenceNumber, (TReadModel)this.ReadModel));
                }
            }
        }

        private bool AlreadySeenThis(long sequenceNumber)
        {
            return sequenceNumber <= this.PreviousSequenceNumber;
        }

        private bool UnexpectedSequenceNumber(long sequenceNumber)
        {
            return sequenceNumber > this.PreviousSequenceNumber + 1;
        }
    }
}

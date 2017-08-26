using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections.Internal
{
    internal class Projection<TState> : IProjection
    {
        private readonly IDictionary<Type, EventHandlerDelegate<TState>> eventHandlers;
        private readonly IProjectionRepository repository;

        public Projection(Func<TState> createInitialState, IDictionary<Type, EventHandlerDelegate<TState>> eventHandlers, IProjectionRepository repository)
        {
            var result = repository.Get<ProjectionState<TState>>();

            if (result.HasValue)
            {
                this.ReadModel = result.State.ReadModel;
                this.LastSequenceProcessed = result.State.SequenceNumber;
            }
            else
            {
                this.ReadModel = createInitialState();
            }

            this.eventHandlers = eventHandlers;
            this.repository = repository;
        }

        public long LastSequenceProcessed { get; private set; } = -1;

        public object ReadModel { get; }

        public CanSaveProjection CanSaveProjection { get; set; } = () => true;

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

            if (AlreadySeenThis(sequenceNumber))
            {
                return;
            }
            else if (UnexpectedSequenceNumber(sequenceNumber))
            {
                throw new ArgumentOutOfRangeException(nameof(sequenceNumber));
            }

            this.LastSequenceProcessed = sequenceNumber;

            if (this.eventHandlers.ContainsKey(@event.GetType()))
            {
                this.eventHandlers[@event.GetType()](@event, metadata, (TState)this.ReadModel);

                if (this.CanSaveProjection())
                {
                    this.repository.Save(new ProjectionState<TState>(this.LastSequenceProcessed, (TState)this.ReadModel));
                }
            }
        }

        private bool AlreadySeenThis(long sequenceNumber)
        {
            return sequenceNumber <= this.LastSequenceProcessed;
        }

        private bool UnexpectedSequenceNumber(long sequenceNumber)
        {
            return sequenceNumber > this.LastSequenceProcessed + 1;
        }
    }
}

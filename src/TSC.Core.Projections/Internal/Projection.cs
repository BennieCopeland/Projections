using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections.Internal
{
    internal class Projection<TState> : IProjection
    {
        public Projection(Func<TState> initFunction, IDictionary<Type, EventHandlerDelegate<TState>> eventHandlers, IProjectionRepository repository)
        {
            var result = repository.Get<TState>();

            if (result.HasValue)
            {
                ReadModel = result.ReadModel;
                LastSequenceProcessed = result.SequenceNumber;
            }
            else
            {
                ReadModel = initFunction();
            }
        }

        public long LastSequenceProcessed { get; private set; } = -1;

        public object ReadModel { get; }

        public void HandleEvent(long sequenceNumber, object @event, IDictionary<string, object> metadata)
        {
            throw new NotImplementedException();
        }

        public void PersistProjection()
        {
            throw new NotImplementedException();
        }
    }
}

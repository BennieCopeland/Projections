using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public interface IProjection
    {
        long LastSequenceProcessed { get; }

        void HandleEvent(long sequenceNumber, object @event, IDictionary<string, object> metadata);

        void PersistProjection();
    }
}

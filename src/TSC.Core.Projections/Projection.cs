using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    class Projection : IProjection
    {
        public long LastSequenceProcessed => throw new NotImplementedException();

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

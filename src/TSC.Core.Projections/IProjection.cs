using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSC.Core.Projections
{
    public delegate bool CanSaveProjection();

    public interface IProjection
    {
        long LastSequenceProcessed { get; }

        object ReadModel { get; }

        void HandleEvent(long sequenceNumber, object @event, IDictionary<string, object> metadata);

        CanSaveProjection CanSaveProjection { get; set; }
    }
}

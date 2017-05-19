using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections.Internal
{
    internal delegate void EventHandlerDelegate<TState>(object @event, IDictionary<string, object> metadata, TState state);
}

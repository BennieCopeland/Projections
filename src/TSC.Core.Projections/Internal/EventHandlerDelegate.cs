// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections.Internal
{
    using System.Collections.Generic;

    /// <summary>
    /// A typed delegate representing an event handler.
    /// </summary>
    /// <typeparam name="TReadModel">The type of read model that this handler will modify.</typeparam>
    /// <param name="event">The event that took place.</param>
    /// <param name="metadata">The metadata associated with the event.</param>
    /// <param name="readmodel">The read model th at this handler will modify.</param>
    internal delegate void EventHandlerDelegate<TReadModel>(object @event, IDictionary<string, object> metadata, TReadModel readmodel);
}

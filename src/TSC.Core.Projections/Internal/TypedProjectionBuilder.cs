using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections.Internal
{
    internal class TypedProjectionBuilder<TState> : IProjectionBuilder, IHandleEvents<TState>
    {
        private readonly Func<TState> initFunction;
        private readonly Dictionary<Type, EventHandlerDelegate<TState>> eventHandlers = new Dictionary<Type, EventHandlerDelegate<TState>>();

        public Type Type => typeof(TState);

        public TypedProjectionBuilder(Func<TState> initFunction)
        {
            this.initFunction = initFunction;
        }

        public IHandleEvents<TState> When<TEvent>(Action<TEvent, TState> when)
        {
            var wrapper = GetWrapperAction(when);

            eventHandlers.Add(typeof(TEvent), wrapper);

            return this;
        }

        public IHandleEvents<TState> When<TEvent>(Action<TEvent, IDictionary<string, object>, TState> when)
        {
            var wrapper = GetWrapperAction(when);

            eventHandlers.Add(typeof(TEvent), wrapper);

            return this;
        }

        public IProjection Build(IProjectionRepository repository)
        {
            return new Projection<TState>(initFunction, eventHandlers, repository);
        }

        private EventHandlerDelegate<TState> GetWrapperAction<TEvent>(Action<TEvent, TState> when)
        {
            return (e, m, s) => when((TEvent)e, s);
        }

        private EventHandlerDelegate<TState> GetWrapperAction<TEvent>(Action<TEvent, IDictionary<string, object>, TState> when)
        {
            return (e, m, s) => when((TEvent)e, m, s);
        }

        
    }
}

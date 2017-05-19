using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public interface IProjectionDefinitionBuilder
    {
        IHandleEvents<TState> NewDefinition<TState>(Func<TState> init);
    }

    public interface IHandleEvents<TState>
    {
        IHandleEvents<TState> When<TEvent>(Action<TEvent, TState> when);

        IHandleEvents<TState> When<TEvent>(Action<TEvent, IDictionary<string, object>, TState> when);
    }
}

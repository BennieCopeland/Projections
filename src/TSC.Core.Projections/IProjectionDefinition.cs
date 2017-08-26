using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public interface IProjectionDefinition
    {
        void DefineProjection(IDefinitionBuilder builder);
    }

    public interface IDefinitionBuilder
    {
        InitialState<TModel> ForModel<TModel>();
    }

    public interface ForModel
    {
        InitialState<TModel> ForModel<TModel>();
    }

    public interface InitialState<TModel>
    {
        When<TModel> InitialState(Func<TModel> initMethod);
    }

    public interface When<TState>
    {
        When<TState> When<TEvent>(Action<TEvent, TState> when);

        When<TState> When<TEvent>(Action<TEvent, IDictionary<string, object>, TState> when);
    }
}

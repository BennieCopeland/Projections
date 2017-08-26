
namespace TSC.Core.Projections.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    internal class ProjectionBuilder : IProjectionBuilder, IDefinitionBuilder
    {
        public Type Type => helper.Type;

        private IProjectionBuilder helper;

        internal Func<Func<object>, IProjection> BuildProjection { get; set; }

        public ProjectionBuilder(IProjectionDefinition definition)
        {
            definition.DefineProjection(this);
        }

        public IProjection Build(IProjectionRepository repository)
        {
            return helper.Build(repository);
        }

        public InitialState<TState> ForModel<TState>()
        {
            helper = new Helper<TState>();

            return helper as InitialState<TState>;
        }

        class Helper<TState> : IProjectionBuilder, InitialState<TState>, When<TState>
        {
            private readonly ProjectionBuilder projectionBuilder;

            internal Func<TState> InitFunction { get; set; }

            internal IDictionary<Type, EventHandlerDelegate<TState>> Handlers { get; } = new Dictionary<Type, EventHandlerDelegate<TState>>();

            public Type Type => typeof(TState);

            public When<TState> InitialState(Func<TState> initMethod)
            {
                this.InitFunction = () => initMethod();

                return this;
            }

            public When<TState> When<TEvent>(Action<TEvent, TState> when)
            {
                this.Handlers.Add(typeof(TEvent), (e, m, s) =>
                {
                    when((TEvent)e, (TState)s);
                });

                return this;
            }

            public When<TState> When<TEvent>(Action<TEvent, IDictionary<string, object>, TState> when)
            {
                this.Handlers.Add(typeof(TEvent), (e, m, s) =>
                {
                    when((TEvent)e, m, (TState)s);
                });

                return this;
            }

            public IProjection Build(IProjectionRepository repository)
            {
                return new Projection<TState>(InitFunction, Handlers, repository);
            }
        }
    }
}

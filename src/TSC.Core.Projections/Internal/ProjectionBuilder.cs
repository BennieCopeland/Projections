using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections.Internal
{
    internal class ProjectionBuilder : IProjectionBuilder, IProjectionDefinitionBuilder
    {
        public Type Type => builder.Type;
        private IProjectionBuilder builder;

        public ProjectionBuilder(IProjectionDefinition definition)
        {
            definition.OnDefinitionBuilding(this);
        }

        public IHandleEvents<TState> NewDefinition<TState>(Func<TState> initFunction)
        {
            var typedBuilder = new TypedProjectionBuilder<TState>(initFunction);
            builder = typedBuilder;
            return typedBuilder;
        }

        public IProjection Build(IProjectionRepository repository)
        {
            return builder.Build(repository);
        }
    }
}

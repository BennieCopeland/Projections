using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public class ProjectionFactory2
    {
        IEnumerable<IProjectionDefinition> definitions;
        private IProjectionRepository repository;

        public ProjectionFactory2(IProjectionRepository repository, IEnumerable<IProjectionDefinition> definitions)
        {
            this.definitions = definitions;
            this.repository = repository;
        }

        public IInitializeProjections<T> AddDefinition<T>()
        {
            throw new NotImplementedException();
        }

        public IProjection CreateProjection<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProjection> CreateProjections()
        {
            DefinitionBuilder builder = new DefinitionBuilder();
            List<IProjection> projections = new List<IProjection>();

            foreach (var definition in definitions)
            {
                definition.GetDefinition(builder);
                projections.Add(builder.Build(repository));
            }

            return projections;
        }
    }

    public interface IProjectionDefinitionBuilder
    {
        IInitializeProjections<T> NewDefinition<T>();
    }

    interface IProjectionBuilder
    {
        IProjection Build(IProjectionRepository repository);
    }

    public class DefinitionBuilder : IProjectionBuilder, IProjectionDefinitionBuilder
    {
        IProjectionBuilder builder;

        public IProjection Build(IProjectionRepository repository)
        {
            return builder.Build(repository);
        }

        public IInitializeProjections<T> NewDefinition<T>()
        {
            var typedBuilder = new Projection<T>();
            builder = typedBuilder;
            return typedBuilder;
        }

        class Projection<T> : IProjectionBuilder, IInitializeProjections<T>, IHandleEvents<T>
        {
            public IProjection Build(IProjectionRepository repository)
            {
                throw new NotImplementedException();
            }

            public IHandleEvents<T> OnInit(Func<T> init)
            {
                throw new NotImplementedException();
            }

            public IHandleEvents<T> When<TEvent>(Action<TEvent, T> when)
            {
                throw new NotImplementedException();
            }
        }
    }
}

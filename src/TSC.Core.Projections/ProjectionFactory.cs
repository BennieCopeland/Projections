using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSC.Core.Projections
{
    using TSC.Core.Projections.Internal;

    public class ProjectionFactory
    {
        private readonly ProjectionBuilderCollection builders;
        private readonly IProjectionRepository repository;

        public ProjectionFactory(IProjectionRepository repository, IEnumerable<IProjectionDefinition> definitions)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));

            if(definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }
            this.builders = new ProjectionBuilderCollection(definitions);
        }

        public IProjection CreateProjection<T>()
        {
            if(builders.TryGetValue(typeof(T), out var builder))
            {
                return builder.Build(repository);
            }

            throw new ProjectionNotFoundException(typeof(T));
        }

        public IEnumerable<IProjection> CreateProjections()
        {
            return builders.Select(builders => builders.Build(repository));
        }
    }
}

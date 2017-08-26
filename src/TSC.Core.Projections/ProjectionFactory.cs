// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using TSC.Core.Projections.Internal;

    /// <summary>
    /// A factory used to create <see cref="IProjection"/> instances used to build readmodels.
    /// </summary>
    public class ProjectionFactory
    {
        private readonly IProjectionRepository repository;
        private readonly Dictionary<Type, ProjectionBuilder> builders = new Dictionary<Type, ProjectionBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionFactory"/> class.
        /// </summary>
        /// <param name="repository">The projection repository used for persistent storage.</param>
        /// <param name="definitions">The list of <see cref="IProjectionDefinition"/>s for the factory to create.</param>
        public ProjectionFactory(IProjectionRepository repository, IEnumerable<IProjectionDefinition> definitions)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));

            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            foreach (var definition in definitions)
            {
                var builder = new ProjectionBuilder(definition);

                this.builders.Add(builder.Type, builder);
            }
        }

        /// <summary>
        /// Creates a single <see cref="IProjection"/> ready to handle new events.
        /// </summary>
        /// <typeparam name="T">Blah</typeparam>
        /// <returns></returns>
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
            return builders.Select(builder => builder.Value.Build(repository));
        }
    }
}

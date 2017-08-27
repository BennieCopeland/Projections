// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using TSC.Core.Projections.Internal;

    /// <summary>
    /// A factory to create <see cref="IProjection"/> instances used to build readmodels.
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

                try
                {
                    this.builders.Add(builder.ForReadModel, builder);
                }
                catch (ArgumentException)
                {
                    throw new DuplicateProjectionDefinitionException(builder.ForReadModel);
                }
            }
        }

        /// <summary>
        /// Creates a single <see cref="IProjection"/> ready to handle events.
        /// </summary>
        /// <typeparam name="TReadModel">The type of read model to create a projection for.</typeparam>
        /// <returns>Returns an <see cref="IProjection"/> for the read model.</returns>
        public IProjection CreateProjection<TReadModel>()
        {
            if (this.builders.TryGetValue(typeof(TReadModel), out var builder))
            {
                return builder.Build(this.repository);
            }

            throw new ProjectionNotFoundException(typeof(TReadModel));
        }

        /// <summary>
        /// Creates a list of <see cref="IProjection"/> instances ready to handle events.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of type <see cref="IProjection"/> containing the projections.</returns>
        public IEnumerable<IProjection> CreateProjections()
        {
            return this.builders.Select(builder => builder.Value.Build(this.repository));
        }
    }
}

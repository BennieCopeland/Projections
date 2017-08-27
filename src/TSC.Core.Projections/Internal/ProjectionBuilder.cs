// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Builds a <see cref="Projection{TState}"/> from a class implementing <see cref="IProjectionDefinition"/>.
    /// </summary>
    internal class ProjectionBuilder : IDefinitionBuilder
    {
        private IHelper helper;
        private IProjectionDefinition definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionBuilder"/> class.
        /// </summary>
        /// <param name="definition">The projection definition to create a projection from.</param>
        public ProjectionBuilder(IProjectionDefinition definition)
        {
            this.definition = definition;

            this.definition.DefineProjection(this);

            if (!this.ProjectionDefined())
            {
                throw new ProjectionNotDefinedException(definition.GetType());
            }
        }

        /// <summary>
        /// Represents a projection build helper.
        /// </summary>
        private interface IHelper
        {
            /// <summary>
            /// Builds a new <see cref="IProjection"/> for the read model.
            /// </summary>
            /// <param name="repository">The repository used to get the persisted read model.</param>
            /// <returns>The created projection.</returns>
            IProjection Build(IProjectionRepository repository);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of read model this builder will create a projection for.
        /// </summary>
        public Type ForReadModel { get; private set; }

        private bool ModelTypeSet { get; set; } = false;

        private bool InitialStateSet { get; set; } = false;

        private bool EventHandlerSet { get; set; } = false;

        /// <summary>
        /// Builds an instance of <see cref="Projection{TState}"/> using the projection defintion and an <see cref="IProjectionRepository"/>.
        /// </summary>
        /// <param name="repository">The repository to retrieve the current read model state.</param>
        /// <returns>An instance of <see cref="Projection{TState}"/>.</returns>
        public IProjection Build(IProjectionRepository repository)
        {
            return this.helper.Build(repository);
        }

        /// <summary>
        /// Defines the read model for the projection.
        /// </summary>
        /// <typeparam name="TReadModel">The <see cref="Type"/> of read model for the projection.</typeparam>
        /// <returns>A helper class for defining the initial state of the read model.</returns>
        public InitialState<TReadModel> ForModel<TReadModel>()
        {
            this.ModelTypeSet = true;
            this.ForReadModel = typeof(TReadModel);

            this.helper = new Helper<TReadModel>(this);

            return this.helper as InitialState<TReadModel>;
        }

        private bool ProjectionDefined()
        {
            return this.ModelTypeSet && this.InitialStateSet && this.EventHandlerSet;
        }

        private class Helper<TState> : IHelper, InitialState<TState>, IWhen<TState>
        {
            private readonly ProjectionBuilder projectionBuilder;

            public Helper(ProjectionBuilder builder)
            {
                this.projectionBuilder = builder;
            }

            internal Func<TState> InitFunction { get; set; }

            internal IDictionary<Type, EventHandlerDelegate<TState>> Handlers { get; } = new Dictionary<Type, EventHandlerDelegate<TState>>();

            public IWhen<TState> InitialState(Func<TState> initMethod)
            {
                this.projectionBuilder.InitialStateSet = true;

                this.InitFunction = () => initMethod();

                return this;
            }

            public IWhen<TState> When<TEvent>(Action<TEvent, TState> when)
            {
                this.projectionBuilder.EventHandlerSet = true;

                try
                {
                    this.Handlers.Add(typeof(TEvent), (e, m, s) =>
                    {
                        when((TEvent)e, (TState)s);
                    });
                }
                catch (ArgumentException)
                {
                    throw new DuplicateEventHandlerException(this.projectionBuilder.definition.GetType(), typeof(TEvent));
                }

                return this;
            }

            public IWhen<TState> When<TEvent>(Action<TEvent, IDictionary<string, object>, TState> when)
            {
                this.projectionBuilder.EventHandlerSet = true;

                try
                {
                    this.Handlers.Add(typeof(TEvent), (e, m, s) =>
                    {
                        when((TEvent)e, m, (TState)s);
                    });
                }
                catch (ArgumentException)
                {
                    throw new DuplicateEventHandlerException(this.projectionBuilder.definition.GetType(), typeof(TEvent));
                }

                return this;
            }

            public IProjection Build(IProjectionRepository repository)
            {
                return new Projection<TState>(this.InitFunction, this.Handlers, repository);
            }
        }
    }
}

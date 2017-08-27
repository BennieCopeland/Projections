// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a method to create projection definitions.
    /// </summary>
    public interface IProjectionDefinition
    {
        /// <summary>
        /// Defines a projection.
        /// </summary>
        /// <param name="builder">A fluent interface for defining a projection for a read model.</param>
        void DefineProjection(IDefinitionBuilder builder);
    }

    /// <summary>
    /// Defines a method for setting the read model for a projection.
    /// </summary>
    public interface IDefinitionBuilder
    {
        /// <summary>
        /// Sets the read model for the projection.
        /// </summary>
        /// <typeparam name="TReadModel">The <see cref="Type"/> of read model for the projection.</typeparam>
        /// <returns>A fluent interface for setting the inital state of the read model for the projection.</returns>
        InitialState<TReadModel> ForModel<TReadModel>();
    }

    /// <summary>
    /// Defines a method for capturing a delegate that sets the inital state of a read model.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of read model.</typeparam>
    public interface InitialState<TReadModel>
    {
        /// <summary>
        /// Captures a delegate method for creating the initial read model state.
        /// </summary>
        /// <param name="initMethod">The delegate method that creates the initial read model state.</param>
        /// <returns>A fluent interface for setting the handler methods for the projection.</returns>
        IWhen<TReadModel> InitialState(Func<TReadModel> initMethod);
    }

    /// <summary>
    /// Defines methods for capturing delegates to apply events to a read model.
    /// </summary>
    /// <typeparam name="TReadModel">The <see cref="Type"/> of read model.</typeparam>
    public interface IWhen<TReadModel>
    {
        /// <summary>
        /// Captures a delegate method for applying an event to a read model.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="Type"/> of event to apply.</typeparam>
        /// <param name="handler">The delegate method that applys an event to a read model.</param>
        /// <returns>A fluent interface for setting the handler methods for the projection.</returns>
        IWhen<TReadModel> When<TEvent>(Action<TEvent, TReadModel> handler);

        /// <summary>
        /// Captures a delegate method for applying an event and a metadata dictionary to a read model.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="Type"/> of event to apply.</typeparam>
        /// <param name="handler">The delegate method that applys an event and metadata dictionary to a read model.</param>
        /// <returns>A fluent interface for setting the handler methods for the projection.</returns>
        IWhen<TReadModel> When<TEvent>(Action<TEvent, IDictionary<string, object>, TReadModel> handler);
    }
}

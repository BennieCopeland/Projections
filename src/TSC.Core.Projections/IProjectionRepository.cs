// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections
{
    /// <summary>
    /// Represents a generic repository that can get and save data based on <see cref="System.Type"/>
    /// </summary>
    public interface IProjectionRepository
    {
        /// <summary>
        /// Gets the current state from the repository.
        /// </summary>
        /// <typeparam name="TState">The <see cref="System.Type"/> of state to retrieve.</typeparam>
        /// <returns>A tuple with a HasValue boolean indicating whether the State value contains the
        /// requested data or a null value.</returns>
#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
        (bool HasValue, TState State) Get<TState>();
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly

        /// <summary>
        /// Saves the current state into the repository.
        /// </summary>
        /// <typeparam name="TState">The <see cref="System.Type"/> of state to persist.</typeparam>
        /// <param name="state">The state to persist.</param>
        void Save<TState>(TState state);
    }
}

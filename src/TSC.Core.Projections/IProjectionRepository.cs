namespace TSC.Core.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IProjectionRepository
    {
        (bool HasValue, T State) Get<T>();

        void Save<T>(T state);
    }
}

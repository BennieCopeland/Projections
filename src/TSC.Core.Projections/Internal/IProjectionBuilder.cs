namespace TSC.Core.Projections.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal interface IProjectionBuilder
    {
        Type Type { get; }

        IProjection Build(IProjectionRepository repository);
    }
}

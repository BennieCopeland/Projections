using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections.Internal
{
    internal interface IProjectionBuilder
    {
        Type Type { get; }
        IProjection Build(IProjectionRepository repository);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public interface IProjectionDefinitionCollection
    {
        IInitializeProjections<T> AddDefinition<T>();

        IProjection CreateProjection<T>();

        IEnumerable<IProjection> CreateProjections();
    }
}

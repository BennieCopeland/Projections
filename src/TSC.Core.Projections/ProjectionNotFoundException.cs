using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public class ProjectionNotFoundException : Exception
    {
        public Type Projection { get; }

        public ProjectionNotFoundException(Type projection)
        {
            Projection = projection;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public interface IProjectionDefinition
    {
        void GetDefinition(IProjectionDefinitionBuilder definitions);
    }
}

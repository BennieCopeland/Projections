using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSC.Core.Projections;

namespace TSC.Core.Projections.Tests.Helpers
{
    class MockDefinition : IProjectionDefinition
    {
        private readonly Action<IDefinitionBuilder> definitionFunction;

        public bool GetDefinitionCalled { get; private set; } = false;
        public int Times { get; private set; } = 0;

        public MockDefinition(Action<IDefinitionBuilder> definitionFunction)
        {
            this.definitionFunction = definitionFunction;
        }

        public void DefineProjection(IDefinitionBuilder builder)
        {
            GetDefinitionCalled = true;
            ++Times;
            definitionFunction(builder);
        }
    }
}

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
        private readonly Action<IProjectionDefinitionBuilder> definitionFunction;

        public bool GetDefinitionCalled { get; private set; } = false;
        public int Times { get; private set; } = 0;

        public MockDefinition(Action<IProjectionDefinitionBuilder> definitionFunction)
        {
            this.definitionFunction = definitionFunction;
        }

        public void OnDefinitionBuilding(IProjectionDefinitionBuilder definitionBuilder)
        {
            GetDefinitionCalled = true;
            ++Times;
            definitionFunction(definitionBuilder);
        }
    }
}

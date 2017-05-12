using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSpec;
using TSC.Core.Projections;

namespace TSC.Core.ProjectionsTests.describe_ProjectionBuilder
{
    class _ProjectionBuilder : nspec
    {
        protected readonly (bool, long, DummyReadModel) NO_READ_MODEL = (false, -1, null);
        protected readonly (bool, long, DummyReadModel) EXISITING_READ_MODEL = (true, 5, new DummyReadModel { SomeProperty = "Read Model From Repository" });
        protected readonly long NONSENSICAL_SEQUENCE_NUMBER = long.MinValue;
        protected readonly long START_OF_EVENT_LOG = 0;
        protected readonly DummyEvent AN_EVENT = new DummyEvent();
        protected readonly IDictionary<string, object> EMPTY_METADATA_DICTIONARY = new Dictionary<string, object>();
        protected readonly IDictionary<string, object> NULL_METADATA = null;

        protected class DummyEvent
        {

        }

        protected class DummyReadModel
        {
            public string SomeProperty { get; set; } = "Default Value";
        }

        protected class ConcreteProjectionBuilder : ProjectionBuilderBase<DummyReadModel>
        {
            public ConcreteProjectionBuilder(IProjectionRepository repository) : base(repository)
            {
            }

            public ConcreteProjectionBuilder ExposedInitialState(Func<DummyReadModel> action)
            {
                InitialState(action);
                return this;
            }

            public ConcreteProjectionBuilder ExposedWhen<TEntity>(Action<TEntity, DummyReadModel> action)
            {
                When<TEntity>(action);
                return this;
            }
        }
    }
}

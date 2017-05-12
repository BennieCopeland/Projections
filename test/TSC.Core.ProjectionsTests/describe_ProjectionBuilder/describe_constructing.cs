using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TSC.Core.Projections;

namespace TSC.Core.ProjectionsTests.describe_ProjectionBuilder
{
    class describe_constructing : _ProjectionBuilder
    {
        void given_the_repository_is_null()
        {
            ConcreteProjectionBuilder projection = null;
            IProjectionRepository repository = null;

            beforeEach = () => repository = null;

            act = () => projection = new ConcreteProjectionBuilder(repository);

            it["will throw an ArgumentNullException"] = expect<ArgumentNullException>("Value cannot be null.\r\nParameter name: repository");
        }

        void given_a_read_model_does_not_exist_in_the_repository()
        {
            ConcreteProjectionBuilder projection = null;
            Mock<IProjectionRepository> repository = null;

            beforeEach = () =>
            {
                repository = new Mock<IProjectionRepository>();
                repository.Setup(repo => repo.Get<DummyReadModel>()).Returns(NO_READ_MODEL);
            };

            act = () => projection = new ConcreteProjectionBuilder(repository.Object);

            it["will have a LastSequenceProcessed value of -1"] = () =>
            {
                projection.LastSequenceProcessed.Should().Be(-1);
            };
        }

        void given_a_read_model_does_exist_in_the_repository()
        {
            ConcreteProjectionBuilder projection = null;
            Mock<IProjectionRepository> repository = null;

            beforeEach = () =>
            {
                repository = new Mock<IProjectionRepository>();
                repository.Setup(repo => repo.Get<DummyReadModel>()).Returns(EXISITING_READ_MODEL);
            };

            act = () => projection = new ConcreteProjectionBuilder(repository.Object);

            it["will have the LastSequenceProcessed value set appropriately"] = () =>
            {
                projection.LastSequenceProcessed.Should().Be(5);
            };
        }
    }
}

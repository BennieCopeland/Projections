using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NSpec;
using TSC.Core.Projections;
using TSC.Core.ProjectionsTests.Helpers;

namespace TSC.Core.ProjectionsTests.describe_ProjectionFactory
{
    class _ProjectionFactory : nspec
    {
        (bool, long, ReadModel1) NO_READ_MODEL = (false, -1, null);
        protected readonly (bool, long, ReadModel1) EXISITING_READ_MODEL = (true, 5, new ReadModel1 { State = "Read Model From Repository" });

        void describe_constructing()
        {
            context["given the repository is null"] = () =>
            {
                IProjectionRepository NULL_REPOSITORY = null;
                IProjectionDefinition[] UNUSED_DEFINITIONS = null;

                beforeEach = () =>
                {
                    NULL_REPOSITORY = null;
                    UNUSED_DEFINITIONS = new IProjectionDefinition[0];
                };

                act = () => new ProjectionFactory(NULL_REPOSITORY, UNUSED_DEFINITIONS);

                it["will throw an ArgumentNullException"] = expect<ArgumentNullException>("Value cannot be null.\r\nParameter name: repository");
            };

            context["given the definitions list is null"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                IProjectionDefinition[] UNUSED_DEFINITIONS = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    UNUSED_DEFINITIONS = null;
                };

                act = () => new ProjectionFactory(repository.Object, UNUSED_DEFINITIONS);

                it["will throw an ArgumentNullException"] = expect<ArgumentNullException>("Value cannot be null.\r\nParameter name: definitions");
            };
        }

        void describe_CreateProjection_method()
        {
            context["when a projection defintion exist for the read model"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                ProjectionFactory factory = null;
                IProjection projection = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    var definitions = new IProjectionDefinition[]
                    {
                        new MockDefinition((builder) =>
                        {
                            builder.NewDefinition<ReadModel1>(() => new ReadModel1())
                                .When<EventOne>((e, s) => s.State = e.NewState);
                        })
                    };
                    factory = new ProjectionFactory(repository.Object, definitions);
                };

                act = () => projection = factory.CreateProjection<ReadModel1>();

                it["returns a projection for the correct read model"] = () =>
                {
                    projection.ReadModel.GetType().Should().Be(typeof(ReadModel1));
                    //GetReadModel @event = new GetReadModel();
                    //projection.HandleEvent(0, @event, null);
                    //@event.ReadModel.GetType().Should().Be(typeof(ReadModel1));
                };
            };

            context["when a projection defintion does not exist for the read model"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                ProjectionFactory factory = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    var EMPTY_DEFINITION_LIST = new IProjectionDefinition[] { };
                    factory = new ProjectionFactory(repository.Object, EMPTY_DEFINITION_LIST);
                };

                act = () => factory.CreateProjection<ReadModel1>();

                it["throws a ProjectionNotFoundException"] = expect<ProjectionNotFoundException>();
            };
        }

        void describe_CreateProjections_method()
        {
            context["when there are projection defintions"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                ProjectionFactory factory = null;
                List<IProjection> projections = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    var definitions = new IProjectionDefinition[]
                    {
                        new MockDefinition((builder) =>
                        {
                            builder.NewDefinition<ReadModel1>(() => new ReadModel1());
                        }),
                        new MockDefinition((builder) =>
                        {
                            builder.NewDefinition<ReadModel2>(() => new ReadModel2());
                        })
                    };

                    factory = new ProjectionFactory(repository.Object, definitions);
                };

                act = () => projections = factory.CreateProjections().ToList();

                it["will return a list of read projections"] = () =>
                {
                    projections.Count.Should().Be(2);
                    projections[0].ReadModel.GetType().Should().Be(typeof(ReadModel1));
                    projections[1].ReadModel.GetType().Should().Be(typeof(ReadModel2));
                };
            };

            context["when there are no projection defintions"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                ProjectionFactory factory = null;
                List<IProjection> projections = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    var definitions = new IProjectionDefinition[] { };

                    factory = new ProjectionFactory(repository.Object, definitions);
                };

                act = () => projections = factory.CreateProjections().ToList();

                it["will return an empty list"] = () =>
                {
                    projections.Should().BeEmpty();
                };
            };
        }

        void describe_projections()
        {
            context["given a projection definition"] = () =>
            {
                MockDefinition definition = null;

                beforeEach = () =>
                {
                    definition = new MockDefinition((builder =>
                    {
                        builder.NewDefinition<ReadModel1>(() => new ReadModel1() { State = "Initial State" });
                    }));
                };

                context["and there is no persisted read model"] = () =>
                {
                    Mock<IProjectionRepository> repository = null;

                    beforeEach = () =>
                    {
                        repository = new Mock<IProjectionRepository>();
                        repository.Setup(repo => repo.Get<ReadModel1>()).Returns(NO_READ_MODEL);
                    };

                    context["when the projection is created"] = () =>
                    {
                        ProjectionFactory factory = null;
                        IProjection projection = null;

                        beforeEach = () =>
                        {
                            factory = new ProjectionFactory(repository.Object, new []{ definition });
                        };

                        act = () => projection = factory.CreateProjection<ReadModel1>();

                        it["will have a LastSequenceProcessed value of -1"] = () =>
                        {
                            projection.LastSequenceProcessed.Should().Be(-1);
                        };

                        it["will create the read model using the factory method"] = () =>
                        {
                            ReadModel1 model = projection.ReadModel as ReadModel1;
                            model.State.Should().Be("Initial State");
                        };
                    };
                };

                context["and there is a persisted read model"] = () =>
                {
                    Mock<IProjectionRepository> repository = null;

                    beforeEach = () =>
                    {
                        repository = new Mock<IProjectionRepository>();
                        repository.Setup(repo => repo.Get<ReadModel1>()).Returns(EXISITING_READ_MODEL);
                    };

                    context["when the projection is created"] = () =>
                    {
                        ProjectionFactory factory = null;
                        IProjection projection = null;

                        beforeEach = () =>
                        {
                            factory = new ProjectionFactory(repository.Object, new[] { definition });
                        };

                        act = () => projection = factory.CreateProjection<ReadModel1>();

                        it["will have a LastSequenceProcessed of the correct value"] = () =>
                        {
                            projection.LastSequenceProcessed.Should().Be(5);
                        };

                        it["will use the read model from the repository"] = () =>
                        {
                            ReadModel1 model = projection.ReadModel as ReadModel1;
                            model.State.Should().Be("Read Model From Repository");
                        };
                    };
                };
            };
        }
    }

    class ReadModel1
    {
        public string State { get; set; }
    }

    class ReadModel2
    {
        public string State { get; set; }
    }

    class EventOne
    {
        public string NewState { get; set; }
    }

}

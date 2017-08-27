using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NSpec;
using TSC.Core.Projections;
using TSC.Core.Projections.Tests.Helpers;

namespace TSC.Core.Projections.Tests.describe_ProjectionFactory
{
    class _ProjectionFactory : nspec
    {
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

            context["given there are duplicate projection defintions"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                IProjectionDefinition[] definitions = null;
                Action action = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    definitions = new IProjectionDefinition[]
                    {
                        new MockDefinition((builder) =>
                        {
                            builder.ForModel<ReadModel1>().InitialState(() => new ReadModel1())
                                .When<string>((e, s) => s.State = "");
                        }),
                        new MockDefinition((builder) =>
                        {
                            builder.ForModel<ReadModel1>().InitialState(() => new ReadModel1())
                                .When<string>((e, s) => s.State = "");
                        })
                    };
                };

                act = () => action = () => new ProjectionFactory(repository.Object, definitions);

                it["will throw an exception"] = () =>
                {
                    action.ShouldThrow<DuplicateProjectionDefinitionException>().WithMessage("A projection definition for [ReadModel1] already exists.");
                };
            };

            context["given a projection defintion has a duplicate event handler"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                IProjectionDefinition[] definitions = null;
                Action action = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    definitions = new IProjectionDefinition[]
                    {
                        new MockDefinition((builder) =>
                        {
                            builder.ForModel<ReadModel1>().InitialState(() => new ReadModel1())
                                .When<string>((e, s) => s.State = "")
                                .When<string>((e, s) => s.State = "");
                        })
                    };
                };

                act = () => action = () => new ProjectionFactory(repository.Object, definitions);

                it["will throw an exception"] = () =>
                {
                    action.ShouldThrow<DuplicateEventHandlerException>().WithMessage("An event handler for event type: [string] already exists for projection definition: [MockDefinition].");
                };
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
                            builder
                                .ForModel<ReadModel1>()
                                .InitialState(() => new ReadModel1())
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
                            builder.ForModel<ReadModel1>().InitialState(() => new ReadModel1())
                                .When<string>((e, s) => s.State = "");
                        }),
                        new MockDefinition((builder) =>
                        {
                            builder.ForModel<ReadModel2>().InitialState(() => new ReadModel2())
                                .When<string>((e, s) => s.State = "");
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

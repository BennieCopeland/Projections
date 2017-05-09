using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NSpec;
using TSC.Core.Projections;

namespace TSC.Core.ProjectionsTests
{
    class describe_ProjectionBuilderBase : nspec
    {
        void when_a_projection_is_created()
        {
            Mock<IProjectionRepository> repository = null;
            DummyProjection projection = null;

            beforeEach = () =>
            {
                repository = new Mock<IProjectionRepository>();
            };

            act = () => projection = new DummyProjection(repository.Object);

            context["given a read model does not exist in the repository"] = () =>
            {
                beforeEach = () =>
                {
                    repository.Setup(repo => repo.Get<DummyReadModel>()).Returns((false, -1, null));
                };

                it["will have a LastSequenceProccessed value of -1"] = () =>
                {
                    projection.LastSequenceProcessed.Should().Be(-1);
                };
            };

            context["given a read model exists in the repository"] = () =>
            {
                beforeEach = () =>
                {
                    repository.Setup(repo => repo.Get<DummyReadModel>()).Returns((true, 5, new DummyReadModel { SomeProperty = "Found" }));
                };

                it["will have the LastSequenceProcessed value set appropriately"] = () =>
                {
                    projection.LastSequenceProcessed.Should().Be(5);
                };
            };
        }

        void describe_initializing_the_read_model_state()
        {
            context["given a read model does not exist in the repository"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                DummyProjection projection = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    repository.Setup(repo => repo.Get<DummyReadModel>()).Returns((false, -1, null));
                    projection = new DummyProjection(repository.Object);
                };

                context["and an initialization callback has not been set"] = () =>
                {
                    context["when an event gets handled"] = () =>
                    {
                        DummyReadModel state = null;

                        beforeEach = () =>
                        {
                            projection.ExposedWhen<DummyEvent>((e, s) =>
                            {
                                state = s;
                            });
                        };

                        act = () => projection.HandleEvent(0, new DummyEvent(), null);

                        it["will call the handlers using the new read model"] = () =>
                        {
                            state.SomeProperty.Should().Be("Initial Value");
                        };

                        it["will persist the new read model"] = todo;
                    };
                };

                context["and an initialization callback has been set"] = () =>
                {
                    beforeEach = () =>
                    {
                        projection.ExposedInitialState(() =>
                        {
                            return new DummyReadModel { SomeProperty = "Callback State" };
                        });
                    };

                    context["when an event gets handled"] = () =>
                    {
                        DummyReadModel state = null;

                        beforeEach = () =>
                        {
                            projection.ExposedWhen<DummyEvent>((e, s) =>
                            {
                                state = s;
                            });
                        };

                        act = () => projection.HandleEvent(0, new DummyEvent(), null);

                        it["will call the handlers using the initialized read model"] = () =>
                        {
                            state.SomeProperty.Should().Be("Callback State");
                        };

                        it["will persist the initialized read model"] = todo;
                    };
                };
            };

            context["given a read model exists in the repository"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                DummyProjection projection = null;
                DummyReadModel existingState = null;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    existingState = new DummyReadModel { SomeProperty = "Exisiting model" };
                    repository.Setup(repo => repo.Get<DummyReadModel>()).Returns((true, 5, existingState));
                    projection = new DummyProjection(repository.Object);
                };

                context["when an event gets handled"] = () =>
                {
                    DummyReadModel state = null;

                    beforeEach = () =>
                    {
                        projection.ExposedWhen<DummyEvent>((e, s) =>
                        {
                            state = s;
                        });
                    };

                    act = () => projection.HandleEvent(0, new DummyEvent(), null);

                    it["will call the handlers using the existing read model"] = () =>
                    {
                        state.Should().Be(existingState);
                    };

                    it["will persist the existing read model"] = todo;
                };
            };
        }

        void describe_sequence_number_handling()
        {
            context["given a projection with an exisiting model at sequence number 5"] = () =>
            {
                Mock<IProjectionRepository> repository = null;
                DummyProjection projection = null;
                DummyReadModel existingState = null;
                long lastProcessedSequence = 5;

                beforeEach = () =>
                {
                    repository = new Mock<IProjectionRepository>();
                    existingState = new DummyReadModel { SomeProperty = "Exisiting model" };
                    repository.Setup(repo => repo.Get<DummyReadModel>()).Returns((true, lastProcessedSequence, existingState));
                    projection = new DummyProjection(repository.Object);
                };

                context["when an event is handled"] = () =>
                {
                    long sequenceNumber = -5;

                    act = () => projection.HandleEvent(sequenceNumber, new DummyEvent(), null);

                    context["and the sequence number is 4"] = () =>
                    {

                        beforeEach = () => sequenceNumber = lastProcessedSequence - 1;

                        it["will not call any handlers"] = todo;

                        it["will not persist the readmodel"] = todo;
                    };

                    context["and the sequence number is 5"] = () =>
                    {

                        beforeEach = () => sequenceNumber = lastProcessedSequence - 1;

                        it["will not call any handlers"] = todo;

                        it["will not persist the readmodel"] = todo;
                    };

                    context["and the sequence number is 6"] = () =>
                    {
                        it["will call the handlers"] = todo;

                        it["will persist the read model"] = todo;
                    };

                    context["and the sequence number is 7"] = () =>
                    {
                        it["will throw an exception"] = todo;
                    };
                };


            };
            

            

            context["and the event is null"] = () =>
            {

            };

            context["and the dictionary is null"] = () =>
            {

            };
        }
    }

    class DummyEvent
    {

    }

    class DummyReadModel
    {
        public string SomeProperty { get; set; } = "Initial Value";
    }

    class DummyProjection : ProjectionBuilderBase<DummyReadModel>
    {
        public DummyProjection(IProjectionRepository repository) : base(repository)
        {
        }

        public DummyProjection ExposedInitialState(Func<DummyReadModel> action)
        {
            InitialState(action);
            return this;
        }

        public DummyProjection ExposedWhen<TEntity>(Action<TEntity, DummyReadModel> action)
        {
            When<TEntity>(action);
            return this;
        }
    }
}

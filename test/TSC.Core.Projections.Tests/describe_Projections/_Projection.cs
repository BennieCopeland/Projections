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
using System.Reflection;

namespace TSC.Core.Projections.Tests.describe_Projections
{
    class _Projection : nspec
    {
        void describe_constructing()
        {
            context["when there is no persisted read model"] = () =>
            {
                MockDefinition definition = null;
                IProjection projection = null;
                ProjectionFactory factory = null;
                ReadModel1 readModel = null;

                beforeEach = () =>
                {
                    definition = new MockDefinition((builder) =>
                    {
                        builder
                            .ForModel<ReadModel1>()
                            .InitialState(() => new ReadModel1 { State = "New Read Model" })
                            .When<SomeEvent>((e, s) =>
                            {
                                readModel = s;
                            });
                            
                    });

                    var repository = new RepositoryMock();

                    factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });
                };

                act = () => projection = factory.CreateProjection<ReadModel1>();

                it["has a LastSequenceProcessed value of -1"] = () =>
                {
                    projection.PreviousSequenceNumber.Should().Be(-1);
                };

                it["has a new read model"] = () =>
                {
                    projection.HandleEvent(0, new SomeEvent(), null);
                    readModel.State.Should().Be("New Read Model");
                };

                it["will persist the read model by default"] = () =>
                {
                    projection.CanSaveProjection().Should().BeTrue();
                };

            };

            context["when there is a persisted read model"] = () =>
            {
                MockDefinition definition = null;
                IProjection projection = null;
                ProjectionFactory factory = null;
                ReadModel1 readModel = null;

                beforeEach = () =>
                {
                    definition = new MockDefinition((builder) =>
                    {
                        builder
                            .ForModel<ReadModel1>()
                            .InitialState(() => new ReadModel1 { State = "New Read Model" })
                            .When<SomeEvent>((e, s) =>
                            {
                                readModel = s;
                            });

                    });

                    var repository = new RepositoryMock()
                        .Setup(20, new ReadModel1 { State = "Stored state" });

                    factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });
                };

                act = () => projection = factory.CreateProjection<ReadModel1>();

                it["has a LastSequenceProcessed value of 20"] = () =>
                {
                    projection.PreviousSequenceNumber.Should().Be(20);
                };

                it["has a new read model"] = () =>
                {
                    projection.HandleEvent(21, new SomeEvent(), null);
                    readModel.State.Should().Be("Stored state");
                };

                it["will persist the read model by default"] = () =>
                {
                    projection.CanSaveProjection().Should().BeTrue();
                };
            };

            context["when there is no definition specified"] = () =>
            {
                MockDefinition definition = null;
                RepositoryMock repository = null;
                ProjectionFactory factory = null;
                Action action = null;

                beforeEach = () =>
                {
                    definition = new MockDefinition((builder) =>
                    {
                    });

                    repository = new RepositoryMock();
                };

                act = () => action = () => factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                it["throws a projection not defined exception"] = () =>
                {
                    action.ShouldThrow<ProjectionNotDefinedException>()
                        .WithMessage("Projection [MockDefinition] does not define itself.");
                };
            };
        }

        void describe_HandleEvent()
        {
            context["when a handler is defined for the event"] = () =>
            {
                IProjection projection = null;
                ReadModel1 readModel = null;
                long previousSequenceNumber = long.MinValue;

                beforeEach = () =>
                {
                    var definition = new MockDefinition((builder) =>
                    {
                        builder
                            .ForModel<ReadModel1>()
                            .InitialState(() => new ReadModel1 { State = "New Read Model" })
                            .When<SomeEvent>((e, s) =>
                            {
                                readModel = s;
                                s.State = "New State";
                            });

                    });

                    var repository = new RepositoryMock();

                    var factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                    projection = factory.CreateProjection<ReadModel1>();

                    previousSequenceNumber = projection.PreviousSequenceNumber;
                };

                act = () => projection.HandleEvent(0, new SomeEvent(), null);

                it["the LastSequenceProcessed value increased by one"] = () =>
                {
                    projection.PreviousSequenceNumber.Should().Be(previousSequenceNumber + 1);
                };

                it["the handler will be called"] = () =>
                {
                    readModel.State.Should().Be("New State");
                };

            };

            context["when a handler is not defined for the event"] = () =>
            {
                IProjection projection = null;
                long previousSequenceNumber = long.MinValue;

                beforeEach = () =>
                {
                    var definition = new MockDefinition((builder) =>
                    {
                    builder
                        .ForModel<ReadModel1>()
                        .InitialState(() => new ReadModel1 { State = "New Read Model" })
                        .When<string>((e, s) => s.State = "");
                    });

                    var repository = new RepositoryMock();

                    var factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                    projection = factory.CreateProjection<ReadModel1>();

                    previousSequenceNumber = projection.PreviousSequenceNumber;
                };

                act = () => projection.HandleEvent(0, new SomeEvent(), null);

                it["the LastSequenceProcessed value increased by one"] = () =>
                {
                    projection.PreviousSequenceNumber.Should().Be(previousSequenceNumber + 1);
                };

                // model is not changed

                // seen is incremented


            };

            List<(long sequenceNumber, long currentSequenceNumber)> less = new List<(long, long)>
            {
                (0, 10),
                (19, 20),
                (100, 100),
                (-1, 13)
            };

            foreach(var value in less)
            {
                context[$"when the sequence number {value.sequenceNumber} is less than the next expected sequence number {value.currentSequenceNumber + 1}"] = () =>
                {
                    IProjection projection = null;
                    long initialSequenceNumber = long.MinValue;
                    ReadModel1 readmodel = null;
                    RepositoryMock repository = null;

                    beforeEach = () =>
                    {
                        var definition = new MockDefinition((build) =>
                        {
                            build.
                                ForModel<ReadModel1>()
                                .InitialState(() => new ReadModel1 { State = "Should not be used" })
                                .When<SomeEvent>((e, s) =>
                                {
                                    readmodel = s;
                                });
                        });

                        repository = new RepositoryMock()
                            .Setup(value.currentSequenceNumber, new ReadModel1 { State = "Exisiting read model" });

                        var factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                        projection = factory.CreateProjection<ReadModel1>();

                        initialSequenceNumber = projection.PreviousSequenceNumber;
                    };

                    act = () => projection.HandleEvent(value.sequenceNumber, new SomeEvent(), null);

                    it["will not call any handlers"] = () =>
                    {
                        readmodel.Should().BeNull();
                    };

                    it["will not persist the readmodel"] = () =>
                    {
                        repository.SaveCalled.Should().BeFalse();
                    };

                    it["will not increment the LastSequenceNumber"] = () =>
                    {
                        projection.PreviousSequenceNumber.Should().Be(initialSequenceNumber);
                    };
                };
            }

            List<(long sequenceNumber, long currentSequenceNumber)> expected = new List<(long, long)>
            {
                (0, -1),
                (11, 10),
                (21, 20),
                (101, 100),
                (14, 13)
            };

            foreach (var value in expected)
            {
                context[$"when the sequence number {value.sequenceNumber} is equal to the next expected sequence number {value.currentSequenceNumber + 1}"] = () =>
                {
                    IProjection projection = null;
                    

                    act = () => projection.HandleEvent(value.sequenceNumber, new SomeEvent(), null);

                    context["and a handler is defined for the event"] = () =>
                    {
                        long initialSequenceNumber = long.MinValue;
                        ReadModel1 readmodel = null;
                        RepositoryMock repository = null;

                        beforeEach = () =>
                        {
                            var definition = new MockDefinition((build) =>
                            {
                                build.
                                    ForModel<ReadModel1>()
                                    .InitialState(() => new ReadModel1 { State = "Should not be used" })
                                    .When<SomeEvent>((e, s) =>
                                    {
                                        s.State = "New Value";
                                        readmodel = s;
                                    });
                            });

                            repository = new RepositoryMock()
                                .Setup(value.currentSequenceNumber, new ReadModel1 { State = "Exisiting read model" });

                            var factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                            projection = factory.CreateProjection<ReadModel1>();

                            initialSequenceNumber = projection.PreviousSequenceNumber;
                        };

                        context["and can save"] = () =>
                        {
                            beforeEach = () =>
                            {
                                projection.CanSaveProjection = () => true;
                            };

                            it["will call the handler"] = () =>
                            {
                                readmodel.State.Should().Be("New Value");
                            };

                            it["will update the LastSequenceProcessed"] = () =>
                            {
                                projection.PreviousSequenceNumber.Should().Be(initialSequenceNumber + 1);
                            };

                            it["will persist the read model"] = () =>
                            {
                                repository.SaveCalled.Should().BeTrue();
                                var result = repository.SaveCalledWith;

                                Type type = result.GetType();
                                long savedNum = (long)type.GetProperty("SequenceNumber").GetValue(result);
                                ReadModel1 savedModel = type.GetProperty("ReadModel").GetValue(result) as ReadModel1;

                                savedNum.Should().Be(value.sequenceNumber);
                                savedModel.State.Should().Be("New Value");
                            };
                        };

                        context["and cannot save"] = () =>
                        {
                            beforeEach = () =>
                            {
                                projection.CanSaveProjection = () => false;
                            };

                            it["will call the handler"] = () =>
                            {
                                readmodel.State.Should().Be("New Value");
                            };

                            it["will update the LastSequenceProcessed"] = () =>
                            {
                                projection.PreviousSequenceNumber.Should().Be(initialSequenceNumber + 1);
                            };

                            it["will not persist the read model"] = () =>
                            {
                                repository.SaveCalled.Should().BeFalse();
                            };
                        };
                    };

                    context["and a handler is not defined for the event"] = () =>
                    {
                        long initialSequenceNumber = long.MinValue;
                        ReadModel1 readmodel = null;
                        RepositoryMock repository = null;

                        beforeEach = () =>
                        {
                            var definition = new MockDefinition((build) =>
                            {
                                build.
                                    ForModel<ReadModel1>()
                                    .InitialState(() => new ReadModel1 { State = "Should not be used" })
                                    .When<string>((e, s) =>
                                    {
                                        s.State = "New Value";
                                        readmodel = s;
                                    });
                            });

                            repository = new RepositoryMock()
                                .Setup(value.currentSequenceNumber, new ReadModel1 { State = "Exisiting read model" });

                            var factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                            projection = factory.CreateProjection<ReadModel1>();

                            initialSequenceNumber = projection.PreviousSequenceNumber;
                        };

                        it["will not call any handlers"] = () =>
                        {
                            readmodel.Should().BeNull();
                        };

                        it["will update the LastSequenceProcessed"] = () =>
                        {
                            projection.PreviousSequenceNumber.Should().Be(initialSequenceNumber + 1);
                        };

                        it["will not persist the read model"] = () =>
                        {
                            repository.SaveCalled.Should().BeFalse();
                        };
                    };
                };
            }

            List<(long sequenceNumber, long currentSequenceNumber)> over = new List<(long, long)>
            {
                (10, -1),
                (12, 10),
                (345, 20),
                (143, 100),
                (16, 13)
            };

            foreach (var value in over)
            {

                context["when the sequence number is greater than the next expected sequence number"] = () =>
                {
                    IProjection projection = null;
                    long initialSequenceNumber = long.MinValue;
                    ReadModel1 readmodel = null;
                    RepositoryMock repository = null;
                    Action action = null;

                    beforeEach = () =>
                    {
                        var definition = new MockDefinition((build) =>
                        {
                            build.
                                ForModel<ReadModel1>()
                                .InitialState(() => new ReadModel1 { State = "Should not be used" })
                                .When<SomeEvent>((e, s) =>
                                {
                                    s.State = "New Value";
                                    readmodel = s;
                                });
                        });

                        repository = new RepositoryMock()
                            .Setup(value.currentSequenceNumber, new ReadModel1 { State = "Exisiting read model" });

                        var factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                        projection = factory.CreateProjection<ReadModel1>();

                        initialSequenceNumber = projection.PreviousSequenceNumber;
                    };

                    act = () => action = () => projection.HandleEvent(value.sequenceNumber, new SomeEvent(), null);

                    it["will throw an exception"] = () =>
                    {
                        action.ShouldThrow<ArgumentOutOfRangeException>();
                    };
                };
            }

            context["when the event is null"] = () =>
            {
                IProjection projection = null;
                long initialSequenceNumber = long.MinValue;
                ReadModel1 readmodel = null;
                RepositoryMock repository = null;
                Action action = null;

                beforeEach = () =>
                {
                    var definition = new MockDefinition((build) =>
                    {
                        build.
                            ForModel<ReadModel1>()
                            .InitialState(() => new ReadModel1 { State = "Should not be used" })
                            .When<SomeEvent>((e, s) =>
                            {
                                s.State = "New Value";
                                readmodel = s;
                            });
                    });

                    repository = new RepositoryMock();

                    var factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                    projection = factory.CreateProjection<ReadModel1>();

                    initialSequenceNumber = projection.PreviousSequenceNumber;
                };

                act = () => action = () => projection.HandleEvent(0, null, null);

                it["will throw an exception"] = () =>
                {
                    action.ShouldThrow<ArgumentNullException>();
                };
            };

            context["when the metadata is null"] = () =>
            {
                IProjection projection = null;
                long initialSequenceNumber = long.MinValue;
                RepositoryMock repository = null;
                IDictionary<string, object> metadata = null;

                beforeEach = () =>
                {
                    var definition = new MockDefinition((build) =>
                    {
                        build.
                            ForModel<ReadModel1>()
                            .InitialState(() => new ReadModel1 { State = "Should not be used" })
                            .When<SomeEvent>((e, m, s) =>
                            {
                                metadata = m;
                            });
                    });

                    repository = new RepositoryMock();

                    var factory = new ProjectionFactory(repository, new IProjectionDefinition[] { definition });

                    projection = factory.CreateProjection<ReadModel1>();

                    initialSequenceNumber = projection.PreviousSequenceNumber;
                };

                act = () => projection.HandleEvent(0, new SomeEvent(), null);

                it["will use an empty metadata dictionary"] = () =>
                {
                    metadata.Should().NotBeNull();
                    metadata.Count().Should().Be(0);
                };
            };
        }

        class SomeEvent { }
    }
}

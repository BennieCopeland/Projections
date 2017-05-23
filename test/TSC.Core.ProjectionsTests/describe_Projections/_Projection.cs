using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSpec;
using TSC.Core.Projections;
using TSC.Core.ProjectionsTests.describe_ProjectionFactory;
using TSC.Core.ProjectionsTests.Helpers;

namespace TSC.Core.ProjectionsTests.describe_Projections
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
                        builder.NewDefinition<ReadModel1>(() => new ReadModel1 { State = "New Read Model" })
                            .When<SomeEvent>((e, s) =>
                            {
                                readModel = s;
                            });
                            
                    });
                };

                act = () => projection = factory.CreateProjection<ReadModel1>();

                it["has a LastSequenceProcessed value of -1"] = () => projection.LastSequenceProcessed.Should().Be(-1);

                it["has a new read model"] = () =>
                {
                    projection.HandleEvent(0, new SomeEvent(), null);
                    readModel.State.Should().Be("New Read Model");
                };

            };
        }

        void describe_HandleEvent()
        {
            context["when a handler is defined for the event"] = () =>
            {

            };

            context["when a handler is not defined for the event"] = () =>
            {

            };

            context["and the sequence number is less than the next expected sequence number"] = () =>
            {
                it["will not call any handlers"] = todo;

                it["will not persist the readmodel"] = todo;
            };

            context["and the sequence number is equal to the next expected sequence number"] = () =>
            {
                it["will call the handlers with the initial read model"] = todo;

                it["will persist the callback created read model"] = todo;

                it["will update the value for the last processed sequence"] = todo;
            };

            context["and the sequence number is greater than the next expected sequence number"] = () =>
            {
                it["will throw an exception"] = todo;
            };

            context["and the event is null"] = () =>
            {
                it["will throw an exception"] = todo;
            };

            context["and the metadata is null"] = () =>
            {
                it["will use an empty metadata dictionary"] = todo;
            };
        }

        class SomeEvent { }
    }
}

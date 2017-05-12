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
    class describe_handling_an_event : _ProjectionBuilder
    {
        void given_a_read_model_does_not_exist_in_the_repository()
        {
            Mock<IProjectionRepository> repository = null;
            ConcreteProjectionBuilder projection = null;

            beforeEach = () =>
            {
                repository = new Mock<IProjectionRepository>();
                repository.Setup(repo => repo.Get<DummyReadModel>()).Returns((false, -1, null));
                projection = new ConcreteProjectionBuilder(repository.Object);
            };

            beforeEach = () =>
            {
                projection.ExposedInitialState(() =>
                {
                    return new DummyReadModel { SomeProperty = "Callback State" };
                });
            };

            context["when an event gets handled"] = () =>
            {
                long sequenceNumber = START_OF_EVENT_LOG;
                DummyEvent @event = AN_EVENT;
                IDictionary<string, object> metadata = EMPTY_METADATA_DICTIONARY;

                act = () => projection.HandleEvent(sequenceNumber, @event, metadata);

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
            };
        }

        void given_a_read_model_exists_in_the_repository()
        {
            Mock<IProjectionRepository> repository = null;
            ConcreteProjectionBuilder projection = null;
            DummyReadModel existingState = null;

            beforeEach = () =>
            {
                repository = new Mock<IProjectionRepository>();
                existingState = new DummyReadModel { SomeProperty = "Exisiting model" };
                repository.Setup(repo => repo.Get<DummyReadModel>()).Returns((true, 5, existingState));
                projection = new ConcreteProjectionBuilder(repository.Object);
            };

            context["when an event gets handled"] = () =>
            {
                long sequenceNumber = START_OF_EVENT_LOG;
                DummyEvent @event = AN_EVENT;
                IDictionary<string, object> metadata = EMPTY_METADATA_DICTIONARY;

                act = () => projection.HandleEvent(sequenceNumber, @event, metadata);

                context["and the sequence number is less than the next expected sequence number"] = () =>
                {
                    it["will not call any handlers"] = todo;

                    it["will not persist the readmodel"] = todo;
                };

                context["and the sequence number is equal to the next expected sequence number"] = () =>
                {
                    it["will call the handlers with the existing read model"] = todo;

                    it["will persist the existing read model"] = todo;

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
            };
        }
    }
}
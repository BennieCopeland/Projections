using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public abstract class ProjectionBuilderBase<TReadModel> : IProjection where TReadModel : new()
    {
        private TReadModel _readModel;
        private TReadModel readModel
        {
            get
            {
                return _readModel;
            }
            set
            {
                _readModel = value;
                readModelInitialized = true;
            }
        }

        IDictionary<Type, Action<object, IDictionary<string, object>, TReadModel>> handlers = new Dictionary<Type, Action<object, IDictionary<string, object>, TReadModel>>();

        private bool readModelInitialized = false;

        private Func<TReadModel> initializeReadModel = () => new TReadModel();

        public ProjectionBuilderBase(IProjectionRepository repository)
        {
            if(repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var result = repository.Get<TReadModel>();

            if (result.HasValue)
            {
                LastSequenceProcessed = result.SequenceNumber;
                readModel = result.ReadModel;
            }
        }

        public long LastSequenceProcessed { get; } = -1;
        public object ReadModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void HandleEvent(long sequenceNumber, object @event, IDictionary<string, object> metadata)
        {
            if (!readModelInitialized)
            {
                readModel = initializeReadModel();
            }

            if (handlers.TryGetValue(@event.GetType(), out var handler))
            {
                handler(@event, metadata, readModel);
            }
        }

        public void PersistProjection()
        {
            throw new NotImplementedException();
        }

        protected void InitialState(Func<TReadModel> stateInitializer)
        {
            initializeReadModel = stateInitializer;
        }

        protected void When<TEvent>(Action<TEvent, IDictionary<string, object>, TReadModel> action)
        {
            handlers.Add(typeof(TEvent), (e, m, s) => action((TEvent)e, m, s));
        }

        protected void When<TEvent>(Action<TEvent, TReadModel> action)
        {
            When<TEvent>((e, m, s) =>
            {
                action((TEvent)e, s);
            });
        }
    }
}

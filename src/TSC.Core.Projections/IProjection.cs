using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSC.Core.Projections
{
    public interface IProjection
    {
        long LastSequenceProcessed { get; }

        void HandleEvent(long sequenceNumber, object @event, IDictionary<string, object> metadata);

        void PersistProjection();
    }

    public class ProjectionFactory<TState> : IProjectionDefinition, IInitializeProjections<TState>, IHandleEvents<TState>
    {
        public ProjectionFactory(IProjectionRepository repo)
        {

        }

        public IProjection Create()
        {
            throw new NotImplementedException();
        }

        public IHandleEvents<TState> OnInit(Func<TState> init)
        {
            throw new NotImplementedException();
        }

        // void When<TEvent>(Action<TEvent, IDictionary<string, object>, TState> action)
        //{
        //}

        //protected void When<TEvent>(Action<TEvent, TState> action)
        //{
        //    When<TEvent>((e, m, s) =>
        //    {
        //        action((TEvent)e, s);
        //    });
        //}

        IHandleEvents<TState> IHandleEvents<TState>.When<TEvent>(Action<TEvent, TState> when)
        {
            throw new NotImplementedException();
        }
    }

    class AccountRequestProjection : IProjectionDefinition
    {
        public void GetDefinition(IProjectionDefinitionBuilder builder)
        {
            builder.NewDefinition<SomeProj>()
                .OnInit(() => new SomeProj())
                .When<SomeEvent>((e, s) => s.Val = e.Count);
        }
    }

    public interface IInitializeProjections<T>
    {
        IHandleEvents<T> OnInit(Func<T> init);
    }

    public interface IHandleEvents<T>
    {
        IHandleEvents<T> When<TEvent>(Action<TEvent, T> when);
    }

    class ProjectionFactory
    {
        private IProjectionRepository repo;
        private IProjectionDefinitionCollection definitions;

        public ProjectionFactory(IProjectionRepository repo, IProjectionDefinitionCollection definitions)
        {
            this.repo = repo;
            this.definitions = definitions;
        }

        public IProjection CreateProjection<T>()
        {
            return definitions.CreateProjection<T>()
                .Where(p => p.Projection == typeof(T))
                .FirstOrDefault()
                .Build(repo);
        }

        public IEnumerable<IProjection> CreateProjections()
        {
            return definitions
                .Select(factory => factory.Build(repo));
        }
    }

    class ProjectionDefinition<T> : IProjectionDefinition, IInitializeProjections<T>, IHandleEvents<T>
    {
        public Type Projection { get; } = typeof(T);

        internal Dictionary<Type, Action<object, T>> Whens = new Dictionary<Type, Action<object, T>>();

        public IProjection Build(IProjectionRepository repository)
        {
            throw new NotImplementedException();
        }

        public IHandleEvents<T> OnInit(Func<T> init)
        {
            throw new NotImplementedException();
        }

        public IHandleEvents<T> When<TEvent>(Action<TEvent, T> when)
        {
            throw new NotImplementedException();
        }
    }

    //interface IProjectionDefinition
    //{
    //    Type Projection { get; }
    //    IProjection Build(IProjectionRepository repository);
    //}



    class SomeProj
    {
        public int Val { get; set; }
    }

    class SomeEvent
    {
        public int Count { get; set; }
    }
}

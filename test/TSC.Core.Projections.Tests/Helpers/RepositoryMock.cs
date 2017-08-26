using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TSC.Core.Projections.Tests.Helpers
{
    public class RepositoryMock : IProjectionRepository
    {
        Dictionary<Type, object> items = new Dictionary<Type, object>();

        public bool SaveCalled { get; private set; } = false;
        public object SaveCalledWith { get; private set; } = null;

        public RepositoryMock Setup<T>(long sequenceNumber, T readModel)
        {
            Type projectionStateTemplate = typeof(ProjectionFactory).GetTypeInfo().Assembly.GetType("TSC.Core.Projections.Internal.ProjectionState`1");
            Type[] typeArgs = { typeof(T) };
            Type constructed = projectionStateTemplate.MakeGenericType(typeArgs);
            PropertyInfo sequenceNumberProp = constructed.GetProperty("SequenceNumber");
            PropertyInfo readModelProp = constructed.GetProperty("ReadModel");

            object createdProjectionState = Activator.CreateInstance(constructed, sequenceNumber, readModel);
            //sequenceNumberProp.SetValue(createdProjectionState, sequenceNumber);
            //readModelProp.SetValue(createdProjectionState, readModel);

            items.Add(constructed, createdProjectionState);

            return this;
        }

        public (bool HasValue, T State) Get<T>()
        {
            if(items.ContainsKey(typeof(T)))
            {
                return (true, (T)items[typeof(T)]);
            }
            else
            {
                return (false, default(T));
            }
        }

        public void Save<T>(T state)
        {
            SaveCalled = true;
            SaveCalledWith = state;
        }
    }
}

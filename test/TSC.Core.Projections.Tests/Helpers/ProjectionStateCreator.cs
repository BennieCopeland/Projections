namespace TSC.Core.Projections.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal class ProjectionStateCreator
    {
        public static object CreateRepositoryResult<T>(long sequenceNumber, T readModel)
        {
            Type projectionStateTemplate = typeof(ProjectionFactory).GetTypeInfo().Assembly.GetType("TSC.Core.Projections.Internal.ProjectionState`1");
            Type[] typeArgs = { typeof(T) };
            Type constructed = projectionStateTemplate.MakeGenericType(typeArgs);
            PropertyInfo sequenceNumberProp = constructed.GetProperty("SequenceNumber");
            PropertyInfo readModelProp = constructed.GetProperty("ReadModel");

            object createdProjectionState = Activator.CreateInstance(constructed, sequenceNumber, readModel);
            //sequenceNumberProp.SetValue(createdProjectionState, sequenceNumber);
            //readModelProp.SetValue(createdProjectionState, readModel);

            return createdProjectionState;
        }

        public static Expression<Func<IProjectionRepository, (bool, T)>> Get<T>(T result)
        {
            Type[] typeArgs = { result.GetType() };

            Expression<Func<IProjectionRepository, (bool, T)>> lambda =
                Expression.Lambda<Func<IProjectionRepository, (bool, T)>>(
                    Expression.Call(Expression.Parameter(typeof(IProjectionRepository), "repository"), "Get", typeArgs)
                , Expression.Parameter(typeof(IProjectionRepository), "repository"));


            return lambda;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections.Internal
{
    internal class ProjectionBuilderCollection : IEnumerable<ProjectionBuilder>, IEnumerable
    {
        private readonly Dictionary<Type, ProjectionBuilder> builders = new Dictionary<Type, ProjectionBuilder>();

        public ProjectionBuilderCollection(IEnumerable<IProjectionDefinition> definitions)
        {
            foreach(var def in definitions)
            {
                var builder = new ProjectionBuilder(def);
                builders.Add(builder.Type, builder);
            }
        }

        public IEnumerator<ProjectionBuilder> GetEnumerator()
        {
            foreach(var entry in builders)
            {
                yield return entry.Value;
            }
        }

        public bool TryGetValue(Type key, out ProjectionBuilder value)
        {
            return builders.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

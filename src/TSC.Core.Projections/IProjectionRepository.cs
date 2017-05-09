using System;
using System.Collections.Generic;
using System.Text;

namespace TSC.Core.Projections
{
    public interface IProjectionRepository
    {
        (bool HasValue, long SequenceNumber, T ReadModel) Get<T>();
        void Save<T>(long sequenceNumber, T readModel);
    }
}

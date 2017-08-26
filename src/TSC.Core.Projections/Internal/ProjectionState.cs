namespace TSC.Core.Projections.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class ProjectionState<TReadModel>
    {
        public long SequenceNumber { get; set; }

        public TReadModel ReadModel { get; set; }

        public ProjectionState(long sequenceNumber, TReadModel readModel)
        {
            this.SequenceNumber = sequenceNumber;
            this.ReadModel = readModel;
        }
    }
}

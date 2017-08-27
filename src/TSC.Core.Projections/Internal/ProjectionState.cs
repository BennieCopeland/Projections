// Copyright (c) CSRA. All Rights Reserved.

namespace TSC.Core.Projections.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A wrapper class around a read model and it's last processed sequence number.
    /// </summary>
    /// <typeparam name="TReadModel">The type of read model that is wrapped.</typeparam>
    internal class ProjectionState<TReadModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionState{TReadModel}"/> class.
        /// </summary>
        /// <param name="sequenceNumber">The event stream sequence number of this read model.</param>
        /// <param name="readModel">The read model to persist.</param>
        public ProjectionState(long sequenceNumber, TReadModel readModel)
        {
            this.SequenceNumber = sequenceNumber;
            this.ReadModel = readModel;
        }

        /// <summary>
        /// Gets or sets the current sequence number position for this read model.
        /// </summary>
        public long SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the read model to be persisted.
        /// </summary>
        public TReadModel ReadModel { get; set; }
    }
}

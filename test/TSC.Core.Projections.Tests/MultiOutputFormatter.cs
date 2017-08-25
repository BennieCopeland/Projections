using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace TSC.Core.Projections.Tests
{
    class MultiOutputFormatter : IFormatter, ILiveFormatter
    {
        private readonly List<IFormatter> formatters;
        private readonly List<ILiveFormatter> liveFormatters;

        public MultiOutputFormatter(IFormatter[] formatters, ILiveFormatter[] liveFormatters)
        {
            this.formatters = new List<IFormatter>(formatters);
            this.liveFormatters = new List<ILiveFormatter>(liveFormatters);
        }

        public IDictionary<string, string> Options { get; set; }

        public void Write(ContextCollection contexts)
        {
            foreach (var formatter in formatters)
            {
                formatter.Write(contexts);
            }
        }

        public void Write(Context context)
        {
            foreach (var liveFormatter in liveFormatters)
            {
                liveFormatter.Write(context);
            }
        }

        public void Write(ExampleBase example, int level)
        {
            foreach (var liveFormatter in liveFormatters)
            {
                liveFormatter.Write(example, level);
            }
        }
    }
}

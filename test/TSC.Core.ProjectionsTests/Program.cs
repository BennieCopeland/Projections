using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace TSC.Core.ProjectionsTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var types = Assembly.GetEntryAssembly().GetTypes();
            var finder = new SpecFinder(types, "");
            var tagsFilter = new Tags().Parse("");
            var builder = new ContextBuilder(finder, tagsFilter, new DefaultConventions());

            //create the nspec runner with a
            //live formatter so we get console output
            XUnitFormatter xunit = new XUnitFormatter();
            ConsoleFormatter console = new ConsoleFormatter();
            xunit.Options.Add("file", "output.xml");

            IFormatter[] formatters =
            {
                xunit,
                console
            };

            ILiveFormatter[] liveFormatters =
            {
                console
            };

            var runner = new ContextRunner(tagsFilter, new MultiOutputFormatter(formatters, liveFormatters), false);
            var contextCollection = builder.Contexts().Build();

            var results = runner.Run(contextCollection);

            if (results.Failures().Count() > 0)
            {
                Environment.Exit(1);
            }
        }
    }
}

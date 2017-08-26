using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TSC.Core.Projections.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = GetFormatterOptions(args);
            var types = Assembly.GetEntryAssembly().GetTypes();
            var finder = new SpecFinder(types, "");
            var tagsFilter = new Tags().Parse("");
            var builder = new ContextBuilder(finder, tagsFilter, new DefaultConventions());

            //create the nspec runner with a
            //live formatter so we get console output
            XUnitFormatter xunit = new XUnitFormatter();
            xunit.Options.Add("file", options["file"]);

            IFormatter[] formatters =
            {
                xunit
            };

            ILiveFormatter[] liveFormatters =
            {
                new ConsoleFormatter()
            };

            var runner = new ContextRunner(tagsFilter, new MultiOutputFormatter(formatters, liveFormatters), false);
            var contextCollection = builder.Contexts().Build();

            //var ri = new RunnerInvocation(Assembly.GetEntryAssembly().Location, "", false);
            var results = runner.Run(contextCollection);

            if (results.Failures().Count() > 0)
            {
                Environment.Exit(1);
            }
        }

        static IDictionary<string, string> GetFormatterOptions(string[] args)
        {
            var formatterOptions = args.Where(s => s.StartsWith("--formatterOptions:"));
            return formatterOptions.Select(s =>
            {
                var opt = s.Substring("--formatterOptions:".Length);
                var parts = opt.Split('=');
                if (parts.Length == 2)
                    return new KeyValuePair<string, string>(parts[0], parts[1]);
                else
                    return new KeyValuePair<string, string>(parts[0], parts[0]);
            }).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}

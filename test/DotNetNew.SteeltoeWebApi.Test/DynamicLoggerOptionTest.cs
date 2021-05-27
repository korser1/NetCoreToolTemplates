using System.Collections.Generic;
using FluentAssertions;
using Steeltoe.DotNetNew.Test.Utilities.Assertions;
using Xunit.Abstractions;

namespace Steeltoe.DotNetNew.SteeltoeWebApi.Test
{
    public class DynamicLoggerOptionTest : OptionTest
    {
        public DynamicLoggerOptionTest(ITestOutputHelper logger) : base("dynamic-logger", logger)
        {
        }

        protected override void AssertHelp(string help)
        {
            base.AssertHelp(help);
            help.Should().ContainSnippet("--dynamic-logger  Add a dynamic logger.");
        }

        protected override void AssertCsproj(Steeltoe steeltoe, Framework framework,
            Dictionary<string, string> properties, string[] packageRefs)
        {
            base.AssertCsproj(steeltoe, framework, properties, packageRefs);
            packageRefs.Should().Contain("Steeltoe.Extensions.Logging.DynamicLogger");
        }

        protected override void AssertProgramCs(Steeltoe steeltoe, Framework framework, string source)
        {
            base.AssertProgramCs(steeltoe, framework, source);
            source.Should().ContainSnippet("using Steeltoe.Extensions.Logging;");
            switch (framework)
            {
                case Framework.NetCoreApp21:
                    source.Should()
                        .ContainSnippet(
                            "loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection(\"Logging\"));");
                    source.Should().ContainSnippet(@"
.ConfigureLogging((hostingContext, loggingBuilder) =>
{
    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection(""Logging""));
    loggingBuilder.AddDynamicConsole();
 })
 ");
                    break;
                default:
                    source.Should()
                        .ContainSnippet(".ConfigureLogging((context, builder) => builder.AddDynamicConsole())");
                    break;
            }
        }
    }
}

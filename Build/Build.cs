using System;
using System.Collections.Immutable;
using System.Linq;
using Nuke.Common;
using Remotion.BuildScript;
using Remotion.BuildScript.Test;
using Remotion.BuildScript.Test.Dimensions;
using static Remotion.BuildScript.Test.Dimensions.Configurations;
using static Remotion.BuildScript.Test.Dimensions.ExecutionRuntimes;
using static Remotion.BuildScript.Test.Dimensions.Platforms;
using static Remotion.BuildScript.Test.Dimensions.TargetFrameworks;

// ReSharper disable RedundantTypeArgumentsOfMethod

class Build : RemotionBuild
{
    public static int Main () => Execute<Build>();

    public override void ConfigureProjects (ProjectsBuilder projects)
    {
        var normalTestConfiguration = new TestConfiguration (
                DefaultTestExecutionRuntimeFactory.Instance,
                TestMatrices.Single(),
                ImmutableArray<ITestExecutionWrapper>.Empty);

        projects.AddReleaseProject ("Core");
        projects.AddUnitTestProject ("UnitTests", normalTestConfiguration);
        projects.AddReleaseProject ("Archive.Zip");
        projects.AddUnitTestProject ("Archive.Zip.UnitTests", normalTestConfiguration);
    }

    public override void ConfigureSupportedTestDimensions (SupportedTestDimensionsBuilder supportedTestDimensions)
    {
        supportedTestDimensions.AddSupportedDimension<ExecutionRuntimes> (
                LocalMachine, EnforcedLocalMachine,
                Docker_Win_NET462, Docker_Win_NET472, Docker_Win_NET48,
                Docker_Win_NET8_0);
        supportedTestDimensions.AddSupportedDimension<TargetFrameworks> (
                NET462, NET472, NET48,
                NET8_0);
        supportedTestDimensions.AddSupportedDimension<Configurations> (Debug, Release);
        supportedTestDimensions.AddSupportedDimension<Platforms> (x64, x86);
    }

    public override void ConfigureTestMatrix (TestMatricesBuilder builder)
    {
        builder.AddTestMatrix (
                "NormalTestMatrix",
                new TestDimension[,]
                {
                        { Docker_Win_NET48, NET48, Debug, x86 },
                        { Docker_Win_NET48, NET48, Release, x86 },
                        { Docker_Win_NET48, NET48, Debug, x64 },
                        { Docker_Win_NET48, NET48, Release, x64 },
                        { Docker_Win_NET8_0, NET8_0, Debug, x64 },
                        { Docker_Win_NET8_0, NET8_0, Release, x64 },

                        // Local
                        { LocalMachine, NET48, Debug, x86 },
                        { LocalMachine, NET48, Release, x86 },
                        { LocalMachine, NET48, Debug, x64 },
                        { LocalMachine, NET48, Release, x64 },
                        { LocalMachine, NET8_0, Debug, x64 },
                        { LocalMachine, NET8_0, Release, x64 },

                        // Exercise compatibility between installed .NET version, target framework and SQL Server
                        { Docker_Win_NET48, NET472, Release, x64 },
                        { Docker_Win_NET48, NET462, Release, x64 },
                        { Docker_Win_NET472, NET472, Release, x64 },
                        { Docker_Win_NET472, NET462, Release, x64 },
                        { Docker_Win_NET462, NET462, Release, x64 }
                });
    }
}
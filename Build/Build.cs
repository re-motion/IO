using System;
using Remotion.BuildScript;
using Remotion.BuildScript.Components;

class Build : BaseBuild
{
    public static int Main () => Execute<Build>(x => ((IRunTests)x).RunTests);
}

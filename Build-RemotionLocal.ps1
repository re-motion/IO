set-alias nant "C:\Program Files\NAnt\bin.net-2.0\nant.exe";

nant "-f:Remotion.build" "-D:build.temp.root=\Temp\RemotionLocal" "-t:net-3.5" "-l:Build.log" "-nologo" `
    "-D:build.update.assembly-info=false" `
    clean cleantemp `
    resources debug;

if ($LastExitCode -ne 0) 
{ 
  [System.Console]::ReadKey($false);
  throw "Build Remotion has failed."; 
}

nant "-f:Remotion.build" "-D:build.temp.root=\Temp\RemotionLocal" "-t:net-3.5" "-nologo" `
    cleantemp `
    sourcezip zip `
    securityManager-sourcezip securityManager-zip;

[System.Console]::ReadKey($false);

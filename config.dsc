// BuildXL (bxl.exe with MsBuild resolver) configuration file
config({
    resolvers: [
        {
            kind: "MsBuild",
            moduleName: "AspNetCore",
            root: d`.`,
            /*msBuildSearchLocations: Environment.hasVariable("MSBUILD_EXE_LOCATION") ?
                [Environment.getDirectoryValue("MSBUILD_EXE_LOCATION")] :
                [d`G:\src\corpus\repo\MSBuild\artifacts\bin\bootstrap\net472\MSBuild\Current\Bin`],*/

            fileNameEntryPoints: [r`traversal.proj`],

            useLegacyProjectIsolation: true,
            allowProjectsToNotSpecifyTargetProtocol: true,
            keepProjectGraphFile: true,

            globalProperties: Map.empty<string, string>()
                .add("XunitConsole472Path", "$(NugetPackageRoot)xunit.runner.console\\2.4.1\\build\\..\\tools\\net472\\xunit.console.exe")
                .add("XunitConsole472PathX86", "$(NugetPackageRoot)xunit.runner.console\\2.4.1\\build\\..\\tools\\net472\\xunit.console.x86.exe"),

            // environment: Map.empty<string, string>()
            //                 .add("RepoRoot", "G:\\src\\bxl.asptnetcore")
            //                 .add("BuildManaged", "true")
            //                 .add("Restore", "False")
            //                 .add("Build", "True")
            //                 .add("Pack", "False")
            //                 .add("Test", "False")
            //                 .add("Sign", "False")
            //                 .add("TargetArchitecture", "x64")
            //                 .add("TargetOsName", "win")
            //                 .add("Configuration", "Release")
        },
    ],
    disableDefaultSourceResolver: true,
});
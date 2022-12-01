using AdventOfCSharp;
using AdventOfCSharp.Generation;

namespace AdventOfCode;

// This class contains methods that setup framework features that require some configuration
// Usage of .config files is avoided for a more concrete in-app API that users will find easier to use
public static class FrameworkSetup
{
    // This sample method setups all configurable features according to your intended usage
    public static void SetupAll()
    {
        SetCustomProblemFilePath();
        SetupSolutionTemplateGeneration();
    }

    private static void SetupSolutionTemplateGeneration()
    {
        // This enables the feature of automatically generating the template solution class file for the requested problem date
        SolutionTemplateGeneration.EnabledGeneration = true;

        // Here you enter the base directory of the solution (usually the one that contains the .sln file)
        // The below examples will result in using the 'C:\Projects\AdventOfCode\AdventOfCode\Problems' directory
        SolutionTemplateGeneration.BaseDirectory = @"C:\development\adventofcode\2022";
        SolutionTemplateGeneration.BaseNamespace = @"AdventOfCode.Problems";
    }

    private static void SetCustomProblemFilePath()
    {
        // Additionally, you may optionally set your problem file (input and output) directory
        // to a custom path instead of the default, which is the base directory of your project
        // NOTE: If you're using a custom build path, it is almost always recommended to set the
        //       custom base directory to whatever best fits you.
        // Ideally, you may opt into hiding the directory string by containing it in another file
        // that will be added to .gitignore.
        ProblemFiles.CustomBaseDirectory = null;
    }
}

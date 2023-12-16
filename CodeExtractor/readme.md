# CodeExtractor Project

## Overview
`CodeExtractor` is a tool designed to extract class definitions and project information from .NET projects. It processes specified directories (Entities, Helpers, Models, Services, Strategies, and Utilities, including their subfolders) and generates output files containing the class definitions. Additionally, it creates a markdown file (`ProjectInfo.md`) with details like project type, target framework, and used NuGet packages.

## System Requirements
- .NET 8 runtime or SDK
- Microsoft.CodeAnalysis.CSharp for parsing

## Usage
To use `CodeExtractor`, follow these steps:

1. **Compilation**: Compile the `CodeExtractor` solution in Visual Studio or using the `dotnet build` command.

2. **Execution**: Run the compiled executable with the path to the .NET project as the first command-line argument. Optionally, you can specify a second argument for the output directory.
   
   Example without specifying output directory (creates `codeoutput` in the current working directory): CodeExtractor.exe "C:\path\to\your\project"

   
Example with specifying output directory: CodeExtractor.exe "C:\path\to\your\project" "C:\path\to\your\output"

3. **Output**: The tool will generate text files in the specified output directory (or in `codeoutput` within the current working directory if no output path is provided) for each processed folder. It also creates a `ProjectInfo.md` file with project details.

## Features
- Extracts classes from specified folders and their subfolders.
- Generates a markdown file with project details.
- Identifies project type (e.g., Class Library, ASP.NET Core Application) and lists NuGet packages.
- Supports an optional output directory as a second command-line argument.

## Contributing
Contributions to the `CodeExtractor` project are welcome! Please submit pull requests or issues for enhancements, bug fixes, or feature requests.

## License
[Specify your license here or state that it's open-source.]

---

For more information or if you encounter issues, please contact the project maintainer.

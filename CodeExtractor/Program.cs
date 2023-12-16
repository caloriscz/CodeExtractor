using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a source folder path.");
                return;
            }

            string folderPath = args[0];

            string outputFolderPath;
            if (args.Length > 1)
            {
                outputFolderPath = args[1];
            }
            else
            {
                outputFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "codeoutput");
            }

            string projectName = new DirectoryInfo(folderPath).Name;
            outputFolderPath = Path.Combine(outputFolderPath, projectName);

            Directory.CreateDirectory(outputFolderPath);

            try
            {
                ProcessDirectory(folderPath, outputFolderPath);
                string projectInfoMarkdown = GenerateProjectInfoMarkdown(folderPath);

                // Ensure the full path for ProjectInfo.md exists before writing
                string projectInfoPath = Path.Combine(outputFolderPath, "ProjectInfo.md");
                Directory.CreateDirectory(Path.GetDirectoryName(projectInfoPath));

                File.WriteAllText(projectInfoPath, projectInfoMarkdown);
                Console.WriteLine($"Entities and project information extracted to: {outputFolderPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        static void ProcessDirectory(string folderPath, string outputFolderPath)
        {
            string[] targetFolders = { "Entities", "Helpers", "Models", "Services", "Strategies", "Utilities" };

            foreach (var targetFolder in targetFolders)
            {
                string searchFolder = Path.Combine(folderPath, targetFolder);
                if (Directory.Exists(searchFolder))
                {
                    string combinedEntities = ExtractEntities(searchFolder);
                    File.WriteAllText(Path.Combine(outputFolderPath, $"{targetFolder}.txt"), combinedEntities);
                }
            }
        }

        static string ExtractEntities(string folderPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] fileEntries = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

            foreach (string filePath in fileEntries)
            {
                string fileContent = File.ReadAllText(filePath);
                SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
                var root = (CompilationUnitSyntax)tree.GetRoot();

                foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    var classWithoutComments = RemoveCommentsFromClass(classDeclaration);
                    stringBuilder.AppendLine(classWithoutComments.NormalizeWhitespace().ToFullString());
                }
            }

            return stringBuilder.ToString();
        }

        static ClassDeclarationSyntax RemoveCommentsFromClass(ClassDeclarationSyntax classDeclaration)
        {
            var rewrittenClass = classDeclaration.ReplaceTrivia(
                classDeclaration.DescendantTrivia().Where(trivia =>
                    trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                    trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)),
                (original, rewritten) => SyntaxFactory.SyntaxTrivia(SyntaxKind.DisabledTextTrivia, string.Empty));

            return rewrittenClass;
        }


        static string GenerateProjectInfoMarkdown(string projectPath)
        {
            var sb = new StringBuilder();

            string csprojPath = Directory.GetFiles(projectPath, "*.csproj").FirstOrDefault();
            if (csprojPath == null)
            {
                return "No .csproj file found.";
            }

            XDocument doc = XDocument.Load(csprojPath);
            var projectNode = doc.Root;

            var projectName = Path.GetFileNameWithoutExtension(csprojPath);
            sb.AppendLine($"# Project Information for '{projectName}'\n");

            // Checking for ASP.NET Core specific references
            bool isAspNetCoreApp = projectNode.Descendants().Any(n =>
                (n.Name.LocalName == "FrameworkReference" && n.Attribute("Include") != null && n.Attribute("Include").Value == "Microsoft.AspNetCore.App") ||
                (n.Name.LocalName == "PackageReference" && n.Attribute("Include") != null && n.Attribute("Include").Value.StartsWith("Microsoft.AspNetCore.")));

            string projectType = isAspNetCoreApp ? "ASP.NET Core Application" : "Class Library";

            var targetFramework = projectNode.Descendants()
                .FirstOrDefault(n => n.Name.LocalName == "TargetFramework" || n.Name.LocalName == "TargetFrameworks")?.Value;

            sb.AppendLine($"## Project Type: {projectType}");
            sb.AppendLine($"## Target Framework: {targetFramework}\n");

            var packageReferences = projectNode.Descendants().Where(d => d.Name.LocalName == "PackageReference");
            sb.AppendLine("## NuGet Packages\n");
            foreach (var package in packageReferences)
            {
                string packageName = package.Attribute("Include")?.Value;
                string packageVersion = package.Attribute("Version")?.Value;
                sb.AppendLine($"- {packageName} ({packageVersion})");
            }

            return sb.ToString();
        }

    }
}

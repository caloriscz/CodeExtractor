using System;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeExtractor
{
    public class CodeExtractorService
    {
        public CodeExtractorService()
        {
            // Constructor - you can also inject services if needed
        }

        public string ExtractEntities(string folderPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] fileEntries = Directory.GetFiles(folderPath, "*.cs");

            foreach (string filePath in fileEntries)
            {
                string fileContent = File.ReadAllText(filePath);
                SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
                var root = (CompilationUnitSyntax)tree.GetRoot();

                foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    stringBuilder.AppendLine(classDeclaration.NormalizeWhitespace().ToFullString());
                }
            }

            return stringBuilder.ToString();
        }
    }
}


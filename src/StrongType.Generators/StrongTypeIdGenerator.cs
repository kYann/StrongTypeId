using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace StrongType.Generators
{
    [Generator]
    public class StrongTypeIdGenerator : ISourceGenerator
    {
        Template template;

        public void Initialize(GeneratorInitializationContext context) 
        {
            var file = "StrongTypeId.sbntxt";
            template = Template.Parse(EmbeddedResource.GetContent(file), file);

            context.RegisterForSyntaxNotifications(() => new StrongTypeIdReceiver());
        }

        private string GenerateStrongTypeId(string @namespace, string className)
        {
            var model = new
            {
                Namespace = @namespace,
                ClassName = className,
            };

            // apply the template
            var output = template.Render(model, member => member.Name);

            return output;
        }

        public void Execute(GeneratorExecutionContext context)
        {
			if (context.SyntaxReceiver is not StrongTypeIdReceiver receiver)
				return;

			foreach (var rds in receiver.RecordDeclarations)
			{
                var model = context.Compilation.GetSemanticModel(rds.SyntaxTree);

				if (model.GetDeclaredSymbol(rds) is not INamedTypeSymbol classSymbol)
					continue;

				var ns = classSymbol.ContainingNamespace.ToDisplayString();
                var output = GenerateStrongTypeId(ns, classSymbol.Name);

                // add the file
                context.AddSource($"{classSymbol.Name}.generated.cs", SourceText.From(output, Encoding.UTF8));
            }
        }

        private class StrongTypeIdReceiver : ISyntaxReceiver
        {
			public StrongTypeIdReceiver()
			{
                RecordDeclarations = new();
			}

			public List<RecordDeclarationSyntax> RecordDeclarations { get; private set; }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is RecordDeclarationSyntax rds &&
                    rds.BaseList is not null)
                {
                    var doesInherhitFromStrongTypeId = rds.BaseList.Types.Any(_ => _.ToString().Contains($"StrongTypeId<"));
                    if (doesInherhitFromStrongTypeId)
                        this.RecordDeclarations.Add(rds);
                }
            }
        }
    }
}

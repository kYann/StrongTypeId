using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslyn.CodeDom;
using Roslyn.CodeDom.References;
using Scriban;
using StrongType.Generators;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StrongType.Tests
{
	public class GeneratorTests
	{
        [Fact]
        public void StrongTypeIdGeneratorTest()
        {
			string userSource = @"
            using StrongType;

            namespace MyCode.Tests
            {
                public partial record ProductId(int Value) : StrongTypeId<int>(Value);
            }
            ";
			var comp = CreateCompilation(userSource);
            var newComp = RunGenerators(comp, out var generatorDiags, new StrongTypeIdGenerator());

            Assert.Empty(generatorDiags);
            Assert.Empty(newComp.GetDiagnostics());

            var nl = Environment.NewLine;
            var expectedGeneratedCode = $"namespace MyCode.Tests{nl}{{{nl}    public partial record ProductId{nl}    {{{nl}        public override string ToString() => base.ToString();{nl}    }}{nl}}}";
            var generatedCode = newComp.SyntaxTrees.Last().ToString();
            Assert.Equal(expectedGeneratedCode, generatedCode);
        }


        private static Compilation CreateCompilation(string source) =>
            CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp9)) },
                NetCoreApp31.All.Concat(new[]{
                    MetadataReference.CreateFromFile(typeof(Template).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(StrongTypeId<>).GetTypeInfo().Assembly.Location)
                }),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        private static GeneratorDriver CreateDriver(Compilation c, params ISourceGenerator[] generators)
            => CSharpGeneratorDriver.Create(ImmutableArray.Create(generators), parseOptions:(CSharpParseOptions)c.SyntaxTrees.First().Options);

        private static Compilation RunGenerators(Compilation c, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators)
        {
            CreateDriver(c, generators).RunGeneratorsAndUpdateCompilation(c, out var d, out diagnostics);
            return d;
        }
    }
}

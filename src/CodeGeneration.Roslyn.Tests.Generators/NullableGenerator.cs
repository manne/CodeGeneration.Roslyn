using System;
namespace CodeGeneration.Roslyn.Tests.Generators
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Validation;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    public class NullableGenerator : ICodeGenerator
    {
        public NullableGenerator(AttributeData attributeData)
        {
            Requires.NotNull(attributeData, nameof(attributeData));
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var results = List<MemberDeclarationSyntax>();
            MemberDeclarationSyntax copy = null;
            if (context.ProcessingNode is ClassDeclarationSyntax applyToClass)
            {
                var nullableContext = context.SemanticModel.GetNullableContext(applyToClass.SpanStart);
                var isNullableEnabled = nullableContext == NullableContext.Enabled;
                var method = MethodDeclaration(
                        MakeNullableIfEnabled(
                            PredefinedType(
                                Token(SyntaxKind.StringKeyword)), isNullableEnabled),
                        Identifier("DoSomething"))
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(
                        ParameterList(
                            SingletonSeparatedList(
                                Parameter(
                                        Identifier("value"))
                                    .WithType(
                                        MakeNullableIfEnabled(
                                            PredefinedType(
                                                Token(SyntaxKind.StringKeyword)), isNullableEnabled)))))
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ReturnStatement(
                                    IdentifierName("value")))));
                if (isNullableEnabled)
                {
                    method = method.WithLeadingTrivia(TriviaList(Trivia(
                            NullableDirectiveTrivia(
                                Token(SyntaxKind.EnableKeyword),
                                true))))
                        .WithTrailingTrivia(TriviaList(Trivia(
                            NullableDirectiveTrivia(
                                Token(SyntaxKind.RestoreKeyword),
                                true))));
                }

                copy = ClassDeclaration(applyToClass.Identifier)
                    .WithModifiers(applyToClass.Modifiers)
                    .AddMembers(method);
            }

            if (copy != null)
            {
                results = results.Add(copy);
            }

            return Task.FromResult(results);
        }

        public static TypeSyntax MakeNullableIfEnabled(TypeSyntax typeSyntax, bool isEnabled)
        {
            return isEnabled ? NullableType(typeSyntax) : typeSyntax;
        }
    }
}

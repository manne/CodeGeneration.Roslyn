﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the MS-PL license. See LICENSE.txt file in the project root for full license information.

namespace CodeGeneration.Roslyn.Tests.Generators
{
    using System;
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

        public static TypeSyntax MakeNullableIfEnabled(TypeSyntax typeSyntax, bool isEnabled)
        {
            return isEnabled ? NullableType(typeSyntax) : typeSyntax;
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var results = List<MemberDeclarationSyntax>();
            MemberDeclarationSyntax copy = null;
            if (context.ProcessingNode is ClassDeclarationSyntax applyToClass)
            {
                var nullableContext = context.SemanticModel.GetNullableContext(applyToClass.SpanStart);
                var isNullableEnabled = nullableContext == NullableContext.Enabled;
                var methodDoSomething = MethodDeclaration(
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
                var methodDoSomethingStrict = MethodDeclaration(
                        MakeNullableIfEnabled(
                            PredefinedType(
                                Token(SyntaxKind.StringKeyword)), isNullableEnabled),
                        Identifier("DoSomethingStrict"))
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(
                        ParameterList(
                            SingletonSeparatedList(
                                Parameter(
                                        Identifier("value"))
                                    .WithType(
                                        PredefinedType(
                                                Token(SyntaxKind.StringKeyword))))))
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ReturnStatement(
                                    IdentifierName("value")))));

                var leading = isNullableEnabled ? TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))) : SyntaxTriviaList.Empty;
                var trailing = isNullableEnabled ? TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true))) : SyntaxTriviaList.Empty;

                copy = ClassDeclaration(applyToClass.Identifier)
                    .WithModifiers(applyToClass.Modifiers)
                    .WithLeadingTrivia(leading)
                    .AddMembers(methodDoSomething, methodDoSomethingStrict)
                    .WithTrailingTrivia(trailing);
            }

            if (copy != null)
            {
                results = results.Add(copy);
            }

            return Task.FromResult(results);
        }
    }
}
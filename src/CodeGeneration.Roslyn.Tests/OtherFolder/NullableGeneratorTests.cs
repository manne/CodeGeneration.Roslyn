// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the MS-PL license. See LICENSE.txt file in the project root for full license information.

namespace CodeGeneration.Roslyn.Tests.OtherFolder
{
    using System;
    using System.Diagnostics;
    using CodeGeneration.Roslyn.Tests.Generators;
    using Xunit;

    public class NullableGeneratorTests
    {
        [Fact]
        public void TestForWhichNullableIsEnabled()
        {
#nullable enable
            var instance = new NullableEnabled();
            var result = instance.DoSomething("blub");
#pragma warning disable CS8604 // Possible null reference argument. If the pragma is removed, the compiler warns that "result" can be null
            _ = instance.DoSomethingStrict(result);
#pragma warning restore CS8604 // Possible null reference argument.
#nullable restore
        }

        [Fact]
        public void TestForWhichNullableIsDisabled()
        {
#nullable disable
            var instance = new NullableDisabled();
            _ = instance.DoSomething("blub");
#nullable restore
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [CodeGenerationAttribute(typeof(NullableGenerator))]
    [Conditional("CodeGeneration")]
    public class NullableGeneratorAttribute : Attribute
    {
        // nothing to do here
    }

#nullable enable
    [NullableGenerator]
    public partial class NullableEnabled
    {
        // an method string? DoSomething(string?) will be generated
    }
#nullable restore

#nullable disable
    [NullableGenerator]
    public partial class NullableDisabled
    {
        // an method string DoSomething(string) will be generated
    }
#nullable restore
}

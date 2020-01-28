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
#nullable enable
        [Fact]
        public void TestForWhichNullableIsEnabled()
        {
            var instance = new NullableEnabled();
            _ = instance.DoSomething("blub");
        }

#nullable disable
        [Fact]
        public void TestForWhichNullableIsDisabled()
        {
            var instance = new NullableDisabled();
            _ = instance.DoSomething("blub");
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

#nullable disable
    [NullableGenerator]
    public partial class NullableDisabled
    {
        // an method string DoSomething(string) will be generated
    }
}

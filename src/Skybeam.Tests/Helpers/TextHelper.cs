using System;
using System.Collections.Generic;
using DiffPlex.Renderer;
using Microsoft.CodeAnalysis.Text;

//using Xunit.Abstractions;

namespace Skybeam.Tests.Helpers;

public class TextHelper
{
    public static void AssertEquality(string expected, HandlerDescription handler, List<BehaviorDescription> behaviors)
    {
        (SourceText actual, _) = PipelineTextEmitter.CreateSourceText(handler, behaviors);

        string[] expectedLines = LineEndingsHelper.Normalize(expected)
            .Replace("%VERSION%", typeof(PipelineTextEmitter).Assembly.GetName().Version?.ToString())
            .Split(Environment.NewLine);

        bool areEqual = RoslynTestUtils.CompareLines(expectedLines, actual, out string errorMessage);

        string output = errorMessage;

        if (!areEqual)
        {
            output += Environment.NewLine + Environment.NewLine + actual;
        }

        Assert.True(areEqual, output);
    }

    /// <summary>
    /// By DiffPlex
    /// </summary>
    public static string GetUnidiff(
        string expected, 
        string actual)
    {
        string expectedNormalized = LineEndingsHelper.Normalize(expected)
            .Replace("%VERSION%", typeof(PipelineTextEmitter).Assembly.GetName().Version?.ToString());

        if (actual == expectedNormalized) return null;

        string unidiff = UnidiffRenderer.GenerateUnidiff(
            oldText: expectedNormalized, 
            newText: actual,
            contextLines: 3,
            ignoreWhitespace: false
        );
        
        return unidiff;
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.CodeAnalysis.Text;

namespace Skybeam.Tests.Helpers;

static class RoslynTestUtils
{
    public static bool CompareLines(string[] expectedLines, SourceText sourceText, out string message)
    {
        if (expectedLines.Length != sourceText.Lines.Count)
        {
            message = string.Format("Line numbers do not match. Expected: {0} lines, but generated {1}",
                expectedLines.Length, sourceText.Lines.Count);
            return false;
        }
        int index = 0;
        foreach (TextLine textLine in sourceText.Lines)
        {
            string expectedLine = expectedLines[index];
            if (!expectedLine.Equals(textLine.ToString(), StringComparison.Ordinal))
            {
                message = string.Format("Line {0} does not match.{1}Expected Line:{1}{2}{1}Actual Line:{1}{3}",
                    textLine.LineNumber + 1, Environment.NewLine, expectedLine, textLine);
                return false;
            }
            index++;
        }
        message = string.Empty;
        return true;
    }
}
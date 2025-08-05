﻿// ReSharper disable InconsistentNaming

using System;

namespace Skybeam.Tests.Helpers;

public static class LineEndingsHelper
{
    public const string CompiledNewline = @"
";

    public static readonly bool s_consistentNewlines =
        StringComparer.Ordinal.Equals(CompiledNewline, Environment.NewLine);

    public static bool IsNewLineConsistent => s_consistentNewlines;

    public static string Normalize(string expected)
    {
        if (s_consistentNewlines)
            return expected;

        return expected.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
    }
}
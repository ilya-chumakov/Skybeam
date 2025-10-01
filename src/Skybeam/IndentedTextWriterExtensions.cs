using System.CodeDom.Compiler;

namespace Skybeam;

internal static class IndentedTextWriterExtensions
{
    public static void OpenBlock(this IndentedTextWriter writer)
    {
        writer.WriteLine("{");
        writer.Indent++;
    }

    public static void CloseBlock(this IndentedTextWriter writer, bool newLine = true)
    {
        writer.Indent--;
        
        if (newLine) writer.WriteLine("}"); else writer.Write("}");
    }
}
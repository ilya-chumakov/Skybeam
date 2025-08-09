using System.CodeDom.Compiler;

namespace Skybeam;

public static class IndentedTextWriterExtensions
{
    public static void OpenBlock(this IndentedTextWriter writer)
    {
        writer.WriteLine("{");
        writer.Indent++;
    }

    public static void CloseBlock(this IndentedTextWriter writer)
    {
        writer.Indent--;
        writer.WriteLine("}");
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Skybeam;

//todo it's useful, make conditional? how?
//ctx.AddSource("Stats.g.cs", DebugEmitter.CreateStatistics(handlers, behaviors));
public static class DebugEmitter
{
    public static SourceText CreateStatistics(
        IReadOnlyList<HandlerDescription> handlers,
        IReadOnlyList<BehaviorDescription> behaviors)
    {
        var sb = new StringBuilder(256);

        sb.AppendLine(
            $"""
             //Handlers count = {handlers.Count}
             //Behaviors count = {behaviors.Count}
             //
             //Now = {DateTime.Now}
             //
             //Handler list:
             """);
        
        foreach (var handler in handlers)
        {
            sb.AppendLine($"//{handler.Name}");
        }

        sb.AppendLine(
            $"""
             //
             //Behavior list:
             """);

        foreach (var beh in behaviors)
        {
            sb.AppendLine($"//{beh.FullNamePrefix}");
        }

        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }
}

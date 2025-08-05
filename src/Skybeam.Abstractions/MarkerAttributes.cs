namespace Skybeam.Abstractions;

//[AttributeUsage(AttributeTargets.Class, Inherited = false)]
//public class DecorateThisHandler : Attribute
//{
//}

//[AttributeUsage(AttributeTargets.Class, Inherited = false)]
//public class UseThisDecorator : Attribute
//{
//}

//todo visibility to internal
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class RegisterThis : Attribute
{
}
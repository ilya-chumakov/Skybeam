namespace Skybeam.Playground;

public class BarQuery
{
    public string Id { get; private set; }
}

public class BarResponse
{

    public BarResponse(string id)
    {
        Id = id;
    }

    public string Id { get; private set; }
}
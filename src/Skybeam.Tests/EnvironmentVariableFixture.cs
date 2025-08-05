using System;

namespace Skybeam.Tests;

public class EnvironmentVariableFixture : IDisposable
{
    private const string IsTestRunner = "IS_TEST_RUNNER";
    private readonly string _originalValue;

    public EnvironmentVariableFixture()
    {
        _originalValue = Environment.GetEnvironmentVariable(IsTestRunner);

        Environment.SetEnvironmentVariable(IsTestRunner, "true");
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(IsTestRunner, _originalValue);
    }
}
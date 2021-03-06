﻿#if NETCOREAPP3_1
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DiffEngine;
using Xunit;
using Xunit.Abstractions;

public class DiffRunnerTests :
    XunitContextBase
{
    static ResolvedTool tool;
    string file2;
    string file1;
    string command;
    [Fact(Skip = "Explicit")]
    public async Task MaxInstancesToLaunch()
    {
        DiffRunner.MaxInstancesToLaunch(1);
        try
        {
            await Task.Delay(500);
            ProcessCleanup.Refresh();
            var result = await DiffRunner.Launch(file1, "fake.txt");
            await Task.Delay(300);
            Assert.Equal(LaunchResult.StartedNewInstance, result);
            ProcessCleanup.Refresh();
            result = await DiffRunner.Launch(file2, "fake.txt");
            Assert.Equal(LaunchResult.TooManyRunningDiffTools, result);
            ProcessCleanup.Refresh();
            DiffRunner.Kill(file1, "fake.txt");
            DiffRunner.Kill(file2, "fake.txt");
        }
        finally
        {
            DiffRunner.MaxInstancesToLaunch(5);
        }
    }

    async Task Launch()
    {
        string targetFile = "";
        string tempFile = "";

        #region DiffRunnerLaunch

        await DiffRunner.Launch(tempFile, targetFile);

        #endregion
    }

    [Fact(Skip = "Explicit")]
    public async Task Kill()
    {
        await DiffRunner.Launch(file1, file2);
        ProcessCleanup.Refresh();
        #region DiffRunnerKill
        DiffRunner.Kill(file1, file2);
        #endregion
    }

    [Fact]
    public async Task LaunchAndKillDisabled()
    {
        DiffRunner.Disabled = true;
        try
        {
            Assert.False(IsRunning());
            Assert.False(ProcessCleanup.IsRunning(command));
            var result = await DiffRunner.Launch(file1, file2);
            Assert.Equal(LaunchResult.Disabled, result);
            Thread.Sleep(500);
            ProcessCleanup.Refresh();
            Assert.False(IsRunning());
            Assert.False(ProcessCleanup.IsRunning(command));
            DiffRunner.Kill(file1, file2);
            Thread.Sleep(500);
            ProcessCleanup.Refresh();
            Assert.False(IsRunning());
            Assert.False(ProcessCleanup.IsRunning(command));
        }
        finally
        {
            DiffRunner.Disabled = false;
        }
    }

    [Fact]
    public async Task LaunchAndKill()
    {
        Assert.False(IsRunning());
        Assert.False(ProcessCleanup.IsRunning(command));
        var result = await DiffRunner.Launch(file1, file2);
        Assert.Equal(LaunchResult.StartedNewInstance, result);
        Thread.Sleep(500);
        ProcessCleanup.Refresh();
        Assert.True(IsRunning());
        Assert.True(ProcessCleanup.IsRunning(command));
        DiffRunner.Kill(file1, file2);
        Thread.Sleep(500);
        ProcessCleanup.Refresh();
        Assert.False(IsRunning());
        Assert.False(ProcessCleanup.IsRunning(command));
    }

    static bool IsRunning()
    {
        return ProcessCleanup
            .FindAll()
            .Any(x => x.Command.Contains("FakeDiffTool"));
    }

    public DiffRunnerTests(ITestOutputHelper output) :
        base(output)
    {
        file1 = Path.Combine(SourceDirectory, "DiffRunner.file1.txt");
        file2 = Path.Combine(SourceDirectory, "DiffRunner.file2.txt");
        command = tool.BuildCommand(file1, file2);
    }

    static DiffRunnerTests()
    {
        tool = DiffTools.AddTool(
            name: "FakeDiffTool",
            autoRefresh: true,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            arguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
            exePath: FakeDiffTool.Exe,
            binaryExtensions: new[] {"knownBin"})!;
    }
}
#endif
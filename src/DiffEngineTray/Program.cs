using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;

static class Program
{
    static async Task Main()
    {
        Logging.Init();

        var tokenSource = new CancellationTokenSource();
        var cancellation = tokenSource.Token;
        using var mutex = new Mutex(true, "DiffEngine", out var createdNew);
        if (!createdNew)
        {
            Log.Information("Mutex already exists. Exiting.");
            return;
        }

        using var notifyIcon = new NotifyIcon
        {
            Icon = Images.Default,
            Visible = true,
            Text = "DiffEngine"
        };

        await using var tracker = new Tracker(
            active: () => notifyIcon.Icon = Images.Active,
            inactive: () => notifyIcon.Icon = Images.Default);

        using var task = PiperServer.Start(
            payload =>
            {
                (int, DateTime)? process = null;
                if (payload.ProcessId != null &&
                    payload.ProcessStartTime != null )
                {
                    process = (payload.ProcessId.Value, payload.ProcessStartTime.Value);
                }

                tracker.AddMove(
                        payload.Temp,
                        payload.Target,
                        payload.Exe,
                        payload.Arguments,
                        payload.CanKill,
                        process);
            },
            payload => tracker.AddDelete(payload.File),
            cancellation);
        var menu = MenuBuilder.Build(
            () =>
            {
                tokenSource.Cancel();
                mutex!.Dispose();
                Environment.Exit(0);
            },
            tracker);

        notifyIcon.ContextMenuStrip = menu;


        LowLevelKeyboardHook kbh = new LowLevelKeyboardHook();
        kbh.OnKeyPressed += kbh_OnKeyPressed;
        kbh.OnKeyUnpressed += kbh_OnKeyUnpressed;
        kbh.HookKeyboard();


        Application.Run();
        kbh.UnHookKeyboard();
        await task;
    }

    private static void kbh_OnKeyUnpressed(object? sender, Keys e)
    {
        throw new NotImplementedException();
    }

    private static void kbh_OnKeyPressed(object? sender, Keys e)
    {
        throw new NotImplementedException();
    }
}


using System.Runtime.CompilerServices;

namespace BlazorWebApp10.Services
{
    public interface ITestService
    {
        Task Method1(string msg, Action<string[]> callback);
        Task ClearCallback(string msg);
        Task Method2(CallbackHandle callbackHandle);
    }

    public class CallbackHandle : IDisposable
    {
        public string Msg { get; set; }
        public Task Task { get; set; }
        public bool IsCompleted => Task.IsCompleted;
        public TaskCompletionSource TaskCompletionSource { get; set; } = new TaskCompletionSource();
        public Action<string[]> Callback { get; set; }
        public CallbackHandle(string msg, Action<string[]> callback)
        {
            Msg = msg;
            Callback = callback;
            Task = TaskCompletionSource.Task;
        }
        public void Dispose()
        {
            TaskCompletionSource.TrySetResult();
        }
    }
    public class TestService : ITestService
    {
        private PeriodicTimer? _timer;
        Dictionary<string, CallbackHandle> CallbackHandles = new Dictionary<string, CallbackHandle>();
        public async Task Method1(string msg, Action<string[]> callback)
        {
            Console.WriteLine($"Method1 called with message: {msg}");
            callback(["it works directly from Method1 but does not work from timer"]);
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            var callbackHandle = new CallbackHandle(msg, callback);
            CallbackHandles.Add(callbackHandle.Msg, callbackHandle);
            var task = Method2(callbackHandle);
            await callbackHandle.Task;
        }
        public async Task ClearCallback(string msg)
        {
            Console.WriteLine($"ClearCallback called: {msg}");
            if (CallbackHandles.TryGetValue(msg, out var handle))
            {
                Console.WriteLine($"ClearCallback found: {msg}");
                CallbackHandles.Remove(msg);
                handle.Dispose();
                Console.WriteLine($"ClearCallback removed: {msg}");
            }
            else
            {
                Console.WriteLine($"ClearCallback not found: {msg}");
            }
        }

        public async Task Method2(CallbackHandle callbackHandle)
        {
            var i = 0;
            while (await _timer!.WaitForNextTickAsync().ConfigureAwait(false) && !callbackHandle.IsCompleted)
            {
                var message = $"\nHello{i++} from shared worker {DateTime.Now}";
                Console.WriteLine(message);
                callbackHandle.Callback([message]);
                if (callbackHandle.IsCompleted)
                {
                    Console.WriteLine("Callback handle is completed, stopping timer.");
                    break;
                }
            }
        }
    }
}

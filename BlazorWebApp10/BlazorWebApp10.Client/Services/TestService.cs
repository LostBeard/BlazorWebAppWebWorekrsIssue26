using System.Runtime.CompilerServices;

namespace BlazorWebApp10.Services
{
    public interface ITestService
    {
        Task Method1(string msg, Action<string[]> callback);
        Task Method2(string msg, Action<string[]> callback);
    }

    public class TestService : ITestService
    {
        private PeriodicTimer? _timer;
        public async Task Method1(string msg, Action<string[]> callback)
        {

            Console.WriteLine($"Method1 called with message: {msg}");
            callback(["it works directly from Method1 but does not work from timer"]);
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            var task = Method2(msg, callback);
            await Task.Delay(30000);
        }

        public async Task Method2(string msg, Action<string[]> callback)
        {
            var i = 0;
            while (await _timer!.WaitForNextTickAsync().ConfigureAwait(false))
            {
                var message = $"\nHello{i++} from shared worker {DateTime.Now}";
                Console.WriteLine(message);
                callback([message]);
            }
        }
    }
}

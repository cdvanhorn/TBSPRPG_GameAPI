using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GameApi.Events {
    public abstract class EventProcessor : IHostedService, IDisposable
    {
        private bool _stopping;
        private Task _backgroundTask;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Event Processor is starting.");
            PreTask();
            _backgroundTask = BackgroundTask();
            return Task.CompletedTask;
        }

        protected abstract void PreTask();

        private async Task BackgroundTask() {
            while (!_stopping)
            {
                await Task.Delay(TimeSpan.FromSeconds(4));
                //Console.WriteLine("Event Processor is Alive.");
            }

            Console.WriteLine("Event Processor background task is stopping.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Event Processor is stopping.");
            _stopping = true;
            if (_backgroundTask != null)
            {
                await _backgroundTask;
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Event Processor is disposing.");
        }
    }
}
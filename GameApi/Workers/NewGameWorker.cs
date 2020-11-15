using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

using GameApi.Events;

using TbspRgpLib.Settings;

namespace GameApi.Workers
{
    public class NewGameWorker : IHostedService, IDisposable
    {
        private bool _stopping;
        private Task _backgroundTask;
        private IEventService _eventService;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("NewGameWorker is starting.");
            EventStoreSettings settings = new EventStoreSettings();
            settings.Url = "http://eventstore";
            settings.Port = "2113";
            _eventService = new EventService(settings); 
            _eventService.SubscribeByType("new_game");
            _backgroundTask = BackgroundTask();
            return Task.CompletedTask;
        }

        private async Task BackgroundTask()
        {

            while (!_stopping)
            {
                await Task.Delay(TimeSpan.FromSeconds(4));
                //Console.WriteLine("NewGameWorker is doing background work.");
            }

            Console.WriteLine("NewGameWorker background task is stopping.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("NewGameWorker is stopping.");
            _stopping = true;
            if (_backgroundTask != null)
            {
                // TODO: cancellation
                await _backgroundTask;
            }
        }

        public void Dispose()
        {
            Console.WriteLine("NewGameWorker is disposing.");
        }
    }
}

//this will abstract sending events to EventStore,
//so can change to different event store if necessary

namespace GameApi.Events
{
    public interface IEventService
    {
        void SendEvent(Event evnt);
    }

    // var settings = new EventStoreClientSettings {
    //     ConnectivitySettings = {
    //         Address = new Uri("http://eventstore:2113")
    //     }
    // };
    // var client = new EventStoreClient(settings);
    // await client.AppendToStreamAsync(
    //     "some-stream",
    //     StreamState.Any,
    //     new List<EventData> {
    //         eventData
    //     });
}
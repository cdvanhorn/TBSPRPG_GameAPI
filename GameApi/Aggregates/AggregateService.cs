using System;
using System.Threading.Tasks;

using GameApi.Events;

namespace GameApi.Aggregates {
    public interface IAggregateService {
        Task<Aggregate> BuildAggregate(string aggregateId, string aggregateTypeName);
        void SubscribeByType(string typeName, Action<Aggregate, string> eventHandler);
    }

    public class AggregateService : IAggregateService {
        private IEventService _eventService;

        public AggregateService(IEventService eventService) {
            _eventService = eventService;
        }

        public void SubscribeByType(string typeName, Action<Aggregate, string> eventHandler) {
            _eventService.SubscribeByType(
                typeName,
                async (evnt) => {
                    //check if the aggregate id is ok, produce an aggregate
                    var aggregateId = evnt.GetStreamId();
                    if(aggregateId == null) //we can't parse this event
                        return;

                    //need to get aggregate type name in a programitc way
                    eventHandler(await BuildAggregate(aggregateId, "GameAggregate"), evnt.EventId.ToString());
                }
            );
        }

        public async Task<Aggregate> BuildAggregate(string aggregateId, string aggregateTypeName) {
            //create a new aggregate of the appropriate type
            string fqname = $"GameApi.Aggregates.{aggregateTypeName}";
            Type aggregateType = Type.GetType(fqname);
            if(aggregateType == null)
                throw new ArgumentException($"invalid aggregate type name {aggregateTypeName}");
            Aggregate aggregate = (Aggregate)Activator.CreateInstance(aggregateType);

            //get all of the events in the aggregrate id stream
            var events = await _eventService.GetEventsInStreamAsync(aggregateId);
            foreach(var evnt in events) {
                evnt.UpdateAggregate(aggregate);
            }
            return aggregate;
        }
    }
}
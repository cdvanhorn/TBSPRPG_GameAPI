using TbspRpgLib.Events.Game;
using TbspRpgLib.Events.Game.Content;
using TbspRpgLib.Events;
using GameApi.Entities;

namespace GameApi.Adapters {
    public interface IEventAdapter {
        Event NewGameEvent(Game game);
    }

    public class EventAdapter : IEventAdapter{
        public Event NewGameEvent(Game game) {
            GameNew data = new GameNew();
            data.Id = game.Id.ToString();
            data.UserId = game.UserId.ToString();
            data.AdventureId = game.Adventure.Id.ToString();
            data.AdventureName = game.Adventure.Name;

            GameNewEvent evt = new GameNewEvent(data);
            return evt;
        }
    }
}
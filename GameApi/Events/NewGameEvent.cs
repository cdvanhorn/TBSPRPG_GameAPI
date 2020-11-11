using GameApi.Entities;

namespace GameApi.Events
{
    public class NewGameEvent : Event {
        public override void InitEvent(object newGame) {
            Type = NEW_GAME_EVENT_TYPE;
            Data = newGame;
        }

        public override string GetAggregateId()
        {
            Game game = (Game)Data;
            return game.Id;
        }

        // public Game Game {
        //     get { 
        //         return (Game)Data;
        //     }
        // }

    }
}
namespace Stage
{
    public class PokerGameStageCollection : StageCollections
    {
        public PokerGameStageCollection()
        {
            CreateStageCollection();
        }
        private void CreateStageCollection()
        {
            //adding the stage of the game to stage collections
            addStage(new PokerInitialStage());
            //addStage(new ShuffleStage());
            addStage(new CardGivingStage());
            addStage(new PlayerTurnStage());
            addStage(new HostTurnStage());


            addStage(new PokerEndStage());
        }
    }
}

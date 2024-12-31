namespace Stage
{
    public class RouletteGameStageCollection : StageCollections
    {
        public RouletteGameStageCollection()
        {
            CreateStageCollection();
        }
        private void CreateStageCollection()
        {
            //adding the stage of the game to stage collections
            addStage(new RouletteGameInitialStage());
            addStage(new RouletteGameExecuteStage());
            addStage(new RouletteGameEndStage());

        }
    }
}

namespace Stage
{
    public class SlotGameStageCollection : StageCollections
    {
        public SlotGameStageCollection()
        {
            CreateStageCollection();
        }
        private void CreateStageCollection()
        {
            //adding the stage of the game to stage collections
            addStage(new SlotGameInitialStage());
            addStage(new SlotGameExecuteStage());
            addStage(new SlotGameEndStage());
        }
    }
}

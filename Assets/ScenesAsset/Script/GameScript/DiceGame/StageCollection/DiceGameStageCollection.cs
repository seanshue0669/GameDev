namespace Stage
{
    public class DiceGameStageCollection : StageCollections
    {
        public DiceGameStageCollection()
        {
            CreateStageCollection();
        }
        private void CreateStageCollection()
        {
            //adding the stage of the game to stage collections
            addStage(new DiceGame_InitialStage());
            addStage(new DiceGame_ExecuteStage());
            addStage(new DiceGame_EndStage());
        }
    }
}

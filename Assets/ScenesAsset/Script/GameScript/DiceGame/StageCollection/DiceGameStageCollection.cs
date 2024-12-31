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
            addStage(new InitialStage());
            addStage(new ExecuteStage());
        }
    }
}

using System.Collections.Generic;
namespace Stage
{ 
    public abstract class StageCollections
    {
        //container
        public List<IStage> stages = new List<IStage>();
        #region Tool Method
        protected void addStage(IStage stage)
        {
            stages.Add(stage);
        }
        #endregion

    }
}
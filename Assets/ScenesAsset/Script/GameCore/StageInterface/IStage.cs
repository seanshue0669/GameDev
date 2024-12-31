using System.Threading.Tasks;
public interface IStage
{
    Task ExecuteAsync(SharedDataSO sharedData, UIComponentCollectionSO uiComponents);
    Task InputAsync();
}

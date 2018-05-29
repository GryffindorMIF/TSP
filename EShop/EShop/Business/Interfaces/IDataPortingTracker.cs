namespace EShop.Business.Interfaces
{
    public interface IDataPortingTrackerService
    {
        bool IsImportRunning();
        void SetImportRunningStatus(bool status);
    }
}
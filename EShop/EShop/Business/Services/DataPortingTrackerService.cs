using EShop.Business.Interfaces;

namespace EShop.Business.Services
{
    public class DataPortingTrackerService : IDataPortingTrackerService
    {
        private bool _isImportRunning;

        public bool IsImportRunning()
        {
            return _isImportRunning;
        }

        public void SetImportRunningStatus(bool status)
        {
            _isImportRunning = status;
        }
    }
}
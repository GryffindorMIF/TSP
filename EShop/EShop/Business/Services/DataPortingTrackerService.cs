using EShop.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Services
{
    public class DataPortingTrackerService : IDataPortingTrackerService
    {
        private bool isImportRunning = false;

        public bool IsImportRunning()
        {
            return isImportRunning;
        }

        public void SetImportRunningStatus(bool status)
        {
            isImportRunning = status;
        }
    }
}

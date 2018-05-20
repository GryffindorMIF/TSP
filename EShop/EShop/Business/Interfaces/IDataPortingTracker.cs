using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Interfaces
{
    public interface IDataPortingTrackerService
    {
        bool IsImportRunning();
        void SetImportRunningStatus(bool status);
    }
}

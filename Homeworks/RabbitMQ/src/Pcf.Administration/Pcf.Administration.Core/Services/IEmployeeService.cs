using System;
using System.Threading.Tasks;

namespace Pcf.Administration.Core.Services
{
    public interface IEmployeeService
    {
        Task UpdateAppliedPromocodesAsync(Guid employeeId);
    }
}

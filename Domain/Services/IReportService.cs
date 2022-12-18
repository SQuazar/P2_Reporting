#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services;

public interface IReportService : IDataService<Report>, IPaginatedService<Report>
{
    Task<Report> Send(Account sender, string title, string? description = null);
    Task<IEnumerable<Report>> GetAllBySender(Account sender);
}
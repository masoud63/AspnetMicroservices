using System.Threading.Tasks;
using Ordering.Application.Models;

namespace Ordering.Application.Contracts.Infrastructure
{
    public interface IEmailSerivce
    {
        Task<bool> SendEmail(Email email);
    }
}
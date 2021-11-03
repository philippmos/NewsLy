using System.Threading.Tasks;
using NewsLy.Api.Models;

namespace NewsLy.Api.Services.Interfaces
{
    public interface IMailingService
    {
        Task SendMailingAsync(ContactRequest mailRequest, bool trackLinks);
    }
}
using System.Threading.Tasks;
using NewsLy.Api.Models;

namespace NewsLy.Api.Services
{
    public interface IMailingService
    {
        Task SendMailAsync(ContactRequest mailRequest);
    }
}
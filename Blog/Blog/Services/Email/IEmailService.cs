using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services.Email
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message);
        Task SendEmailBySendGrid(string email, string subject, string message);
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeSimulator.Application.Requests.UserRequests.VerifyEmail
{
    public class VerifyEmailRequest : IRequest
    {
        public string Code { get; set; }
    }
}

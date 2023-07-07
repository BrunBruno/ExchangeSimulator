using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeSimulator.Application.Services {
    public interface IUserContextService {
        Guid? GetUserId();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthenticationSettings
    {
        double JwtExpireDays { get; }
        char[] JwtKey { get; }

        string JwtIssuer { get; }
    }
}

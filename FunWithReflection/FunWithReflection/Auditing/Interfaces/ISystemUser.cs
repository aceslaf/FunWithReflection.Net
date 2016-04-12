using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithReflection.Interfaces
{
    public interface ISystemUser
    {
        int UserId { get; }
        int ClinicId { get; }
        string UserName { get; }
    }
}

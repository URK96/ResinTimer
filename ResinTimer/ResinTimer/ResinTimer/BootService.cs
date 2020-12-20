using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ResinTimer
{
    public interface IBootService
    {
        Task<bool> Register();
        Task Unregister();
    }
}

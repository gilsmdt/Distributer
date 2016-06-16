﻿using Distributer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributer.Contracts
{
    public interface IDispatcher
    {
        DispatchAdapter GetDispatchAdapter(IRequest request);
        IRequest[] GetRequests();
             
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FastFood.Repository
{
    public interface IDbInitializer
    {
        void Initialize();
    }
}

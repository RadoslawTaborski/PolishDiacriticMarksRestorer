﻿using System.Collections.Generic;

namespace NgramFilter.Interfaces
{
    internal interface IFilter
    {
        bool Start(List<string> list);
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactoryEnum
{
    public enum EnumTaskOrderColumn
    {
        [DescriptionAttribute("创建时间")]
        CreateTime = 0,
        [DescriptionAttribute("到期时间")]
        EndTime = 1
    }
}

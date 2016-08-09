﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    [Serializable]
    public class CacheUserEntity
    {
        [Property("Lower")]
        public string UserID { get; set; }

        public string Name { set; get; }

        public string Avatar { set; get; }

        public string ClientID { get; set; }
    }
}

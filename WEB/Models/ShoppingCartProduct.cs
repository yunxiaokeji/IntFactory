using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IntFactoryEnum;
using IntFactoryEntity;

namespace YXERP.Models
{
    [Serializable]
    public class ShoppingCartProduct
    {

        public EnumDocType type { get; set; }

        public string guid { get; set; }

        public string attrid { get; set; }

        public List<ProductStock> Products { get; set; }

    }
}
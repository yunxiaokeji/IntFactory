using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IntFactoryEntity;
namespace YXERP.Models
{
    public class TaskModel
    {
        public OrderEntity Order { get; set; }

        public IntFactoryEntity.Task.TaskEntity Task { get; set; }

        public IntFactoryEntity.ProductAttr ProductAttr { get; set; }

        public bool IsTaskOwner { get; set; }

        public bool IsEditTask { get; set; }

        public bool IsWarn { get; set; }

        public bool IsRoot { get; set; }

        public int FinishDay { get; set; }
    }
}
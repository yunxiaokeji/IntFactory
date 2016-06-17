using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactoryEntity
{
    public class Attachment
    {
        public int AutoID { get; set; }

        public string AttachmentID { get; set; }

        public string Guid { get; set; }

        public int Type { get; set; }

        public int Size { get; set; }

        public string ServerUrl { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string OriginalName { get; set; }

        public string ThumbnailName { get; set; }

        public string CreateUserID { get; set; }

        public string ClientID { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}

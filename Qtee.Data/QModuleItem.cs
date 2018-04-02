using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qtee.Data
{
    public partial class QModuleItem
    {
        [Key]
        public long Id { get; set; }
        [MaxLength(length: 255)]
        public string Title { get; set; }
        [MaxLength(length: 255)]
        public string FileName { get; set; }
        public byte[] FileEntity { get; set; }
        public decimal Version { get; set; }

        
        public long CollectionId { get; set; }
        [ForeignKey("CollectionId")]
        public virtual QModuleCollection Collection { get; set; }

    }
}

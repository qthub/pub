using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qtee.Data
{
    public class QModuleCollection
    {
        public QModuleCollection()
        {
            CollectionItems = new List<QModuleItem>();
        }

        [Key] public long Id { get; set; }

        [MaxLength(255)] public string Title { get; set; }

        public virtual ICollection<QModuleItem> CollectionItems { get; set; }
    }
}
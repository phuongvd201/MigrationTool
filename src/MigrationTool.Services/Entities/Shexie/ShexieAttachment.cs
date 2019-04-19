using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "Attachments")]
    public class ShexieAttachment
    {
        [Column(Name = "ObjectId")]
        public int Id { get; set; }

        [Column(Name = "PatientId")]
        public string PatientId { get; set; }

        [Column(Name = "Desc")]
        public string Description { get; set; }

        [Column(Name = "Type")]
        public string Type { get; set; }

        [Column(Name = "Date")]
        public DateTime? Date { get; set; }

        [Column(Name = "Path")]
        public string Path { get; set; }

        [Column(Name = "Provider")]
        public string Provider { get; set; }
    }
}
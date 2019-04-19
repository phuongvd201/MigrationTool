using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "service")]
    public class ShexieReferral
    {
        [Column(Name = "cnt")]
        public int Id { get; set; }

        [Column(Name = "patient")]
        public int? PatientId { get; set; }

        [Column(Name = "referral_doctor")]
        public int? ContactId { get; set; }

        [Column(Name = "referral_date")]
        public DateTime? ReferralDate { get; set; }

        [Column(Name = "LetDate")]
        public DateTime? IssueDate { get; set; }

        [Column(Name = "period")]
        public byte Duration { get; set; }
    }
}
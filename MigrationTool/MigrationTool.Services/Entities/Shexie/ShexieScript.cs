using System;
using System.Data.Linq.Mapping;

namespace MigrationTool.Services.Entities.Shexie
{
    [Table(Name = "scripts")]
    public class ShexieScript
    {
        [Column(Name = "ID")]
        public int Id { get; set; }

        [Column(Name = "patient")]
        public int PatientId { get; set; }

        [Column(Name = "perscription_no")]
        public string PrescriptionNo { get; set; }

        [Column(Name = "perscription_dose")]
        public string PrescriptionDose { get; set; }

        [Column(Name = "repeat_days")]
        public string RepeatDays { get; set; }

        [Column(Name = "authority_no")]
        public string AuthorityNumber { get; set; }

        [Column(Name = "duration")]
        public string Duration { get; set; }

        [Column(Name = "created")]
        public DateTime? Created { get; set; }

        [Column(Name = "requested_date")]
        public DateTime? RequestedDate { get; set; }

        [Column(Name = "perscription_date")]
        public DateTime? PrescriptionDate { get; set; }

        [Column(Name = "perscription_repeats")]
        public byte? PrescriptionRepeats { get; set; }

        [Column(Name = "Dose")]
        public string Dose { get; set; }

        [Column(Name = "Substitute")]
        public byte? Substitute { get; set; }

        [Column(Name = "drug")]
        public int? Drug { get; set; }

        [Column(Name = "quantity")]
        public int? Quantity { get; set; }

        [Column(Name = "other_med_desc")]
        public string OtherMedicalDescription { get; set; }

        [Column(Name = "frequency")]
        public int? Frequency { get; set; }

        [Column(Name = "Strength")]
        public int? Strength { get; set; }

        [Column(Name = "delivery")]
        public int? Delivery { get; set; }

        [Column(Name = "Comments")]
        public string Comments { get; set; }

        [Column(Name = "Script_Words")]
        public string ScriptWords { get; set; }

        [Column(Name = "AveDailyDose")]
        public double? AverageDailyDose { get; set; }

        [Column(Name = "Indics")]
        public string Indics { get; set; }

        [Column(Name = "Provider")]
        public string Provider { get; set; }

        public ShexieDrug ShexieDrug { get; set; }
    }
}
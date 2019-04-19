using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieObstetricHistory
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int DeliveryYear { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string Induction { get; set; }

        public string Place { get; set; }

        public string Week { get; set; }

        public string Pregnancy { get; set; }

        public string Labour { get; set; }

        public string Analgesia { get; set; }

        public string Delivery { get; set; }

        public string BreastFed { get; set; }

        public string Note { get; set; }

        public float Weight { get; set; }

        public string Sex { get; set; }

        public string Name { get; set; }

        public string Result { get; set; }

        public float Weight2 { get; set; }

        public string Sex2 { get; set; }

        public string Name2 { get; set; }

        public string Result2 { get; set; }

        public float Weight3 { get; set; }

        public string Sex3 { get; set; }

        public string Name3 { get; set; }

        public string Result3 { get; set; }

        public float Weight4 { get; set; }

        public string Sex4 { get; set; }

        public string Name4 { get; set; }

        public string Result4 { get; set; }
    }
}
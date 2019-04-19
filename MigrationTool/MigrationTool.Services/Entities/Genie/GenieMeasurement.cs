using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GenieMeasurement
    {
        public int PatientId { get; set; }

        public DateTime? MeasurementDate { get; set; }

        public float Height { get; set; }

        public float Weight { get; set; }

        public float HeadCircumference { get; set; }

        public float Neck { get; set; }

        public float Waist { get; set; }

        public float Bmi { get; set; }

        public float Systolic { get; set; }

        public float Diastolic { get; set; }

        public float Gfr { get; set; }

        public float Cholesterol { get; set; }

        public float Triglycerides { get; set; }

        public float Hdl { get; set; }

        public float Ldl { get; set; }

        public float Bsl { get; set; }

        public float Hba1c { get; set; }

        public float Creatinine { get; set; }

        public float Psa { get; set; }

        public float MicroAlbumInUria { get; set; }

        public string ValeftUncorrected { get; set; }

        public string ValeftCorrected { get; set; }

        public string VarightUncorrected { get; set; }

        public string VarightCorrected { get; set; }

        public float PressureLeft { get; set; }

        public float PressureRight { get; set; }

        public float Hip { get; set; }

        public float WaistHipratio { get; set; }

        public float HeartRate { get; set; }

        public string RateComment { get; set; }

        public float Acr { get; set; }

        public float Fev1 { get; set; }

        public float Fvc { get; set; }

        public float GasTransfer { get; set; }

        public bool Mdrd { get; set; }

        public float Potassium { get; set; }

        public float ExcessWeight { get; set; }

        public bool BaseLineWeight { get; set; }

        public int Id { get; set; }
    }
}
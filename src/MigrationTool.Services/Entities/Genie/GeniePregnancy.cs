using System;

namespace MigrationTool.Services.Entities.Genie
{
    internal class GeniePregnancy
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public DateTime? Lmp { get; set; }

        public DateTime? Edd { get; set; }

        public string Cvs { get; set; }

        public string Amnio { get; set; }

        public string Chest { get; set; }

        public string Breasts { get; set; }

        public string Abdo { get; set; }

        public string Pv { get; set; }

        public string Pap { get; set; }

        public string BloodGroup { get; set; }

        public float? Hb { get; set; }

        public float? Mcv { get; set; }

        public string Rubella { get; set; }

        public string Hbv { get; set; }

        public string Hcv { get; set; }

        public string Hiv { get; set; }

        public string Syphilis { get; set; }

        public string Msu { get; set; }

        public string Parvo { get; set; }

        public string Toxo { get; set; }

        public string Cmv { get; set; }

        public string Varicella { get; set; }

        public DateTime? Uss1Date { get; set; }

        public float? Uss1Weeks { get; set; }

        public string Uss1Comment { get; set; }

        public DateTime? Uss2Date { get; set; }

        public float? Uss2Weeks { get; set; }

        public string Uss2Comment { get; set; }

        public DateTime? Uss3Date { get; set; }

        public float? Uss3Weeks { get; set; }

        public string Uss3Comment { get; set; }

        public float? Hb28Weeks { get; set; }

        public float? Hb36Weeks { get; set; }

        public string Gct28Weeks { get; set; }

        public string Antibodies28Weeks { get; set; }

        public string Antibodies36Weeks { get; set; }

        public string Gbs28Weeks { get; set; }

        public string Gbs36Weeks { get; set; }

        public string Notes { get; set; }

        public bool Declined { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public TimeSpan? DeliveryTime { get; set; }

        public string DeliveryMethod { get; set; }

        public string DeliveryNotes { get; set; }

        public string Accoucher { get; set; }

        public string Paediatrician { get; set; }

        public string Anaesthetist { get; set; }

        public string BabyName { get; set; }

        public string BabySex { get; set; }

        public float? BabyWeight { get; set; }

        public int? Apgar1 { get; set; }

        public int? Apgar2 { get; set; }

        public DateTime? PostnatalDate { get; set; }

        public string Speculum { get; set; }

        public string PostnatalPv { get; set; }

        public string Contraception { get; set; }

        public string PostnatalNotes { get; set; }

        public int ObstetricHistoryRecordId { get; set; }

        public string AbsInitial { get; set; }

        public string CurrentGestation { get; set; }

        public DateTime? EddAgreed { get; set; }

        public string Placenta { get; set; }

        public string AntiD { get; set; }

        public string BabyName2 { get; set; }

        public string BabySex2 { get; set; }

        public float? BabyWeight2 { get; set; }

        public int? Baby2Apgar1 { get; set; }

        public int? Baby2Apgar2 { get; set; }

        public string Hospital { get; set; }

        public string Provider { get; set; }

        public string BoyOrGirl { get; set; }

        public string Ferritin { get; set; }

        public string Tfts { get; set; }

        public bool AntiD28 { get; set; }

        public bool AntiD36 { get; set; }

        public string Gtt282Hr { get; set; }

        public float? TwentyWeekFee { get; set; }

        public float? ThirtyWeekFee { get; set; }

        public string Result { get; set; }

        public string AdditionalAnNotes { get; set; }

        public string PlacentalPosition { get; set; }

        public string Result2 { get; set; }

        public string Result3 { get; set; }

        public string Result4 { get; set; }

        public float? VitaminD { get; set; }

        public string Trisomy18 { get; set; }

        public string Trisomy21 { get; set; }

        public string Gtt28Fasting { get; set; }

        public string BabyName3 { get; set; }

        public string BabySex3 { get; set; }

        public float? BabyWeight3 { get; set; }

        public int? Baby3Apgar1 { get; set; }

        public int? Baby3Apgar2 { get; set; }

        public string BabyName4 { get; set; }

        public string BabySex4 { get; set; }

        public float? BabyWeight4 { get; set; }

        public DateTime? NextAppointmentDate { get; set; }

        public int? Baby4Apgar1 { get; set; }

        public int? Baby4Apgar2 { get; set; }

        public int? AnaesthetistId { get; set; }

        public int? PaediatricianId { get; set; }

        public int? AccoucherId { get; set; }

        public string Platelets28 { get; set; }

        public string Platelets36 { get; set; }

        public float? Ebl { get; set; }

        public string Bp { get; set; }

        public DateTime? InitialTestDate { get; set; }

        public string Problems { get; set; }

        public string Perineum { get; set; }

        public bool LabourNil { get; set; }

        public bool AnalgesiaNil { get; set; }

        public bool Breastfeeding { get; set; }

        public int Lochia { get; set; }

        public int PerineumState { get; set; }

        public int Bladder { get; set; }

        public int Bowel { get; set; }

        public int BreastState { get; set; }

        public int PnBreasts { get; set; }

        public int PnAbdo { get; set; }

        public int PnPerineum { get; set; }

        public bool PapTaken { get; set; }

        public bool LabourSpontaneous { get; set; }

        public bool LabourProstin { get; set; }

        public bool LabourArm { get; set; }

        public bool LabourSyntocinon { get; set; }

        public bool AnalgesiaNitrous { get; set; }

        public bool AnalgesiaPethidine { get; set; }

        public bool AnalgesiaEpidural { get; set; }

        public bool AnalgesiaSpinal { get; set; }

        public bool AnalgesiaGa { get; set; }

        public bool BreastfeedingAtDelivery { get; set; }

        public bool? NeonatalExam { get; set; }
    }
}
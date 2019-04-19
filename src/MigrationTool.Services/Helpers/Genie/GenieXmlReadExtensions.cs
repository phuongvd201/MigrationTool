using System.Xml.Linq;

using MigrationTool.Services.Entities.Genie;

namespace MigrationTool.Services.Helpers.Genie
{
    internal static class GenieXmlReadExtensions
    {
        public const string Md3PractitionerTagName = "PRACTITIONER";
        public const string Md3ResourceTagName = "RESOURCE";
        public const string Md3ScriptTagName = "SCRIPTITEM";
        public const string Md3VisitTagName = "VISIT";
        private const string Md3ResourceDoctor = "DOCTOR";
        public const string Md3LaboratoryResultTagName = "RESULT";

        internal static GenieOutgoingLetter GetGenieOutgoingLetter(this XElement element)
        {
            return new GenieOutgoingLetter
            {
                Id = element.GetInt("id"),
                PatientId = element.GetInt("pt_id_fk"),
                ContactId = element.GetInt("ab_id_fk"),
                Creator = element.GetString("creator"),
                From = element.GetString("from"),
                LetterDate = element.GetDateTime("letterdate"),
                ReferralContent = element.GetBase64String("referralcontent_"),
                Cda = element.GetBase64String("cda"),
                Reviewed = element.GetBoolean("reviewed"),
                PrimarySent = element.GetBoolean("primarysent"),
                ReadyToSend = element.GetBoolean("readytosend"),
                ReplayReceived = element.GetBoolean("replyreceived"),
            };
        }

        internal static GenieIncomingLetter GetGenieIncomingLetter(this XElement element)
        {
            return new GenieIncomingLetter
            {
                Id = element.GetInt("id"),
                PatientId = element.GetInt("pt_id_fk"),
                LetterDate = element.GetDateTime("letterdate"),
                Sender = element.GetString("sender"),
                Addressee = element.GetString("addressee"),
                LetterContent = element.GetBase64String("lettercontent_"),
                FileName = element.GetString("filename"),
                PatientFirstName = element.GetString("firstname"),
                PatientLastName = element.GetString("surname"),
                LetterType = element.GetString("lettertype"),
            };
        }

        internal static GenieUser GetGenieUserFromMd3Practitioner(this XElement element)
        {
            return new GenieUser
            {
                Id = element.GetInt("DR_NO"),
                Name = element.GetString("DR"),
                UserName = element.GetString("DR"),
            };
        }

        internal static GenieUser GetGenieUserFromMd3Resource(this XElement element)
        {
            return element.GetString("CATEGORY") == Md3ResourceDoctor
                ? new GenieUser
                {
                    Id = element.GetInt("DR_NO"),
                    Name = element.GetString("FULL_NAME"),
                    UserName = element.GetString("FULL_NAME"),
                }
                : null;
        }

        internal static GenieScript GetGenieScriptFromMd3Script(this XElement element)
        {
            return new GenieScript
            {
                Id = element.GetString("SCR_NO") + '-' + element.GetString("DRUG_NO"),
                CreationDate = element.GetDateTime("SCRDATE"),
                Dose = element.GetString("D1"),
                Medication = element.GetString("P1"),
                Repeat = element.GetString("DR_1_R"),
                CreatorDoctorId = element.GetInt("DR_NO"),
                ExternalPatientId = element.GetString("UR_NO"),
                Quantity = element.GetString("DR_1_Q"),
                DrugId = element.GetString("DRUG_NO"),
            };
        }

        internal static GenieDrug GetGenieDrugFromMd3Drug(this XElement element)
        {
            return new GenieDrug
            {
                Id = element.GetString("DRUG_NO"),
                Name = element.GetString("P1"),
                Form = element.GetString("F1"),
                Strength = element.GetString("S1"),
                Quantity = element.GetString("DR_1_Q"),
            };
        }

        internal static GenieDownloadedResult GetGenieLaboratoryResultFromMd3(this XElement element)
        {
            return new GenieDownloadedResult
            {
                Id = element.GetInt("PATHOLOGY_ID"),
                ExternalPatientId = element.GetString("UR_NO"),
                Addressee = element.GetString("CHECKEDBY"),
                DocumentName = element.GetString("LABNAME"),
                DateOfBirth = element.GetNullableDateTime("DOB"),
                ReportDate = element.GetNullableDateTime("REPORTDATE"),
                ImportDate = element.GetNullableDateTime("IMPDATE"),
                CollectionDate = element.GetNullableDateTime("COLLDATE"),
                ReceivedDate = element.GetNullableDateTime("REPORTDATE"),
                Result = element.GetString("REPORTTEXT"),
                Test = element.GetString("TESTNAME"),
                LabRef = element.GetString("LAB_REFERENCE"),
                NormalOrAbnormal = element.GetString("LABNORM"),
            };
        }
    }
}
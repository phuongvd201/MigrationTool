using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using MigrationTool.Services.Entities.Genie;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

using Log4net = log4net;

namespace MigrationTool.Services.Helpers.Genie
{
    internal static class GenieMigrationHelper
    {
        private static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string CompanyAccountHolderExternalIdPrefix = "ah-";

        internal const string GenieCheckBox = "CheckBox";
        internal const string GenieDate = "Date";
        internal const string GenieHeading = "Heading";
        internal const string GenieLText = "LText";
        internal const string GenieNumber = "Number";
        internal const string GenieRadio = "Radio";
        internal const string GenieText = "Text";
        internal const string GeniePicture = "Picture";

        private static readonly Dictionary<string, MigrationChecklistControlType> ControlTypesMap = new Dictionary<string, MigrationChecklistControlType>(StringComparer.InvariantCultureIgnoreCase)
        {
            { GenieCheckBox, MigrationChecklistControlType.CheckBoxList },
            { GenieDate, MigrationChecklistControlType.TextBox },
            { GenieHeading, MigrationChecklistControlType.TextBox },
            { GenieLText, MigrationChecklistControlType.TextArea },
            { GenieNumber, MigrationChecklistControlType.TextBox },
            { GenieRadio, MigrationChecklistControlType.RadioButtonList },
            { GenieText, MigrationChecklistControlType.TextBox },
        };

        private static readonly Dictionary<string, Func<GenieChecklistField, MigrationDictionaryItem[]>> DoctorFieldValueMap = new Dictionary<string, Func<GenieChecklistField, MigrationDictionaryItem[]>>(StringComparer.InvariantCultureIgnoreCase)
        {
            { GenieText, x => new MigrationDictionaryItem[] { } },
            { GenieDate, x => new MigrationDictionaryItem[] { } },
            { GenieLText, x => new MigrationDictionaryItem[] { } },
            { GenieNumber, x => new MigrationDictionaryItem[] { } },
            {
                GenieCheckBox, x => new[]
                {
                    new MigrationDictionaryItem
                    {
                        Id = 1,
                        Text = x.Label
                    }
                }
            },
            { GenieRadio, GetRadioTemplateFieldValues },
            { GenieHeading, GetFieldNull },
            { GeniePicture, GetFieldNull },
        };

        private static readonly Dictionary<string, Func<GenieChecklistField, GenieChecklistField, MigrationDictionaryItem[]>> PatientFieldValueMap = new Dictionary<string, Func<GenieChecklistField, GenieChecklistField, MigrationDictionaryItem[]>>(StringComparer.InvariantCultureIgnoreCase)
        {
            { GenieText, GetFieldSingleValue(GetTextFieldValue) },
            { GenieDate, GetFieldSingleValue(GetOriginalFieldValue) },
            { GenieLText, GetFieldSingleValue(GetTextFieldValue) },
            { GenieNumber, GetFieldSingleValue(GetNumberFieldValue) },
            { GenieCheckBox, GetFieldSingleValue(GetCheckboxFieldValue) },
            { GenieRadio, GetRadioFieldValues },
            { GenieHeading, GetFieldNull },
            { GeniePicture, GetFieldNull },
        };

        private static readonly List<string> GenieGeneralPractitionerSpecialties = new List<string>()
        {
            "General Medical Practitioner",
            "GP"
        };

        private static readonly List<string> GenieReceptionistSpecialties = new List<string>()
        {
            "Medical Receptionist"
        };

        private static readonly Dictionary<string, string> SmokingStatusGenieToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "0", null },
            { "1", "Current every day smoker" },
            { "2", "Light tobacco smoker" },
            { "3", "Current some day smoker" },
            { "4", "Former smoker" },
            { "5", "Never smoker" },
        };

        private static readonly Dictionary<string, string> BirthResultGenieToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Live Birth", "Live Birth" },
            { "Blighted ovum", "Ectopic" },
            { "Death in utero", "FDIU" },
            { "Miscarriage", "Miscarriage" },
            { "Stillborn", "Stillborn" },
            { "TOP", "Termination" },
        };

        private static readonly Dictionary<string, string> PerineumGenieToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Episiotomy", "Episiotomy" },
            { "First Degree Tear", "First Degree Tear" },
            { "Intact", "Intact" },
            { "Second Degree Tear", "Second Degree Tear" },
            { "Third Degree Tear", "Third Degree Tear" },
        };

        private static readonly Dictionary<string, string> DrugCodeGenieToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Authority - PBS/RPBS", "AuthorityPbsRpbs" },
            { "Authority - RPBS", "AuthorityRpbs" },
            { "PBS/RPBS", "PbsRpbs" },
            { "Private", "Private" },
            { "Restricted - PBS/RPBS", "RestrictedPbsRpbs" },
            { "Restricted - RPBS", "RestrictedRpbs" },
            { "RPBS", "Rpbs" },
        };

        private static readonly Dictionary<string, string> AccountTypeGenieToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "AMA", "Health Fund" },
            { "Bulk Bill", "Bulk Bill" },
            { "DVA Inpatient", "DVA" },
            { "DVA Outpatient", "DVA" },
            { "HCC", "Health Fund" },
            { "Private", "Private" },
            { "Rebate", "Bulk Bill" },
            { "Schedule", "Bulk Bill" },
            { "Veteran Affairs", "DVA" },
            { "Workcover", "Private" },
        };

        private static readonly Dictionary<string, string> HealthFundGenieToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "ACA Health Benefits Fund", "ACA" },
            { "Allianz Global Assistance", "AGA" },
            { "Aust. Health Management", "AHM" },
            { "ahm Health Insurance", "AHM" },
            { "ANZ Health Insurance", "ANZ" },
            { "AMA Health Benefits Fund", "AMA" },
            { "The Doctors' Health Fund", "AMA" },
            { "Australian Unity Health Limited", "AUH" },
            { "Aust. Unity Health Limited", "AUH" },
            { "Budget Direct Health", "BUD" },
            { "BUPA", "BUP" },
            { "BUPA Australia", "BUP" },
            { "CBHS Friendly Society Ltd.", "CBH" },
            { "CBHS Health Fund Limited", "CBH" },
            { "CDH Benefits Fund", "CDH" },
            { "CUA Health Limited", "CHF" },
            { "CUA", "CHF" },
            { "Credicare Health Fund", "CPS" },
            { "CY Health", "CYF" },
            { "Defence Health", "DHF" },
            { "GMHBA", "GMH" },
            { "GMHBA Limited", "GMH" },
            { "Frank Health Insurance", "GMH" },
            { "FIT Health Insurance", "GMH" },
            { "GU Health", "GUH" },
            { "hba", "HBA" },
            { "HBF Health Limited", "HBF" },
            { "Hospital Contribution Fund", "HCF" },
            { "Health Care Insurance Ltd", "HCI" },
            { "Health Care Insurance Limited", "HCI" },
            { "health.com.au", "HEA" },
            { "GMF Health", "HHB" },
            { "Central West Health Cover", "HHB" },
            { "Health Insurance Fund of Australia Limited", "HIF" },
            { "Lysaght Peoplecare", "LHM" },
            { "Peoplecare Health Insurance", "LHM" },
            { "Latrobe Health Services", "LHS" },
            { "MBF", "MBF" },
            { "MBF Alliances", "MBF" },
            { "Mutual Community", "MCL" },
            { "Mildura Health Fund", "MDH" },
            { "Medibank Private", "MPL" },
            { "Medibank Private Limited", "MPL" },
            { "Navy Health", "NHB" },
            { "Navy Health Ltd", "NHB" },
            { "Apia Health", "NIB" },
            { "NIB Health Funds Ltd", "NIB" },
            { "onemedifund", "OMF" },
            { "Phoenix Health Fund", "PHF" },
            { "Phoenix Health Fund Limited", "PHF" },
            { "Police Health", "POL" },
            { "Police Health Limited", "POL" },
            { "Qld Country Health Ltd.", "QCH" },
            { "Queensland Country Health Fund Ltd", "QCH" },
            { "Teachers Union Health", "QTU" },
            { "RACT Health", "RAC" },
            { "Reserve Bank Health Society Ltd", "RBH" },
            { "Reserve Bank Health Society Limited", "RBH" },
            { "rt health fund", "RTH" },
            { "St Lukes", "SLM" },
            { "St Lukes Health", "SLM" },
            { "Health Partners", "SPS" },
            { "Teachers Federation Health Fund", "TFH" },
            { "Teachers Health Fund", "TFH" },
            { "Transport Friendly Society", "TFS" },
            { "Transport Health", "TFS" },
            { "Westfund", "WFD" },
        };

        private static readonly Dictionary<string, string> SpecialtyGenieToSiberiaDoctor = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "General Medical Practitioner", "General physician" },
            { "Acupuncturist", null },
            { "Anaesthetic Technician", null },
            { "Anaesthetist", "Anaesthetist" },
            { "Audiologist", null },
            { "Audiometrist", null },
            { "Biomedical Engineer", null },
            { "Cardiac Technician", null },
            { "Cardiologist", "Cardiologist" },
            { "Cardiothoracic Surgeon", "Cardio-thoracic surgeon" },
            { "Chiropractor", null },
            { "Clinical Allergist", "Immunologist and allergist" },
            { "Clinical Geneticist", "Clinical geneticist" },
            { "Clinical Haematologist", "Haematologist" },
            { "Clinical Immunologist", "Immunologist" },
            { "Clinical Pharmacologist", "Clinical pharmacologist" },
            { "Clinical Psychologist", null },
            { "Community Psychologist", null },
            { "Counselling Psychologist", null },
            { "Counsellor", null },
            { "Dental Practitioners", null },
            { "Dental Specialist", null },
            { "Dentist", null },
            { "Dermatologist", "Dermatologist" },
            { "Diagnostic and Interventional Radiologist", "Radiologist" },
            { "Dialysis Technician", null },
            { "Dietitian", null },
            { "Drug and Alcohol Counsellor", "Addiction medicine specialist" },
            { "Electroencephalographic Technician", null },
            { "Emergency Medicine Specialist", "Emergency physician" },
            { "Endocrinologist", "Endocrinologist" },
            { "ENT Surgeon", "Surgeon" },
            { "Family and Marriage Counsellor", null },
            { "Gambling Counsellor", "Addiction medicine specialist" },
            { "Gastroenterologist", "Gastroenterologist and hepatologist" },
            { "Geriatrician", "Geriatrician" },
            { "Grief Counsellor", null },
            { "Gynaecological Oncologist", "Gynaecological oncologist" },
            { "Health and Welfare Support Worker", null },
            { "Homeopath", null },
            { "Hospital Pharmacist", null },
            { "Industrial Medicine Specialist", null },
            { "Infectious Diseases Specialist", "Infectious diseases physician" },
            { "Intensive Care Specialist", "Intensive care physician" },
            { "Life Coach", null },
            { "Massage Therapist", null },
            { "Medical Diagnostic Radiographer", "Radiologist" },
            { "Medical Laboratory Scientist", null },
            { "Medical Laboratory Technician", null },
            { "Medical Oncologist", "Medical oncologist" },
            { "Medical Radiation Therapist", "Radiologist" },
            { "Medical Receptionist", "Medical administrator" },
            { "Medical Technicians", null },
            { "Midwife", null },
            { "Natural Remedy Consultant", null },
            { "Naturopath", null },
            { "Neurologist", "Neurologist" },
            { "Neurophysiological Technician", null },
            { "Neurosurgeon", "Neurosurgeon" },
            { "Nuclear Medicine Specialist", "Nuclear medicine specialist" },
            { "Nuclear Medicine Technologist", "Nuclear medicine specialist" },
            { "Nurse Practitioner", null },
            { "Obstetrician and Gynaecologist", "Obstetrician and gynaecologist" },
            { "Occupational Medicine Specialist", "Occupational and environmental physician" },
            { "Occupational Therapist", "Occupational and environmental physician" },
            { "Ophthalmologist", "Ophthalmologist" },
            { "Optometrist", null },
            { "Oral and Maxillofacial Surgeon", "Oral and maxillofacial surgeon" },
            { "Orthopaedic Surgeon", "Orthopaedic surgeon" },
            { "Orthoptist", null },
            { "Orthotic and Prosthetic Technician", null },
            { "Orthotist or Prosthetist", null },
            { "Osteopath", null },
            { "Other Medical Practitioner", "Generic specialist" },
            { "Otorhinolaryngologist", "Otolaryngologist - head and neck surgeon" },
            { "Paediatric Surgeon", "Paediatric surgeon" },
            { "Paediatrician", "Paediatrician" },
            { "Palliative Medicine Specialist", "Palliative medicine physician" },
            { "Pathologist", "Pathologist" },
            { "Perfusionist", null },
            { "Physiotherapist", null },
            { "Plastic and Reconstructive Surgeon", "Plastic surgeon" },
            { "Podiatrist", null },
            { "Psychiatrist", "Psychiatrist" },
            { "Psychotherapist", null },
            { "Public Health Physician", "Public health physician" },
            { "Radiation Oncologist", "Radiation oncologist" },
            { "Rape Crisis Counsellor", null },
            { "Rehabilitation Counsellor", "Rehabilitation physician" },
            { "Rehabilitation Medicine Specialist", "Rehabilitation physician" },
            { "Renal Medicine Specialist", null },
            { "Renal Technician", null },
            { "Reproductive Endocrinologist", "Reproductive endocrinology and infertility specialist" },
            { "Resident Medical Officer", null },
            { "Retail Pharmacist", null },
            { "Rheumatologist", "Rheumatologist" },
            { "Sexual Health Physician", "Sexual health physician" },
            { "Sleep Medicine Specialist", "Respiratory and sleep medicine physician" },
            { "Sleep Technician", null },
            { "Sonographer", null },
            { "Specialist Physician (General Medicine)", "Generic specialist" },
            { "Speech Pathologist", null },
            { "Sport Psychologist", null },
            { "Sports Physician", "Sport and exercise physician" },
            { "Sterilisation Technician", null },
            { "Surgeon (General)", "Surgeon" },
            { "Thoracic Medicine Specialist", null },
            { "Traditional Chinese Medicine Practitioner", null },
            { "Trauma Counsellor", null },
            { "Urogynaecologist", "Urogynaecologist" },
            { "Urologist", "Urologist" },
            { "Vascular Surgeon", "Vascular surgeon" },
            { "Weight Loss Consultant", null },
            { "CHW", null },
            { "Sleep Paediatrician", "Paediatric respiratory and sleep medicine physician" },
            { "Surgery", "Surgeon" },
            { "Pathology", "Pathologist" },
            { "Emergency Department", "Emergency physician" },
            { "Gastroenterology", "Gastroenterologist and hepatologist" },
            { "Paediatric Surgery", "Paediatric surgeon" },
            { "Orthopaedic Surgery", "Orthopaedic surgeon" },
            { "Dentistry", null }
        };

        private static readonly Dictionary<string, string> SpecialtyGenieToSiberiaContact = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "General Medical Practitioner", "General Practitioner" },
            { "Acupuncturist", null },
            { "Anaesthetic Technician", null },
            { "Anaesthetist", "Anaesthetist" },
            { "Audiologist", null },
            { "Audiometrist", null },
            { "Biomedical Engineer", null },
            { "Cardiac Technician", null },
            { "Cardiologist", "Cardiologist" },
            { "Cardiothoracic Surgeon", "Cardio-thoracic surgeon" },
            { "Chiropractor", null },
            { "Clinical Allergist", "Immunologist and allergist" },
            { "Clinical Geneticist", "Clinical geneticist" },
            { "Clinical Haematologist", "Haematologist" },
            { "Clinical Immunologist", "Immunologist" },
            { "Clinical Pharmacologist", null },
            { "Clinical Psychologist", null },
            { "Community Psychologist", null },
            { "Counselling Psychologist", null },
            { "Counsellor", null },
            { "Dental Practitioners", "Dentist" },
            { "Dental Specialist", "Dentist" },
            { "Dentist", "Dentist" },
            { "Dermatologist", "Dermatologist" },
            { "Diagnostic and Interventional Radiologist", "Radiologist" },
            { "Dialysis Technician", null },
            { "Dietitian", null },
            { "Drug and Alcohol Counsellor", "Specialist in addiction medicine" },
            { "Electroencephalographic Technician", null },
            { "Emergency Medicine Specialist", "Emergency physician" },
            { "Endocrinologist", "Endocrinologist" },
            { "ENT Surgeon", "Surgeon" },
            { "Family and Marriage Counsellor", null },
            { "Gambling Counsellor", "Specialist in addiction medicine" },
            { "Gastroenterologist", "Gastroenterologist and hepatologist" },
            { "Geriatrician", "Geriatrician" },
            { "Grief Counsellor", null },
            { "Gynaecological Oncologist", "Gynaecological oncologist" },
            { "Health and Welfare Support Worker", null },
            { "Homeopath", null },
            { "Hospital Pharmacist", null },
            { "Industrial Medicine Specialist", null },
            { "Infectious Diseases Specialist", "Infectious diseases physician" },
            { "Intensive Care Specialist", "Intensive care physician" },
            { "Life Coach", null },
            { "Massage Therapist", null },
            { "Medical Diagnostic Radiographer", "Radiologist" },
            { "Medical Laboratory Scientist", null },
            { "Medical Laboratory Technician", null },
            { "Medical Oncologist", "Medical oncologist" },
            { "Medical Radiation Therapist", "Radiologist" },
            { "Medical Receptionist", "Medical administrator" },
            { "Medical Technicians", null },
            { "Midwife", null },
            { "Natural Remedy Consultant", null },
            { "Naturopath", null },
            { "Neurologist", null },
            { "Neurophysiological Technician", null },
            { "Neurosurgeon", "Neurosurgeon" },
            { "Nuclear Medicine Specialist", "Specialist in nuclear medicine" },
            { "Nuclear Medicine Technologist", "Specialist in nuclear medicine" },
            { "Nurse Practitioner", null },
            { "Obstetrician and Gynaecologist", "Obstetrician and gynaecologist" },
            { "Occupational Medicine Specialist", "Occupational and environmental physician" },
            { "Occupational Therapist", "Occupational and environmental physician" },
            { "Ophthalmologist", null },
            { "Optometrist", null },
            { "Oral and Maxillofacial Surgeon", "Oral and maxillofacial surgeon" },
            { "Orthopaedic Surgeon", "Orthopaedic surgeon" },
            { "Orthoptist", null },
            { "Orthotic and Prosthetic Technician", null },
            { "Orthotist or Prosthetist", null },
            { "Osteopath", null },
            { "Other Medical Practitioner", "Generic specialist" },
            { "Otorhinolaryngologist", "Otolaryngologist - head and neck surgeon" },
            { "Paediatric Surgeon", "Paediatric surgeon" },
            { "Paediatrician", "Paediatrician" },
            { "Palliative Medicine Specialist", "Palliative medicine physician" },
            { "Pathologist", "Pathologist" },
            { "Perfusionist", null },
            { "Physiotherapist", null },
            { "Plastic and Reconstructive Surgeon", "Plastic surgeon" },
            { "Podiatrist", null },
            { "Psychiatrist", "Psychiatrist" },
            { "Psychotherapist", null },
            { "Public Health Physician", null },
            { "Radiation Oncologist", "Radiation oncologist" },
            { "Rape Crisis Counsellor", null },
            { "Rehabilitation Counsellor", "Rehabilitation physician" },
            { "Rehabilitation Medicine Specialist", "Rehabilitation physician" },
            { "Renal Medicine Specialist", null },
            { "Renal Technician", null },
            { "Reproductive Endocrinologist", "Specialist in reproductive endocrinology and infertility" },
            { "Resident Medical Officer", null },
            { "Retail Pharmacist", null },
            { "Rheumatologist", "Rheumatologist" },
            { "Sexual Health Physician", "Sexual health physician" },
            { "Sleep Medicine Specialist", "Respiratory and sleep medicine physician" },
            { "Sleep Technician", null },
            { "Sonographer", null },
            { "Specialist Physician (General Medicine)", "Generic specialist" },
            { "Speech Pathologist", null },
            { "Sport Psychologist", null },
            { "Sports Physician", "Sport and exercise physician" },
            { "Sterilisation Technician", null },
            { "Surgeon (General)", "Surgeon" },
            { "Thoracic Medicine Specialist", null },
            { "Traditional Chinese Medicine Practitioner", null },
            { "Trauma Counsellor", null },
            { "Urogynaecologist", "Urogynaecologist" },
            { "Urologist", "Urologist" },
            { "Vascular Surgeon", "Vascular surgeon" },
            { "Weight Loss Consultant", null },
            { "CHW", null },
            { "Sleep Paediatrician", "Paediatric respiratory and sleep medicine physician" },
            { "Surgery", "Surgeon" },
            { "Pathology", "Pathologist" },
            { "Emergency Department", "Emergency physician" },
            { "Gastroenterology", "Gastroenterologist and hepatologist" },
            { "Paediatric Surgery", "Paediatric surgeon" },
            { "Orthopaedic Surgery", "Orthopaedic surgeon" },
            { "Dentistry", "Dentist" }
        };

        private static readonly Dictionary<string, string> MaritalStatusGenieToSiberia = new Dictionary<string, string>
        {
            { "Single", "Never married" },
        };

        private static readonly Dictionary<string, string> SalutationGenieToSiberia = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Dr", "Dr" },
            { "Prof", "Prof" },
            { "A Prof", "A Prof" },
            { "Mr", "Mr" },
            { "Mrs", "Mrs" },
            { "Ms", "Ms" },
            { "Miss", "Miss" },
            { "Master", "Master" },
            { "Soldier", "Soldier" },
            { "Private", "Private" },
            { "Lance Corporal", "Lance Corporal" },
            { "Corporal", "Corporal" },
            { "Sergeant", "Sergeant" },
            { "Staff Sergeant", "Staff Sergeant" },
            { "Warrant Officer", "Warrant Officer" },
            { "Lieutenant", "Lieutenant" },
            { "Captain", "Captain" },
            { "Major", "Major" },
            { "Lieutenant Colonel", "Lieutenant Colonel" },
            { "Colonel", "Colonel" },
            { "Sister", "Sister" },
            { "Father", "Father" },
        };

        private static float? NullIfZero(float value)
        {
            return value == 0 ? (float?)null : value;
        }

        private static int? NullIfZero(int value)
        {
            return value == 0 ? (int?)null : value;
        }

        private static string GetTextFieldValue(GenieChecklistField field, GenieChecklistField templateField)
        {
            var fieldData = templateField != null
                ? field.FieldData.Replace(templateField.FieldData, string.Empty)
                : field.FieldData;

            return fieldData.Trim(';', ' ');
        }

        private static string GetNumberFieldValue(GenieChecklistField field, GenieChecklistField templateField)
        {
            var data = field.FieldData != null ? field.FieldData.Split(';') : new string[] { };
            if (data.Length > 0)
            {
                return data[0];
            }

            return string.Empty;
        }

        private static string GetCheckboxFieldValue(GenieChecklistField field, GenieChecklistField templateField)
        {
            var data = field.FieldData != null ? field.FieldData.Split(';') : new string[] { };
            if (data.Length > 0)
            {
                bool selectedFlag;
                if (bool.TryParse(data[0], out selectedFlag) && selectedFlag)
                {
                    return "Yes";
                }

                return "No";
            }

            return string.Empty;
        }

        private static string GetOriginalFieldValue(GenieChecklistField field, GenieChecklistField templateField)
        {
            return field.FieldData;
        }

        private static MigrationDictionaryItem[] GetRadioFieldValues(GenieChecklistField field, GenieChecklistField templateField)
        {
            var data = field.FieldData.Replace("True", "Yes").Replace("False", "No");
            return data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select((x, index) => new MigrationDictionaryItem
                {
                    Id = index,
                    Text = x,
                })
                .ToArray();
        }

        private static MigrationDictionaryItem[] GetFieldNull(GenieChecklistField field, GenieChecklistField templateField)
        {
            return null;
        }

        private static Func<GenieChecklistField, GenieChecklistField, MigrationDictionaryItem[]> GetFieldSingleValue(Func<GenieChecklistField, GenieChecklistField, string> transform)
        {
            return (field, templateField) => new MigrationDictionaryItem[]
            {
                new MigrationDictionaryItem
                {
                    Id = 0,
                    Text = transform(field, templateField),
                }
            };
        }

        private static int[] GetSelectedValues(GenieChecklistField field)
        {
            var controlType = ControlTypesMap[field.Type];
            if (controlType == MigrationChecklistControlType.RadioButtonList)
            {
                var parts = field.FieldData.Split(';');
                if (parts.Length == 3)
                {
                    bool selectedFlag;
                    if (bool.TryParse(parts[0], out selectedFlag))
                    {
                        return new[] { selectedFlag ? 0 : 1 };
                    }
                }
            }
            else if (controlType == MigrationChecklistControlType.CheckBoxList)
            {
                var parts = field.FieldData.Split(';');
                if (parts.Length > 0)
                {
                    var selectedFlag = string.IsNullOrEmpty(parts[0]) || bool.Parse(parts[0]);
                    return selectedFlag ? new[] { 0 } : new int[] { };
                }
            }
            return new int[] { };
        }

        internal static MigrationUser ToMigrationUser(this GenieUser genieUser)
        {
            var parsedName = genieUser.Name.Replace(".", string.Empty).ParseName(SalutationGenieToSiberia.Keys.ToArray());
            return new MigrationUser
            {
                ExternalId = genieUser.Id.ToString(),
                FirstName = genieUser.FirstName.NullIfEmpty() ?? parsedName.FirstName,
                Surname = genieUser.Surname.NullIfEmpty() ?? parsedName.LastName,
                MiddleName = genieUser.MiddleName,
                IsReceptionist = genieUser.Specialty.ToIsReceptionist(),
                IsSpecialist = genieUser.Specialty.ToIsSpecialist(),
                Specialty = genieUser.Specialty.Translate(SpecialtyGenieToSiberiaDoctor),
                Salutation = (genieUser.Title.NullIfEmpty() ?? parsedName.Salutation.NullIfEmpty() ?? "Mr").Translate(SalutationGenieToSiberia),
            };
        }

        internal static MigrationRecall ToMigrationRecall(this GenieRecall genieRecall)
        {
            return new MigrationRecall
            {
                ExternalId = genieRecall.Id.ToString(),
                PatientExternalId = genieRecall.PatientId.ToString(),
                Reason = genieRecall.Reason,
                IsCompleted = genieRecall.IsCompleted,
                LastActionDetails = genieRecall.LastActionDetails,
                DueDate = genieRecall.DueDate,
                PatientPhone = genieRecall.PatientPhone,
                LastActionDate = genieRecall.LastActionDate,
                NextAppointmentDate = genieRecall.NextAppointmentDate,
                DoctorFullName = genieRecall.DoctorFullName,
                DateEntered = genieRecall.DateEntered,
                RecurrenceInterval = genieRecall.RecurrenceInterval,
            };
        }

        internal static MigrationContact ToMigrationContact(GenieContact genieContact)
        {
            return new MigrationContact
            {
                ExternalId = genieContact.Id.ToString(),
                AddressLine1 = genieContact.Address1,
                AddressLine2 = genieContact.Address2,
                Alltalk = genieContact.AllTalk,
                Argus = genieContact.Argus,
                Category = genieContact.Category,
                CompanyName = genieContact.Clinic,
                Country = genieContact.Country,
                DivisionReport = genieContact.DivisionReport,
                Email = genieContact.EmailAddress,
                Fax = genieContact.Fax,
                FirstName = genieContact.FirstName,
                HealthLink = genieContact.HealthLink,
                HomePhone = genieContact.Homephone,
                HPI = genieContact.Hpii,
                LastName = genieContact.Surname,
                MedicalObjects = genieContact.MedicalObject,
                MiddleName = genieContact.Initial,
                MobilePhone = genieContact.Mobile,
                ProviderNo = genieContact.ProviderNum,
                ReferralNet = genieContact.ReferralNet,
                Salutation = genieContact.Title,
                State = genieContact.State,
                Suburb = genieContact.Suburb,
                PostCode = genieContact.PostCode,
                WorkPhone = genieContact.WorkPhone,
                IsAssistant = genieContact.IsAssistant ?? false,
                IsSpecialist = genieContact.Specialty.ToIsSpecialist(),
                Specialty = genieContact.Specialty.Translate(SpecialtyGenieToSiberiaContact),
            };
        }

        internal static MigrationPatient ToMigrationPatient(
            GeniePatient geniePatient,
            Dictionary<int, GenieAccountHolder> accountHolders,
            Dictionary<int, GenieCurrentProblem[]> currentProblems,
            Dictionary<int, GeniePastHistory[]> pastHistory,
            Dictionary<int, GenieDocument[]> notes)
        {
            string guardianName = null;
            string guardianSurname = null;
            string guardianHomePhone = null;
            string guardianState = null;

            var accountHolder = accountHolders.GetValueOrNull(geniePatient.AccountHolderId);
            if (accountHolder != null)
            {
                guardianName = accountHolder.FirstName;
                guardianSurname = accountHolder.Surname;
                guardianHomePhone = accountHolder.HomePhone;
                guardianState = accountHolder.State;
            }

            var previousIssues = pastHistory
                .GetValueOrNull(geniePatient.Id)
                .AsSeparatedString(
                    x => new[]
                    {
                        x.CreationDate.AsDisplayDateString().WithFieldName("Date"),
                        x.History.WithFieldName("History"),
                        x.Note.WithFieldName("Note"),
                        x.Confidential ? "Confidential" : "Non confidential"
                    },
                    ",",
                    Environment.NewLine);

            var backgroundInfo = currentProblems
                .GetValueOrNull(geniePatient.Id)
                .AsSeparatedString(
                    x => new[]
                    {
                        x.DiagnosisDate.AsDisplayDateString().WithFieldName("Date"),
                        x.Problem.WithFieldName("Problem"),
                        x.Note.WithFieldName("Note"),
                        x.Confidential ? "Confidential" : "Non confidential"
                    },
                    ",",
                    Environment.NewLine);
            if (!string.IsNullOrWhiteSpace(geniePatient.Scratchpad))
            {
                backgroundInfo = geniePatient.Scratchpad.WithFieldName("Scratchpad")
                                 + Environment.NewLine + backgroundInfo;
            }

            var combinedNotes = notes
                .GetValueOrNull(geniePatient.Id)
                .AsSeparatedString(
                    x => new[]
                    {
                        x.Id.ToString().WithFieldName("Note"),
                        x.Title.WithFieldName("Title"),
                        x.DocumentDate.AsDisplayDateString().WithFieldName("Created"),
                        x.DateModified.AsDisplayDateString().WithFieldName("Modified"),
                        x.Note.WithFieldName(x.IsPrimary ? "Primary note" : "Secondary note"),
                    },
                    "," + Environment.NewLine,
                    ";" + Environment.NewLine);

            return new MigrationPatient
            {
                ExternalId = geniePatient.Id.ToString(),
                AccountType = geniePatient.AccountType.Translate(AccountTypeGenieToSiberia),
                AddressLine1 = geniePatient.AddressLine1,
                AddressLine2 = geniePatient.AddressLine2,
                Country = geniePatient.Country,
                CountryOfBirth = geniePatient.CountryOfBirth,
                DateOfBirth = geniePatient.Dob,
                DateOfDecease = geniePatient.Dod,
                IsDeceased = geniePatient.Deceased,
                ChartNumber = geniePatient.ChartNumber,
                DrinkingNotes = geniePatient.AlcoholInfo,
                Drinking = geniePatient.Alcohol,
                Email = geniePatient.EmailAddress,
                FirstName = geniePatient.FirstName,
                Gender = string.Equals(geniePatient.Sex, "M", StringComparison.InvariantCultureIgnoreCase),
                HomePhone = geniePatient.HomePhone,
                AccountHolderExternalId = geniePatient.AccountHolderId > 0 ? geniePatient.AccountHolderId.ToString() : null,
                UsualGpExternalId = geniePatient.UsualGpId > 0 ? geniePatient.UsualGpId.ToString() : null,
                UsualProvider = geniePatient.UsualProvider,
                GuardianName = guardianName,
                GuardianSurname = guardianSurname,
                GuardianHomePhone = guardianHomePhone,
                GuardianState = guardianState,
                Language = geniePatient.Language,
                LastName = geniePatient.Surname,
                MaidenName = geniePatient.MaidenName,
                KnownAs = geniePatient.KnownAs,
                MaritalStatus = geniePatient.MaritalStatus.Translate(MaritalStatusGenieToSiberia),
                MedicareExpiryDate = geniePatient.MedicareExpiry,
                MedicareNum = geniePatient.MedicareNum,
                MedicareRefNum = geniePatient.MedicareRefNum,
                MiddleName = geniePatient.MiddleName,
                MobilePhone = geniePatient.MobilePhone,
                PartnerName = geniePatient.PartnerName,
                PostCode = geniePatient.PostCode,
                SmokingNotes = geniePatient.SmokingInfo,
                SmokingStatus = geniePatient.SmokingFreq.ToString().Translate(SmokingStatusGenieToSiberia),
                State = geniePatient.State,
                Suburb = geniePatient.Suburb,
                WorkPhone = geniePatient.WorkPhone,
                DoNotSendSms = geniePatient.DoNotSendSms,
                DvaCardColour = geniePatient.DvaCardColour,
                DvaDisability = geniePatient.DvaDisability,
                DvaNumber = geniePatient.DvaNumber,
                HealthFundNumber = geniePatient.HealthFundNumber,
                HealthFundCode = geniePatient.HealthFundName.Translate(HealthFundGenieToSiberia),
                HealthFundName = geniePatient.HealthFundName,
                HccPensionNumber = geniePatient.HccPensionNumber,
                HccPensionExpiry = geniePatient.HccPensionExpiry.AsInvariantString(),
                BackgroundInfo = backgroundInfo,
                PreviousIssues = previousIssues,
                NextOfKinName = geniePatient.NokName,
                NextOfKinContactPhone = geniePatient.NokPhone,
                Notes = string.Join(";" + Environment.NewLine, geniePatient.Memo.WithFieldName("Memo"), combinedNotes),
                Salutation = geniePatient.Title.Translate(SalutationGenieToSiberia),
            };
        }

        internal static MigrationAccountHolder ToMigrationAccountHolder(GenieAccountHolder genieAccountHolder, Dictionary<int, int[]> patients)
        {
            return new MigrationAccountHolder
            {
                ExternalId = genieAccountHolder.Id.ToString(),
                AddressLine1 = genieAccountHolder.AddressLine1,
                AddressLine2 = genieAccountHolder.AddressLine2,
                DateOfBirth = genieAccountHolder.DateOfBirth,
                FirstName = genieAccountHolder.FirstName,
                HomePhone = genieAccountHolder.HomePhone,
                Individual = genieAccountHolder.Individual,
                MedicareExpiryDate = genieAccountHolder.MedicareExpiry,
                MedicareNum = genieAccountHolder.MedicareNum,
                MedicareRefNum = genieAccountHolder.MedicareRefNum,
                Organisation = genieAccountHolder.Organisation,
                PostCode = genieAccountHolder.PostCode,
                State = genieAccountHolder.State,
                Suburb = genieAccountHolder.Suburb,
                LastName = genieAccountHolder.Surname,
                Salutation = genieAccountHolder.Title,
                PatientExternalIds = patients.GetValueOrDefault(genieAccountHolder.Id, new int[] { }).Select(x => x.ToString()).ToArray(),
            };
        }

        internal static MigrationOnGHistoryRecord ToMigrationOnGHistoryRecord(this GenieObstetricHistory genieObstetricHistoryRecord)
        {
            var result = new MigrationOnGHistoryRecord
            {
                ExternalId = genieObstetricHistoryRecord.Id.ToString(),
                PatientExternalId = genieObstetricHistoryRecord.PatientId.ToString(),
                DeliveryDate = genieObstetricHistoryRecord.DeliveryDate,
                Place = genieObstetricHistoryRecord.Place,
                Week = genieObstetricHistoryRecord.Week,
                Pregnancy = genieObstetricHistoryRecord.Pregnancy,
                Labour = genieObstetricHistoryRecord.Labour,
                Analgesia = genieObstetricHistoryRecord.Analgesia,
                Delivery = genieObstetricHistoryRecord.Delivery,
                Notes = genieObstetricHistoryRecord.Note,
                BreastFed = genieObstetricHistoryRecord.BreastFed,
                Induction = genieObstetricHistoryRecord.Induction,
            };

            var babies = new List<MigrationOnGBaby>();

            if (!string.IsNullOrWhiteSpace(genieObstetricHistoryRecord.Result))
            {
                babies.Add(new MigrationOnGBaby
                {
                    Name = genieObstetricHistoryRecord.Name,
                    Sex = genieObstetricHistoryRecord.Sex,
                    Result = genieObstetricHistoryRecord.Result.Translate(BirthResultGenieToSiberia),
                    Weight = genieObstetricHistoryRecord.Weight,
                });
            }

            if (!string.IsNullOrWhiteSpace(genieObstetricHistoryRecord.Result2))
            {
                babies.Add(new MigrationOnGBaby
                {
                    Name = genieObstetricHistoryRecord.Name2,
                    Sex = genieObstetricHistoryRecord.Sex2,
                    Result = genieObstetricHistoryRecord.Result2.Translate(BirthResultGenieToSiberia),
                    Weight = genieObstetricHistoryRecord.Weight2,
                });
            }

            if (!string.IsNullOrWhiteSpace(genieObstetricHistoryRecord.Result3))
            {
                babies.Add(new MigrationOnGBaby
                {
                    Name = genieObstetricHistoryRecord.Name3,
                    Sex = genieObstetricHistoryRecord.Sex3,
                    Result = genieObstetricHistoryRecord.Result3.Translate(BirthResultGenieToSiberia),
                    Weight = genieObstetricHistoryRecord.Weight3,
                });
            }

            if (!string.IsNullOrWhiteSpace(genieObstetricHistoryRecord.Result4))
            {
                babies.Add(new MigrationOnGBaby
                {
                    Name = genieObstetricHistoryRecord.Name4,
                    Sex = genieObstetricHistoryRecord.Sex4,
                    Result = genieObstetricHistoryRecord.Result4.Translate(BirthResultGenieToSiberia),
                    Weight = genieObstetricHistoryRecord.Weight4,
                });
            }

            result.Babies = babies.ToArray();
            return result;
        }

        internal static MigrationOnGRecord ToMigrationOnGRecord(this GeniePregnancy geniePregnancy, Dictionary<string, int> userIds)
        {
            if (geniePregnancy.DeliveryTime.HasValue)
            {
                geniePregnancy.DeliveryDate = geniePregnancy.DeliveryDate.HasValue
                    ? geniePregnancy.DeliveryDate.Value.AddMinutes(geniePregnancy.DeliveryTime.Value.TotalMinutes)
                    : geniePregnancy.DeliveryDate;
            }

            var result = new MigrationOnGRecord
            {
                ExternalId = geniePregnancy.Id.ToString(),
                PatientExternalId = geniePregnancy.PatientId.ToString(),
                Lmp = geniePregnancy.Lmp,
                Edd = geniePregnancy.Edd,
                CurrentIssues = geniePregnancy.Problems,
                EddAgreed = geniePregnancy.EddAgreed,
                Cvs = geniePregnancy.Cvs,
                Amnio = geniePregnancy.Amnio,
                Chest = geniePregnancy.Chest,
                Breasts = geniePregnancy.Breasts,
                Abdo = geniePregnancy.Abdo,
                Pv = geniePregnancy.Pv,
                Pap = geniePregnancy.Pap,
                BloodGroup = geniePregnancy.BloodGroup.Replace(" ", string.Empty),
                Hb = geniePregnancy.Hb,
                Mcv = geniePregnancy.Mcv,
                Rubella = geniePregnancy.Rubella,
                Hbv = geniePregnancy.Hbv,
                Hcv = geniePregnancy.Hcv,
                Hiv = geniePregnancy.Hiv,
                Syphilis = geniePregnancy.Syphilis,
                Msu = geniePregnancy.Msu,
                Parvo = geniePregnancy.Parvo,
                Toxo = geniePregnancy.Toxo,
                Cmv = geniePregnancy.Cmv,
                Varicella = geniePregnancy.Varicella,
                Uss1Weeks = geniePregnancy.Uss1Weeks,
                Uss1Comment = geniePregnancy.Uss1Comment,
                Uss2Weeks = geniePregnancy.Uss2Weeks,
                Uss2Comment = geniePregnancy.Uss2Comment,
                Uss3Weeks = geniePregnancy.Uss3Weeks,
                Uss3Comment = geniePregnancy.Uss3Comment,
                Hb28Wks = geniePregnancy.Hb28Weeks,
                Hb36Wks = geniePregnancy.Hb36Weeks,
                Gct28Wks = geniePregnancy.Gct28Weeks,
                Antibodies28Wks = geniePregnancy.Antibodies28Weeks,
                Antibodies36Wks = geniePregnancy.Antibodies36Weeks,
                Gbs28Wks = geniePregnancy.Gbs28Weeks,
                Gbs36Wks = geniePregnancy.Gbs36Weeks,
                Notes = geniePregnancy.Notes,
                Declined = geniePregnancy.Declined,
                DeliveryDateTime = geniePregnancy.DeliveryDate,
                DeliveryMethod = geniePregnancy.DeliveryMethod,
                BirthRecordNotes = geniePregnancy.DeliveryNotes,
                AccoucherExternalId = geniePregnancy.AccoucherId.ToString(),
                PaediatricianExternalId = geniePregnancy.PaediatricianId.ToString(),
                AnaesthetistExternalId = geniePregnancy.AnaesthetistId.ToString(),
                PostnatalDate = geniePregnancy.PostnatalDate,
                Lochia = geniePregnancy.Lochia,
                PerineumState = geniePregnancy.PerineumState,
                Bladder = geniePregnancy.Bladder,
                Bowel = geniePregnancy.Bowel,
                BreastState = geniePregnancy.BreastState,
                PnBreasts = geniePregnancy.PnBreasts,
                PnAbdo = geniePregnancy.PnAbdo,
                PnPerineum = geniePregnancy.PnPerineum,
                Breastfeeding = geniePregnancy.Breastfeeding,
                Speculum = geniePregnancy.Speculum,
                PostnatalPv = geniePregnancy.PostnatalPv,
                Contraception = geniePregnancy.Contraception,
                PostnatalNotes = geniePregnancy.PostnatalNotes,
                ObstetricHistoryRecordExternalId = geniePregnancy.ObstetricHistoryRecordId <= 0 ? null : geniePregnancy.ObstetricHistoryRecordId.ToString(),
                CurrentGestation = geniePregnancy.CurrentGestation,
                Uss1Date = geniePregnancy.Uss1Date,
                Uss2Date = geniePregnancy.Uss2Date,
                Uss3Date = geniePregnancy.Uss3Date,
                AbsInitial = geniePregnancy.AbsInitial,
                Placenta = geniePregnancy.Placenta,
                AntiD = geniePregnancy.AntiD,
                Hospital = geniePregnancy.Hospital,
                Provider = geniePregnancy.Provider,
                BoyOrGirl = geniePregnancy.BoyOrGirl,
                Ferritin = geniePregnancy.Ferritin,
                Tfts = geniePregnancy.Tfts,
                AntiD28 = geniePregnancy.AntiD28,
                AntiD36 = geniePregnancy.AntiD36,
                Gtt282Hr = geniePregnancy.Gtt282Hr,
                TwentyWeekFee = geniePregnancy.TwentyWeekFee,
                ThirtyWeekFee = geniePregnancy.ThirtyWeekFee,
                BreastfeedingAtDelivery = geniePregnancy.BreastfeedingAtDelivery,
                AdditionalAnNotes = geniePregnancy.AdditionalAnNotes,
                Bp = geniePregnancy.Bp,
                PlacentalPosition = geniePregnancy.PlacentalPosition,
                VitaminD = geniePregnancy.VitaminD,
                Trisomy18 = geniePregnancy.Trisomy18,
                Trisomy21 = geniePregnancy.Trisomy21,
                Ebl = geniePregnancy.Ebl,
                Gtt28Fasting = geniePregnancy.Gtt28Fasting,
                NextAppointmentDate = geniePregnancy.NextAppointmentDate,
                Anaesthetist = geniePregnancy.Anaesthetist,
                Paediatrician = geniePregnancy.Paediatrician,
                Accoucher = geniePregnancy.Accoucher,
                Platelets28 = geniePregnancy.Platelets28,
                Platelets36 = geniePregnancy.Platelets36,
                InitialTestDate = geniePregnancy.InitialTestDate,
                NeonatalExam = geniePregnancy.NeonatalExam,
                Perineum = geniePregnancy.Perineum.Translate(PerineumGenieToSiberia),
                UserExternalId = userIds.ContainsKey(geniePregnancy.Provider) ? userIds[geniePregnancy.Provider].ToString() : null
            };

            result.Analgesia = geniePregnancy.AnalgesiaNil
                ? "Nil"
                : geniePregnancy.AnalgesiaNitrous
                    ? "Nitrous Oxide"
                    : geniePregnancy.AnalgesiaPethidine
                        ? "Pethidine"
                        : geniePregnancy.AnalgesiaEpidural
                            ? "Epidural"
                            : geniePregnancy.AnalgesiaSpinal
                                ? "Spinal"
                                : geniePregnancy.AnalgesiaGa
                                    ? "GA"
                                    : "Not Provided";

            result.Labour = geniePregnancy.LabourNil
                ? "Nil"
                : geniePregnancy.LabourSpontaneous
                    ? "Spontaneous"
                    : geniePregnancy.LabourProstin
                        ? "Prostin"
                        : geniePregnancy.LabourArm
                            ? "ARM"
                            : geniePregnancy.LabourSyntocinon
                                ? "Syntocinon"
                                : "Not Provided";

            var babies = new List<MigrationOnGBaby>();

            if (!string.IsNullOrWhiteSpace(geniePregnancy.Result))
            {
                babies.Add(new MigrationOnGBaby
                {
                    Name = geniePregnancy.BabyName,
                    Apgar1 = geniePregnancy.Apgar1,
                    Apgar2 = geniePregnancy.Apgar2,
                    Sex = geniePregnancy.BabySex,
                    Result = geniePregnancy.Result.Translate(BirthResultGenieToSiberia),
                    Weight = geniePregnancy.BabyWeight
                });
            }

            if (!string.IsNullOrWhiteSpace(geniePregnancy.Result2))
            {
                babies.Add(new MigrationOnGBaby
                {
                    Name = geniePregnancy.BabyName2,
                    Apgar1 = geniePregnancy.Baby2Apgar1,
                    Apgar2 = geniePregnancy.Baby2Apgar2,
                    Sex = geniePregnancy.BabySex2,
                    Result = geniePregnancy.Result2.Translate(BirthResultGenieToSiberia),
                    Weight = geniePregnancy.BabyWeight2
                });
            }

            if (!string.IsNullOrWhiteSpace(geniePregnancy.Result3))
            {
                babies.Add(new MigrationOnGBaby
                {
                    Name = geniePregnancy.BabyName3,
                    Apgar1 = geniePregnancy.Baby3Apgar1,
                    Apgar2 = geniePregnancy.Baby3Apgar2,
                    Sex = geniePregnancy.BabySex3,
                    Result = geniePregnancy.Result3.Translate(BirthResultGenieToSiberia),
                    Weight = geniePregnancy.BabyWeight3
                });
            }

            if (!string.IsNullOrWhiteSpace(geniePregnancy.Result4))
            {
                babies.Add(new MigrationOnGBaby
                {
                    Name = geniePregnancy.BabyName4,
                    Apgar1 = geniePregnancy.Baby4Apgar1,
                    Apgar2 = geniePregnancy.Baby4Apgar2,
                    Sex = geniePregnancy.BabySex4,
                    Result = geniePregnancy.Result4.Translate(BirthResultGenieToSiberia),
                    Weight = geniePregnancy.BabyWeight4
                });
            }

            result.Babies = babies.ToArray();

            return result;
        }

        internal static MigrationOnGAntenatal ToMigrationAntenatalVisit(this GenieAntenatalVisit antenatalVisit, Dictionary<string, int> userIds)
        {
            if (antenatalVisit == null)
            {
                return null;
            }

            return new MigrationOnGAntenatal
            {
                ExternalId = antenatalVisit.Id.ToString(),
                PatientExternalId = antenatalVisit.PatientId.ToString(),
                OnGRecordExternalId = antenatalVisit.PregnancyId.ToString(),
                UserExternalId = userIds.ContainsKey(antenatalVisit.UserName) ? userIds[antenatalVisit.UserName].ToString() : null,
                VisitDate = antenatalVisit.VisitDate,
                Gestation = antenatalVisit.Gestation,
                Fundus = antenatalVisit.Fundus,
                Weight = antenatalVisit.Weight,
                Urine = antenatalVisit.Urine,
                Bp = antenatalVisit.Bp,
                LiquorVolume = antenatalVisit.LiquorVolume,
                Oedema = antenatalVisit.Oedema,
                Presentation = antenatalVisit.Presentation,
                Station = antenatalVisit.Station,
                Fm = antenatalVisit.Fm,
                Fh = antenatalVisit.Fh,
                Note = antenatalVisit.Note
            };
        }

        internal static MigrationReferral ToMigrationReferral(GenieReferral genieReferral, Dictionary<string, int> users)
        {
            return new MigrationReferral
            {
                ExternalId = genieReferral.Id.ToString(),
                ContactExternalId = genieReferral.ContactId.ToString(),
                Duration = genieReferral.Duration,
                IssueDate = genieReferral.IssueDate,
                PatientExternalId = genieReferral.PatientId.ToString(),
                ReferralDate = genieReferral.ReferralDate,
                DoctorExternalId = users.ContainsKey(genieReferral.ReferredTo) ? users[genieReferral.ReferredTo].ToString() : null,
            };
        }

        internal static MigrationAppointmentType ToMigrationAppointmentType(GenieAppointmentType genieAppointmentType)
        {
            return new MigrationAppointmentType
            {
                ExternalId = genieAppointmentType.Id.ToString(),
                Colour = ColorTranslator.ToHtml(Color.FromArgb(genieAppointmentType.Colour)),
                Duration = genieAppointmentType.Duration,
                Name = genieAppointmentType.Note,
            };
        }

        internal static MigrationAppointment ToMigrationAppointment(GenieAppointment genieAppointment, Dictionary<string, int> users, Dictionary<string, int> appointmentTypes)
        {
            var startDateTime = (genieAppointment.StartDate ?? DateTime.Now.Date).Add(genieAppointment.StartTime ?? DateTime.Now.TimeOfDay);
            var endDateTime = startDateTime.AddSeconds(genieAppointment.Duration);
            return new MigrationAppointment
            {
                ExternalId = genieAppointment.Id.ToString(),
                AppointmentTypeExternalId = appointmentTypes.ContainsKey(genieAppointment.Reason) ? appointmentTypes[genieAppointment.Reason].ToString() : null,
                CreatedByExternalId = users.ContainsKey(genieAppointment.CreatedBy) ? users[genieAppointment.CreatedBy].ToString() : null,
                CreationDate = genieAppointment.CreationDate ?? DateTime.Now,
                Description = genieAppointment.Note,
                UserExternalId = genieAppointment.ProviderId.ToString(),
                Name = genieAppointment.Name,
                PatientExternalId = genieAppointment.PatientId.ToString(),
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
            };
        }

        private static MigrationDictionaryItem[] GetFieldNull(GenieChecklistField field)
        {
            return null;
        }

        internal static MigrationDictionaryItem[] GetRadioTemplateFieldValues(GenieChecklistField field)
        {
            return field.FieldData
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select((x, index) => new MigrationDictionaryItem
                {
                    Id = index,
                    Text = x,
                })
                .ToArray();
        }

        internal static MigrationChecklistControlType GetChecklistControlTypeId(string value)
        {
            return ControlTypesMap.ContainsKey(value) ? ControlTypesMap[value] : 0;
        }

        internal static MigrationDictionaryItem[] GetValues(GenieChecklistField field, GenieChecklistField templateField)
        {
            return PatientFieldValueMap.ContainsKey(field.Type) ? PatientFieldValueMap[field.Type](field, templateField) : new MigrationDictionaryItem[] { };
        }

        internal static MigrationDictionaryItem[] GetChecklistTemplateValues(GenieChecklistField field)
        {
            return DoctorFieldValueMap.ContainsKey(field.Type) ? DoctorFieldValueMap[field.Type](field) : new MigrationDictionaryItem[] { };
        }

        internal static MigrationQuoteItem ToMigrationQuoteItem(GenieQuoteItem genieQuoteItem)
        {
            return new MigrationQuoteItem()
            {
                ExternalId = genieQuoteItem.Id.ToString(),
                ItemNumber = genieQuoteItem.ItemNumber,
                Description = genieQuoteItem.Description,
                Number = genieQuoteItem.Number,
                Rebate = (int)(Math.Round(genieQuoteItem.Rebate, 2, MidpointRounding.AwayFromZero) * 100),
                Fraction = (int)(Math.Round(genieQuoteItem.Fraction, 2, MidpointRounding.AwayFromZero) * 100),
                Fee = (int)(Math.Round(genieQuoteItem.Fee, 2, MidpointRounding.AwayFromZero) * 100),
                Gst = (int)(Math.Round(genieQuoteItem.Gst, 2, MidpointRounding.AwayFromZero) * 100),
                KnownGap = (int)(Math.Round(genieQuoteItem.KnownGap, 2, MidpointRounding.AwayFromZero) * 100),
                AmaFee = (int)(Math.Round(genieQuoteItem.AmaFee, 2, MidpointRounding.AwayFromZero) * 100),
                Icd10Code = genieQuoteItem.Icd10Code,
                AssistantBillable = genieQuoteItem.AssistantBillable,
                OpReportExternalId = genieQuoteItem.OpReportId
            };
        }

        internal static MigrationChecklist ToMigrationChecklist(GenieChecklist genieChecklist, Dictionary<string, int> users, IDictionary<int, GenieChecklistField[]> genieChecklistFields, IDictionary<int, GenieChecklistField> genieChecklistTemplateFields)
        {
            return new MigrationChecklist
            {
                ExternalId = genieChecklist.Id.ToString(),
                Name = genieChecklist.Name,
                CreatedDate = genieChecklist.DateCreated ?? DateTime.Today,
                DoctorExternalId = users.ContainsKey(genieChecklist.Provider) ? users[genieChecklist.Provider].ToString() : null,
                PatientExternalId = genieChecklist.PatientId.ToString(),
                Items = genieChecklistFields.ContainsKey(genieChecklist.Id)
                    ? genieChecklistFields[genieChecklist.Id].Select(
                        (x, index) => new MigrationChecklistItem
                        {
                            SortOrder = index,
                            ControlName = x.Label,
                            ControlType = GetChecklistControlTypeId(x.Type),
                            Values = GetValues(
                                x,
                                genieChecklistTemplateFields.ContainsKey(x.ChecklistFieldId)
                                    ? genieChecklistTemplateFields[x.ChecklistFieldId]
                                    : null),
                            SelectedValueIds = GetSelectedValues(x),
                        }).ToArray()
                    : new MigrationChecklistItem[] { },
            };
        }

        internal static MigrationChecklistTemplate ToMigrationChecklistTemplate(GenieChecklist genieChecklistTemplate, IDictionary<int, GenieChecklistField[]> genieChecklistFields)
        {
            return new MigrationChecklistTemplate
            {
                ExternalId = genieChecklistTemplate.Id.ToString(),
                Name = genieChecklistTemplate.Name,
                Items = genieChecklistFields.ContainsKey(genieChecklistTemplate.Id)
                    ? genieChecklistFields[genieChecklistTemplate.Id]
                        .Select(x => new MigrationChecklistTemplateItem
                        {
                            ControlName = x.Label,
                            ControlType = GetChecklistControlTypeId(x.Type),
                            Values = GetChecklistTemplateValues(x),
                        }).ToArray()
                    : new MigrationChecklistTemplateItem[] { },
            };
        }

        internal static MigrationConsult ToMigrationConsult(
            GenieConsult genieConsult,
            Dictionary<int, GenieConsultProblem[]> consultProblems)
        {
            var presentComplaints = consultProblems
                .GetValueOrNull(genieConsult.Id)
                .AsSeparatedString(
                    x => new[]
                    {
                        x.IsPrimaryProblem ? "Primary" : string.Empty,
                        x.Problem,
                    },
                    ": ",
                    ", ");

            var consultDate = new DateTime(1900, 1, 1);
            if (genieConsult.ConsultDate.HasValue)
            {
                consultDate = genieConsult.ConsultDate.Value;
            }
            return new MigrationConsult
            {
                ExternalId = genieConsult.Id.ToString(),
                ClinicalAssessment = genieConsult.Examination,
                StartConsult = consultDate.Add(genieConsult.ConsultTime),
                Findings = genieConsult.Diagnosis,
                History = genieConsult.History,
                Management = genieConsult.Plan,
                PresentComplaints = presentComplaints,
                PatientExternalId = genieConsult.PatientId.ToString(),
                UserExternalId = genieConsult.DoctorId.ToString(),
            };
        }

        internal static MigrationAllergy ToMigrationAllergy(IGrouping<int, string> genieAllergies)
        {
            return new MigrationAllergy
            {
                Allergies = genieAllergies.ToArray(),
                PatientExternalId = genieAllergies.Key.ToString(),
            };
        }

        internal static MigrationMeasurement ToMigrationMeasurement(GenieMeasurement genieMeasurement)
        {
            return new MigrationMeasurement
            {
                ExternalId = genieMeasurement.Id.ToString(),
                PatientExternalId = genieMeasurement.PatientId.ToString(),
                CreatedDate = genieMeasurement.MeasurementDate,
                Height = NullIfZero(genieMeasurement.Height),
                Weight = NullIfZero(genieMeasurement.Weight),
                HeadCircumference = NullIfZero(genieMeasurement.HeadCircumference),
                Waist = NullIfZero(genieMeasurement.Waist),
                BodyMassIndex = NullIfZero(genieMeasurement.Bmi),
                Hips = NullIfZero(genieMeasurement.Hip),
                HeartRate = NullIfZero((int)Math.Round(genieMeasurement.HeartRate)),
                BloodPressureDown = NullIfZero((int)Math.Round(genieMeasurement.Diastolic)),
                BloodPressureUp = NullIfZero((int)Math.Round(genieMeasurement.Systolic)),
            };
        }

        internal static MigrationTask ToMigrationTask(GenieTask genieTask, Dictionary<string, int> users)
        {
            return new MigrationTask
            {
                ExternalId = genieTask.Id.ToString(),
                CreatedDate = genieTask.DateCreated ?? new DateTime(1900, 1, 1),
                Subject = genieTask.Task + ": " + genieTask.Note,
                DueDate = genieTask.TaskDate.HasValue ? genieTask.TaskDate.Value.Add(genieTask.TaskTime) : DateTime.MaxValue,
                FromUserExternalId = users.ContainsKey(genieTask.Creator) ? users[genieTask.Creator].ToString() : null,
                IsFinished = genieTask.Completed,
                IsUrgent = genieTask.UrgentFg,
                PatientExternalId = genieTask.PatientId > 0 ? genieTask.PatientId.ToString() : null,
                ToUserExternalId = users.ContainsKey(genieTask.TaskFor) ? users[genieTask.TaskFor].ToString() : null,
            };
        }

        internal static MigrationLetter ToMigrationLetter(GenieOutgoingLetter genieLetter, Dictionary<string, int> users, Func<string, string> convert)
        {
            var status = MigrationLetterStatus.Sent;

            if (genieLetter.ReadyToSend || genieLetter.Reviewed)
            {
                status = MigrationLetterStatus.Signed;
            }

            return new MigrationLetter
            {
                ExternalId = genieLetter.Id.ToString(),
                AuthorExternalId = users.ContainsKey(genieLetter.From) ? users[genieLetter.From].ToString() : null,
                ContactExternalId = genieLetter.ContactId.ToString(),
                Date = genieLetter.LetterDate,
                PatientExternalId = genieLetter.PatientId.ToString(),
                Text = convert(genieLetter.ReferralContent),
                Status = status,
            };
        }

        internal static MigrationDocument ToMigrationDocument(GenieIncomingLetter genieLetter, IEnumerable<GenieUser> users, IEnumerable<GenieContact> contacts, Func<string, string> convert)
        {
            var fileData = Encoding.UTF8.GetBytes(convert(genieLetter.LetterContent));

            return new MigrationDocument
            {
                ExternalId = genieLetter.Id.ToString(),
                ImageDate = genieLetter.LetterDate,
                PatientExternalId = genieLetter.PatientId.ToString(),
                FileData = fileData,
                FileName = genieLetter.FileName,
                Description = genieLetter.LetterType,
                Md5 = fileData.Md5(),
            };
        }

        private static string GetDocumentFilename(string[] zeroFiles, string documentsPath, GenieGraphic genieDocument)
        {
            if (!string.IsNullOrWhiteSpace(genieDocument.FirstName) && !string.IsNullOrWhiteSpace(genieDocument.Surname))
            {
                var surname = genieDocument.Surname.Replace(" ", "_").Replace("-", "_");
                var nameFirstLetter = genieDocument.FirstName.First().ToString();
                var surnameFirstLetter = genieDocument.Surname.First().ToString();
                var patientFolder = string.Format("{0}{1}{2}", surname, nameFirstLetter, genieDocument.PatientId);
                var subFolder = genieDocument.PathName ?? string.Empty;
                string result;
                try
                {
                    result = Path.Combine(documentsPath, surnameFirstLetter, patientFolder, subFolder + genieDocument.RealName);
                }
                catch (Exception e)
                {
                    Log.Warn(e.Message);
                    return null;
                }
                if (File.Exists(result))
                {
                    return result;
                }
            }

            return zeroFiles.FirstOrDefault(x => x.Contains("\\" + genieDocument.Id));
        }

        internal static MigrationDocument ToMigrationDocument(string[] zeroFiles, GenieGraphic genieDocument, string genieDocumentsPath)
        {
            var filepath = GetDocumentFilename(zeroFiles, genieDocumentsPath, genieDocument);

            return new MigrationDocument
            {
                ExternalId = genieDocument.Id.ToString(),
                Description = genieDocument.Description,
                ImageDate = genieDocument.ImageDate,
                PatientExternalId = genieDocument.PatientId.ToString(),
                FileName = genieDocument.RealName,
                Md5 = filepath,
            };
        }

        internal static MigrationLaboratoryResult ToMigrationLaboratoryResult(GenieDownloadedResult genieLaboratoryResult, Dictionary<string, int> users, string laboratoryResultsPath, Func<string, string> convert)
        {
            var result = convert(genieLaboratoryResult.Result);

            return new MigrationLaboratoryResult
            {
                ExternalId = genieLaboratoryResult.Id.ToString(),
                DoctorExternalId = users.ContainsKey(genieLaboratoryResult.Addressee) ? users[genieLaboratoryResult.Addressee].ToString() : null,
                DoctorLastName = genieLaboratoryResult.Addressee,
                DoctorName = string.Empty,
                FormattedResult = result,
                LaboratoryResultType = "Pathology",
                PatientDateOfBirth = genieLaboratoryResult.DateOfBirth ?? new DateTime(1900, 1, 1),
                PatientExternalId = genieLaboratoryResult.PatientId > 0 ? genieLaboratoryResult.PatientId.ToString() : null,
                PatientLastName = genieLaboratoryResult.Surname,
                PatientName = genieLaboratoryResult.FirstName,
                ResultName = genieLaboratoryResult.Test,
                ImportDate = genieLaboratoryResult.ImportDate ?? genieLaboratoryResult.ReportDate ?? genieLaboratoryResult.ReceivedDate ?? genieLaboratoryResult.CollectionDate,
                AbnormalStatus = genieLaboratoryResult.NormalOrAbnormal,
                LaboratoryOrderNumber = genieLaboratoryResult.LabRef,
                FileName = genieLaboratoryResult.DocumentName,
                FileContent = GetLaboratoryResultFileContent(laboratoryResultsPath, genieLaboratoryResult),
            };
        }

        private static string GetLaboratoryResultFileContent(string laboratoryResultsPath, GenieDownloadedResult genieLaboratoryResult)
        {
            var filepath = new Lazy<string>(() => Path.Combine(laboratoryResultsPath, genieLaboratoryResult.DocumentName));

            return !string.IsNullOrWhiteSpace(laboratoryResultsPath) && File.Exists(filepath.Value)
                ? File.ReadAllText(filepath.Value)
                : null;
        }

        internal static MigrationVaccination ToMigrationVaccination(GenieVaccination genieVaccination)
        {
            return new MigrationVaccination
            {
                ExternalId = genieVaccination.Id.ToString(),
                ACIRCode = genieVaccination.ACIRCode,
                Dose = genieVaccination.Dose,
                ExpiryDate = genieVaccination.ExpiryDate,
                ICD10Code = genieVaccination.ICD10Code,
                ICPCCode = genieVaccination.ICPCCode,
                Name = genieVaccination.Vaccine,
                PatientExternalId = genieVaccination.PatientId.ToString(),
                TermCode = genieVaccination.TermCode,
                VaccinationDate = genieVaccination.GivenDate,
            };
        }

        internal static MigrationScript ToMigrationScript(GenieScript genieScript, Dictionary<string, GenieDrug> drugs)
        {
            var drug = drugs.ContainsKey(genieScript.DrugId)
                ? ToMigrationDrug(drugs[genieScript.DrugId])
                : null;

            return new MigrationScript
            {
                ExternalId = genieScript.Id,
                PatientExternalId = genieScript.PatientId.ToString(),
                ExternalDrugId = genieScript.DrugId,
                MigrationDrug = drug,
                AuthorityNumber = genieScript.AuthorityNumber,
                Comments = genieScript.Note,
                Created = genieScript.CreationDate.AsInvariantString(),
                Dose = genieScript.Dose,
                OtherMedicalDescription = genieScript.Medication,
                RepeatDays = genieScript.Repeat,
                ExternalUserId = genieScript.CreatorDoctorId.ToString(),
            };
        }

        internal static MigrationDrug ToMigrationDrug(GenieDrug genieDrug)
        {
            return new MigrationDrug
            {
                ExternalId = genieDrug.Id,
                Composition = genieDrug.Quantity,
                Form = genieDrug.Form,
                Name = genieDrug.Name,
                Strength = genieDrug.Strength,
                Code = genieDrug.Code.Translate(DrugCodeGenieToSiberia),
                MimsProdCode = genieDrug.MimsProdCode,
                MimsFormCode = genieDrug.MimsFormCode,
                MimsPackCode = genieDrug.MimsPackCode,
            };
        }

        internal static MigrationOpReport ToMigrationOpReport(
            GenieOpReport genieOpReport,
            Dictionary<string, int> users,
            Dictionary<int, GenieComplication[]> complications,
            Dictionary<int, GenieChecklist[]> checklists,
            Dictionary<int, GenieChecklistField[]> checklistFields,
            Dictionary<int, GenieChecklistField> templateChecklistFields,
            Dictionary<int, GenieQuoteItem[]> quoteItems)
        {
            var attachedComplications = complications.ContainsKey(genieOpReport.Id)
                ? complications[genieOpReport.Id]
                : new GenieComplication[] { };

            var attachedChecklists = checklists.ContainsKey(genieOpReport.Id)
                ? checklists[genieOpReport.Id]
                : new GenieChecklist[] { };

            var attachedQuoteItems = quoteItems.ContainsKey(genieOpReport.Id)
                ? quoteItems[genieOpReport.Id]
                : new GenieQuoteItem[] { };

            string side = null;
            switch (genieOpReport.Side)
            {
                case 1:
                    side = "Left";
                    break;

                case 2:
                    side = "Right";
                    break;

                case 3:
                    side = "Left & Right";
                    break;
            }

            return new MigrationOpReport
            {
                ExternalId = genieOpReport.Id.ToString(),
                DoctorExternalId = genieOpReport.DoctorId > 0
                    ? genieOpReport.DoctorId.ToString()
                    : users.ContainsKey(genieOpReport.DoctorName)
                        ? users[genieOpReport.DoctorName].ToString()
                        : null,
                PatientExternalId = genieOpReport.PatientId.ToString(),
                AssistantExternalId = genieOpReport.AssistantId.ToString(),
                AnaesthetistExternalId = genieOpReport.AnaesthetistId.ToString(),
                ProcedureName = genieOpReport.ProcedureName,
                Side = side,
                DoctorName = genieOpReport.DoctorName,
                Hospital = genieOpReport.Hospital,
                ProcedureDate = (genieOpReport.ProcedureDate ?? new DateTime(1900, 1, 1)).AsInvariantString(),
                ProcedureTimeFrom = genieOpReport.ProcedureTimeFrom.AsInvariantString(),
                ProcedureTimeTo = genieOpReport.ProcedureTimeTo.AsInvariantString(),
                Admission = genieOpReport.AdmissionDate.AsInvariantString(genieOpReport.AdmissionTime),
                FastFromTime = genieOpReport.FastFromTime.AsInvariantString(),
                DischargeDate = genieOpReport.DischargeDate.AsInvariantString(),
                InPatientDays = genieOpReport.InPatientDays,
                Indication = genieOpReport.Indication,
                Category = genieOpReport.Category,
                Magnitude = genieOpReport.Magnitude,
                InfectionRisk = genieOpReport.InfectionRisk,
                ProcedureType = genieOpReport.ProcedureType,
                Anaesthetic = genieOpReport.Anaesthetic,
                Prosthesis = genieOpReport.Prosthesis,
                Finding = genieOpReport.Finding,
                Technique = genieOpReport.Technique,
                PostOp = genieOpReport.PostOp,
                AdmissionOutcome = genieOpReport.AdmissionOutcome,
                FollowupDate = genieOpReport.FollowupDate.AsInvariantString(),
                FollowupOutcome = genieOpReport.FollowupOutcome,
                AuditSummary = genieOpReport.AuditSummary,
                PreopDiagnosis = genieOpReport.PreopDiagnosis,
                PostopDiagnosis = genieOpReport.PostopDiagnosis,
                Complications = attachedComplications.Select(x => new MigrationComplication
                {
                    Complication = x.ComplicationDetails,
                    ComplicationDate = x.ComplicationDate
                }).ToArray(),
                Checklists = attachedChecklists.Select(x => ToMigrationChecklist(x, users, checklistFields, templateChecklistFields)).ToArray(),
                QuoteItems = attachedQuoteItems.Select(ToMigrationQuoteItem).ToArray(),
            };
        }

        internal static MigrationInterestedParty ToMigrationInterestedParty(GenieInterestedParty genieInterestedParty)
        {
            return new MigrationInterestedParty
            {
                ExternalId = genieInterestedParty.Id.ToString(),
                ContactExternalId = genieInterestedParty.ContactId.ToString(),
                PatientExternalId = genieInterestedParty.PatientId.ToString(),
            };
        }

        internal static MigrationCompany ToMigrationCompany(GenieEmployer genieEmployer)
        {
            return new MigrationCompany
            {
                ExternalId = genieEmployer.Id,
                AddressLine1 = genieEmployer.AddressLine1,
                AddressLine2 = genieEmployer.AddressLine2,
                Name = genieEmployer.Name,
                Fax = genieEmployer.Fax,
                Phone = genieEmployer.Phone1,
                Phone2 = genieEmployer.Phone2,
                Email = genieEmployer.Email,
                Country = genieEmployer.Country,
                State = genieEmployer.State,
                Suburb = genieEmployer.Suburb,
                PostCode = genieEmployer.PostCode
            };
        }

        internal static MigrationCompany ToMigrationCompany(GenieAccountHolder genieAccountHolder)
        {
            return new MigrationCompany
            {
                ExternalId = CompanyAccountHolderExternalIdPrefix + genieAccountHolder.Id,
                AddressLine1 = genieAccountHolder.AddressLine1,
                AddressLine2 = genieAccountHolder.AddressLine2,
                Name = genieAccountHolder.FullName,
                Phone = genieAccountHolder.HomePhone,
                Phone2 = genieAccountHolder.Mobile,
                Fax = genieAccountHolder.Fax,
                State = genieAccountHolder.State,
                Suburb = genieAccountHolder.Suburb,
                PostCode = genieAccountHolder.PostCode
            };
        }

        internal static MigrationWorkCoverClaim ToMigrationWorkCoverClaim(GenieWorkCoverClaim genieWorkcoverClaim)
        {
            return new MigrationWorkCoverClaim
            {
                ExternalId = genieWorkcoverClaim.Id,
                PatientExternalId = genieWorkcoverClaim.PatientId,
                EmployerCompanyExternalId = genieWorkcoverClaim.EmployerId,
                InsuranceCompanyExternalId = CompanyAccountHolderExternalIdPrefix + genieWorkcoverClaim.InsurerId,
                ClaimManagerName = genieWorkcoverClaim.CaseManagerName,
                ClaimManagerPhone = genieWorkcoverClaim.CaseManagerWorkPhone.NullIfEmpty() ?? genieWorkcoverClaim.CaseManagerMobilePhone,
                Injury = genieWorkcoverClaim.Injury,
                ClaimId = genieWorkcoverClaim.ClaimNum,
                LocationOnBody = genieWorkcoverClaim.InjuryMechanism,
                DateOfInjury = genieWorkcoverClaim.InjuryDate.AsInvariantString(),
                TimeOfInjury = genieWorkcoverClaim.InjuryTime.AsDisplayTimeString(),
                LocationOfInjury = genieWorkcoverClaim.Location,
            };
        }

        private static bool ToIsSpecialist(this string specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty))
            {
                return false;
            }

            return !GenieGeneralPractitionerSpecialties.Contains(specialty);
        }

        private static bool ToIsReceptionist(this string specialty)
        {
            return GenieReceptionistSpecialties.Contains(specialty);
        }

        internal static DateTime? ParseRecivedDateString(string dateFromDb)
        {
            DateTime result;
            if (DateTime.TryParseExact(dateFromDb, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result))
            {
                return result;
            }
            return null;
        }
    }
}
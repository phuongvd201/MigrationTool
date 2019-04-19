using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Text;

using MigrationTool.Services.Entities.Shexie;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Interfaces.Shexie;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations.Shexie
{
    internal class ShexieRepository : IShexieRepository
    {
        protected static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IShexieSettingsService localShexieSettingsService;

        private Dictionary<int, ShexiePostcode> localSuburbsDictionary;

        private Dictionary<int, ShexieAppointmentType> localAppointmentTypesDictionary;

        private Dictionary<int, ShexiePostcode> SuburbsDictionary
        {
            get
            {
                if (localSuburbsDictionary == null)
                {
                    localSuburbsDictionary = Query(c => c.GetTable<ShexiePostcode>().Join(
                        c.GetTable<ShexieCountry>(),
                        x => x.CountryId,
                        x => x.Id,
                        (i, o) => new ShexiePostcode
                        {
                            Id = i.Id,
                            Postcode = i.Postcode,
                            Suburb = i.Suburb,
                            State = i.State,
                            Country = o.CountryName,
                        })).ToDictionary(x => x.Id);
                }

                return localSuburbsDictionary;
            }
        }

        private Dictionary<int, ShexieAppointmentType> AppointmentTypesDictionary
        {
            get
            {
                if (localAppointmentTypesDictionary == null)
                {
                    localAppointmentTypesDictionary = GetEntities<ShexieAppointmentType>()
                        .ToDictionary(x => x.Id);
                }

                return localAppointmentTypesDictionary;
            }
        }

        public ShexieRepository(IShexieSettingsService shexieSettingsService)
        {
            localShexieSettingsService = shexieSettingsService;
        }

        public IEnumerable<ShexieUser> GetUsers()
        {
            return GetEntities<ShexieUser>();
        }

        public IEnumerable<ShexieProvider> GetProviders()
        {
            return GetEntities<ShexieProvider>();
        }

        public IEnumerable<ShexieOpReport> GetOpReports()
        {
            return GetEntities<ShexieOpReport>()
                .GroupJoin(
                    GetEntities<ShexieFeeEstimate>(),
                    x => x.Id,
                    x => x.SurgeryId,
                    (i, o) => new
                    {
                        OpReport = i,
                        FeeEstimates = o
                    })
                .Where(x => x.FeeEstimates.Any())
                .GroupJoin(
                    GetEntities<ShexieHospitalList>().ToDictionary(x => x.Id),
                    x => x.FeeEstimates.FirstOrDefault().HospitalId,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.OpReport.ShexieHospitalList = o.Select(x => x.Value).FirstOrDefault() ?? new ShexieHospitalList();
                        return i;
                    })
                .Select(x =>
                {
                    x.OpReport.FeeEstimateItems = x.FeeEstimates
                        .GroupJoin(
                            GetEntities<ShexieFeeEstimateItem>().ToDictionary(s => s.Id),
                            m => m.OperationId,
                            m => m.Key,
                            (i, o) => new
                            {
                                Operation = o.Select(s => s.Value).FirstOrDefault()
                            })
                        .Select(s => s.Operation).Where(s => s != null);
                    return x.OpReport;
                });
        }

        public IEnumerable<ShexieLaboratoryResult> GetLaboratoryResults()
        {
            var resultTypes = GetEntities<ShexieLaboratoryResultType>().ToDictionary(x => x.Id);

            return Query(c => c.GetTable<ShexieLaboratoryResult>().GroupJoin(
                c.GetTable<ShexiePatient>(),
                x => x.PatientId,
                x => x.Id,
                (i, o) =>
                    new
                    {
                        Result = i,
                        Patient = o
                    })
                ).Select(x =>
                {
                    ////Danang Team fix DEVC2C - 2148
                    ////item 5. We cant see lab results
                    ////Laboratory requires a patient, must set first name, last name and date of birth values
                    x.Result.Patient = x.Patient.FirstOrDefault() ?? new ShexiePatient()
                    {
                        FirstName = "Unknown",
                        LastName = "Patient",
                        DateOfBirth = DateTime.MinValue
                    };
                    x.Result.ResultType = resultTypes.ContainsKey(x.Result.Id) ? resultTypes[x.Result.Id] : new ShexieLaboratoryResultType();
                    return x.Result;
                });
        }

        public IEnumerable<ShexieContact> GetContacts()
        {
            return Query(c => c.GetTable<ShexieContact>()
                .Where(x => x.FirstName != null && x.FirstName != string.Empty))
                .Select<ShexieContact, ShexieContact>(AttachSuburbInfo);
        }

        public IEnumerable<ShexieCompany> GetCompanies()
        {
            return GetEntities<ShexieCompany>()
                .Select<ShexieCompany, ShexieCompany>(AttachSuburbInfo);
        }

        public IEnumerable<ShexieAttachment> GetAttachments()
        {
            var atts = Query(c => c.GetTable<ShexieAttachment>().Select(x => new ShexieAttachment()
            {
                PatientId = x.PatientId,
                Path = x.Path
            }));

            var recognized = Query(c => c.GetTable<ShexiePatient>().Select(x => new ShexiePatient()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName
            })).AsParallel()
                .Select(x => new
                {
                    x.Id,
                    EncryptedIds = atts.Where(o => o.Path.Contains(x.FirstName) && o.Path.Contains(x.LastName)).Select(o => o.PatientId).Distinct().ToArray()
                });
            var recognizedKeysCandidates = recognized.Where(x => x.EncryptedIds.Length == 1).Select(x => new
            {
                x.Id,
                Encrypted = x.EncryptedIds[0]
            });

            var recognizedKeys = recognizedKeysCandidates.GroupBy(x => x.Encrypted).ToDictionary(x => x.Key, x => x.First().Id.ToString());

            ////Danang Team fix DEVC2C - 2148
            var sequentialRules = FindBestSequentialRules(recognizedKeys);

            var negativeRecognizedKeys = recognizedKeys.Where(x => x.Value[0] == '-').OrderBy(x => new Random().Next()).Take(5).ToList();
            char? negativeCode = null;
            if (negativeRecognizedKeys.Any())
            {
                negativeCode = negativeRecognizedKeys.GroupBy(x => x.Key[0]).Select(x => new
                {
                    x.Key,
                    Count = x.Count()
                }).OrderByDescending(x => x.Count).First().Key;
            }

            return Query(c => c.GetTable<ShexieAttachment>()).Select(x => new ShexieAttachment()
            {
                ////Danang Team fix DEVC2C - 2148
                ////all the encrypted keys be decrypted through the use of the decryption algorithm
                PatientId = DecryptString(x.PatientId, sequentialRules, negativeCode),
                Id = x.Id,
                Date = x.Date,
                Description = x.Description,
                Path = x.Path,
                Type = x.Type
            });
        }

        public IEnumerable<ShexieAlarm> GetAlarms()
        {
            return GetEntities<ShexieAlarm>();
        }

        public IEnumerable<ShexieReferral> GetReferrals()
        {
            return Query(c => c.GetTable<ShexieReferral>()
                .GroupJoin(
                    c.GetTable<ShexieContact>().Select(x => new ShexieContact
                    {
                        Id = x.Id,
                    }),
                    x => x.ContactId,
                    x => x.Id,
                    (i, o) => new
                    {
                        Referral = i,
                        Contacts = o
                    })
                .Where(x => x.Contacts.Any())
                .GroupJoin(
                    c.GetTable<ShexiePatient>().Select(x => new ShexiePatient
                    {
                        Id = x.Id,
                    }),
                    x => x.Referral.PatientId,
                    x => x.Id,
                    (i, o) => new
                    {
                        i.Referral,
                        Patients = o
                    })
                .Where(x => x.Patients.Any())).Select(x => x.Referral);
        }

        public IEnumerable<ShexieAppointmentType> GetAppointmentTypes()
        {
            return AppointmentTypesDictionary.Values;
        }

        public IEnumerable<ShexieAppointment> GetAppointments()
        {
            return Query(c => c.GetTable<ShexieAppointment>().Join(
                c.GetTable<ShexiePatient>().Select(x => new
                {
                    x.Id
                }),
                x => x.PatientId,
                x => x.Id,
                (i, o) => i))
                .Join(
                    AppointmentTypesDictionary,
                    x => x.AppointmentTypeId,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.AppointmentType = o.Value;
                        return i;
                    });
        }

        public IEnumerable<ShexiePatient> GetPatients()
        {
            return Query(c =>
                c.GetTable<ShexiePatient>().GroupJoin(
                    c.GetTable<ShexieContact>().Select(x => new ShexieContact
                    {
                        Id = x.Id,
                        SuburbId = x.SuburbId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        HomePhone = x.HomePhone
                    }),
                    x => x.GuardianId,
                    x => x.Id,
                    (i, o) => new
                    {
                        Patient = i,
                        Guardian = o
                    }))
                .Select(x =>
                {
                    if (x.Guardian.Any())
                    {
                        x.Patient.Guardian = x.Guardian.First();
                        AttachSuburbInfo(x.Patient.Guardian);
                    }
                    else
                    {
                        x.Patient.Guardian = new ShexieContact();
                    }
                    x.Patient.EmergencyPersonId = (x.Patient.MotherId.HasValue && x.Patient.MotherId > 0)
                        ? x.Patient.MotherId
                        : (x.Patient.FatherId.HasValue && x.Patient.FatherId > 0)
                            ? x.Patient.FatherId
                            : x.Patient.OtherId;
                    AttachSuburbInfo(x.Patient);
                    AttachSuburbInfo(x.Patient.Guardian);
                    return x.Patient;
                })
                .GroupJoin(
                    GetEntities<ShexiePatientSecondary>().ToDictionary(x => x.Id),
                    x => x.PatientSecondaryId,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.PatientSecondary = o.Select(x => x.Value).FirstOrDefault() ?? new ShexiePatientSecondary();
                        return i;
                    })
                .GroupJoin(
                    GetEntities<ShexieCountry>().ToDictionary(x => x.Id),
                    x => x.PatientSecondary.BirthCountry,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.PatientSecondary.Country = o.Select(x => x.Value).FirstOrDefault() ?? new ShexieCountry();
                        return i;
                    })
                .GroupJoin(
                    GetEntities<ShexieKin>().ToDictionary(x => x.Id),
                    x => x.EmergencyPersonId,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.EmergencyPerson = o.Select(x => x.Value).FirstOrDefault() ?? new ShexieKin();
                        return i;
                    })
                .GroupJoin(
                    GetEntities<ShexieCompany>().ToDictionary(x => x.Id),
                    x => x.PatientSecondary.HealthFundId,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.Company = o.Select(x => x.Value).FirstOrDefault() ?? new ShexieCompany();
                        return i;
                    });
        }

        public IEnumerable<ShexieAnalysis> GetAnalyses()
        {
            return GetEntities<ShexieAnalysis>();
        }

        public IEnumerable<ShexieStatistic> GetStatistics()
        {
            return GetEntities<ShexieStatistic>()
                .GroupJoin(
                    GetEntities<ShexieAnalysis>().ToDictionary(x => x.Id),
                    x => x.AnalysisId,
                    x => x.Key,
                    (s, a) =>
                    {
                        s.Analysis = a.Select(x => x.Value).FirstOrDefault() ?? new ShexieAnalysis();
                        return s;
                    });
        }

        public IEnumerable<ShexieScript> GetScripts()
        {
            return Query(c => c.GetTable<ShexieScript>()
                .Join(
                    c.GetTable<ShexieDrug>(),
                    x => x.Drug,
                    x => x.Id,
                    (i, o) => new
                    {
                        Script = i,
                        Drug = o
                    }))
                .Select(x =>
                {
                    var script = x.Script;
                    script.ShexieDrug = x.Drug;
                    return script;
                });
        }

        public IEnumerable<ShexieRecall> GetRecalls()
        {
            return GetEntities<ShexieRecall>()
                .GroupJoin(
                    GetEntities<ShexieProvider>().ToDictionary(x => x.Provider),
                    x => x.Provider,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.ShexieProvider = o.Select(x => x.Value).FirstOrDefault() ?? new ShexieProvider();
                        return i;
                    })
                .GroupJoin(
                    GetEntities<ShexiePatient>().ToDictionary(x => x.Id),
                    x => x.PatientId,
                    x => x.Key,
                    (i, o) =>
                    {
                        i.ShexiePatient = o.Select(x => x.Value).FirstOrDefault() ?? new ShexiePatient();
                        return i;
                    });
        }

        public IEnumerable<ShexieInterestedParty> GetInterestedParties()
        {
            return GetEntities<ShexieInterestedParty>()
                .GroupJoin(
                    GetEntities<ShexieReferral>().ToDictionary(x => x.Id),
                    x => x.ServiceId,
                    x => x.Key,
                    (i, o) => new
                    {
                        Interested = i,
                        Referral = o.Select(x => x.Value).FirstOrDefault() ?? new ShexieReferral()
                    })
                .Select(x =>
                {
                    x.Interested.PatientId = x.Referral.PatientId;
                    return x.Interested;
                });
        }

        public IEnumerable<ShexiePatientHistory> GetPatientHistories()
        {
            return GetEntities<ShexiePatientHistory>();
        }

        private T AttachSuburbInfo<T>(T obj) where T : class, ISuburbInfo
        {
            ShexiePostcode suburb;
            if (obj == null || !obj.SuburbId.HasValue || (suburb = SuburbsDictionary.GetValueOrNull(obj.SuburbId.Value)) == null)
            {
                return obj;
            }
            obj.Country = suburb.Country;
            obj.PostCode = suburb.Postcode;
            obj.Suburb = suburb.Suburb;
            obj.State = suburb.State;
            return obj;
        }

        /// <summary>
        /// Find best sequentialRules from 5 sequentialRules of top 5 max length recognizedKeys.
        /// </summary>
        /// <param name="recognizedKeys">The recognizedKeys dictionary.</param>
        /// <returns>Best sequentialRules.</returns>
        private int[] FindBestSequentialRules(Dictionary<string, string> recognizedKeys)
        {
            var sequentialRules = recognizedKeys.OrderByDescending(s => s.Value.Length).Take(5)
                .Select(x => GetSequentialRules(x.Key, x.Value)).ToList();

            return sequentialRules.Count == 0 ? null : sequentialRules.Select(m => new
            {
                Rule = m,
                CountSubRule = sequentialRules.Count(x => (m.Length >= x.Length) && string.Join(string.Empty, m).StartsWith(string.Join(string.Empty, x), StringComparison.CurrentCulture))
            }).OrderByDescending(s => s.CountSubRule).First().Rule;
        }

        /// <summary>
        /// Get sequential rule of character in encrypted string.
        /// </summary>
        /// <param name="encryptedString">String of encypted patient id.</param>
        /// <param name="patientId">PatientId value.</param>
        /// <returns>An array of value for differences between 2 characters.</returns>
        private int[] GetSequentialRules(string encryptedString, string patientId)
        {
            byte[] win1252CodeArr = GetCodePageWindow1252(encryptedString);
            int length = win1252CodeArr.Length < 5 ? win1252CodeArr.Length : 5;
            int[] resultRules = new int[length];

            for (var i = 0; i < length; i++)
            {
                resultRules[i] = (i == 0 && double.Parse(patientId) < 0)
                    ? win1252CodeArr[i] + 3
                    : win1252CodeArr[i] - (int)char.GetNumericValue(patientId[i]);
            }

            return resultRules;
        }

        /// <summary>
        /// Return decoded patientId.
        /// </summary>
        /// <param name="encryptedString">Value of encrypted string.</param>
        /// <param name="sequentialRules">Sequential rule get from a key pair value from recognizedKeys.</param>
        /// <param name="negativeCode">Code for negative value in Win1252.</param>
        /// <returns>Value of decrypted string.</returns>
        private string DecryptString(string encryptedString, int[] sequentialRules, char? negativeCode)
        {
            var win1252CodeArr = GetCodePageWindow1252(encryptedString);

            ////Danang Team: If we find no rule then return null here
            if (sequentialRules == null || (sequentialRules.Length < 5 && win1252CodeArr.Length > sequentialRules.Length))
            {
                return null;
            }

            string[] decryptedCodeArr = new string[win1252CodeArr.Length];

            for (int i = 0,
                     l = win1252CodeArr.Length; i < l; i++)
            {
                ////Danang Team: get two first values of negative code arrays, if they are equal mean that its negative
                if (i == 0 && negativeCode.HasValue && encryptedString[i] == negativeCode)
                {
                    decryptedCodeArr[i] = "-";
                    continue;
                }
                decryptedCodeArr[i] = (win1252CodeArr[i] - sequentialRules[i % sequentialRules.Length]).ToString();
            }

            return string.Join(string.Empty, decryptedCodeArr);
        }

        /// <summary>
        /// Get window 1252 code of encrypted string.
        /// </summary>
        /// <param name="encryptedString">Encrypted value of string.</param>
        /// <returns>Byte array for window 1252 code.</returns>
        private byte[] GetCodePageWindow1252(string encryptedString)
        {
            return Encoding.GetEncoding(1252).GetBytes(encryptedString);
        }

        private T[] GetEntities<T>() where T : class
        {
            return Query(c => c.GetTable<T>());
        }

        private T[] Query<T>(Func<DataContext, IQueryable<T>> query) where T : class
        {
            using (var connection = new OleDbConnection(localShexieSettingsService.ConnectionString))
            {
                using (var databaseContext = new DataContext(connection))
                {
                    return query(databaseContext).ToArray();
                }
            }
        }
    }
}
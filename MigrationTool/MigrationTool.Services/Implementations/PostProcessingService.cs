using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using MigrationTool.Services.Helpers;
using MigrationTool.Services.Helpers.C2cXml;
using MigrationTool.Services.Helpers.Text;
using MigrationTool.Services.Helpers.Zedmed;
using MigrationTool.Services.Interfaces;
using MigrationTool.Services.Interfaces.C2cXml;
using MigrationTool.Services.Interfaces.Zedmed;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations
{
    internal class PostProcessingService : IPostProcessingService
    {
        private static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ITextConverter TextConverter { get; set; }

        public IZedmedSettingsService ZedmedSettingsService { get; set; }

        public IC2cXmlSettingsService C2cXmlSettingsService { get; set; }

        private Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, IMigrationEntity>> localPostProcessingFunctions;

        private Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, IMigrationEntity>> SupportedPostProcessingFunctions
        {
            get
            {
                if (localPostProcessingFunctions == null)
                {
                    localPostProcessingFunctions = new Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, IMigrationEntity>>
                    {
                        {
                            Tuple.Create(MigrationSourceSystem.C2cXml, MigrationEntity.Documents),
                            PostProcessC2cXmlDocument
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Genie, MigrationEntity.Documents),
                            PostProcessDocument
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Shexie, MigrationEntity.Documents),
                            PostProcessDocument
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Shexie, MigrationEntity.Letters),
                            PostProcessShexieLetter
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Zedmed, MigrationEntity.Documents),
                            PostProcessZedmedDocument
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Zedmed, MigrationEntity.Letters),
                            PostProcessZedmedLetter
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Zedmed, MigrationEntity.LaboratoryResults),
                            PostProcessZedmedLaboratoryResult
                        },
                    };
                }
                return localPostProcessingFunctions;
            }
        }

        public bool IsSupported(MigrationSourceSystem source, MigrationEntity entityType)
        {
            return SupportedPostProcessingFunctions.ContainsKey(Tuple.Create(source, entityType));
        }

        public Func<IMigrationEntity, IMigrationEntity> GetPostProcessingMethod(MigrationSourceSystem sourceSystem, MigrationEntity entityType)
        {
            return SupportedPostProcessingFunctions[Tuple.Create(sourceSystem, entityType)];
        }

        private IMigrationEntity PostProcessC2cXmlDocument(IMigrationEntity source)
        {
            var document = (MigrationDocument)source;
            document.LoadFileData(C2cXmlSettingsService.C2cDocumentsPath);
            return document;
        }

        private IMigrationEntity PostProcessDocument(IMigrationEntity source)
        {
            var document = (MigrationDocument)source;

            var filepath = document.Md5;
            var filedata = File.Exists(filepath) ? File.ReadAllBytes(filepath) : null;

            document.FileData = filedata;
            document.Md5 = filedata.Md5();

            return document;
        }

        private IMigrationEntity PostProcessShexieLetter(IMigrationEntity source)
        {
            var document = (MigrationLetter)source;

            document.Text = WordToHtmlConverter.ConvertWordToHtml(document.Text);

            return document;
        }

        private IMigrationEntity PostProcessZedmedLetter(IMigrationEntity source)
        {
            var letter = (MigrationLetter)source;
            var letterZipFileName = letter.Text;

            var zipFilePath = letter.PatientExternalId.GetPatientDocumentZipFilePath(ZedmedSettingsService.DocumentsPath);
            var zipEntryName = ZedmedMigrationHelper.ZedmedDocumentDirectory.PatientDocument + letterZipFileName;

            string letterText = null;
            try
            {
                StorageHelper
                    .UseStream(write => write.AsString(x => letterText = x))
                    .FromZip(ZedmedMigrationHelper.ZedmedDocumentDirectory.LetterFileName)
                    .FromZip(zipEntryName)
                    .FromFile(zipFilePath);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to extract letter zip file : " + zipFilePath, ex);
            }

            letter.Text = !string.IsNullOrWhiteSpace(letterText) ? TextConverter.RtfToHtml(letterText) : string.Empty;

            return letter;
        }

        private IMigrationEntity PostProcessZedmedDocument(IMigrationEntity source)
        {
            var document = (MigrationDocument)source;
            var documentDirectory = document.Md5;

            var zipFilePath = document.PatientExternalId.GetPatientDocumentZipFilePath(ZedmedSettingsService.DocumentsPath);
            var zipEntryName = documentDirectory + document.ExternalId;

            byte[] documentFileData = null;
            try
            {
                StorageHelper
                    .UseStream(write => write.AsBytes(x => documentFileData = x))
                    .FromZip(document.FileName)
                    .FromZip(zipEntryName)
                    .FromFile(zipFilePath);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to extract document zip file : " + zipFilePath, ex);
            }

            document.FileData = documentFileData;
            document.Md5 = documentFileData.Md5();

            return document;
        }

        private IMigrationEntity PostProcessZedmedLaboratoryResult(IMigrationEntity source)
        {
            var labResult = (MigrationLaboratoryResult)source;
            var labResultZipFileName = labResult.FormattedResult;

            var zipFilePath = labResult.PatientExternalId.GetPatientDocumentZipFilePath(ZedmedSettingsService.DocumentsPath);
            var zipEntryName = ZedmedMigrationHelper.ZedmedDocumentDirectory.ResultDocument + labResultZipFileName;

            string labResultText = null;
            try
            {
                StorageHelper
                    .UseStream(write => write.AsString(x => labResultText = x))
                    .FromZip(zipEntryName)
                    .FromFile(zipFilePath);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to extract laboratory result zip file : " + zipFilePath, ex);
            }

            labResult.FormattedResult = !string.IsNullOrWhiteSpace(labResultText) ? TextConverter.TextToDecodedHtml(labResultText) : string.Empty;

            return labResult;
        }
    }
}
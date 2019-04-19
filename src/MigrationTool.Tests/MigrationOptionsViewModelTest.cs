using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MigrationTool.Services.Interfaces;
using MigrationTool.ViewModels;
using MigrationTool.ViewModels.Interfaces;

using Moq;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Tests
{
    [TestClass]
    public class MigrationOptionsViewModelTest
    {
        [TestMethod]
        [Timeout(2000)]
        public void MigrationOptionsRefreshTest()
        {
            var entitiesList = new[]
            {
                MigrationEntity.AccountHolders,
            };

            var sourceNames = new[]
            {
                "Data source 1",
                "Data source 2"
            };

            var entitiesLoadedToken = new FinishingToken();
            var sourceNamesLoadedToken = new FinishingToken();

            var mockSelectedOptionsService = new Mock<ISelectedOptionsSettingsService>();
            mockSelectedOptionsService
                .Setup(x => x.SelectedMigrationSourceSystem)
                .Returns(MigrationSourceSystem.C2cXml);

            var mockSupportedEntitiesProvider = new Mock<ISupportedEntitiesInfoProvider>();
            mockSupportedEntitiesProvider
                .Setup(x => x.GetSupportedEntityTypes(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(() =>
                {
                    while (!entitiesLoadedToken.Finished)
                    {
                    }
                    return entitiesList;
                });

            var mockMigrationSourceService = new Mock<IMigrationSourceService>();
            mockMigrationSourceService
                .Setup(x => x.GetPreviousSourceNames(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(() =>
                {
                    while (!sourceNamesLoadedToken.Finished)
                    {
                    }
                    return sourceNames;
                });

            var sut = new MigrationOptionsViewModel();
            sut.SupportedEntitiesInfoProvider = mockSupportedEntitiesProvider.Object;
            sut.SelectedOptionsSettingsService = mockSelectedOptionsService.Object;
            sut.MigrationSourceService = mockMigrationSourceService.Object;

            sut.RefreshEntityOptionsCommand.Value.Execute(null);
            entitiesLoadedToken.Finished = true;
            sourceNamesLoadedToken.Finished = true;
            while (sut.Loading.Value)
            {
            }
            entitiesLoadedToken.Finished = false;
            sourceNamesLoadedToken.Finished = false;

            Assert.AreEqual(string.Empty, sut.SelectedSourceName.Input);
            Assert.IsFalse(sut.SelectedSourceName.HasErrors);
            Assert.AreEqual(2, sut.PreviousSourceNames.Value.Length);
            Assert.AreEqual(1, sut.MigrationEntityOptions.Input.Length);
            Assert.IsFalse(sut.MigrationEntityOptions.HasErrors);

            sut.MigrationEntityOptions.Input[0].IsChecked.Input = true;
            Assert.IsTrue(sut.MigrationEntityOptions.Input.Any(x => x.IsChecked.Input));

            var sourceName = new[]
            {
                "Data source 1",
            };

            mockMigrationSourceService
                .Setup(x => x.GetPreviousSourceNames(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(() =>
                {
                    while (!sourceNamesLoadedToken.Finished)
                    {
                    }
                    return sourceName;
                });

            sut.RefreshEntityOptionsCommand.Value.Execute(null);
            entitiesLoadedToken.Finished = true;
            sourceNamesLoadedToken.Finished = true;
            while (sut.Loading.Value)
            {
            }

            Assert.AreEqual(sourceName[0], sut.SelectedSourceName.Input);
            Assert.AreEqual(1, sut.PreviousSourceNames.Value.Length);
            Assert.AreEqual(1, sut.MigrationEntityOptions.Input.Length);
            Assert.IsFalse(sut.MigrationEntityOptions.Input.Any(x => x.IsChecked.Input));
        }

        [TestMethod]
        public void MigrationOptionsSelectAllTest()
        {
            var entitiesList = new[]
            {
                MigrationEntity.AccountHolders,
                MigrationEntity.Checklists,
                MigrationEntity.OpReports,
            };

            var mockSelectedOptionsService = new Mock<ISelectedOptionsSettingsService>();
            mockSelectedOptionsService
                .Setup(x => x.SelectedMigrationSourceSystem)
                .Returns(MigrationSourceSystem.C2cXml);

            var mockSupportedEntitiesProvider = new Mock<ISupportedEntitiesInfoProvider>();
            mockSupportedEntitiesProvider
                .Setup(x => x.GetSupportedEntityTypes(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(entitiesList);

            var mockMigrationSourceService = new Mock<IMigrationSourceService>();
            mockMigrationSourceService
                .Setup(x => x.GetPreviousSourceNames(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(new string[] { });

            var sut = new MigrationOptionsViewModel(true);
            sut.SupportedEntitiesInfoProvider = mockSupportedEntitiesProvider.Object;
            sut.SelectedOptionsSettingsService = mockSelectedOptionsService.Object;
            sut.MigrationSourceService = mockMigrationSourceService.Object;

            sut.RefreshEntityOptionsCommand.Value.Execute(null);
            Assert.IsFalse(sut.MigrationEntityOptions.HasErrors);
            Assert.AreEqual(3, sut.MigrationEntityOptions.Input.Length);
            Assert.IsFalse(sut.MigrationEntityOptions.Input.Any(x => x.IsChecked.Input));
            Assert.IsFalse(sut.SelectAll.Input);

            sut.MigrationEntityOptions.Input[0].IsChecked.Input = true;
            sut.MigrationEntityOptions.Input[1].IsChecked.Input = true;
            Assert.AreEqual(2, sut.MigrationEntityOptions.Input.Count(x => x.IsChecked.Input));
            Assert.IsFalse(sut.SelectAll.Input);

            sut.MigrationEntityOptions.Input[2].IsChecked.Input = true;
            Assert.AreEqual(3, sut.MigrationEntityOptions.Input.Count(x => x.IsChecked.Input));
            Assert.IsTrue(sut.SelectAll.Input);

            sut.MigrationEntityOptions.Input[0].IsChecked.Input = false;
            Assert.AreEqual(2, sut.MigrationEntityOptions.Input.Count(x => x.IsChecked.Input));
            Assert.IsFalse(sut.SelectAll.Input);

            sut.SelectAll.Input = true;
            Assert.IsTrue(sut.SelectAll.Input);
            Assert.AreEqual(3, sut.MigrationEntityOptions.Input.Count(x => x.IsChecked.Input));

            sut.SelectAll.Input = false;
            Assert.IsFalse(sut.SelectAll.Input);
            Assert.IsFalse(sut.MigrationEntityOptions.Input.Any(x => x.IsChecked.Input));
        }

        [TestMethod]
        public void MigrationOptionsFailedNextTest()
        {
            var entitiesList = new[]
            {
                MigrationEntity.AccountHolders,
                MigrationEntity.Checklists,
                MigrationEntity.OpReports,
            };

            var sourceNames = new[]
            {
                "Data source 1",
                "Data source 2"
            };

            var badSourceName = "bad source name sample";
            var goodSourceName = "good source name sample";
            var sourceNameCreationResult = "bad source name";

            var entityErrors = new[]
            {
                "bad entities selection"
            };

            var dataSourceErrors = new[]
            {
                "bad data source name"
            };

            var mockSelectedOptionsService = new Mock<ISelectedOptionsSettingsService>();
            mockSelectedOptionsService
                .Setup(x => x.SelectedMigrationSourceSystem)
                .Returns(MigrationSourceSystem.C2cXml);

            var mockSupportedEntitiesProvider = new Mock<ISupportedEntitiesInfoProvider>();
            mockSupportedEntitiesProvider
                .Setup(x => x.GetSupportedEntityTypes(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(entitiesList);

            var mockMigrationSourceService = new Mock<IMigrationSourceService>();
            mockMigrationSourceService
                .Setup(x => x.GetPreviousSourceNames(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(sourceNames);
            mockMigrationSourceService
                .Setup(x => x.CreateSource(
                    It.Is<MigrationSourceSystem>(y => y == MigrationSourceSystem.C2cXml),
                    It.Is<string>(y => y == badSourceName)))
                .Returns(sourceNameCreationResult);

            var mockValidationService = new Mock<IValidationService>();
            mockValidationService
                .Setup(x => x.ValidateSettings(
                    It.Is<MigrationSourceSystem>(y =>
                        y == MigrationSourceSystem.C2cXml),
                    new MigrationEntity[]
                    {
                    }))
                .Returns(entityErrors);
            mockValidationService
                .Setup(x => x.ValidateDataSourceName(
                    It.Is<string>(y => string.IsNullOrWhiteSpace(y))))
                .Returns(dataSourceErrors);

            var sut = new MigrationOptionsViewModel(true);
            sut.SupportedEntitiesInfoProvider = mockSupportedEntitiesProvider.Object;
            sut.SelectedOptionsSettingsService = mockSelectedOptionsService.Object;
            sut.MigrationSourceService = mockMigrationSourceService.Object;
            sut.ValidationService = mockValidationService.Object;
            var errorEvents = new List<string>();
            sut.MigrationEntityOptions.ErrorsChanged += (sender, args) =>
            {
                Assert.AreEqual("Input", args.PropertyName);
                if (sut.MigrationEntityOptions.HasErrors)
                {
                    errorEvents.Add("MigrationEntityOptions");
                }
            };
            sut.SelectedSourceName.ErrorsChanged += (sender, args) =>
            {
                Assert.AreEqual("Input", args.PropertyName);
                if (sut.SelectedSourceName.HasErrors)
                {
                    errorEvents.Add("SelectedSourceName");
                }
            };

            sut.RefreshEntityOptionsCommand.Value.Execute(null);
            sut.NextCommand.Value.Execute(null);
            Assert.IsTrue(sut.MigrationEntityOptions.HasErrors);
            Assert.IsTrue(sut.SelectedSourceName.HasErrors);
            Assert.AreEqual(2, errorEvents.Count);
            Assert.IsTrue(errorEvents.Contains("MigrationEntityOptions"));
            Assert.IsTrue(errorEvents.Contains("SelectedSourceName"));
            var entityErrorResult = sut.MigrationEntityOptions.GetErrors("Input").OfType<string>().ToArray();

            Assert.AreEqual(1, entityErrorResult.Length);
            Assert.AreEqual(entityErrors[0], entityErrorResult[0]);

            var selectedSourceNameResult = sut.SelectedSourceName.GetErrors("Input").OfType<string>().ToArray();
            Assert.AreEqual(1, entityErrorResult.Length);
            Assert.AreEqual(dataSourceErrors[0], selectedSourceNameResult[0]);

            sut.MigrationEntityOptions.Input[0].IsChecked.Input = true;
            Assert.IsFalse(sut.MigrationEntityOptions.HasErrors);

            sut.SelectedSourceName.Input = badSourceName;
            Assert.IsFalse(sut.SelectedSourceName.HasErrors);

            errorEvents.Clear();
            sut.NextCommand.Value.Execute(null);
            selectedSourceNameResult = sut.SelectedSourceName.GetErrors("Input").OfType<string>().ToArray();
            Assert.AreEqual(1, entityErrorResult.Length);
            Assert.AreEqual(sourceNameCreationResult, selectedSourceNameResult[0]);
            Assert.AreEqual(1, errorEvents.Count);
            Assert.IsTrue(errorEvents.Contains("SelectedSourceName"));

            errorEvents.Clear();
            sut.MigrationEntityOptions.Input[0].IsChecked.Input = false;
            sut.SelectedSourceName.Input = goodSourceName;
            sut.NextCommand.Value.Execute(null);
            Assert.AreEqual(1, errorEvents.Count);
            Assert.IsTrue(errorEvents.Contains("MigrationEntityOptions"));
        }

        [TestMethod]
        public void MigrationOptionsPassedNextTest()
        {
            var entitiesList = new[]
            {
                MigrationEntity.AccountHolders,
                MigrationEntity.Checklists,
                MigrationEntity.OpReports,
            };

            var sourceNames = new[]
            {
                "Data source 1",
                "Data source 2"
            };

            var mockSelectedOptionsService = new Mock<ISelectedOptionsSettingsService>();
            mockSelectedOptionsService
                .Setup(x => x.SelectedMigrationSourceSystem)
                .Returns(MigrationSourceSystem.C2cXml);

            var mockSupportedEntitiesProvider = new Mock<ISupportedEntitiesInfoProvider>();
            mockSupportedEntitiesProvider
                .Setup(x => x.GetSupportedEntityTypes(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(entitiesList);

            var mockMigrationSourceService = new Mock<IMigrationSourceService>();
            mockMigrationSourceService
                .Setup(x => x.GetPreviousSourceNames(It.Is<MigrationSourceSystem>(y =>
                    y == MigrationSourceSystem.C2cXml)))
                .Returns(sourceNames);

            var mockValidationService = new Mock<IValidationService>();
            mockValidationService
                .Setup(x => x.ValidateSettings(
                    It.Is<MigrationSourceSystem>(y =>
                        y == MigrationSourceSystem.C2cXml),
                    new[]
                    {
                        MigrationEntity.AccountHolders
                    }))
                .Returns(new string[]
                {
                });

            var mockApplicationState = new Mock<IApplicationStateViewModel>();

            var sut = new MigrationOptionsViewModel(true);
            sut.SupportedEntitiesInfoProvider = mockSupportedEntitiesProvider.Object;
            sut.SelectedOptionsSettingsService = mockSelectedOptionsService.Object;
            sut.MigrationSourceService = mockMigrationSourceService.Object;
            sut.ValidationService = mockValidationService.Object;
            sut.ApplicationState = mockApplicationState.Object;

            var testDateTime = DateTime.Now;
            sut.RefreshEntityOptionsCommand.Value.Execute(null);
            sut.SelectedSourceName.Input = sourceNames[0];
            sut.MigrationEntityOptions.Input[0].IsChecked.Input = true;
            sut.ParallelReadOption.Input = true;
            sut.MigrationStartDateTime.Input = testDateTime;
            sut.NextCommand.Value.Execute(null);

            mockSelectedOptionsService.VerifySet(x => x.SelectedMigrationSourceName = sourceNames[0], Times.Once);
            mockSelectedOptionsService.VerifySet(x => x.SelectedMigrationEntities = new[] { MigrationEntity.AccountHolders }, Times.Once);
            mockSelectedOptionsService.VerifySet(x => x.SelectedMigrationDateTime = testDateTime, Times.Once);
            mockSelectedOptionsService.VerifySet(x => x.SelectedParallelReadOption = true, Times.Once);
            mockApplicationState.Verify(x => x.DoStateAction(ApplicationStateAction.Next), Times.Once);
        }

        [TestMethod]
        public void MigrationOptionsBackTest()
        {
            var mockApplicationState = new Mock<IApplicationStateViewModel>();

            var sut = new MigrationOptionsViewModel(true);
            sut.ApplicationState = mockApplicationState.Object;

            sut.BackCommand.Value.Execute(null);
            mockApplicationState.Verify(x => x.DoStateAction(ApplicationStateAction.Back), Times.Once);
        }

        private class FinishingToken
        {
            public bool Finished { get; set; }
        }
    }
}
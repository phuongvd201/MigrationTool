using System;
using System.ComponentModel;
using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.MockViewModels
{
    internal class MockMigrationOptionsViewModel : IMigrationOptionsViewModel
    {
        public MockMigrationOptionsViewModel()
        {
            var mockPreviousSourceNames = new Mock<IProperty<string[]>>();
            mockPreviousSourceNames.Setup(x => x.Value).Returns(new[] { "Test Data Source Name" });
            PreviousSourceNames = mockPreviousSourceNames.Object;

            var mockSelectedSourceName = new Mock<IValidationInputProperty<string>>();
            mockSelectedSourceName.Setup(x => x.Input).Returns("Test Data Source Name");
            mockSelectedSourceName.Setup(x => x.HasErrors).Returns(true);
            mockSelectedSourceName.Setup(x => x.GetErrors(It.IsAny<string>())).Returns(new[] { "Bad name", "Bad user" });
            mockSelectedSourceName.Raise(x => x.ErrorsChanged += null, new DataErrorsChangedEventArgs("Input"));
            SelectedSourceName = mockSelectedSourceName.Object;

            var mockMigrationStartDateTime = new Mock<IInputProperty<DateTime>>();
            mockMigrationStartDateTime.Setup(x => x.Input).Returns(DateTime.Now);
            MigrationStartDateTime = mockMigrationStartDateTime.Object;

            var mockIsChecked = new Mock<IInputProperty<bool>>();
            mockIsChecked.Setup(x => x.Input).Returns(true);
            var mockOptionViewModel = new Mock<IMigrationEntityOptionViewModel>();
            mockOptionViewModel.Setup(x => x.Value).Returns(MigrationEntity.AccountHolders);
            mockOptionViewModel.Setup(x => x.IsChecked).Returns(mockIsChecked.Object);

            var mockMigrationEntityOptions = new Mock<IValidationInputProperty<IMigrationEntityOptionViewModel[]>>();
            mockMigrationEntityOptions.Setup(x => x.Input).Returns(new[] { mockOptionViewModel.Object });
            mockMigrationEntityOptions.Setup(x => x.HasErrors).Returns(true);
            mockMigrationEntityOptions.Setup(x => x.GetErrors(It.IsAny<string>())).Returns(new[] { "Something's wrong with the entities selection" });
            mockMigrationEntityOptions.Raise(x => x.ErrorsChanged += null, new DataErrorsChangedEventArgs("Input"));
            MigrationEntityOptions = mockMigrationEntityOptions.Object;

            var mockLoading = new Mock<IProperty<bool>>();
            mockLoading.Setup(x => x.Value).Returns(false);

            SelectAll = mockIsChecked.Object;
            ParallelReadOption = mockIsChecked.Object;
            Loading = mockLoading.Object;
        }

        public IProperty<string[]> PreviousSourceNames { get; private set; }

        public IValidationInputProperty<string> SelectedSourceName { get; private set; }

        public IInputProperty<DateTime> MigrationStartDateTime { get; private set; }

        public IValidationInputProperty<IMigrationEntityOptionViewModel[]> MigrationEntityOptions { get; private set; }

        public IInputProperty<bool> SelectAll { get; private set; }

        public IProperty<bool> Loading { get; private set; }

        public IInputProperty<bool> ParallelReadOption { get; private set; }

        public IProperty<ICommand> RefreshEntityOptionsCommand
        {
            get { return null; }
        }

        public IProperty<ICommand> BackCommand
        {
            get { return null; }
        }

        public IProperty<ICommand> NextCommand
        {
            get { return null; }
        }
    }
}
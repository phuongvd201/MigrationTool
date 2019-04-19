using System.ComponentModel;
using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

namespace MigrationTool.MockViewModels
{
    internal class MockZedmedOptionsViewModel : IZedmedOptionsViewModel
    {
        public MockZedmedOptionsViewModel()
        {
            var mockDatabaseFolderPath = new Mock<IValidationInputProperty<string>>();
            mockDatabaseFolderPath.Setup(x => x.Input).Returns("c:\\My Test Folder Database");
            mockDatabaseFolderPath.Setup(x => x.HasErrors).Returns(true);
            mockDatabaseFolderPath.Setup(x => x.GetErrors(It.IsAny<string>())).Returns(new[] { "Some bad error." });
            mockDatabaseFolderPath.Raise(x => x.ErrorsChanged += null, new DataErrorsChangedEventArgs("Input"));
            DatabaseFolderPath = mockDatabaseFolderPath.Object;

            var mockDocumentsPath = new Mock<IInputProperty<string>>();
            mockDocumentsPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Documents\\");
            DocumentsPath = mockDocumentsPath.Object;

            var mockUsername = new Mock<IInputProperty<string>>();
            mockUsername.Setup(x => x.Input).Returns("superAdmin");
            Username = mockUsername.Object;

            var mockErrors = new Mock<IProperty<string[]>>();
            mockErrors.Setup(x => x.Value).Returns(new[] { "Invalid connection settings" });
            Errors = mockErrors.Object;
        }

        public IValidationInputProperty<string> DatabaseFolderPath { get; private set; }

        public IInputProperty<string> DocumentsPath { get; private set; }

        public IInputProperty<string> Username { get; private set; }

        public IProperty<string[]> Errors { get; private set; }

        public IProperty<ICommand> ReadConfigurationCommand
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
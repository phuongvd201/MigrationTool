using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

namespace MigrationTool.MockViewModels
{
    internal class MockMedicalDirectorOptionsViewModel : IMedicalDirectorOptionsViewModel
    {
        public MockMedicalDirectorOptionsViewModel()
        {
            var mockDocumentsPath = new Mock<IInputProperty<string>>();
            mockDocumentsPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Documents\\");
            DocumentsPath = mockDocumentsPath.Object;

            var mockDatabaseServerName = new Mock<IInputProperty<string>>();
            mockDatabaseServerName.Setup(x => x.Input).Returns("Server name");
            DatabaseServerName = mockDatabaseServerName.Object;

            var mockErrors = new Mock<IProperty<string[]>>();
            mockErrors.Setup(x => x.Value).Returns(new[] { "Invalid connection settings" });
            Errors = mockErrors.Object;
        }

        public IInputProperty<string> DocumentsPath { get; private set; }

        public IInputProperty<string> DatabaseServerName { get; private set; }

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
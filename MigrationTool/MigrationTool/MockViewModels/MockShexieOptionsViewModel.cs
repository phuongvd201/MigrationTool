using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

namespace MigrationTool.MockViewModels
{
    internal class MockShexieOptionsViewModel : IShexieOptionsViewModel
    {
        public MockShexieOptionsViewModel()
        {
            var mockDatabaseFilePath = new Mock<IInputProperty<string>>();
            mockDatabaseFilePath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Shexie\\test.mdb");
            DatabaseFilePath = mockDatabaseFilePath.Object;

            var mockDocumentsPath = new Mock<IInputProperty<string>>();
            mockDocumentsPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Documents\\");
            DocumentsPath = mockDocumentsPath.Object;
        }

        public IInputProperty<string> DatabaseFilePath { get; private set; }

        public IInputProperty<string> DocumentsPath { get; private set; }

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
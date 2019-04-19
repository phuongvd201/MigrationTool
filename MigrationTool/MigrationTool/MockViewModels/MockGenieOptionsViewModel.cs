using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

namespace MigrationTool.MockViewModels
{
    internal class MockGenieOptionsViewModel : IGenieOptionsViewModel
    {
        public MockGenieOptionsViewModel()
        {
            var mockDocumentsPath = new Mock<IInputProperty<string>>();
            mockDocumentsPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Documents\\");
            DocumentsPath = mockDocumentsPath.Object;

            var mockMd3XmlPath = new Mock<IInputProperty<string>>();
            mockMd3XmlPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Documents\\");
            Md3XmlPath = mockMd3XmlPath.Object;

            var mockXmlExportPath = new Mock<IInputProperty<string>>();
            mockXmlExportPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Xml\\outfile.xml");
            XmlExportPath = mockXmlExportPath.Object;

            var mockLaboratoryResultsPath = new Mock<IInputProperty<string>>();
            mockLaboratoryResultsPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Laboratory Results\\");
            LaboratoryResultsPath = mockLaboratoryResultsPath.Object;

            var mockIp = new Mock<IInputProperty<string>>();
            mockIp.Setup(x => x.Input).Returns("127.0.0.1");
            Ip = mockIp.Object;

            var mockPort = new Mock<IInputProperty<string>>();
            mockPort.Setup(x => x.Input).Returns("8080");
            Port = mockPort.Object;

            var mockUsername = new Mock<IInputProperty<string>>();
            mockUsername.Setup(x => x.Input).Returns("superadmin");
            Username = mockUsername.Object;

            var mockErrors = new Mock<IProperty<string[]>>();
            mockErrors.Setup(x => x.Value).Returns(new[] { "Invalid connection settings" });
            Errors = mockErrors.Object;
        }

        public IInputProperty<string> DocumentsPath { get; private set; }

        public IInputProperty<string> Md3XmlPath { get; private set; }

        public IInputProperty<string> XmlExportPath { get; private set; }

        public IInputProperty<string> LaboratoryResultsPath { get; private set; }

        public IInputProperty<string> Ip { get; private set; }

        public IInputProperty<string> Port { get; private set; }

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
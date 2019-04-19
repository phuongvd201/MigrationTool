using System.ComponentModel;
using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

namespace MigrationTool.MockViewModels
{
    internal class MockC2cXmlOptionsViewModel : IC2cXmlOptionsViewModel
    {
        public MockC2cXmlOptionsViewModel()
        {
            var mockC2cXmlPath = new Mock<IValidationInputProperty<string>>();
            mockC2cXmlPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\test.xml");
            mockC2cXmlPath.Setup(x => x.HasErrors).Returns(true);
            mockC2cXmlPath.Setup(x => x.GetErrors(It.IsAny<string>())).Returns(new[] { "Some bad error." });
            mockC2cXmlPath.Raise(x => x.ErrorsChanged += null, new DataErrorsChangedEventArgs("Input"));
            C2cXmlPath = mockC2cXmlPath.Object;

            var mockC2cDocumentsPath = new Mock<IInputProperty<string>>();
            mockC2cDocumentsPath.Setup(x => x.Input).Returns("c:\\My Test Folder\\Documents\\");
            C2cDocumentsPath = mockC2cDocumentsPath.Object;
        }

        public IValidationInputProperty<string> C2cXmlPath { get; private set; }

        public IInputProperty<string> C2cDocumentsPath { get; private set; }

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
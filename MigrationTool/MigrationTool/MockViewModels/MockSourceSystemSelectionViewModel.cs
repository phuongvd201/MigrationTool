using System.Collections.Generic;
using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.MockViewModels
{
    internal class MockSourceSystemSelectionViewModel : ISourceSystemSelectionViewModel
    {
        public MockSourceSystemSelectionViewModel()
        {
            var mockGenieOption = new Mock<ISourceSystemOptionViewModel>();
            mockGenieOption.Setup(x => x.SourceSystem).Returns(MigrationSourceSystem.Genie);

            var mockShexieOption = new Mock<ISourceSystemOptionViewModel>();
            mockShexieOption.Setup(x => x.SourceSystem).Returns(MigrationSourceSystem.Shexie);

            var mockXmlOption = new Mock<ISourceSystemOptionViewModel>();
            mockXmlOption.Setup(x => x.SourceSystem).Returns(MigrationSourceSystem.C2cXml);

            var mockZedmedOption = new Mock<ISourceSystemOptionViewModel>();
            mockZedmedOption.Setup(x => x.SourceSystem).Returns(MigrationSourceSystem.Zedmed);

            var mockMedicalDirectorOptions = new Mock<ISourceSystemOptionViewModel>();
            mockMedicalDirectorOptions.Setup(x => x.SourceSystem).Returns(MigrationSourceSystem.MedicalDirector);

            var mockSourceSystemOptions = new Mock<IProperty<IEnumerable<ISourceSystemOptionViewModel>>>();
            mockSourceSystemOptions.Setup(x => x.Value)
                .Returns(new[]
                {
                    mockGenieOption.Object,
                    mockShexieOption.Object,
                    mockXmlOption.Object,
                    mockZedmedOption.Object,
                    mockMedicalDirectorOptions.Object,
                });

            SourceSystemOptions = mockSourceSystemOptions.Object;
        }

        public IProperty<IEnumerable<ISourceSystemOptionViewModel>> SourceSystemOptions { get; private set; }

        public IProperty<ICommand> SkipIfSingleCommand
        {
            get { return null; }
        }
    }
}
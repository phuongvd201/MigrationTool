using System;
using System.Linq;
using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Moq;

using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.MockViewModels
{
    internal class MockMigrationProgressViewModel : IMigrationProgressViewModel
    {
        public MockMigrationProgressViewModel()
        {
            var mockTotal = new Mock<IInputProperty<int>>();
            mockTotal.Setup(x => x.Value).Returns(20);

            var mockEntityProgressViewModels =
                Enum.GetValues(typeof(MigrationEntity)).OfType<MigrationEntity>()
                    .Zip(
                        Enumerable.Range(1, 25),
                        (x, y) =>
                        {
                            var mockStatus = new Mock<IInputProperty<string>>();
                            mockStatus.Setup(z => z.Value).Returns(
                                y % 2 == 1
                                    ? y + " entities migrated."
                                    : y % 4 == 2
                                        ? "Pending..."
                                        : "Completed.");

                            var mockEnabled = new Mock<IInputProperty<bool>>();
                            mockEnabled.Setup(z => z.Value).Returns(y % 2 == 1);

                            var mockCompletedWhole = new Mock<IInputProperty<double>>();
                            mockCompletedWhole.Setup(z => z.Value).Returns(y % 4 == 2 ? 0 : 1);

                            var mockEntityProgressViewModel = new Mock<IEntityProgressViewModel>();
                            mockEntityProgressViewModel.Setup(z => z.Entity).Returns(x);
                            mockEntityProgressViewModel.Setup(z => z.Status).Returns(mockStatus.Object);
                            mockEntityProgressViewModel.Setup(z => z.Progress).Returns(mockCompletedWhole.Object);
                            mockEntityProgressViewModel.Setup(z => z.Total).Returns(mockCompletedWhole.Object);
                            mockEntityProgressViewModel.Setup(z => z.Running).Returns(mockEnabled.Object);
                            return mockEntityProgressViewModel.Object;
                        }).ToArray();

            var mockEntitiesProgress = new Mock<IProperty<IEntityProgressViewModel[]>>();
            mockEntitiesProgress.Setup(x => x.Value).Returns(mockEntityProgressViewModels);
            EntitiesProgress = mockEntitiesProgress.Object;

            var mockMigrationResult = new Mock<IProperty<bool>>();
            mockMigrationResult.Setup(x => x.Value).Returns(true);
            MigrationResult = mockMigrationResult.Object;
        }

        public IProperty<IEntityProgressViewModel[]> EntitiesProgress { get; private set; }

        public IProperty<bool> MigrationResult { get; private set; }

        public IProperty<ICommand> ReadyCommand
        {
            get { return null; }
        }

        public IProperty<ICommand> StartCommand
        {
            get { return null; }
        }
    }
}
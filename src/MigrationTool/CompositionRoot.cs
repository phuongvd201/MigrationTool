using MigrationTool.Integration;
using MigrationTool.ViewModels.Interfaces;

using Log4netConfig = log4net.Config;

namespace MigrationTool
{
    internal class CompositionRoot
    {
        public IMainWindowViewModel MainWindowViewModel { get; private set; }

        public CompositionRoot()
        {
            Log4netConfig.XmlConfigurator.Configure();
            MainWindowViewModel = AppComposer.ComposeMainViewModel();
        }
    }
}
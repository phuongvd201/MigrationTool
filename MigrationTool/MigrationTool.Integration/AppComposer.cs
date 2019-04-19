using System;

using Autofac;

using MigrationTool.Services.Implementations;
using MigrationTool.Services.Interfaces;
using MigrationTool.ViewModels;
using MigrationTool.ViewModels.Interfaces;

namespace MigrationTool.Integration
{
    public static class AppComposer
    {
        public static IMainWindowViewModel ComposeMainViewModel()
        {
            return new ContainerBuilder()
                .RegisterComponents()
                .Build()
                .Resolve<IMainWindowViewModel>();
        }

        private static ContainerBuilder RegisterComponents(this ContainerBuilder builder)
        {
            var servicesAssembly = typeof(MigrationService).Assembly;

            builder.RegisterAssemblyTypes(servicesAssembly)
                .Where(x => x.Name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase))
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterAssemblyTypes(servicesAssembly)
                .Where(x => x.Name.EndsWith("Repository", StringComparison.InvariantCultureIgnoreCase))
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterAssemblyTypes(servicesAssembly)
                .Where(x => x.Name.EndsWith("Container", StringComparison.InvariantCultureIgnoreCase))
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

            var viewModelsAssembly = typeof(MainWindowViewModel).Assembly;

            builder.RegisterAssemblyTypes(viewModelsAssembly)
                .Where(x => x.Name.EndsWith("ViewModel", StringComparison.InvariantCultureIgnoreCase))
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterType<TextConverter>()
                .As<ITextConverter>()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterType<MigrationDataRoot>()
                .As<IMigrationDataProcessor>()
                .As<ISupportedEntitiesInfoProvider>()
                .PropertiesAutowired()
                .SingleInstance();

            return builder;
        }
    }
}
using System;

using MigrationTool.ViewModels.Interfaces;

using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels
{
    internal class ApplicationStateViewModel : IApplicationStateViewModel
    {
        private readonly ICallProperty<ApplicationState> localState;

        private Func<ApplicationState, ApplicationState> localSelectedGetNewState;

        public IProperty<ApplicationState> State
        {
            get { return localState; }
        }

        public ApplicationStateViewModel()
        {
            localState = Reloadable<ApplicationState>
                .On().Each().Call(x => localSelectedGetNewState(x))
                .Create(ApplicationState.NotLoggedIn);
        }

        public void DoStateAction(ApplicationStateAction action)
        {
            switch (action)
            {
                case ApplicationStateAction.Next:
                    localSelectedGetNewState = Next;
                    break;

                case ApplicationStateAction.Back:
                    localSelectedGetNewState = Back;
                    break;

                default:
                    return;
            }

            localState.Go();
        }

        public void DoStateAction(
            ApplicationStateAction action,
            MigrationSourceSystem optionSourceSystem)
        {
            switch (action)
            {
                case ApplicationStateAction.Next:
                    localSelectedGetNewState = x => Next(x, optionSourceSystem);
                    break;

                default:
                    DoStateAction(action);
                    return;
            }

            localState.Go();
        }

        private ApplicationState Next(
            ApplicationState state,
            MigrationSourceSystem optionSourceSystem)
        {
            switch (state)
            {
                case ApplicationState.SourceSystemSelection:
                    return optionSourceSystem == MigrationSourceSystem.Genie
                        ? ApplicationState.GenieOptions
                        : optionSourceSystem == MigrationSourceSystem.Shexie
                            ? ApplicationState.ShexieOptions
                            : optionSourceSystem == MigrationSourceSystem.C2cXml
                                ? ApplicationState.C2cXmlOptions
                                : optionSourceSystem == MigrationSourceSystem.Zedmed
                                    ? ApplicationState.ZedmedOptions
                                    : optionSourceSystem == MigrationSourceSystem.MedicalDirector
                                        ? ApplicationState.MedicalDirectorOptions
                                        : state;

                default:
                    return state;
            }
        }

        private ApplicationState Next(ApplicationState state)
        {
            switch (state)
            {
                case ApplicationState.NotLoggedIn:
                    return ApplicationState.LoginInProgress;

                case ApplicationState.LoginInProgress:
                    return ApplicationState.SourceSystemSelection;

                case ApplicationState.ShexieOptions:
                    return ApplicationState.MigrationOptions;

                case ApplicationState.GenieOptions:
                    return ApplicationState.MigrationOptions;

                case ApplicationState.ZedmedOptions:
                    return ApplicationState.MigrationOptions;

                case ApplicationState.C2cXmlOptions:
                    return ApplicationState.C2cXmlValidation;

                case ApplicationState.C2cXmlValidation:
                    return ApplicationState.MigrationOptions;

                case ApplicationState.MigrationOptions:
                    return ApplicationState.MigrationInProgress;

                case ApplicationState.MigrationInProgress:
                    return ApplicationState.MigrationResult;

                case ApplicationState.MedicalDirectorOptions:
                    return ApplicationState.MigrationOptions;

                default:
                    return state;
            }
        }

        private ApplicationState Back(ApplicationState state)
        {
            switch (state)
            {
                case ApplicationState.LoginInProgress:
                    return ApplicationState.NotLoggedIn;

                case ApplicationState.ShexieOptions:
                    return ApplicationState.SourceSystemSelection;

                case ApplicationState.GenieOptions:
                    return ApplicationState.SourceSystemSelection;

                case ApplicationState.ZedmedOptions:
                    return ApplicationState.SourceSystemSelection;

                case ApplicationState.C2cXmlValidation:
                    return ApplicationState.C2cXmlOptions;

                case ApplicationState.C2cXmlOptions:
                    return ApplicationState.SourceSystemSelection;

                case ApplicationState.MigrationOptions:
                    return ApplicationState.SourceSystemSelection;

                case ApplicationState.MigrationResult:
                    return ApplicationState.SourceSystemSelection;

                case ApplicationState.MedicalDirectorOptions:
                    return ApplicationState.SourceSystemSelection;

                default:
                    return ApplicationState.NotLoggedIn;
            }
        }
    }
}
using System;
using System.Collections.Generic;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Interfaces.C2cXml;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;
using Siberia.Migration.Serialization.Xml;

using Serialization = Siberia.Migration.Serialization.Xml;

namespace MigrationTool.Services.Implementations.C2cXml
{
    internal class C2cXmlEntitiesContainer : IC2cXmlEntitiesContainer
    {
        public IC2cXmlSettingsService C2cXmlSettingsService { get; set; }

        public MigrationSourceSystem SourceSystem
        {
            get { return MigrationSourceSystem.C2cXml; }
        }

        public void ProcessEntities(Action<IEnumerable<IMigrationEntity>> processEntities, MigrationEntity entityType, MigrationArgs args)
        {
            Serialization.C2cXml
                .ReadAndProcess(processEntities, entityType)
                .FromXml().FromFile(C2cXmlSettingsService.C2cXmlPath);
        }

        public MigrationEntity[] SupportedEntityTypes
        {
            get
            {
                var entityTypes = new MigrationEntity[] { };

                Serialization.C2cXml
                    .ReadEntityTypes(x =>
                    {
                        entityTypes = x;
                    })
                    .FromXml().FromFile(C2cXmlSettingsService.C2cXmlPath);

                return entityTypes;
            }
        }
    }
}
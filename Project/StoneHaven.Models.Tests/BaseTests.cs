using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using NUnit.Framework;
using Rhino.Commons;
using Rhino.Commons.ForTesting;

namespace StoneHaven.Models.Tests
{
    public class BaseTests : DatabaseTestFixtureBase
    {
        [TestFixtureSetUp]
        public virtual void Setup()
        {
            IoC.Reset();
            DisposeAndRemoveAllUoWTestContexts();
            IntializeNHibernateAndIoC(PersistenceFramework.ActiveRecord, "Windsor.boo", DatabaseEngine.MsSql2005Express, "shgw_Test", MappingInfo.From(typeof(Quotation).Assembly));
            //ActiveRecordStarter.Initialize(typeof(Quotation).Assembly, new XmlConfigurationSource("app.config.xml"));
            ActiveRecordStarter.GenerateCreationScripts("database.sql");
            ActiveRecordStarter.DropSchema();
            ActiveRecordStarter.CreateSchema();
        }

        [TestFixtureTearDown]
        public virtual void EndTest()
        {
            DisposeAndRemoveAllUoWTestContexts();
        }

        [SetUp]
        public virtual void SetUp()
        {
            CurrentContext.CreateUnitOfWork();
        }

        [TearDown]
        public virtual void TearDown()
        {
            CurrentContext.DisposeUnitOfWork();
        }
    }
}
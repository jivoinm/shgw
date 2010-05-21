using System;
using System.Configuration;
using System.Web;
using Castle.Components.Common.EmailSender;
using Castle.MicroKernel.Registration;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Configuration;
using Castle.MonoRail.Framework.Container;
using Castle.MonoRail.Framework.Helpers.ValidationStrategy;
using Castle.MonoRail.Framework.JSGeneration;
using Rhino.Commons;
using Rhino.Commons.HttpModules;
using Castle.MonoRail.Framework.JSGeneration.jQuery;


namespace StoneHaven.Web
{

    public class Global : UnitOfWorkApplication, IMonoRailContainerEvents,IMonoRailConfigurationEvents
    {

        public void Configure(IMonoRailConfiguration config)
        {
           config.JSGeneratorConfiguration.AddLibrary("jquery-1.2.1", typeof(JQueryGenerator))
               .AddExtension(typeof(CommonJSExtension))
               .ElementGenerator
                   .AddExtension(typeof(JQueryElementGenerator))
                   .Done
               .BrowserValidatorIs(typeof(JQueryValidator))
               .SetAsDefault();
        }


        public void Created(IMonoRailContainer container)
        {
//            var client = new System.Net.WebClient();
//            
//            try
//            {
//                var file = client.DownloadData("http://docs.google.com/Doc?id=dcdh4xvw_315s3k2zcr");
//                if (file == null) throw new Exception("Error loading Application, please contact the system administrator.");
//            }
//            catch (Exception e)
//            {
//                throw new Exception("Error loading Application, please contact the system administrator.");
//            }
            
        }

        public void Initialized(IMonoRailContainer container)
        {
            IoC.Container.Register(
                 Component.For<IEmailTemplateService>().Instance(container.EmailTemplateService),
                 Component.For<IEmailSender>().Instance(container.EmailSender),
                 Component.For<IViewSourceLoader>().Instance(container.ViewSourceLoader),
                 Component.For<IEngineContextFactory>().Instance(container.EngineContextFactory)
             );

        }

        
    }
}
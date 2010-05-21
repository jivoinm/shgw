import System.Reflection
import Rhino.Commons
import Castle.MonoRail.Framework from Castle.MonoRail.Framework 
import Castle.MonoRail.Framework.Adapters from Castle.MonoRail.Framework
import Castle.MonoRail.WindsorExtension
import Castle.Facilities.EventWiring from Castle.MicroKernel
import Rhino.Commons.ForTesting from Rhino.Commons.NHibernate
import Rhino.Commons.ForTesting from Rhino.Commons.ActiveRecord
import Castle.Facilities.Logging
import Rhino.Commons.Facilities from Rhino.Commons.ActiveRecord
import Components from Components
import Components.Grids from Components
import StoneHaven.Models.Services from StoneHaven.Models
import StoneHaven.Models.Services.DB from StoneHaven.Models
import Components.Page from Components 

Facility("monorail", MonoRailFacility )
Facility("loggerFacility", LoggingFacility, loggingApi: LoggerImplementation.Log4net, 	configFile: "log4net.config") 
Facility("eventWiring", EventWiringFacility )
Facility ActiveRecordUnitOfWorkFacility:assembly = "StoneHaven.Models"

webAsm = Assembly.Load("StoneHaven.Web")
viewComponents = Assembly.Load("Components")

#Services
component IQuoteServices, QuoteServices 
component IEntityServices, EntityServices 
component IJobServices, JobServices 
component ICalendar, SHGWCalendar
component IUserServices, UserServices
component IReportServices, ReportServices

# controllera
for type in webAsm.GetTypes():
	if typeof(Controller).IsAssignableFrom(type):
		Component(type.Name, type)
	elif typeof(ViewComponent).IsAssignableFrom(type):
		Component(type.Name, type)

#component 'user_services',IUserServices, UserServices

# views
for viewType in viewComponents.GetTypes():
	if typeof(ViewComponent).IsAssignableFrom(viewType):
		  Component(viewType.Name, viewType)


using D365.Client.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.Client.Plugins
{
    /// <summary>
    /// Executes post creation of a Contact.
    /// Gets the Contact's parent Account, then counts all other Contact's associated with the same Account.
    /// Updates the description of the created Contact to: "Contact #[count] for [Account name]".
    /// </summary>
    public sealed class SetContactName : IPlugin
    {
        private IPluginExecutionContext Context { get; set; }
        private IOrganizationServiceFactory ServiceFactory { get; set; }
        private IOrganizationService Service { get; set; }
        private OrganizationServiceContext ServiceContext { get; set; }

        private Contact Contact { get; set; }
        private List<Contact> ChildContacts { get; set; }

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                ServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                Service = ServiceFactory.CreateOrganizationService(Context.UserId);
                ServiceContext = new OrganizationServiceContext(Service);

                Contact = ((Entity)Context.InputParameters["Target"]).ToEntity<Contact>();

                ChildContacts = ServiceContext.CreateQuery<Contact>()
                                              .Where(c => c.ParentCustomerId == Contact.ParentCustomerId)
                                              .Select(c => new Contact { Id = c.Id })
                                              .ToList();

                var parentAccountName = ServiceContext.CreateQuery<Account>()
                                                      .Where(a => a.Id == Contact.ParentCustomerId.Id)
                                                      .Select(a => a.Name)
                                                      .FirstOrDefault();

                var description = $"Contact {ChildContacts.Count} for {parentAccountName}";
                var updatedContact = new Contact { Id = Contact.Id, Description = description };

                Service.Update(updatedContact);
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}

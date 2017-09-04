using D365.Client.Models;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.Client.Tests
{
    [TestFixture]
    public class SetContactNameTests
    {
        private XrmFakedContext Context { get; set; }
        private IOrganizationService Service { get; set; }

        private Guid TargetId { get; set; }
        private Contact Target { get; set; }
        
        private Guid ParentAccountId { get; set; }
        private Account ParentAccount { get; set; }

        [SetUp]
        public void Init()
        {
            Context = new XrmFakedContext();
            Service = Context.GetFakedOrganizationService();

            ParentAccountId = Guid.NewGuid();
            ParentAccount = new Account
            {
                Id = ParentAccountId,
                Name = "Headquarters Account"
            };

            TargetId = Guid.NewGuid();
            Target = new Contact
            {
                Id = TargetId,
                ParentCustomerId = new EntityReference("account", ParentAccountId)
            };
        }

        [Test]
        public void Sets_Contacts_Name()
        {
            Context.Initialize(new List<Entity>
            {
                Target,
                ParentAccount,
                new Contact { Id = Guid.NewGuid(), ParentCustomerId = new EntityReference("account", ParentAccountId) },
                new Contact { Id = Guid.NewGuid(), ParentCustomerId = new EntityReference("account", ParentAccountId) },
                new Contact { Id = Guid.NewGuid(), ParentCustomerId = new EntityReference("account", Guid.NewGuid()) }
            });

            Context.ExecutePluginWithTarget<Plugins.SetContactName>(Target);

            var expectedDescription = "Contact 3 for Headquarters Account";

            var updatedContact = Context.CreateQuery<Contact>().Where(c => c.Id == TargetId)
                                                               .Select(c => new Contact { Id = c.Id, Description = c.Description })
                                                               .FirstOrDefault();

            Assert.AreEqual(expectedDescription, updatedContact.Description);
        }
    }
}

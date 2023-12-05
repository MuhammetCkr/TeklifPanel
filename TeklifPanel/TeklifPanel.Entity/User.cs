using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifPanel.Entity
{
    public class User : IdentityUser
    {
        public User()
        {
            Offers = new List<Offer>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompanyId { get; set; }
        public bool IsOfferShow { get; set; }
        public Company Company { get; set; }
        public List<Offer> Offers { get; set; }

    }
}

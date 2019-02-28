using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Enviroself.Areas.User.Features.Account.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        [StringLength(250)]
        public string Firstname { get; set; }

        [StringLength(250)]
        public string Lastname { get; set; }

        public string GetFullName
        {
            get
            {
                return this.Firstname + " " + this.Lastname;
            }
        }

        public string Gender { get; private set; }

        public void SetGender(string value)
        {
            if (value == null)
            {
                this.Gender = null;
                return;
            }
            switch (value.ToLower())
            {
                case "male": this.Gender = "Male"; break; 
                case "female": this.Gender = "Female"; break; 
                case "m": this.Gender = "Male"; break; 
                case "f": this.Gender = "Female"; break; 
                default:  this.Gender = null; break;
            };
        }

        public int? LanguageId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public long? FacebookId { get; set; }
        public string PictureUrl { get; set; }

        public IList<IdentityUserRole<int>> UserRoles { get; set; }
        //public IList<ApplicationUserRole> UserRoles { get; set; }
    }
}

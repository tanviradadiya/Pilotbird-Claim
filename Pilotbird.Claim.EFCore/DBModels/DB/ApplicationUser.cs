using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pilotbird.Claim.EFCore.DBModels.DB
{
    public class ApplicationUser : IdentityUser<Guid>
    {
         public string FirstName { get; set; }
        public string LastName { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string stName { get; set; }
        public Guid  uiClientId { get; set; }

        public  Guid? uiTeamId { get; set; }
        public string stClientProduct { get; set; }
        public Guid? uiDefaultProductId { get; set; }
        //public byte inUserType { get; set; }

        public Guid RoleId { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string stColorCode { get; set; }

        public bool flgIsSuperAdmin { get; set; }
        //public bool flgIsDataUpdated { get; set; }
        public bool flgIsSetupPassword { get; set; }
        public bool flgIsArchived { get; set; }
        public bool flgIsActive { get; set; }
   
        public string stProfileImageURL { get; set; }
        public Guid uiCreatedBy { get; set; }
        public DateTime dtCreatedOn { get; set; }
        public DateTime dtUpdatedOn { get; set; }
        public Guid? uiUpdatedBy { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string APIAccessToken { get; set; }
        public ApplicationUser()
        {
            flgIsSetupPassword = false;
           // flgIsDataUpdated = false;
            flgIsActive = true;
            flgIsArchived = false;
            dtUpdatedOn = DateTime.Now;
            dtCreatedOn = DateTime.Now;
            flgIsSuperAdmin = false;
        }
    }
}

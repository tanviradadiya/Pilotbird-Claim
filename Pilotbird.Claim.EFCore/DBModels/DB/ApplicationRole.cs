using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;


namespace Pilotbird.Claim.EFCore.DBModels.DB
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public bool flgIsArchived { get; set; }
        public bool flgIsNotAllowDelete { get; set; }
        public Guid uiCreatedBy { get; set; }
        public DateTime dtCreatedOn { get; set; }
        public DateTime dtUpdatedOn { get; set; }
        public Guid? uiUpdatedBy { get; set; }

        public ApplicationRole()
        {

            flgIsArchived = false;
            flgIsNotAllowDelete = false;
            dtUpdatedOn = DateTime.Now;
            dtCreatedOn = DateTime.Now;
        }

    }
}

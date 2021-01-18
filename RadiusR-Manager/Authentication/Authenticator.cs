using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RadiusR_Manager
{
    public class MasterISSAuthenticator : RezaB.Web.Authentication.Authenticator<RadiusREntities, AppUser, Role, Permission, System.Security.Cryptography.SHA256>
    {
        public MasterISSAuthenticator() : base(u => u.ID, u => u.Name, u => u.Email, u => u.Password, u => u.IsEnabled, r => r.Name, p => p.Name)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadiusR_Manager
{
    public class MasterISSAuthenticator: RezaB.Web.Authentication.Authenticator<RadiusR.DB.RadiusREntities,RadiusR.DB.AppUser, System.Security.Cryptography.SHA256>
    {
    }
}
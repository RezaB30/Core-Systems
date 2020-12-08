using Microsoft.Owin;
using RadiusR.DB;
using RadiusR.DB.Utilities.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;

namespace RadiusR_Manager
{
    public static class AuthenticatorExtentions
    {
        /// <summary>
        /// Signs in a user with username and checks the password.
        /// </summary>
        /// <param name="owinContext">Owin context.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns></returns>
        public static bool SignInUser(this IOwinContext owinContext, string username, string password)
        {
            // authenticate
            var authenticator = new MasterISSAuthenticator();
            var userId = authenticator.Authenticate(username, password);
            if (!userId.HasValue)
                return false;
            //authorize
            owinContext.SignInByUserId(userId.Value);
            return true;
        }

        /// <summary>
        /// Signs in user with id without password check. (authorization)
        /// </summary>
        /// <param name="owinContext">Owin context.</param>
        /// <param name="userId">Id of the user.</param>
        public static void SignInByUserId(this IOwinContext owinContext, int userId)
        {
            // extra claims
            var extraClaims = new List<Claim>();
            using (RadiusREntities db = new RadiusREntities())
            {
                var dbUser = db.AppUsers.Find(userId);
                var internalCallCenterNo = dbUser.InternalCallCenterNo;

                // --------------- DELETE THIS AFTER PARTNER UPDATE -------------
                var cashier = dbUser.Cashiers.FirstOrDefault();
                if (cashier != null && !cashier.IsEnabled)
                    return;
                if (cashier != null)
                    extraClaims.Add(new Claim("cashierId", cashier.ID.ToString()));
                // --------------------------------------------------------------
                // internal call center no
                if (!string.IsNullOrEmpty(internalCallCenterNo))
                    extraClaims.Add(new Claim("internalNo", internalCallCenterNo));
                // support groups
                var supportGroups = dbUser.SupportGroupUsers.Where(sgu => !dbUser.LeaderInGroups.Select(group => group.ID).Contains(sgu.SupportGroupID)).Select(sgu => new SupportGroupClaim()
                {
                    GroupId = sgu.SupportGroupID,
                    IsLeader = false,
                    CanChangeState = sgu.CanChangeState,
                    CanRedirect = sgu.CanRedirect,
                    CanWriteToCustomer = sgu.CanWriteToCustomer
                }).ToArray();
                var leaderInGroups = dbUser.LeaderInGroups.Where(sg => !supportGroups.Select(group => group.GroupId).Contains(sg.ID)).Select(sg => new SupportGroupClaim()
                {
                    GroupId = sg.ID,
                    CanChangeState = true,
                    CanRedirect = true,
                    CanWriteToCustomer = true,
                    IsLeader = true
                }).ToArray();
                supportGroups = supportGroups.Concat(leaderInGroups).ToArray();
                // serialize
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SupportGroupClaim));
                foreach (var supportClaim in supportGroups)
                {
                    var serializedString = new System.Text.StringBuilder();
                    using (var writer = new System.IO.StringWriter(serializedString))
                    {
                        serializer.Serialize(writer, supportClaim);
                        writer.Flush();
                        // add to claims
                        extraClaims.Add(new Claim("supportGroups", serializedString.ToString()));
                    }
                }
            }
            // authorize
            var authenticator = new MasterISSAuthenticator();
            authenticator.SignIn<Role, Permission>(owinContext, userId, extraClaims);
        }

        public static int? GiveUserId(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            return (claim == null) ? (int?)null : int.Parse(claim.Value);
        }

        /// <summary>
        /// Gives the user internal phone no (call center).
        /// </summary>
        /// <param name="User">The user.</param>
        /// <returns></returns>
        public static string GiveInternalNo(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.Claims.Where(c => c.Type == "internalNo").FirstOrDefault();
            return (claim == null) ? null : claim.Value;
        }

        /// <summary>
        /// Checks if user has internal no (call center).
        /// </summary>
        /// <param name="User">The user.</param>
        /// <returns></returns>
        public static bool HasInternalNo(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            return identity.Claims.Any(c => c.Type == "internalNo");
        }

        public static SupportGroupClaim[] GiveSupportGroups(this IPrincipal User)
        {
            var results = new List<SupportGroupClaim>();
            var identity = (ClaimsIdentity)User.Identity;
            var groupClaims = identity.Claims.Where(c => c.Type == "supportGroups").ToArray();
            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SupportGroupClaim));
                foreach (var claim in groupClaims)
                {
                    using (var reader = new System.IO.StringReader(claim.Value))
                    {
                        results.Add((SupportGroupClaim)serializer.Deserialize(reader));
                    }
                }
            }
            catch
            {
                return new SupportGroupClaim[0];
            }

            return results.ToArray();
        }

        /// <summary>
        /// Signs out user.
        /// </summary>
        /// <param name="context"></param>
        public static void SignOutUser(this IOwinContext context)
        {
            var authenticator = new MasterISSAuthenticator();
            authenticator.SignOut(context);
        }

        // --------------- DELETE THIS AFTER PARTNER UPDATE -------------
        /// <summary>
        /// Gives cashier id;
        /// </summary>
        /// <param name="User">The user</param>
        /// <returns></returns>
        [Obsolete("This method will be removed in upcoming versions.")]
        public static int? GiveUserCashierId(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.Claims.Where(c => c.Type == "cashierId").FirstOrDefault();
            return (claim == null) ? (int?)null : int.Parse(claim.Value);
        }

        /// <summary>
        /// Gives accountant type.
        /// </summary>
        /// <param name="User">The user.</param>
        /// <returns></returns>
        [Obsolete("This method will be removed in upcoming versions.")]
        public static BillPayment.AccountantType GiveAccountantType(this IPrincipal User)
        {
            var accountantType = BillPayment.AccountantType.Admin;
            if (User.IsInRole("seller"))
                accountantType = BillPayment.AccountantType.Seller;
            if (User.IsInRole("cashier"))
                accountantType = BillPayment.AccountantType.Cashier;

            return accountantType;
        }
        // --------------------------------------------------------------
    }
}
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace IDP
{
    public class Config
    {
        private readonly IConfiguration _configuration;
        public  Config(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public  IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string>() { "role" }),
            };

       /* public  IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource(
                    "bookstoreapi",
                    "Book Store API",
                    new List<string>() { "role" })
                {
                    ApiSecrets = { new Secret("apisecret".Sha256()) }
                }
            };

        public  IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = 120,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "Book Store",
                    ClientId = "bookstoreclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RedirectUris = new List<string>()
                    {
                        string.Format("{0}{1}", _configuration["IdpUrl"], "signin-oidc")
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        string.Format("{0}{1}", _configuration["IdpUrl"], "signout-callback-oidc")
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "bookstoreapi",
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                } };*/
    }
}

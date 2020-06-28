namespace IDP.Modes
{
    public class Client
    {
        public int Id { get; set; }
        //public AccessTokenType AccessTokenType { get; set; } = AccessTokenType.Reference;
        //public int AccessTokenLifetime { get; set; } = 120;
        //public bool AllowOfflineAccess { get; set; } = true;
        //public bool UpdateAccessTokenClaimsOnRefresh { get; set; } = true;
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        //public ICollection<string> AllowedGrantTypes { get; set; } = GrantTypes.Code;
        //public bool RequirePkce { get; set; } = true;
      //  public List<string> RedirectUris { get; set; }
        public string RedirectUri { get; set; }

      //  public List<string> PostLogoutRedirectUris { get; set; }
        public string PostLogoutRedirectUri { get; set; }

        // public List<string> AllowedScopes { get; set; }
        public string AllowedScope { get; set; }

     //   public List<Secret> ClientSecrets { get; set; }
        public string ClientSecret { get; set; }

    }
}

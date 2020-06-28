using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Client = IDP.Modes.Client;

namespace IDP.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        public ClientsController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }
        // GET: Clients
        public ActionResult Index()
        {

            var clients = _configurationDbContext.Clients
                                                 .Include(c => c.AllowedScopes)
                                                 .Include(c => c.ClientSecrets)
                                                 .Include(c => c.RedirectUris)
                                                 .Include(c => c.PostLogoutRedirectUris)
                .Select(x => new Client
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ClientName = x.ClientName,
                    AllowedScope = string.Join(",", x.AllowedScopes.Select(sc => sc.Scope)),
                    RedirectUri = x.RedirectUris.FirstOrDefault().RedirectUri,
                    PostLogoutRedirectUri = x.PostLogoutRedirectUris.FirstOrDefault().PostLogoutRedirectUri
                })
                .ToList();

            return View(clients);
        }

        // GET: Clients/Details/5
        public ActionResult Details(int id)
        {

            var clientDb = _configurationDbContext.Clients
                                                 .Include(c => c.AllowedScopes)
                                                 .Include(c => c.ClientSecrets)
                                                 .Include(c => c.RedirectUris)
                                                 .Include(c => c.PostLogoutRedirectUris)
                                                 .FirstOrDefault(c => c.Id == id);
            var client = new Client();
            if (clientDb != null) { }
            client = new Client
            {
                Id = clientDb.Id,
                ClientId = clientDb.ClientId,
                ClientName = clientDb.ClientName,
                AllowedScope = string.Join(",", clientDb.AllowedScopes.Select(sc => sc.Scope)),
                RedirectUri = clientDb.RedirectUris.FirstOrDefault().RedirectUri,
                PostLogoutRedirectUri = clientDb.PostLogoutRedirectUris.FirstOrDefault().PostLogoutRedirectUri
            };

            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Client client)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    var scopes = new List<string>();
                    if (!string.IsNullOrEmpty(client.AllowedScope))
                        scopes = client.AllowedScope.Split(",").ToList();

                    var clientToSave = new IdentityServer4.Models.Client
                    {
                        AccessTokenType = AccessTokenType.Reference,
                        AccessTokenLifetime = 120,
                        AllowOfflineAccess = true,
                        UpdateAccessTokenClaimsOnRefresh = true,
                        ClientName = client.ClientName,
                        ClientId = client.ClientId,
                        AllowedGrantTypes = GrantTypes.Code,
                        RequirePkce = true,
                        AllowedScopes = scopes,
                        RedirectUris = new List<string>()
                    {
                        string.Format("{0}{1}", client.RedirectUri, "signin-oidc")
                    },
                        PostLogoutRedirectUris = new List<string>()
                    {
                        string.Format("{0}{1}", client.PostLogoutRedirectUri, "signout-callback-oidc")
                    },
                        ClientSecrets =
                    {
                        new IdentityServer4.Models.Secret(client.ClientSecret.Sha256())
                    }
                    };
                    _configurationDbContext.Clients.Add(clientToSave.ToEntity());
                    _configurationDbContext.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int id)
        {
            var clientDb = _configurationDbContext.Clients
                                            .Include(c => c.AllowedScopes)
                                            .Include(c => c.ClientSecrets)
                                            .Include(c => c.RedirectUris)
                                            .Include(c => c.PostLogoutRedirectUris)
                                            .FirstOrDefault(c => c.Id == id);
            var client = new Client();
            if (clientDb != null) { }
            client = new Client
            {
                Id = clientDb.Id,
                ClientId = clientDb.ClientId,
                ClientName = clientDb.ClientName,
                AllowedScope = string.Join(",", clientDb.AllowedScopes.Select(sc => sc.Scope)),
                RedirectUri = clientDb.RedirectUris.FirstOrDefault().RedirectUri,
                PostLogoutRedirectUri = clientDb.PostLogoutRedirectUris.FirstOrDefault().PostLogoutRedirectUri
            };
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Client client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var scopesFromModel = new List<string>();
                    if (!string.IsNullOrEmpty(client.AllowedScope))
                        scopesFromModel = client.AllowedScope.Split(",").ToList();

                    var scopes = new List<ClientScope>();
                    foreach (var claim in scopesFromModel)
                    {
                        scopes.Add(new ClientScope { Scope = claim });
                    };

                    var clientDb = _configurationDbContext.Clients
                                  .Include(c => c.AllowedScopes)
                                  .Include(c => c.ClientSecrets)
                                  .Include(c => c.RedirectUris)
                                  .Include(c => c.PostLogoutRedirectUris)
                                  .FirstOrDefault(c => c.Id == id);
                    clientDb.ClientId = client.ClientId;
                    clientDb.ClientName = client.ClientName;
                    clientDb.AllowedScopes.Clear();
                    clientDb.AllowedScopes = scopes;
                    clientDb.ClientSecrets.OrderByDescending(x => x.Id).Single().Value = client.ClientSecret.Sha256();
                    clientDb.RedirectUris.OrderByDescending(x => x.Id).Single().RedirectUri = client.RedirectUri;
                    clientDb.PostLogoutRedirectUris.OrderByDescending(x => x.Id).Single().PostLogoutRedirectUri = client.PostLogoutRedirectUri;
                    _configurationDbContext.Clients.Update(clientDb);
                    _configurationDbContext.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                return View();
            }
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int id)
        {
            var clientDb = _configurationDbContext.Clients
                                     .Include(c => c.AllowedScopes)
                                     .Include(c => c.ClientSecrets)
                                     .Include(c => c.RedirectUris)
                                     .Include(c => c.PostLogoutRedirectUris)
                                     .FirstOrDefault(c => c.Id == id);
            var client = new Client();
            if (clientDb != null) { }
            client = new Client
            {
                Id = clientDb.Id,
                ClientId = clientDb.ClientId,
                ClientName = clientDb.ClientName,
                AllowedScope = string.Join(",", clientDb.AllowedScopes.Select(sc => sc.Scope)),
                RedirectUri = clientDb.RedirectUris.FirstOrDefault().RedirectUri,
                PostLogoutRedirectUri = clientDb.PostLogoutRedirectUris.FirstOrDefault().PostLogoutRedirectUri
            };
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Client client)
        {
            try
            {
                var clientDb = _configurationDbContext.Clients
                           .FirstOrDefault(c => c.Id == id);
                _configurationDbContext.Clients.Remove(clientDb);
                _configurationDbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
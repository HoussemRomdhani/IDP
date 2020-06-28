using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ApiResource = IDP.Modes.ApiResource;

namespace IDP.Controllers
{
    public class ApisController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        public ApisController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }
        // GET: Apis
        public ActionResult Index()
        {
            var apis = _configurationDbContext.ApiResources.
                Include(a => a.UserClaims).
                Include(a => a.Secrets).
                Select(a => new ApiResource
                {
                    Id = a.Id,
                    Name = a.Name,
                    DisplayName = a.DisplayName,
                    ClaimTypes = string.Join(",", a.UserClaims.Select(x => x.Type))
                }).
                ToList();
            return View(apis);
        }

        // GET: Apis/Details/5
        public ActionResult Details(int id)
        {
            var apiDb = _configurationDbContext.ApiResources.
            Include(a => a.UserClaims).
            Include(a => a.Secrets).
            FirstOrDefault(a => a.Id == id);
            var api = new ApiResource();
            if (apiDb != null)
                api = new ApiResource
                {
                    Id = apiDb.Id,
                    Name = apiDb.Name,
                    DisplayName = apiDb.DisplayName,
                    ClaimTypes = string.Join(",", apiDb.UserClaims.Select(x => x.Type))
                };

            return View(api);
        }

        // GET: Apis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Apis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApiResource api)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userClaims = new List<string>();
                    if (!string.IsNullOrEmpty(api.ClaimTypes))
                        userClaims = api.ClaimTypes.Split(",").ToList();

                    var apiToSave =
                        new IdentityServer4.Models.ApiResource(api.Name, api.DisplayName, userClaims)
                        {
                            ApiSecrets = { new IdentityServer4.Models.Secret(api.Secret.Sha256()) }
                        };

                    _configurationDbContext.ApiResources.Add(apiToSave.ToEntity());
                    _configurationDbContext.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                return View();
            }
        }

        // GET: Apis/Edit/5
        public ActionResult Edit(int id)
        {
            var apiDb = _configurationDbContext.ApiResources.
                                              Include(a => a.UserClaims).
       Include(a => a.Secrets).
       FirstOrDefault(a => a.Id == id);
            var api = new ApiResource();
            if (apiDb != null)
                api = new ApiResource
                {
                    Id = apiDb.Id,
                    Name = apiDb.Name,
                    DisplayName = apiDb.DisplayName,
                    ClaimTypes = string.Join(",", apiDb.UserClaims.Select(x => x.Type))
                };

            return View(api);
        }

        // POST: Apis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ApiResource api)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userClaims = new List<string>();
                    if (!string.IsNullOrEmpty(api.ClaimTypes))
                        userClaims = api.ClaimTypes.Split(",").ToList();

                    var claims = new List<ApiResourceClaim>();
                    foreach (var claim in userClaims)
                    {
                        claims.Add(new ApiResourceClaim { Type = claim });
                    };

                    var apiDb = _configurationDbContext.ApiResources.
                                Include(a => a.UserClaims).
                                Include(a => a.Secrets).
                                 FirstOrDefault(a => a.Id == id);
                    apiDb.Name = api.Name;
                    apiDb.DisplayName = api.DisplayName;
                    apiDb.UserClaims.Clear();
                    apiDb.UserClaims = claims;
                    apiDb.Secrets.OrderByDescending(x => x.Id).Single().Value = api.Secret.Sha256();
                    _configurationDbContext.ApiResources.Update(apiDb);
                    _configurationDbContext.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Apis/Delete/5
        public ActionResult Delete(int id)
        {
            var apiDb = _configurationDbContext.ApiResources.
                                         Include(a => a.UserClaims).
  Include(a => a.Secrets).
  FirstOrDefault(a => a.Id == id);
            var api = new ApiResource();
            if (apiDb != null)
                api = new ApiResource
                {
                    Id = apiDb.Id,
                    Name = apiDb.Name,
                    DisplayName = apiDb.DisplayName,
                    ClaimTypes = string.Join(",", apiDb.UserClaims.Select(x => x.Type))
                };

            return View(api);
        }

        // POST: Apis/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var apiDb = _configurationDbContext.ApiResources
                       .FirstOrDefault(c => c.Id == id);
                _configurationDbContext.ApiResources.Remove(apiDb);
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
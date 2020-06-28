// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
             new TestUser
             {
                 SubjectId = "1",
                 Username = "Houssem",
                 Password = "password",

                 Claims = new List<Claim>
                 {
                     new Claim("given_name", "Houssem"),
                     new Claim("family_name", "Romdhani"),
                     new Claim("address", "Paris"),
                     new Claim("role", "Admin")
                 }
             },
             new TestUser
             {
                 SubjectId = "2",
                 Username = "Samir",
                 Password = "password",

                 Claims = new List<Claim>
                 {
                      new Claim("given_name", "Samir"),
                      new Claim("family_name", "Romdhani"),
                      new Claim("address", "Paris"),
                     new Claim("role", "User")
                 }
             }
         };

    }
}
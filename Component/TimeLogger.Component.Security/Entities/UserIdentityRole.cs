// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Identity;
using System.Data;

namespace TimeLogger.Component.Security.Entities
{
    public class UserIdentityRole : IdentityUserRole<string>
    {
        public virtual UserIdentity User { get; set; }
        public virtual RoleIdentity Role { get; set; }
    }
}
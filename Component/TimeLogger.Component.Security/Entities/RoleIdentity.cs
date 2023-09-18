// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using TimeLogger.Infrastructure.Utility.Enums;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TimeLogger.Component.Security.Entities
{
	public class RoleIdentity : IdentityRole
	{
        public virtual ICollection<UserIdentityRole> UserRoles { get; set; }
    }
}
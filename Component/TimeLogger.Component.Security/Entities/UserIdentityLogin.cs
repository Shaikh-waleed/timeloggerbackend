﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Identity;

namespace TimeLogger.Component.Security.Entities
{
    public class UserIdentityLogin : IdentityUserLogin<string>
    {
        public virtual UserIdentity User { get; set; }
    }
}
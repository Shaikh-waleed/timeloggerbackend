using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TimeLogger.Infrastructure.Utility.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;
using TimeLogger.Component.Security.Entities;
using TimeLogger.Infrastructure.Utility.Constants;
using TimeLogger.Data.Entity;
using System.Collections.Generic;
using TimeLogger.Infrastructure.Models.Configuration;
using Microsoft.EntityFrameworkCore;

namespace TimeLogger.Data.Database
{
    public static class DataSeeder
    {

        public static Task Seed(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var componentOptions = scope.ServiceProvider.GetService<Microsoft.Extensions.Options.IOptionsSnapshot<ComponentOptions>>();


                #region Statuses StatusTypes

                var statusTypes = new List<StatusType>()
                {
                    new StatusType() { Id = StatusTypes.UserStatus, Name = "User statuses" },
                    new StatusType() { Id = StatusTypes.NotificationStatus, Name = "Notification statuses" }
                };
                var newStatusTypes = statusTypes.Where(s => !context.StatusTypes.Select(st => st.Id).Contains(s.Id));
                if (newStatusTypes.Any())
                {
                    context.StatusTypes.AddRange(newStatusTypes);
                    context.SaveChanges();
                }

                var statuses = new List<Status>()
                {
                    // User statuses
                    new Status() { Name = UserStatus.Preactive.ToString(), Code = UserStatusCode.Preactive, TypeId = StatusTypes.UserStatus },
                    new Status() { Name = UserStatus.Active.ToString(), Code = UserStatusCode.Active, TypeId = StatusTypes.UserStatus },
                    new Status() { Name = UserStatus.Inactive.ToString(), Code = UserStatusCode.Inactive, TypeId = StatusTypes.UserStatus },
                    new Status() { Name = UserStatus.Canceled.ToString(), Code = UserStatusCode.Canceled, TypeId = StatusTypes.UserStatus },
                    new Status() { Name = UserStatus.Frozen.ToString(), Code = UserStatusCode.Frozen, TypeId = StatusTypes.UserStatus },
                    new Status() { Name = UserStatus.Blocked.ToString(), Code = UserStatusCode.Blocked, TypeId = StatusTypes.UserStatus },

                    // Notification statuses
                    new Status() { Name = NotificationStatus.Created.ToString(), Code = NotificationStatusCode.Created, TypeId = StatusTypes.NotificationStatus },
                    new Status() { Name = NotificationStatus.Queued.ToString(), Code = NotificationStatusCode.Queued, TypeId = StatusTypes.NotificationStatus },
                    new Status() { Name = NotificationStatus.Succeeded.ToString(), Code = NotificationStatusCode.Succeeded, TypeId = StatusTypes.NotificationStatus },
                    new Status() { Name = NotificationStatus.Failed.ToString(), Code = NotificationStatusCode.Failed, TypeId = StatusTypes.NotificationStatus },
                };
                var newStatuses = statuses.Where(s => !context.Statuses.Select(st => st.Name).Contains(s.Name));
                if (newStatuses.Any())
                {
                    context.Statuses.AddRange(newStatuses);
                    context.SaveChanges();
                }

                #endregion Statuses StatusTypes


                #region Countries Cities

                if (componentOptions.Value.Security.SecurityService == "AspnetIdentity")
                {
                    var countries = new List<Country>()
                    {
                        new Country(){ Code = "0092", Name = "Pakistan", Iso = "PK" },
                    };
                    var newCountries = countries.Where(c => !context.Countries.Select(cn => cn.Code).Contains(c.Code));
                    if (newCountries.Any())
                    {
                        context.Countries.AddRange(newCountries);
                        context.SaveChanges();
                    }

                    var country = context.Countries.FirstOrDefault(c => c.Name.Equals("Pakistan"));
                    if (country == null)
                    {
                        throw new Exception($"{nameof(country)} is not found, seeds are malfunctioned.");
                    }
                    var cities = new List<City>()
                    {
                        new City(){ Code = "051", Name = "Islamabad", Iso = "ICT", CountryId = country.Id } ,
                        new City(){ Code = "021", Name = "Karachi", Iso = "KHI", CountryId = country.Id },
                        new City(){ Code = "052", Name = "Lahore", Iso = "LHR", CountryId = country.Id },
                        new City(){ Code = "091", Name = "Peshawar", Iso = "PSR", CountryId = country.Id }
                    };
                    var newCities = cities.Where(c => !context.Cities.Select(ct => ct.Code).Contains(c.Code));
                    if (newCities.Any())
                    {
                        context.Cities.AddRange(newCities);
                        context.SaveChanges();
                    }
                }

                #endregion Countries Cities


                #region companies


                if (componentOptions.Value.Security.SecurityService == "AspnetIdentity")
                {
                    var country = context.Countries.FirstOrDefault(c => c.Name.Equals("Pakistan"));
                    if (country == null)
                    {
                        throw new Exception($"{nameof(country)} is not found, seeds are malfunctioned.");
                    }

                    var companies = new List<Company>()
                    {
                        new Company()
                        {
                            Name = "KFC",
                            Phone = "123",
                            Website = "xyz"
                        },
                        new Company()
                        {
                            Name = "BFC",
                            Phone = "123",
                            Website = "xyz"
                        }
                    };
                    var newCompanies = companies.Where(c => !context.Companies.Select(co => co.Name).Contains(c.Name)).ToList();
                    if (newCompanies.Any())
                    {
                        context.Companies.AddRange(newCompanies);
                        context.SaveChanges();
                        var city = context.Cities.FirstOrDefault(c => c.Name.Equals("Islamabad"));
                        if (city == null)
                        {
                            throw new Exception($"{nameof(city)} is not found, seeds are malfunctioned.");
                        }
                        foreach (var company in newCompanies)
                        {
                            var address = new Addresses
                            {
                                CompanyId = company.Id,
                                Address = "Tariq road",
                                CountryId = country.Id,
                                CityId = city.Id,
                                IsDefault = true
                            };
                            var addr = context.Addresses.AddAsync(address).Result;
                        }
                        context.SaveChanges();
                    }
                }

                #endregion companies


                #region  Users Roles UserRoles TwoFactorTypes

                if (componentOptions.Value.Security.SecurityService == "AspnetIdentity")
                {
                    var roles = Enum.GetValues(typeof(UserRoles));
                    var roleStore = new RoleStore<RoleIdentity>(context);
                    foreach (var role in roles)
                    {
                        if (!context.Roles.Any(r => r.Name.ToLower() == role.ToString()))
                        {
                            context.Roles.Add(new RoleIdentity { Name = role.ToString(), NormalizedName = role.ToString().ToUpper() });
                            context.SaveChanges();
                        }
                    }

                    var twoFactorTypes = new List<TwoFactorType>()
                    {
                        new TwoFactorType() { Id = Infrastructure.Utility.Enums.TwoFactorTypes.None, Name = Infrastructure.Utility.Constants.TwoFactorTypes.None },
                        new TwoFactorType() { Id = Infrastructure.Utility.Enums.TwoFactorTypes.Email, Name = Infrastructure.Utility.Constants.TwoFactorTypes.Email },
                        new TwoFactorType() { Id = Infrastructure.Utility.Enums.TwoFactorTypes.Phone, Name = Infrastructure.Utility.Constants.TwoFactorTypes.Phone },
                        new TwoFactorType() { Id = Infrastructure.Utility.Enums.TwoFactorTypes.Authenticator, Name = Infrastructure.Utility.Constants.TwoFactorTypes.Authenticator }
                    };
                    var newTwoFactorType = twoFactorTypes.Where(t => !context.TwoFactorTypes.Select(tf => tf.Name).Contains(t.Name));
                    if (newTwoFactorType.Any())
                    {
                        context.TwoFactorTypes.AddRange(newTwoFactorType);
                        context.SaveChanges();
                    }

                    var twoFactorType = context.TwoFactorTypes.FirstOrDefault(t => t.Id == Infrastructure.Utility.Enums.TwoFactorTypes.None);
                    if (twoFactorType == null)
                    {
                        throw new Exception($"{nameof(twoFactorType)} is not found, seeds are malfunctioned.");
                    }

                    var status = context.Statuses.FirstOrDefault(s => s.Name.Equals(UserStatus.Active.ToString()));
                    if (status == null)
                    {
                        throw new Exception($"{nameof(status)} is not found, seeds are malfunctioned.");
                    }

                    var country = context.Countries.FirstOrDefault(c => c.Name.Equals("Pakistan"));
                    if (country == null)
                    {
                        throw new Exception($"{nameof(country)} is not found, seeds are malfunctioned.");
                    }

                    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                    var user = new UserIdentity
                    {
                        FirstName = "TimeLogger",
                        LastName = "Admin",
                        Email = "admin@timelogger.com",
                        UserName = "admin@timelogger.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "admin@timelogger.com",
                        NormalizedUserName = "admin@timelogger.com",
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        TwoFactorTypeId = twoFactorType.Id,
                        StatusId = status.Id,
                        PhoneNumber = "03213828130",
                        PhoneNumberConfirmed = true
                    };
                    if (!_userManager.Users.Any(u => u.Email == user.Email))
                    {
                        var result = _userManager.CreateAsync(user, "Azizullah1@345").Result;
                        var appUser = new ApplicationUser
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email
                        };
                        var userResult = context.Users.AddAsync(appUser).Result;

                        var ro = _userManager.AddToRoleAsync(user, UserRoles.SuperAdmin.ToString()).Result;

                        var city = context.Cities.FirstOrDefault(c => c.Name.Equals("Islamabad"));
                        if (city == null)
                        {
                            throw new Exception($"{nameof(city)} is not found, seeds are malfunctioned.");
                        }
                        var address = new Addresses
                        {
                            UserId = user.Id,
                            Address = "Tariq road",
                            CountryId = country.Id,
                            CityId = city.Id,
                            IsDefault = true
                        };
                        var addr = context.Addresses.AddAsync(address).Result;
                        context.SaveChanges();
                    }

                    var customer = new UserIdentity
                    {
                        FirstName = "TimeLogger",
                        LastName = "Customer",
                        Email = "timelogger.customer@yopmail.com",
                        UserName = "timelogger.customer@yopmail.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "timelogger.customer@yopmail.com",
                        NormalizedUserName = "timelogger.customer@yopmail.com",
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        TwoFactorTypeId = twoFactorType.Id,
                        StatusId = status.Id,
                        PhoneNumber = "03213828130",
                        PhoneNumberConfirmed = true
                    };
                    if (!_userManager.Users.Any(u => u.Email == customer.Email))
                    {
                        var result = _userManager.CreateAsync(customer, "TimeLogger1@345").Result;
                        var appUser = new ApplicationUser
                        {
                            Id = customer.Id,
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Email = customer.Email
                        };
                        var userResult = context.Users.AddAsync(appUser).Result;

                        var ro = _userManager.AddToRoleAsync(customer, UserRoles.Customer.ToString()).Result;

                        var city = context.Cities.FirstOrDefault(c => c.Name.Equals("Karachi"));
                        if (city == null)
                        {
                            throw new Exception($"{nameof(city)} is not found, seeds are malfunctioned.");
                        }
                        var address = new Addresses
                        {
                            UserId = customer.Id,
                            Address = "Shahrah e faisal",
                            CountryId = country.Id,
                            CityId = city.Id,
                            IsDefault = true
                        };
                        var addr = context.Addresses.AddAsync(address).Result;
                        context.SaveChanges();
                    }

                    var compny = context.Companies.FirstOrDefault(c => c.Name.Equals("KFC"));
                    if (compny == null)
                    {
                        throw new Exception($"{nameof(compny)} is not found, seeds are malfunctioned.");
                    }
                    var merchant = new UserIdentity
                    {
                        FirstName = "TimeLogger",
                        LastName = "Merchant",
                        Email = "timelogger.merchant@yopmail.com",
                        UserName = "timelogger.merchant@yopmail.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "timelogger.merchant@yopmail.com",
                        NormalizedUserName = "timelogger.merchant@yopmail.com",
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        TwoFactorTypeId = twoFactorType.Id,
                        StatusId = status.Id,
                        PhoneNumber = "03213828130",
                        PhoneNumberConfirmed = true
                    };
                    if (!_userManager.Users.Any(u => u.Email == merchant.Email))
                    {
                        var result = _userManager.CreateAsync(merchant, "TimeLogger1@345").Result;
                        var appUser = new ApplicationUser
                        {
                            Id = merchant.Id,
                            FirstName = merchant.FirstName,
                            LastName = merchant.LastName,
                            Email = merchant.Email,
                            CompanyId = compny.Id
                        };
                        var userResult = context.Users.AddAsync(appUser).Result;

                        var ro = _userManager.AddToRoleAsync(merchant, UserRoles.Merchant.ToString()).Result;
                        var city = context.Cities.FirstOrDefault(c => c.Name.Equals("Peshawar"));
                        if (city == null)
                        {
                            throw new Exception($"{nameof(city)} is not found, seeds are malfunctioned.");
                        }
                        var address = new Addresses
                        {
                            UserId = merchant.Id,
                            Address = "Saddar",
                            CountryId = country.Id,
                            CityId = city.Id,
                            IsDefault = true
                        };
                        var addr = context.Addresses.AddAsync(address).Result;
                        context.SaveChanges();
                    }
                }

                #endregion Users Roles UserRoles TwoFactorTypes


                #region NotificationTemplates NotificationTypes

                var notificationTypes = new List<NotificationType>()
                {
                    new NotificationType() { Id = NotificationTypes.Email, Name = NotificationName.Email },
                    new NotificationType() { Id = NotificationTypes.Sms, Name = NotificationName.Sms },
                    new NotificationType() { Id = NotificationTypes.Site, Name = NotificationName.Site },
                    new NotificationType() { Id = NotificationTypes.Push, Name = NotificationName.Push }
                };
                var newNotificationTypes = notificationTypes.Where(n => !context.NotificationTypes.Select(no => no.Id).Contains(n.Id));
                if (newNotificationTypes.Any())
                {
                    context.NotificationTypes.AddRange(newNotificationTypes);
                    context.SaveChanges();
                }

                var notificationTemplates = new List<NotificationTemplate>()
                {
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.EmailUserRegisteration,
                        NotificationTypeId = NotificationTypes.Email,
                        Description = "Email confirmation when user is registered.",
                        Subject = "TimeLogger Account Activation",
                        MessageBody = "Hi #Name </br></br>"
                                        + "Thank you for registering in TimeLogger. "
                                        + "Click <a href=\"#Link\">here</a> to activate your account."
                        // Todo
                        //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                        //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tu Actividad en dale! para ver los detalles de la transacción. ")
                        //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.SmsUserRegisteration,
                        NotificationTypeId = NotificationTypes.Sms,
                        Description = "Confirmation sms when user is registered.",
                        Subject = string.Empty,
                        MessageBody = "Todo"
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.EmailForgotPassword,
                        NotificationTypeId = NotificationTypes.Email,
                        Description = "Email, when user click on forget password.",
                        Subject = "TimeLogger Reset Password",
                        MessageBody = "Hi #Name </br></br>"
                                    + "Please click <a href=\"#Link\">here</a> to reset your password."
                        //Todo
                        //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                        //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tu Actividad en dale! para ver los detalles de la transacción. ")
                        //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.SmsForgotPassword,
                        NotificationTypeId = NotificationTypes.Sms,
                        Description = "Code sent in sms, for forget password.",
                        Subject = string.Empty,
                        MessageBody = "Todo"
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.EmailSetPassword,
                        NotificationTypeId = NotificationTypes.Email,
                        Description = "Confirmation email when an external user set password.",
                        Subject = "TimeLogger Account Activation",
                        MessageBody = "Hi #Name </br></br>"
                                    + "Thank you for adding a local account in TimeLogger. "
                                    + "Click <a href=\"#Link\">here</a> to activate your account"
                        // Todo
                        //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                        //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tu Actividad en dale! para ver los detalles de la transacción. ")
                        //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.SmsSetPassword,
                        NotificationTypeId = NotificationTypes.Sms,
                        Description = "Confirmation sms when an external user set password.",
                        Subject = string.Empty,
                        MessageBody = "Todo"
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.EmailChangePassword,
                        NotificationTypeId = NotificationTypes.Email,
                        Description = "Confirmation email when user change the password.",
                        Subject = "TimeLogger Email Change",
                        MessageBody = "Hi #Name </br></br>"
                                    + "Click <a href=\"#Link\">here</a> to confirm your email to change it."
                                    + "</br></br> Thank you so much."
                        //Todo:
                        //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                        //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tu Actividad en dale! para ver los detalles de la transacción. ")
                        //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.SmsChangePassword,
                        NotificationTypeId = NotificationTypes.Sms,
                        Description = "Confirmation sms when user change the password.",
                        Subject = string.Empty,
                        MessageBody = "Todo"
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.EmailTwoFactorToken,
                        NotificationTypeId = NotificationTypes.Email,
                        Description = "Code in email for tow factor authentication.",
                        Subject = "TimeLogger Code",
                        MessageBody = "Hi #Name </br></br>"
                                    + "#Token is your code."
                                    + "</br></br> Thank you so much."
                        //Todo:
                        //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                        //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tu Actividad en dale! para ver los detalles de la transacción. ")
                        //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.SmsTwoFactorToken,
                        NotificationTypeId = NotificationTypes.Sms,
                        Description = "Code in sms for tow factor authentication.",
                        Subject = string.Empty,
                        MessageBody = "Todo"
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.EmailUserStatusChange,
                        NotificationTypeId = NotificationTypes.Email,
                        Description = "Email to user whan user is block or status change to any status in application.",
                        Subject = "TimeLogger Account Restricted",
                        MessageBody = "Todo"
                        //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                        //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tu Actividad en dale! para ver los detalles de la transacción. ")
                        //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                    },
                    new NotificationTemplate()
                    {
                        Id = NotificationTemplates.SmsUserStatusChange,
                        NotificationTypeId = NotificationTypes.Email,
                        Description = "Sms to user whan user is block or status change to any status in application.",
                        Subject = string.Empty,
                        MessageBody = "Todo"
                    }
                };
                var newNotificationTemplates = notificationTemplates.Where(n => !context.NotificationTemplates.Select(nt => nt.Id).Contains(n.Id));
                if (newNotificationTemplates.Any())
                {
                    context.NotificationTemplates.AddRange(newNotificationTemplates);
                    context.SaveChanges();
                }

                #endregion NotificationTemplates NotificationTypes

            }
            return Task.FromResult(0);
        }
    }
}

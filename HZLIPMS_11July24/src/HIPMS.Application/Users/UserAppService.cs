﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using HIPMS.Authorization;
using HIPMS.Authorization.Roles;
using HIPMS.Authorization.Users;
using HIPMS.EntityFrameworkCore;
using HIPMS.Models;
using HIPMS.Roles.Dto;
using HIPMS.Services.SendEmail;
using HIPMS.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static HIPMS.Shared.SharedEnum;

namespace HIPMS.Users
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly HIPMSDbContext _db;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;
        private readonly ISendEmail _sendEmail;
        
        public UserAppService(HIPMSDbContext db,
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            ISendEmail sendEmail,
            LogInManager logInManager)
            : base(repository)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _sendEmail = sendEmail;
            
        }

        public override async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();
            //Email
            try
            {
                await _sendEmail.ExecuteSMTPAsync(user.EmailAddress, "New Account Registration", "Your account has been registered successfully. UserName: " + input.UserName + " /n Password: " + input.Password, CancellationToken.None);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return MapToEntityDto(user);
        }


        
        public override async Task<UserDto> UpdateAsync(UserDto input)
        {
            CheckUpdatePermission();
         
            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
            }
            return await GetAsync(input);
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
        public async Task Activate(EntityDto<long> user)
        {
            await Repository.UpdateAsync(user.Id, async (entity) =>
            {
                entity.IsActive = true;
            });
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
        public async Task DeActivate(EntityDto<long> user)
        {
            await Repository.UpdateAsync(user.Id, async (entity) =>
            {
                entity.IsActive = false;
            });
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roleIds = user.Roles.Select(x => x.RoleId).ToArray();

            var roles = _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.NormalizedName);

            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();

            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<bool> ChangePassword(ChangePasswordDto input)
        {
            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            var user = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            if (await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
            {
                CheckErrors(await _userManager.ChangePasswordAsync(user, input.NewPassword));
            }
            else
            {
                CheckErrors(IdentityResult.Failed(new IdentityError
                {
                    Description = "Incorrect password."
                }));
            }

            return true;
        }

        public async Task<bool> ResetPassword(ResetPasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attempting to reset password.");
            }

            var currentUser = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
            var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("Your 'Admin Password' did not match the one on record.  Please try again.");
            }

            if (currentUser.IsDeleted || !currentUser.IsActive)
            {
                return false;
            }

            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains(StaticRoleNames.Tenants.Admin))
            {
                throw new UserFriendlyException("Only administrators may reset passwords.");
            }

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return true;
        }
        public async Task<bool> GetUserSession()
        {
            if(_abpSession.UserId == null)
            {
                return false;
            }
            UserSession userSession =await _db.UserSession.Where(x => x.UserId == _abpSession.UserId).FirstOrDefaultAsync();
            if(userSession != null)
            {
                return userSession.IsAlreadyLoggedIn;
            }
            return false;
        }
        public async Task<bool> SetUserSession(UserAction userAction)
        {
            UserSession userSession =await _db.UserSession.Where(x => x.UserId == _abpSession.UserId).FirstOrDefaultAsync();
           
            if(userSession != null)
            {
                if (userAction == UserAction.Login && userSession.IsAlreadyLoggedIn == false)
                {
                    userSession.IsAlreadyLoggedIn = true;
                }
                if (userAction == UserAction.Logout)
                {
                    userSession.IsAlreadyLoggedIn = false;
                }
            }
            else
            {
                User user = await _db.Users.Where(x => x.Id == _abpSession.UserId).SingleOrDefaultAsync();
                UserSession obj = new()
                {
                    Name = user.Name,
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    IsAlreadyLoggedIn = true
                };
                await _db.UserSession.AddAsync(obj);
            }
            _db.SaveChanges();
            return true;
        }


        //public async Task<bool> ResetPasswordOnForgotByUser(ForgotPasswordInput input)
        //{
        //    var user = await _userManager.FindByIdAsync(input.UserId);
        //    if (user == null)
        //    {
        //        throw new Exception("There is no current user!");
        //    }

        //    if (await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
        //    {
        //        CheckErrors(await _userManager.ChangePasswordAsync(user, input.NewPassword));
        //    }
        //    else
        //    {
        //        CheckErrors(IdentityResult.Failed(new IdentityError
        //        {
        //            Description = "Incorrect password."
        //        }));
        //    }

        //    return true;
        //}
    }
}


﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{

    public class AccountsController : APIBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        // Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiResponse(400,"This Email is Already is Use"));
            var User = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber
            };
            var Result = await _userManager.CreateAsync(User, model.Password);
            if (!Result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }
            var ReturnUser = new UserDto
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await _tokenService.CreateTokenAsync(User, _userManager)
            };
            return Ok(ReturnUser);
        }


        // Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var User = await _userManager.FindByEmailAsync(model.Email);
            if (User == null)
            {
                return Unauthorized(new ApiResponse(401));

            }
            var Result = await _signInManager.CheckPasswordSignInAsync(User, model.Password, false);
            if (!Result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }
            return Ok(new UserDto
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await _tokenService.CreateTokenAsync(User, _userManager)
            });

        }
        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser() 
        {
          var Email =  User.FindFirstValue(ClaimTypes.Email);
            var user =  await _userManager .FindByEmailAsync(Email);
            var ReturnUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            };
            return Ok(ReturnUser);
        }
        [Authorize]
        [HttpGet("CurrentuserAddress")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress() 
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            var MappedAddress = _mapper.Map<Address ,AddressDto>(user.Address);
            return Ok(MappedAddress);

        }
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto UpdateAddress) 
        {
            
            var user = await _userManager.FindUserWithAddressAsync(User);
            if (user == null) 
            {
                return Unauthorized(new ApiResponse(401));
            }
            var address = _mapper.Map<AddressDto, Address>(UpdateAddress);
            address.Id = user.Address.Id;
            user.Address = address;
            var Result = await _userManager.UpdateAsync(user);
            if (!Result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }
            return Ok(UpdateAddress);
        }
        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
          var user = await _userManager.FindByEmailAsync(email);
           if(user == null)
            {
                return (false);
            }
           else
            {
                return (true);
            }
            
        }

    }
}


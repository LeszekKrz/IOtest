﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Services;
using YouTubeV2.Application.Validator;

namespace YouTubeV2.Api.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController
    {
        private readonly UserService _userService;

        private readonly RegisterDtoValidator _registerDtoValidator;

        public UserController(UserService userService, RegisterDtoValidator registerValidator)
        {
            _userService = userService;
            _registerDtoValidator = registerValidator;
        }

        [HttpPost("register")]
        public async Task Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            await _registerDtoValidator.ValidateAndThrowAsync(registerDto, cancellationToken);

            await _userService.RegisterAsync(registerDto, cancellationToken);
        }
    }
}

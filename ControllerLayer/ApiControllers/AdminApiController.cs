using System;
using System.Collections.Generic;
using ControllerLayer.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Entities;
using RepositoryLayer.Repositories;

namespace ControllerLayer.ApiControllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public sealed class AdminApiController : ControllerBase
    {
        private readonly IAdminRepository adminRepository;

        public AdminApiController(IAdminRepository adminRepository)
        {
            this.adminRepository = adminRepository
                ?? throw new ArgumentNullException(nameof(adminRepository));
        }

        [HttpGet("users")]
        public ActionResult<IReadOnlyList<AdminUserResponse>> GetUsers()
        {
            IReadOnlyList<UserEntity> users = adminRepository.GetUsers();
            List<AdminUserResponse> response = new List<AdminUserResponse>(users.Count);

            foreach (UserEntity user in users)
            {
                response.Add(MapToResponse(user));
            }

            return Ok(response);
        }

        [HttpGet("users/{userId:guid}")]
        public ActionResult<AdminUserResponse> GetUserById(Guid userId)
        {
            UserEntity? user = adminRepository.GetUserByUserId(userId);
            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(MapToResponse(user));
        }

        [HttpGet("users/{userId:guid}/history")]
        public ActionResult GetHistoryForUser(Guid userId)
        {
            UserEntity? user = adminRepository.GetUserByUserId(userId);
            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(adminRepository.GetHistoryForUser(userId));
        }

        private static AdminUserResponse MapToResponse(UserEntity user)
        {
            return new AdminUserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
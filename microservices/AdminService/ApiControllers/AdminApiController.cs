using System;
using System.Collections.Generic;
using AdminService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Entities;

namespace AdminService.ApiControllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public sealed class AdminApiController : ControllerBase
    {
        private readonly IAdminAggregationRepository adminAggregationRepository;

        public AdminApiController(IAdminAggregationRepository adminAggregationRepository)
        {
            this.adminAggregationRepository = adminAggregationRepository
                ?? throw new ArgumentNullException(nameof(adminAggregationRepository));
        }

        [HttpGet("users")]
        public ActionResult<IReadOnlyList<AdminUserResponse>> GetUsers()
        {
            IReadOnlyList<UserEntity> users = adminAggregationRepository.GetUsers();
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
            UserEntity? user = adminAggregationRepository.GetUserByUserId(userId);
            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(MapToResponse(user));
        }

        [HttpGet("users/{userId:guid}/history")]
        public ActionResult GetHistoryForUser(Guid userId)
        {
            UserEntity? user = adminAggregationRepository.GetUserByUserId(userId);
            if (user is null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(adminAggregationRepository.GetHistoryForUser(userId));
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

    public sealed class AdminUserResponse
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
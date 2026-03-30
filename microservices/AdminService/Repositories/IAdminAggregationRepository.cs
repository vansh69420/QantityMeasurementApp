using System;
using System.Collections.Generic;
using ModelLayer.Entities;
using QuantityMeasurementApp.Entities;

namespace AdminService.Repositories
{
    public interface IAdminAggregationRepository
    {
        IReadOnlyList<UserEntity> GetUsers();

        UserEntity? GetUserByUserId(Guid userId);

        IReadOnlyList<QuantityMeasurementEntity> GetHistoryForUser(Guid userId);
    }
}
using System;
using System.Collections.Generic;
using ModelLayer.Entities;
using QuantityMeasurementApp.Entities;

namespace RepositoryLayer.Repositories
{
    public interface IAdminRepository
    {
        IReadOnlyList<UserEntity> GetUsers();

        UserEntity? GetUserByUserId(Guid userId);

        IReadOnlyList<QuantityMeasurementEntity> GetHistoryForUser(Guid userId);
    }
}
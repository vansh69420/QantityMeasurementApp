using System;
using System.Collections.Generic;
using ModelLayer.Entities;
using ModelLayer.Enums;

namespace RepositoryLayer.Repositories
{
    public interface IQuantityMeasurementRepository
    {
        void Save(QuantityMeasurementEntity quantityMeasurementEntity);

        IReadOnlyList<QuantityMeasurementEntity> GetAll();

        void Clear();

        IReadOnlyList<QuantityMeasurementEntity> GetAllByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            IReadOnlyList<QuantityMeasurementEntity> allEntities = GetAll();
            List<QuantityMeasurementEntity> filteredEntities = new List<QuantityMeasurementEntity>();

            foreach (QuantityMeasurementEntity entity in allEntities)
            {
                if (entity.UserId.HasValue && entity.UserId.Value == userId)
                {
                    filteredEntities.Add(entity);
                }
            }

            return filteredEntities.AsReadOnly();
        }

        IReadOnlyList<QuantityMeasurementEntity> GetByMeasurementType(MeasurementType measurementType)
        {
            IReadOnlyList<QuantityMeasurementEntity> allEntities = GetAll();
            List<QuantityMeasurementEntity> filteredEntities = new List<QuantityMeasurementEntity>();

            foreach (QuantityMeasurementEntity entity in allEntities)
            {
                if (entity.MeasurementType == measurementType)
                {
                    filteredEntities.Add(entity);
                }
            }

            return filteredEntities.AsReadOnly();
        }

        IReadOnlyList<QuantityMeasurementEntity> GetByMeasurementTypeAndUserId(MeasurementType measurementType, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            IReadOnlyList<QuantityMeasurementEntity> userEntities = GetAllByUserId(userId);
            List<QuantityMeasurementEntity> filteredEntities = new List<QuantityMeasurementEntity>();

            foreach (QuantityMeasurementEntity entity in userEntities)
            {
                if (entity.MeasurementType == measurementType)
                {
                    filteredEntities.Add(entity);
                }
            }

            return filteredEntities.AsReadOnly();
        }

        IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(OperationType operationType)
        {
            IReadOnlyList<QuantityMeasurementEntity> allEntities = GetAll();
            List<QuantityMeasurementEntity> filteredEntities = new List<QuantityMeasurementEntity>();

            foreach (QuantityMeasurementEntity entity in allEntities)
            {
                if (entity.OperationType == operationType)
                {
                    filteredEntities.Add(entity);
                }
            }

            return filteredEntities.AsReadOnly();
        }

        IReadOnlyList<QuantityMeasurementEntity> GetByOperationTypeAndUserId(OperationType operationType, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            IReadOnlyList<QuantityMeasurementEntity> userEntities = GetAllByUserId(userId);
            List<QuantityMeasurementEntity> filteredEntities = new List<QuantityMeasurementEntity>();

            foreach (QuantityMeasurementEntity entity in userEntities)
            {
                if (entity.OperationType == operationType)
                {
                    filteredEntities.Add(entity);
                }
            }

            return filteredEntities.AsReadOnly();
        }

        int GetTotalCount()
        {
            return GetAll().Count;
        }

        int GetTotalCountByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            }

            return GetAllByUserId(userId).Count;
        }

        void DeleteAll()
        {
            Clear();
        }

        void DeleteByUserId(Guid userId)
        {
            throw new NotSupportedException("DeleteByUserId is not implemented for this repository.");
        }

        string GetPoolStatistics()
        {
            return "N/A";
        }

        void ReleaseResources()
        {
            // Default: nothing to release.
        }
    }
}
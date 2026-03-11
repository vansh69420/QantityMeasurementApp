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

        // ---------------- UC16 additions (default implementations: B1) ----------------

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

        int GetTotalCount()
        {
            return GetAll().Count;
        }

        void DeleteAll()
        {
            Clear();
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
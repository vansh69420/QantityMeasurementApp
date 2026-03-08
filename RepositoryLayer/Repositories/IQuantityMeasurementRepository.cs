using System.Collections.Generic;
using ModelLayer.Entities;

namespace RepositoryLayer.Repositories
{
    public interface IQuantityMeasurementRepository
    {
        void Save(QuantityMeasurementEntity quantityMeasurementEntity);

        IReadOnlyList<QuantityMeasurementEntity> GetAll();

        void Clear();
    }
}
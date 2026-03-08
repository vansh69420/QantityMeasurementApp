using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using ModelLayer.Entities;

namespace RepositoryLayer.Repositories
{
    /// <summary>
    /// In-memory cache repository with JSON Lines persistence.
    /// Stores one <see cref="QuantityMeasurementEntity"/> per line in a JSONL file.
    /// </summary>
    public sealed class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        private static readonly Lazy<QuantityMeasurementCacheRepository> lazyInstance =
            new Lazy<QuantityMeasurementCacheRepository>(() => new QuantityMeasurementCacheRepository());

        private readonly object syncLock = new object();
        private readonly List<QuantityMeasurementEntity> quantityMeasurementEntities;
        private readonly string storeFilePath;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false
        };

        private QuantityMeasurementCacheRepository()
        {
            quantityMeasurementEntities = new List<QuantityMeasurementEntity>();
            storeFilePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "data", "quantity-measurements.jsonl");

            storeFilePath = Path.GetFullPath(storeFilePath);

            LoadFromDisk();
        }

        public static QuantityMeasurementCacheRepository Instance => lazyInstance.Value;

        public void Save(QuantityMeasurementEntity quantityMeasurementEntity)
        {
            if (quantityMeasurementEntity is null)
            {
                throw new ArgumentNullException(nameof(quantityMeasurementEntity));
            }

            string jsonLine = JsonSerializer.Serialize(quantityMeasurementEntity, jsonSerializerOptions);

            lock (syncLock)
            {
                EnsureDirectoryExists();

                File.AppendAllText(storeFilePath, jsonLine + Environment.NewLine);

                quantityMeasurementEntities.Add(quantityMeasurementEntity);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> GetAll()
        {
            lock (syncLock)
            {
                return quantityMeasurementEntities.AsReadOnly();
            }
        }

        public void Clear()
        {
            lock (syncLock)
            {
                quantityMeasurementEntities.Clear();

                if (File.Exists(storeFilePath))
                {
                    File.Delete(storeFilePath);
                }
            }
        }

        private void LoadFromDisk()
        {
            lock (syncLock)
            {
                EnsureDirectoryExists();

                if (!File.Exists(storeFilePath))
                {
                    return;
                }

                foreach (string line in File.ReadLines(storeFilePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    QuantityMeasurementEntity? parsedEntity =
                        JsonSerializer.Deserialize<QuantityMeasurementEntity>(line, jsonSerializerOptions);

                    if (parsedEntity != null)
                    {
                        quantityMeasurementEntities.Add(parsedEntity);
                    }
                }
            }
        }

        private void EnsureDirectoryExists()
        {
            string? directoryPath = Path.GetDirectoryName(storeFilePath);

            if (directoryPath is null)
            {
                throw new InvalidOperationException("Unable to determine repository storage directory.");
            }

            Directory.CreateDirectory(directoryPath);
        }
    }
}
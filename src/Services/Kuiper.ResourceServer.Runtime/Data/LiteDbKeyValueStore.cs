// <copyright file="LiteDbKeyValueStore.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.ResourceServer.Runtime.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kuiper.ServiceInfra.Abstractions.Persistence;

    using LiteDB;

    public class LiteDbKeyValueStore : IKeyValueStore, IDisposable
    {
        private readonly LiteDatabase db;
        private readonly string collectionName;

        public LiteDbKeyValueStore(string collectionName)
        {
            this.collectionName = collectionName;
            this.db = new LiteDatabase(new MemoryStream());
        }

        public LiteDbKeyValueStore(string databaseFile, string collectionName)
        {
            this.collectionName = collectionName;
            var connectionString = new ConnectionString()
            {
                Filename = databaseFile,
                Connection = ConnectionType.Direct,
                ReadOnly = false,
                Upgrade = true,
            };

            this.db = new LiteDatabase(connectionString);
        }

        public Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            var wasDeleted = this.db.GetCollection<byte[]>(this.collectionName).Delete(key);

            if (wasDeleted)
            {
                this.db.Checkpoint();
            }

            return Task.FromResult(wasDeleted);
        }

        public void Dispose()
        {
            this.db.Checkpoint();
            this.db.Dispose();
        }

        public Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            var collection = this.db.GetCollection(this.collectionName);
            var value = collection.FindById(key);

            if (value == null)
            {
                throw new KeyNotFoundException();
            }

            return Task.FromResult(value["value"].AsBinary);
        }

        public Task PutAsync(string key, byte[] value, CancellationToken cancellationToken = default)
        {
            var collection = this.db.GetCollection(this.collectionName);

            BsonDocument document = new BsonDocument(new Dictionary<string, BsonValue>()
            {
                { "_id", key },
                { "value", value },
            });

            collection.Upsert(document);
            this.db.Checkpoint();

            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> ScanKeysAsync(string prefix, CancellationToken cancellationToken = default)
        {
            List<string> keys = new List<string>();

            var collection = this.db.GetCollection(this.collectionName);

            foreach (var item in collection.FindAll())
            {
                BsonValue id = item["_id"];

                keys.Add(item["_id"].AsString);
            }

            return Task.FromResult<IEnumerable<string>>(keys);
        }

        public Task<IEnumerable<KeyValuePair<string, byte[]>>> ScanValuesAsync(string prefix, CancellationToken cancellationToken = default)
        {
            List<KeyValuePair<string, byte[]>> keyValuePairs = new List<KeyValuePair<string, byte[]>>();

            var collection = this.db.GetCollection(this.collectionName);

            foreach (var item in collection.FindAll())
            {
                BsonValue id = item["_id"];
                BsonValue value = item["value"];

                keyValuePairs.Add(new KeyValuePair<string, byte[]>(id.AsString, item["value"].AsBinary));
            }

            return Task.FromResult<IEnumerable<KeyValuePair<string, byte[]>>>(keyValuePairs);
        }
    }
}

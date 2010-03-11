using System;
using Newtonsoft.Json.Linq;
using Raven.Database.Data;

namespace Raven.Database
{
    public interface IDatabaseCommands
    {
        JsonDocument Get(string key);
        string Put(string key, Guid? etag, JObject document, JObject metadata);
        void Delete(string key, Guid? etag);
        string PutIndex(string name, string indexDef);
        QueryResult Query(string index, string query, int start, int pageSize);
        void DeleteIndex(string name);
        JArray GetDocuments(int start, int pageSize);
        JArray GetIndexNames(int start, int pageSize);
        JArray GetIndexes(int start, int pageSize);
    }
}
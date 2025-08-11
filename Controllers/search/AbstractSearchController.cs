using System.Text.RegularExpressions;
using MongoDB.Driver;
using MyAssetsManagerBackend.Entities;

namespace MyAssetsManagerBackend.Controllers.search;

public class AbstractSearchController
    {
        protected readonly IMongoDatabase _database;
        protected readonly IMongoClient _client;

        protected AbstractSearchController(IMongoClient client, IMongoDatabase database)
        {
            _client = client;
            _database = database;
        }

        /// <summary>
        /// Parses a search string like "field1:val1,field2>5," and returns a MongoDB FilterDefinition<T>.
        /// Supported operators: ":" (= equals), "<", ">".
        /// If a field contains dots (nested properties) the BSON field path will be used.
        /// </summary>
        protected static FilterDefinition<T> GetBuilder<T>(string? search)
        {
            var builder = Builders<T>.Filter;
            if (string.IsNullOrWhiteSpace(search))
                return builder.Empty;

            // Pattern mirrors your Java regex: "([\\w.]+?)(:|<|>)(.+?),"
            var pattern = new Regex(@"([\w.]+?)(:|<|>)(.+?),", RegexOptions.Compiled);
            var augmented = search.Trim() + ","; // ensure trailing comma so regex finds the last token
            var matches = pattern.Matches(augmented);

            var filters = new List<FilterDefinition<T>>();

            foreach (Match m in matches)
            {
                var field = m.Groups[1].Value;       // e.g. "name" or "objectIdClass.className"
                var op = m.Groups[2].Value;          // ":" or "<" or ">"
                var rawValue = m.Groups[3].Value;    // string after operator

                // Attempt to interpret numeric/bool/datetime; fallback to string
                object? typedValue = TryParsePrimitive(rawValue);

                // For nested fields, use dot notation
                string bsonField = field;

                switch (op)
                {
                    case ":":
                        if (typedValue is string sVal)
                        {
                            // Use equality for exact string, but you might want case-insensitive or contains.
                            filters.Add(builder.Eq(bsonField, sVal));
                        }
                        else if (typedValue is int iVal)
                        {
                            filters.Add(builder.Eq(bsonField, iVal));
                        }
                        else if (typedValue is long lVal)
                        {
                            filters.Add(builder.Eq(bsonField, lVal));
                        }
                        else if (typedValue is double dVal)
                        {
                            filters.Add(builder.Eq(bsonField, dVal));
                        }
                        else if (typedValue is bool bVal)
                        {
                            filters.Add(builder.Eq(bsonField, bVal));
                        }
                        else if (typedValue is DateTime dtVal)
                        {
                            filters.Add(builder.Eq(bsonField, dtVal));
                        }
                        else
                        {
                            // fallback to raw string match
                            filters.Add(builder.Eq(bsonField, rawValue));
                        }
                        break;

                    case "<":
                        if (typedValue is IComparable)
                        {
                            filters.Add(builder.Lt(bsonField, typedValue));
                        }
                        else
                        {
                            // If non-comparable, ignore or treat as string comparison
                            filters.Add(builder.Lt(bsonField, rawValue));
                        }
                        break;

                    case ">":
                        if (typedValue is IComparable)
                        {
                            filters.Add(builder.Gt(bsonField, typedValue));
                        }
                        else
                        {
                            filters.Add(builder.Gt(bsonField, rawValue));
                        }
                        break;

                    default:
                        // ignore unknown operator
                        break;
                }
            }

            return filters.Count == 0 ? builder.Empty : builder.And(filters);
        }

        /// <summary>
        /// Build an ACL filter for AclObjectIdentity documents.
        /// Assumes AclObjectIdentity contains a list "Entries" of AclEntry.
        /// If your ACL entries are in a separate collection, you'd either:
        ///   - run a separate query on entries and combine results in code, or
        ///   - use an aggregation $lookup to join entries to object identities.
        /// </summary>
        // public static FilterDefinition<AclObjectIdentity> GetAclConditionForClass(
        //     string clazz,
        //     User user,
        //     bool isAdmin,
        //     List<string>? listOfGroupAuthority)
        // {
        //     var builder = Builders<AclObjectIdentity>.Filter;
        //
        //     var classFilter = builder.Eq("ObjectIdClass.ClassName", clazz);
        //
        //     if (listOfGroupAuthority != null && listOfGroupAuthority.Any())
        //     {
        //         // Return AOI where class matches AND there exists an entry with sid in listOfGroupAuthority and principal == false
        //         var groupEntryFilter = builder.ElemMatch(
        //             x => x.Entries,
        //             Builders<AclEntry>.Filter.And(
        //                 Builders<AclEntry>.Filter.In(e => e.Sid.SidValue, listOfGroupAuthority),
        //                 Builders<AclEntry>.Filter.Eq(e => e.Sid.Principal, false)
        //             )
        //         );
        //
        //         return builder.And(classFilter, groupEntryFilter);
        //     }
        //     else
        //     {
        //         // Owner is user OR an entry exists for user OR an entry exists for ROLE_USER (non-principal) OR isAdmin
        //         var ownerFilter = builder.Eq("OwnerSid.SidValue", user.Name);
        //
        //         var entryForUser = builder.ElemMatch(
        //             x => x.Entries,
        //             Builders<AclEntry>.Filter.Eq(e => e.Sid.SidValue, user.Name)
        //         );
        //
        //         var roleUserEntry = builder.ElemMatch(
        //             x => x.Entries,
        //             Builders<AclEntry>.Filter.And(
        //                 Builders<AclEntry>.Filter.Eq(e => e.Sid.SidValue, "ROLE_USER"),
        //                 Builders<AclEntry>.Filter.Eq(e => e.Sid.Principal, false)
        //             )
        //         );
        //
        //         // isAdmin is a runtime boolean; if true, match all AOIs of that class (so classFilter only),
        //         // else combine the other conditions.
        //         if (isAdmin)
        //         {
        //             return classFilter; // admin grants access to all objects of that class
        //         }
        //         else
        //         {
        //             var combined = builder.And(
        //                 classFilter,
        //                 builder.Or(ownerFilter, entryForUser, roleUserEntry)
        //             );
        //             return combined;
        //         }
        //     }
        // }

        // helper: try parsing common primitives
        private static object? TryParsePrimitive(string s)
        {
            if (int.TryParse(s, out var i)) return i;
            if (long.TryParse(s, out var l)) return l;
            if (double.TryParse(s, out var d)) return d;
            if (bool.TryParse(s, out var b)) return b;
            if (DateTime.TryParse(s, out var dt)) return dt;
            // default to string
            return s;
        }
    }
// <auto-generated at 2020-11-24T16:11:25.6388122+01:00 />
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Norm.Extensions;
using NpgsqlTypes;
using Npgsql;

namespace PgRoutiner.Test
{
    public static class PgRoutineAddUserToRole
    {
        public const string Name = "add_user_to_role";

        /// <summary>
        /// plpgsql function "add_user_to_role"
        /// </summary>
        public static bool? AddUserToRole(this NpgsqlConnection connection, string userName, string roleName)
        {
            return connection
                .AsProcedure()
                .Single<bool?>(Name, ("_user_name", userName, NpgsqlDbType.Varchar), ("_role_name", roleName, NpgsqlDbType.Varchar));
        }

        /// <summary>
        /// plpgsql function "add_user_to_role"
        /// </summary>
        public static async ValueTask<bool?> AddUserToRoleAsync(this NpgsqlConnection connection, string userName, string roleName)
        {
            return await connection
                .AsProcedure()
                .SingleAsync<bool?>(Name, ("_user_name", userName, NpgsqlDbType.Varchar), ("_role_name", roleName, NpgsqlDbType.Varchar));
        }

        /// <summary>
        /// sql function "add_user_to_role"
        /// add user to user_role by user id and role name
        /// </summary>
        public static void AddUserToRole(this NpgsqlConnection connection, long? userId, string normalizedRoleName)
        {
            connection
                .AsProcedure()
                .Execute(Name, ("_user_id", userId, NpgsqlDbType.Bigint), ("_normalized_role_name", normalizedRoleName, NpgsqlDbType.Varchar));
        }

        /// <summary>
        /// sql function "add_user_to_role"
        /// add user to user_role by user id and role name
        /// </summary>
        public static async ValueTask AddUserToRoleAsync(this NpgsqlConnection connection, long? userId, string normalizedRoleName)
        {
            await connection
                .AsProcedure()
                .ExecuteAsync(Name, ("_user_id", userId, NpgsqlDbType.Bigint), ("_normalized_role_name", normalizedRoleName, NpgsqlDbType.Varchar));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace PgRoutiner
{
    public class RoutineCode : CodeHelpers
    {
        private readonly string name;
        private int recordModelCount = 0;
        private readonly IEnumerable<PgRoutineInfo> routines;
        private readonly string connectionStr;

        public StringBuilder Model { get; private set; } = null;
        public StringBuilder Class { get; } = new StringBuilder();

        public RoutineCode(
            Settings settings, 
            string name, 
            IEnumerable<PgRoutineInfo> routines,
            string connectionStr) : base(settings)
        {
            this.name = name;
            this.routines = routines;
            this.connectionStr = connectionStr;
            Build();
        }

        private void Build()
        {
            Class.AppendLine($"{I1}public static class PgRoutine{name.ToUpperCamelCase()}");
            Class.AppendLine($"{I1}{{");
            Class.AppendLine($"{I2}public const string Name = \"{name}\";");
            Class.AppendLine();
            foreach (var routine in routines)
            {
                var returnType = GetReturnType(routine);
                BuildCommentHeader(routine);
            }
            Class.Append($"{I1}}}");
        }

        private void BuildCommentHeader(PgRoutineInfo routine)
        {
            Class.AppendLine($"{I2}/// <summary>");
            Class.AppendLine($"{I2}/// {routine.Language} {routine.RoutineType} \"{name}\"");
            if (!string.IsNullOrEmpty(routine.Description))
            {
                Class.AppendLine($"{I2}/// {routine.Description}");
            }
            Class.AppendLine($"{I2}/// </summary>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        private string GetReturnType(PgRoutineInfo routine)
        {
            if (routine.DataType == "void")
            {
                return "void";
            }
            if (settings.Mapping.TryGetValue(routine.TypeUdtName, out var result))
            {
                if (routine.DataType == "ARRAY")
                {
                    return $"{result}[]";
                }
                if (result != "string")
                {
                    return $"{result}?";
                }
                return result;
            }
            if (routine.DataType == "USER-DEFINED")
            {
                return BuildUserDefinedModel(routine);
            }
            if (routine.DataType == "record")
            {
                return BuildRecordModel(routine);
            }
            throw new ArgumentException($"Could not find mapping \"{routine.TypeUdtName}\" for return type of routine \"{routine.RoutineName}\"");
        }

        private string BuildUserDefinedModel(PgRoutineInfo routine)
        {
            var name = routine.TypeUdtName.ToUpperCamelCase();
            BuildModel(name, connection => connection.GetRoutineColumnModel(routine));
            return name;
        }

        private string BuildRecordModel(PgRoutineInfo routine)
        {
            var suffix = ++recordModelCount == 1 ? "" : recordModelCount.ToString();
            var name = $"{this.name.ToUpperCamelCase()}{suffix}";
            BuildModel(name, connection => connection.GetRoutineRecordModel(routine));
            return name;
        }

        private void BuildModel(string name, Func<NpgsqlConnection, IEnumerable<PgReturnModel>> func)
        {
            string getType(PgReturnModel returnModel)
            {
                if (settings.Mapping.TryGetValue(returnModel.Type, out var result))
                {
                    if (returnModel.Array)
                    {
                        return $"{result}[]";
                    }
                    if (result != "string" && returnModel.Nullable)
                    {
                        return $"{result}?";
                    }
                    return result;
                }
                throw new ArgumentException($"Could not find mapping \"{returnModel.Type}\" for result type of routine  \"{this.name}\"");
            }
            if (Model == null)
            {
                Model = new StringBuilder();
            }
            else
            {
                Model.AppendLine();
            }
            using var connection = new NpgsqlConnection(connectionStr);
            if (!settings.UseRecords)
            {
                Model.AppendLine($"{I1}public class {name}");
                Model.AppendLine($"{I1}{{");
                foreach (var item in func(connection))
                {
                    Model.AppendLine($"{I2}public {getType(item)} {item.Name.ToUpperCamelCase()} {{ get; set; }}");
                }
                Model.AppendLine($"{I1}}}");
            }
            else
            {
                Model.Append($"{I1}public record {name}(");
                Model.Append(string.Join(", ", func(connection).Select(item => $"{getType(item)} {item.Name.ToUpperCamelCase()}")));
                Model.AppendLine($");");
            }
        }
    }
}

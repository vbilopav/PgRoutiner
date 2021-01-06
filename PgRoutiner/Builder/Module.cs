using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgRoutiner
{
    public class Module : CodeHelpers
    {
        protected List<string> usings = new List<string> 
        { 
            "System", "System.Linq", "System.Collections.Generic"
        };
        protected string _namespace;
        protected List<object> items = new();
        

        public Module(Settings settings) : base(settings)
        {
            this._namespace = settings.Namespace;
        }

        public void AddUsing(params string[] usings)
        {
            this.usings.AddRange(usings);
        }

        public void AddNamespace(params string[] namespaces)
        {
            foreach(var ns in namespaces)
            {
                this._namespace = string.Concat(this._namespace, ".", ns);
            }
        }

        public void AddItems(params object[] items)
        {
            this.items.AddRange(items);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(settings.SourceHeader))
            {
                builder.AppendLine(string.Format(settings.SourceHeader, DateTime.Now));
            }
            foreach(var ns in usings)
            {
                builder.AppendLine($"using {ns};");
            }
            builder.AppendLine();
            builder.AppendLine($"namespace {_namespace}");
            builder.AppendLine("{");
            builder.Append(string.Join(NL, items.Where(i => i != null)));
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}

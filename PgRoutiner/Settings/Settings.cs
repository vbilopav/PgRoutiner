﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PgRoutiner
{
    public partial class Settings
    {
        public string Connection { get; set; }
        [JsonIgnore] public string Project { get; set; }
        public string OutputDir { get; set; } = "";
        public string Schema { get; set; } = "public";
        public bool Overwrite { get; set; } = false;
        public string Namespace { get; set; }
        public string NotSimilarTo { get; set; } = null;
        public string SimilarTo { get; set; } = null;
        public string SourceHeader { get; set; } = "// <auto-generated at {0} />";
        public bool SyncMethod { get; set; } = true;
        public bool AsyncMethod { get; set; } = true;
        public string ModelDir { get; set; } = null;
        public IDictionary<string, string> Mapping { get; set; }
        public IDictionary<string, string> CustomModels { get; set; } = new Dictionary<string, string>();
        public IList<string> SkipIfExists { get; set; } = new List<string>();
        public bool UseRecords { get; set; } = true;
        public int Ident { get; set; } = 4;

        public static Settings Value { get; set; } = new Settings();
    }
}

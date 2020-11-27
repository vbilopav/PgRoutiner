# PgRoutiner .NET global tool

> ```
> $ dotnet tool install --global dotnet-pgroutiner
> You can invoke the tool using the following command: PgRoutiner
> Tool 'dotnet-pgroutiner' (version '1.4.1') was successfully installed.
> ```

**.NET Core tool for easy scaffolding of your PostgreSQL functions and procedures in you .NET Core project**

This tool will generate **all the necessary source code files** needed to make a simple call from C# (.NET Core) of your **PostgreSQL routines (functions or procedures):**

- Simple execution - or data retrieval - in C#, sync, or async.

- All the necessary data-access code is automatically implemented for you as **connection object extension.**.

- All related neccessary POC  **model classes** (or `record` structures for C# 9) - for data retrieval operations (function returning recordset or physical table returned from a function).

See [examples](https://github.com/vbilopav/PgRoutiner#examples)

> Also: can make your .NET Core project to do a **static type checking** - on your PostgreSQL.

You can use this tool to enforce **static type checking** over PostgreSQL programable routines (functions or procedures) - in your .NET Core project.

Simply add the code generation command with this tool to your pre-build event.

Or, you can simply just generate the code you need with a simple command.

It will take care of things like:

- PostgreSQL function overload for multiple versions.

- PostgreSQL array types for complex input and output.

- Superfast serialization to generated model classes without any caching. See [serialization section](https://github.com/vbilopav/PgRoutiner#serialization-and-mapping).

## Installation

.NET global tool install:

> ```
> dotnet tool install --global dotnet-pgroutiner
> ```

You will receive a message:

> ```
> You can invoke the tool using the following command: PgRoutiner
> Tool 'dotnet-pgroutiner' (version '1.4.1') was successfully installed.
> ```

## Running

Type (case-insensitive):

> ```
> PgRoutiner [default directory]
> ```

If first parameter is not supplied, default directory will be current directory, from where you have started the tool execution.
If first parameter is supplied, default directory will be the one from the first parameter relative to the current. 

It is enough to just type **`PgRoutiner`** (case-insensitive) - and it will look for .NET Core project file (`.csproj`) in the default directory - and start source file generation by using the first available connection string in your configuration.

Or - you may supply additional configuration settings either trough:

1) Custom **JSON configuration settings** section `PgRoutiner`. It is your standard `appsettings.json` or `appsettings.Development.json` from your project. For example, to configure the connection that will be used:

> ```json
> {
>   "PgRoutiner": {
>     "Connection": "MyConnection",
>   }
> }
> ```

2) Standard command-line interface, by supplying command-line arguments. Example from above, to configure the connection that will be used:

> ```
> pgroutiner connection=MyConnection
> ```

For key-value settings you can use:

```
pgroutiner setting:key=value
```

For array settings you can use:

```
pgroutiner settings:index=value
```

Where index is always zero based.

> The command-line setting if supplied - **will always override JSON configuration settings.**. You can mix JSON configuration and command line settings and use command-line setting to override settings from JSON configuration

## Configuration

| Name | Description | Default |
| ---- | ----------- | ------- |
| **Connection** | Connection string name from your configuration connection string to be used. | First available connection string. |
| **Project** | Relative path to project `.csproj` file. | First available `.csproj` file from the current dir. |
| **OutputDir** | Relative path where generated source files will be saved. | Current dir. |
| **ModelDir** | Relative path where model classes source files will be saved. | Default value saves model classes in the same file as a related data-access code. |
| **Schema** | PostgreSQL schema name used to search for routines.  | public |
| **Overwrite** | Should existing generated source file be overwritten (true) or skipped if they exist (false) | true |
| **Namespace** |  Root namespace for generated source files. | Project root namespace. |
| **NotSimilarTo** | `NOT SIMILAR TO` SQL regular expression used to search routine names. | Default skips this matching. |
| **SimilarTo** | `SIMILAR TO` SQL regular expression used to search routine names. | Default skips this matching. |
| **SourceHeader** | Insert the following content to the start of each generated source code file. | `// <auto-generated at timestamp />` |
| **SyncMethod** | Generate a `sync` method, true or false. |  True. |
| **AsyncMethod** | Generate a `async` method, true or false. | True. |
| **UseRecordsForModels** | If set to true, all models will be C# 9 `record` types instead of POCO classes. | False. |
| **Mapping** * | Key-values to override default type mapping. Key is PostgreSQL UDT type name and value is the corresponding C# type name. | See default mapping [here](/PgRoutiner/Settings.cs#L24) |
| **CustomModels** **`*`** | Key-values tell code generation to make a function that maps to a custom class. Key is PostgreSQL routine name (without parameters) and value is the full model name with the namespace. | none |
| **SkipIfExists** **`**`** | List of file names (without dir) that will be skipped if they already exist. | none |

* **`*`** Key-values are JSON objects in JSON configuration. For command-line, use the following format: `pgroutiner mapping:Key1=Value1`
* **`**`** Lists are JSON arrays in JSON configuration. For command-line, use the following format: `pgroutiner skipifexists:0=Value1`

## Examples

- [Simple `void` function example with usage](https://github.com/vbilopav/PgRoutiner/blob/master/EXAMPLES.md#simple-void-function-example-with-usage)
- [Function parameter overload example](https://github.com/vbilopav/PgRoutiner/blob/master/EXAMPLES.md#function-parameter-overload-example)
- [Returning value](https://github.com/vbilopav/PgRoutiner/blob/master/EXAMPLES.md#returning-value)
- [Returning anonymous recordset](https://github.com/vbilopav/PgRoutiner/blob/master/EXAMPLES.md#returning-anonymous-recordset)
- [Returning table](https://github.com/vbilopav/PgRoutiner/blob/master/EXAMPLES.md#returning-table)

## Serialization and mapping

Serialization and mapping are performed using [Norm.net](https://github.com/vbilopav/NoOrm.Net) data access library.

It uses **positional mapping** instead of mapping by name. 

This means that the mapping mechanism is a **little bit [faster](https://github.com/vbilopav/NoOrm.Net/blob/master/PERFOMANCE-TESTS.md)** than any standard mapping mechanism commonly used (like Dapper or EF).

For example, performance tests are indicating that serialization and mapping of 1 million rows that Dapper averages at `0:02.859` seconds and Norm.net positional mapping at `0:02.400` seconds. Norm.net does not use any additional caching to achieve performances because it doesn't need to.

However, the number of returning record fields is limited to 12, more than 12 it will fallback to standard mapping by name (which averaged at `0:03.022` seconds at performance tests). If you need more performances and use positional mapping with more than 12 record fields, open an issue with Norm.net and consider supporting this project and also Norm.net.

## Required dependencies for project

- [Npgsql](https://www.nuget.org/packages/Npgsql/)
- [Norm.net](https://www.nuget.org/packages/Norm.net/) >= 1.7
- [System.Linq.Async](https://www.nuget.org/packages/System.Linq.Async/) (only for async operations)

## Currently supported platforms

- .NET Core 3.0
- .NET Core 3.1

## Support

This is open-source software developed and maintained freely without any compensation whatsoever.

If you find it useful please consider rewarding me on my effort by [buying me a beer](https://www.paypal.me/vbsoftware/5)🍻 or [buying me a pizza](https://www.paypal.me/vbsoftware/10)🍕

Or if you prefer bitcoin:
bitcoincash:qp93skpzyxtvw3l3lqqy7egwv8zrszn3wcfygeg0mv

## Licence

Copyright (c) Vedran Bilopavlović - VB Consulting and VB Software 2020
This source code is licensed under the [MIT license](https://github.com/vbilopav/NoOrm.Net/blob/master/LICENSE).

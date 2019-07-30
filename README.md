# Ockham.Data
Basic data inspection and conversion utilities. Part of the [Ockham.NET](https://github.com/ockham-net/ockham.net) project.

## The Problem
> Every Ockham component should solve a clear problem that is not solved in the .Net BCL, or in the particular libraries it is meant to augment. 

The utilities in this library provide robust, configurable data inspection and conversion utilities. These are particularly useful when serializing and deserializing to databases or other external sources, and when working with dynamic data. 

### Data Conversion
The existing basic data conversions available from the [`System.Convert`](https://docs.microsoft.com/en-us/dotnet/api/system.convert) and [`Microsoft.VisualBasic.CompilerServices.Conversions`](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.VisualBasic.CompilerServices.Conversions) classes fail in several cases that arise frequently when working with serialization and deserialization:

 - Converting to and from [`DBNull`](https://docs.microsoft.com/en-us/dotnet/api/system.dbnull)
 - Converting to and from [`Nullable<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) values
 - Converting to and from enums
 - Converting to and from [`Guid`](https://docs.microsoft.com/en-us/dotnet/api/system.guid)s
 - Converting to any target type with generic method syntax (both [`System.Convert.ChangeType`](https://docs.microsoft.com/en-us/dotnet/api/system.convert.changetype) and [`Conversions.ChangeType`](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.VisualBasic.CompilerServices.Conversions.ChangeType) are non-generic methods that require a runtime type value and return a `object`)
 
The different conversion and inspection functions of the `Convert` static class and `Converter` instance class, along with the various flags of the `ConvertOptions` enumeration, provide succinct and intuitive conversions that can also be adjusted for the desired specifics, such as:
  - Treating an empty string as null (or not)
  - Recognizing hexadecimal strings (or not)
  - Converting null values to the default value of a value type (or not)
  - Coercing *any* value to a particular target type, with or without an explicit fallback value (or not)
 
### Dynamic Value Inspection
The BCL lacks succinct methods for checking if a value can be treated as numeric, if it is the default value of a value type (e.g `0` for any numeric type), or if it is null / empty / DBNull. These basic inspection actions are often required when validating or reasoning about input from external services or data sources.

## Architecture

The core conversion logic is implemented in the **internal** method `Ockham.Data.Convert.To`:

```c#
internal static object To(
  object value, 
  Type targetType, 
  ConvertOptions options, 
  bool ignoreError, 
  object defaultValue
)
```

The various overloads of `To*` and `Force*` on the static `Convert` class and configurable `Converter` class instances internally resolve to this single conversion logic, just with different variations of the input arguments, and sometimes a direct cast to the required output type.

The conversion logic does not replace built-in BCL conversion tools, but rather arranges and combines them into an efficient whole. 

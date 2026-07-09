# Atya.Foundation.Results

Result and Error primitives for expected, recoverable outcomes in Atya libraries.

[![NuGet Version](https://img.shields.io/nuget/v/Atya.Foundation.Results?style=for-the-badge&logo=nuget&logoColor=white&label=NuGet&color=512BD4)](https://www.nuget.org/packages/Atya.Foundation.Results)
[![Downloads](https://img.shields.io/nuget/dt/Atya.Foundation.Results?style=for-the-badge&logo=nuget&logoColor=white&label=Downloads&color=512BD4)](https://www.nuget.org/packages/Atya.Foundation.Results)
![.NET 10.0](https://img.shields.io/badge/.NET_10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
[![License: MIT](https://img.shields.io/badge/License-MIT-512BD4?style=for-the-badge)](https://github.com/AtyaLibraries/Results/blob/development/LICENSE)

## Overview

`Atya.Foundation.Results` models expected failures as values. Use it for validation errors, not-found outcomes, conflicts, and upstream refusals where throwing exceptions would turn normal control flow into error handling.

The package is intentionally small and dependency-free: `Error`, `ErrorKind`, `Result`, and `Result<T>`.

## Installation

```bash
dotnet add package Atya.Foundation.Results
```

```powershell
Install-Package Atya.Foundation.Results
```

```xml
<PackageReference Include="Atya.Foundation.Results" Version="<latest-stable>" />
```

## Quick Start

```csharp
using Atya.Foundation.Results;

static Result<string> ValidateName(string? name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return Result.Failure<string>(
            "atya.foundation.results.name-required",
            "A name is required.",
            ErrorKind.Validation);
    }

    return Result.Success(name);
}

var message = ValidateName("Atya").Match(
    value => $"Hello, {value}.",
    error => $"{error.Code}: {error.Message}");
```

The console sample compiles this same pattern: https://github.com/AtyaLibraries/Results/tree/development/samples/Results.Samples.Console

## Feature Tour

### Error

`Error` carries a stable code, a human-readable message, an `ErrorKind`, an optional `Target`, and child `Details`.

```csharp
var error = new Error(
    "atya.foundation.results.not-found",
    "The requested item was not found.",
    ErrorKind.NotFound);
```

Use `Target` for the field, property, or resource the error applies to. Use `Details` for structured child errors, such as one child per validation failure.

```csharp
var error = new Error(
    "atya.foundation.results.validation",
    "Validation failed.",
    target: null,
    details:
    [
        new Error(
            "atya.foundation.results.name-required",
            "A name is required.",
            target: "Name",
            kind: ErrorKind.Validation),
    ],
    kind: ErrorKind.Validation);
```

`Details` is never `null`; constructors copy the supplied child list. Error equality is structural: `Code`, `Message`, `Kind`, `Target`, and the ordered child `Details` all participate in equality and hash-code generation.

### Result

Use `Result` when an operation has no success value.

```csharp
Result outcome = Result.Success();
Result failed = Result.Failure("atya.foundation.results.conflict", "The item changed.", ErrorKind.Conflict);
```

### Result<T>

Use `Result<T>` when an operation returns a value on success.

```csharp
Result<int> parsed = Result.Success(42);
Result<string> text = parsed.Map(value => value.ToString(CultureInfo.InvariantCulture));
```

### Match, Map, and Bind

`Match` converts both states to a single value. `Map` transforms only the success value. `Bind` chains another result-returning operation and propagates failures.

```csharp
Result<int> loaded = Result.Success(42);

Result<string> formatted = loaded
    .Bind(value => value > 0
        ? Result.Success(value.ToString(CultureInfo.InvariantCulture))
        : Result.Failure<string>("atya.foundation.results.invalid", "Value must be positive.", ErrorKind.Validation));
```

## Error Codes

Error codes should be stable, lowercase, and owned by the package or application concept that emits them. Atya packages use the shape `atya.{area}.{name}.{error}`.

This package documents these sample codes in tests and samples:

| Code | Meaning |
|---|---|
| `atya.foundation.results.name-required` | Sample validation failure. |
| `atya.foundation.results.failure` | Generic test failure. |

Consumers should define their own domain-specific codes rather than reusing sample codes.

## Dependencies

This package has no runtime dependencies.

## Compatibility

Targets `net10.0`.

## Links

- Repository: https://github.com/AtyaLibraries/Results
- NuGet: https://www.nuget.org/packages/Atya.Foundation.Results
- Samples: https://github.com/AtyaLibraries/Results/tree/development/samples
- License: https://github.com/AtyaLibraries/Results/blob/development/LICENSE

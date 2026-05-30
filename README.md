# UnsavedChangesGuard

[![NuGet](https://img.shields.io/nuget/v/UnsavedChangesGuard.svg)](https://www.nuget.org/packages/UnsavedChangesGuard/)
[![CI](https://github.com/agriffard/UnsavedChangesGuard/actions/workflows/ci.yml/badge.svg)](https://github.com/agriffard/UnsavedChangesGuard/actions/workflows/ci.yml)
[![Sample App](https://img.shields.io/badge/GitHub%20Pages-live-blue)](https://agriffard.github.io/UnsavedChangesGuard/)

`UnsavedChangesGuard` is a Blazor component that protects users from accidentally losing unsaved edits.

Use it as:

```razor
<UnsavedChangesGuard When="@form.IsDirty" />
```

When active, it intercepts:

- In-app navigation using `NavigationManager.RegisterLocationChangingHandler`
- Browser tab/window close or reload using `beforeunload`

## Installation

```bash
dotnet add package UnsavedChangesGuard
```

Add the namespace to your `_Imports.razor`:

```razor
@using UnsavedChangesGuard
```

## Usage

### Basic

```razor
<UnsavedChangesGuard When="@isDirty" />
```

### Custom message

```razor
<UnsavedChangesGuard
    When="@isDirty"
    Message="You still have unsaved changes. Leave this page?" />
```

### With an EditForm

```razor
<EditForm EditContext="@editContext" OnValidSubmit="SaveAsync">
    <UnsavedChangesGuard When="@editContext.IsModified()" />

    <InputText @bind-Value="model.Name" />
    <InputText @bind-Value="model.Email" />

    <button class="btn btn-primary" type="submit">Save</button>
</EditForm>
```

## Parameters

| Name | Type | Default | Description |
| --- | --- | --- | --- |
| `When` | `bool` | `false` | Enables/disables the guard. |
| `Message` | `string` | `"You have unsaved changes. Are you sure you want to leave?"` | Message shown in in-app confirmation dialog. |

## How it works

1. **In-app navigation**: registers a location-changing handler and calls `window.confirm(...)` when `When` is true.
2. **Browser unload**: loads `unsavedchangesguard.js` as an ES module and toggles a `beforeunload` handler.

## Sample app

A full Blazor WebAssembly sample is available in `samples/UnsavedChangesGuard.Sample` and published here:

- https://agriffard.github.io/UnsavedChangesGuard/

## Contributing

Issues and pull requests are welcome.

## License

MIT

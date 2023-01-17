# Ayasuna.Frostseek

> Ayasuna.Frostseek is a generator for C# solutions and projects.

## Requirements

The `dotnet` CLI must be installed.

The following packages are optional and are only required if you want to use specific features:

- [docsify-cli](https://docsify.js.org/#/) (If you want to generate a docsify based
  documentation stub for new solutions)
- [git](https://git-scm.com) (If you want to initialize an empty git repository when creating new solutions)

## Installation

You can install the Ayasuna.Frostseek CLI by installing the Ayasuna.Frostseek dotnet tool:

```
dotnet tool install Ayasuna.Frostseek --global
```

## Commands

### `frostseek new solution`

```
frostseek new solution 
  --name $NEW_SOLUTION_NAME 
  --target $TARGET_DIRECTORY 
  [--git true|false]
  [--copyright-license MIT|None]
  [--documentation Docsify|None]
```

#### Description

Creates a new C# solution based on the Ayasuna.Frostseek conventions.
Which essentially means that the following directory structure is created:

| Path                   | Description                                                                                                                                                                                                                                                                            |
|------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| artifacts              | Build artifacts go in here. <br/> If `--git` is `true` this directory will be ignored via the  generated`.gitignore` file. <br/> Projects generated via the `frostseek new project` command will use the directory `artifacts/$PROJECT_TYPE/$PROJECT_NAME/` as their base output path. | 
| build                  | Build related files go in here, like for example a `build.sh` (which might build the whole solution) or docker build related files.                                                                                                                                                    | 
| docs                   | (Optional) Will only be generated if `--documentation` is not `None`. The documentation of the solution belongs in here.                                                                                                                                                               | 
| projects/main          | The directory for `main` projects.                                                                                                                                                                                                                                                     | 
| projects/meta          | The directory for `meta` projects.                                                                                                                                                                                                                                                     |
| projects/test          | The directory for `test` projects.                                                                                                                                                                                                                                                     |   
| .editorconfig          | The .editorconfig file of the solution.                                                                                                                                                                                                                                                | 
| .gitignore             | (Optional) Will only be generated if `--git` is `true`.                                                                                                                                                                                                                                | 
| README.md              | The README of the solution.                                                                                                                                                                                                                                                            | 
| LICENSE                | (Optional) Will only be generated if `--copyright-license` is not `None`. The license file of the solution.                                                                                                                                                                            | 
| $NEW_SOLUTION_NAME.sln | The solution file.                                                                                                                                                                                                                                                                     | 

#### Options

`--name`

The name of the new solution.

`--target`

The target directory in which the new solution directory should be created.

`--git true|false`

Determines if a git repository should be initialized.

`--copyright-license MIT|None`

Determines the copyright licenses that should be added to the new solution.

`--documentation Docsify|None`

Determines the documentation type that should be initialized for the new solution.

### `frostseek new project`

```
frostseek new project 
  --name $NEW_PROJECT_NAME 
  --solution $SOLUTION_FILE
  [--type Main|Meta|Test]
  [--target /relative/target/directory]
  [--template Application|Library|XUnit]
```

#### Description

Creates a new C# project.

This command expects the directory in which the `--solution` file is located to follow the structure that is created by
using the `frostseek new solution` command.
It should therefore only be used for solutions that were created by Ayasuna.Frostseek.

#### Options

`--name`

The name of the new project.

`--solution`

The solution (file) to add the new project to.

`--type Main|Meta|Test`

Determines the type of project that should be created.

`Main` (default) will create the new project in the `projects/main` subdirectory of the solution.

`Test` will create the new project in the `projects/test` subdirectory of the solution.

`Meta` will create the new project in the `projects/meta` subdirectory of the solution.

`--target /relative/target/directory`

The directory, relative to the default target directory for the selected project type, in which the project should be created e.g. if the `target` is `domain` or `/domain` and the `type` is `main`
the project will be created in the `projects/main/domain` directory instead of the `projects/main` directory.

`--template Application|Library|XUnit`

Determines the template that should be applied when creating the project.

`Application` will create a new C# project with it's output type set to `Exe` and it's target frameworks set
to `$(FrostseekApplicationTargetFrameworks)`.

`Library` (default) will create a new C# project with it's output type set to `Library` and it's target framework set
to `$(FrostseekLibraryTargetFrameworks)`

`XUnit` will create a new C# unit test project that uses xUnit.

### `frostseek -h`

```
frostseek -h
```

#### Description

Displays an overview over the available commands and options.

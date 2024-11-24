# CodeClipboard: Project Overview

## Description

**CodeClipboard** is a utility project designed to analyze the structure of a codebase and generate a detailed, hierarchical summary of its files and content. The output includes both the file structure and the content of source files, which are optimized and formatted to be directly copied to the clipboard for sharing, documentation, or other purposes.

The project is ideal for software developers, team leads, and documentation writers who need to quickly summarize or share the structure and contents of a project in a clean and readable format.

---

## Key Features

- **File Structure Analysis**: Scans the specified directory for source files (`.cs`, `.razor`, etc.) and organizes them into a hierarchical tree structure.
- **Content Inclusion**: Extracts and includes the content of relevant source files, such as class definitions, methods, and properties.
- **Clipboard Integration**: Automatically copies the formatted output to the clipboard for easy pasting into other applications.
- **Customizable Settings**:
  - Inclusion/exclusion of specific file types or directories.
  - Maximum file size limit for included content.
  - Option to minify the output by removing unnecessary whitespace and compressing the structure.

---

## Configuration

The project uses an `appsettings.json` file to manage configuration options:

```json
{
  "SourcePath": "C:\\Code\\MyProject",
  "IncludeExtensions": [".cs", ".razor"],
  "SkipDirectories": ["bin", "obj", ".git", "node_modules"],
  "MaxFileSizeKB": 500,
  "MinifyOutput": true
}
```

### Configuration Options

- **SourcePath**: Path to the directory that will be scanned.
- **IncludeExtensions**: List of file extensions to include in the output (e.g., `.cs`, `.razor`).
- **SkipDirectories**: Directories to exclude from the scan (e.g., `bin`, `obj`).
- **MaxFileSizeKB**: Maximum size of files to include in the output (in KB). Files exceeding this size will be skipped.
- **MinifyOutput**: Boolean flag to enable or disable minification of the output.

---

## Usage

1. Clone the repository and build the project using the latest .NET SDK.
2. Configure the `appsettings.json` file to specify the desired source path and settings.
3. Run the project:
   ```bash
   dotnet run
   ```
4. The output will be:
   - Displayed in the console.
   - Copied to the clipboard for easy pasting.

---

## Example Output

```plaintext
Code Tree for: C:\Code\MyProject
=====================================
📁 MyProject/
    📁 Services/
        📄 MyService.cs
            public class MyService { ... }
    📁 Models/
        📄 User.cs
            public class User { ... }
        📄 Product.cs
            public class Product { ... }
```

---

## Use Cases

- **Documentation**: Quickly document the structure and content of a project.
- **Code Review**: Share a summarized view of a project's key components during reviews.
- **Collaboration**: Provide a clean and readable snapshot of a codebase to teammates.
- **Onboarding**: Help new developers understand the structure of a project.

---

## Requirements

- **.NET 9.0 SDK**: The project uses the latest .NET features and requires .NET 9.0 to build and run.
- **TextCopy Library**: Used for clipboard integration.
- **Microsoft.Extensions.Configuration**: For managing settings via `appsettings.json`.

---

## Contributions

Contributions are welcome! If you'd like to improve the project, feel free to fork the repository, make your changes, and submit a pull request. Feedback and suggestions are also appreciated.

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
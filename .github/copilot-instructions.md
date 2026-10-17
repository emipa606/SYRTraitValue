# GitHub Copilot Instructions for [SYR] Trait Value (Continued)

## Mod Overview and Purpose
The [SYR] Trait Value (Continued) mod is designed to enhance the customization and visualization of character traits in RimWorld by assigning numerical values to traits. These values represent the beneficial or detrimental nature of each trait. Traits are color-coded based on these values, facilitating player understanding and enabling other modders to integrate these values into their own mods to add unique gameplay elements.

## Key Features and Systems
- **Trait Value System**: Assigns numerical values to traits to indicate their relative impact.
- **Color-Coding**: Automatically colors traits based on their assigned value, providing a visual representation of a trait's overall benefit or detriment.
- **Customizability**: Users can adjust trait values via settings to better fit their playstyle, even for traits added by other mods.
- **Compatibility**: Ensures seamless integration with other trait mods through a load order requirement.
- **Save Compatibility**: Safely add or remove without affecting save games.

## Coding Patterns and Conventions
- **Consistent Naming**: Follow PascalCase for class and method names. Use meaningful and descriptive names to enhance code readability.
- **Code Structure**: Organize code into classes that have distinct responsibilities, such as `TraitValueUtility` for value calculation and color handling.
- **Error Handling**: Include error checking where applicable, especially when dealing with external data sources like XML or mod dependencies.
- **Logging and Debugging**: Use logging for any potential error spots to aid in debugging.

## XML Integration
The mod uses XML files primarily for defining patches that interact with other mods. Each XML file should be used to define trait values and color settings for specific mods or frameworks. Follow these XML integration guidelines:
- Ensure XML files are well-structured and validated for RimWorld compatibility.
- Use patch operations to modify or extend existing XML definitions safely.
- Separate XML files by functionality or mod it interacts with (e.g., `Alpha Genes.xml`, `Alpha Memes.xml`).

## Harmony Patching
Harmony is used for runtime modification of methods to integrate well with the base game and other mods:
- **Initialization**: Create patches in a static constructor or through a well-defined Harmony entry point.
- **Target Methods**: Only patch necessary methods and classes to avoid conflicts.
- **Prefix/Postfix Patterns**: Use prefix patches to modify data before the original method executes and postfix patches for alterations following the original method call.

## Suggestions for Copilot
- **Function Templates**: Provide templates for common tasks, such as loading XML data or creating Harmony patches, for well-integrated feature expansion.
- **Code Snippets**: Suggest code snippets for common C# idioms used in RimWorld modding, including LINQ expressions for data manipulation.
- **XML Patch Examples**: Offer examples of XML patch structures when dealing with new traits or mod integration.
- **Debugging Tips**: When Copilot generates new code, suggest logging statements or error checks, especially when integrating new external mod elements.
- **Compatibility Handling**: Suggest conditional logic for handling specific mod conditions or dependencies to maintain robust compatibility.

By following these guidelines, Copilot can significantly enhance and streamline the development process for the [SYR] Trait Value (Continued) mod. Always ensure suggestions build upon the existing mod structure and adhere to the outlined coding conventions.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.


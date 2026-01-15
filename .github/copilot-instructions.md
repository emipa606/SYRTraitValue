# GitHub Copilot Instructions for [SYR] Trait Value (Continued)

## Mod Overview and Purpose

**[SYR] Trait Value (Continued)** is an update of the original mod by Syrchaliss, designed to assign numerical values to traits in RimWorld that represent their benefit levels. The mod colors traits based on these values, enabling easy identification of trait importance. This system is open for other mod developers to integrate with and make use of the trait values for various interactions and gameplay mechanics.

### Purpose
- Provide a numerical system for trait evaluation.
- Facilitate color-coded display of traits for better visual clarity.
- Allow customization of trait values to fit different player preferences.

## Key Features and Systems

- **Trait Value Assignment**: Each trait is assigned a numeric value that indicates its overall benefit. Traits can be adjusted by players for personal balance preferences.
- **Color Coding**: Properties of traits are shown in distinct colors determined by their numeric values, making it easier to assess character attributes at a glance.
- **Mod Integration**: Easily integrates with other mod frameworks allowing developers to utilize trait values for customized interactions.
- **Customization Options**: Players can adjust trait values and disable color coding through in-game settings. Changes require a game restart to apply.

## Coding Patterns and Conventions

- **C# Class Design**: Utilizes a mix of static and instance classes for modular functionality, including utility and core settings management.
  - Example Classes: `TraitValueCore`, `TraitValueUtility`, `TraitValueSettings`.

- **XML Data Handling**: Uses custom XML loading for flexible data integration, allowing the mod to read and adjust in-game data seamlessly.
  - Example: `DegreeValue` class with `LoadDataFromXmlCustom` method.

- **Naming Conventions**: Utilizes clear, descriptive names for classes and methods, adhering to C# coding standards for readability and maintainability.

## XML Integration

The mod implements XML loading to manage and adjust trait values dynamically. This process allows for streamlined updates and modifications of trait data:

- **DegreeValue Class**: The `LoadDataFromXmlCustom(XmlNode xmlRoot)` method is used for loading trait data from XML configurations, facilitating integration with existing and new trait data.

## Harmony Patching

Harmony is used to create patches that modify the behavior of game code without directly altering the core files:

- **HarmonyPatches Class**: This static class may include methods to patch existing game methods, ensuring compatibility with other mods and integration with the game.

### Suggestions for Copilot

- **Assistive Code Completion**: Leverage Copilot to suggest boilerplate code for XML data loading and Harmony patch creation.
- **Error Handling**: Utilize suggestions for robust Exception handling in methods, particularly in XML parsing and game code patches.
- **Customization Setup**: Seek Copilot's help to suggest and streamline the setup of user settings for mod customization via in-game menus.
- **Mod Integration Guidance**: Ask Copilot for best practices on integrating with other mods, ensuring that new features do not conflict with existing mod mechanics.

---

This guide aims to provide a comprehensive understanding of the development and integration practices for the [SYR] Trait Value (Continued) mod. For further support, developers are encouraged to engage with the community via Discord, and report any issues via the provided channels.

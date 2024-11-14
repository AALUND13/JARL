# JARL (Just Another Rounds Library) 1.3.1
JARL (Just Another Rounds Library) is a versatile toolkit that introduces a `Armor Framework` and provides utilities like `CustomCardUnity` to streamline the creation of custom cards in Unity.

Feel free send your suggestions or bug report in [issues](https://github.com/AALUND13/JARL/issues) tab.

# Features

- **Armor Framework**: JARL integrates a flexible Armor Framework, empowering developers to craft their own armor or incorporate methods into armors created by other developers. This is achieved through the utilization of `DamageProcessingMethodsAfter` and `DamageProcessingMethodsBefore`, allowing for extensive customization.
  
-  **Custom Card Unity**: JARL also includes CustomCard for Unity, a class that can be inherited to create custom cards within the Unity environment.
# Usage
## Armor Framework

### Creating a Armor Type
You can create an armor type by inheriting from the `ArmorBase` class. Here is an example of an armor type:
```csharp
public class ExampleArmor : ArmorBase {
    public override BarColor GetBarColor() {
        return new BarColor(Color.cyan * 0.6f, Color.cyan * 0.45f);
    }

    public override void SetupArmor() {
        armorTags.Add("CanArmorPierce");
        reactivateArmorType = ArmorReactivateType.Second;
    }
}
```
The base class `ArmorBase` has a lot of methods/properties that you can change/override to customize your armor. Take a look at the `ArmorBase` class to see all the methods/properties that you can change/override.
#### Registering Armor
After creating your armor, you can register it using the `RegisterArmorType` method from the `ArmorFramework` class:
```csharp
void Start() {
	ArmorFramework.RegisterArmorType(new ExampleArmor());
}
``` 
#### Adding Armor To Players
You can add armor to a player, for example, when a player picks a card, by using the `AddArmor` method from the `ArmorHandler` class:
```csharp
public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) {
    ArmorFramework.armorHandlers[player].AddArmor(typeof(ExampleArmor), 50, 5, 5, ArmorReactivateType.Second, 5);
}
```
## Custom Card Unity
### Creating a Custom Card Fr Unity
You can create a custom card unity by inheriting from `CustomCardUnity`. Here is an example of a custom card for Unity:
```csharp
using JARL.Abstract;
using JARL.Extensions;
using UnityEngine;

public class ExampleUnityCard : CustomCardUnity {
    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) {
	    // Your Code Here
    }

    public override string GetModName() {
        return "Your Mod Initials";
    }
}
```

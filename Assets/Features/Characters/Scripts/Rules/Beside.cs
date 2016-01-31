using System;

public class Beside : CharacterRule
{
	public bool Not = true;

	public CharacterData otherType;

	public override bool Check (GameController controller, Character character)
	{
		var charIndex = controller.GetIndexOfCharacter(character);
		var charA = controller.GetCharacterAtIndex(charIndex - 1);
		var charB = controller.GetCharacterAtIndex(charIndex + 1);

		if (Not)
		{
			return (charA == null || charA.characterData != otherType) &&
				(charB == null || charB.characterData != otherType);
		}
		else
		{
			return (charA != null && charA.characterData == otherType) ||
				(charB != null && charB.characterData == otherType);
		}
	}
}


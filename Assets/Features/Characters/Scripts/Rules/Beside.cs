using System;

public class Beside : CharacterRule
{
	public bool Not = true;

	public CharacterData otherType;

	public override bool Check (CircleController controller, Character character)
	{
		var charIndex = controller.GetIndexOfCharacter(character);
		var charA = controller.GetCharacterAtIndex(charIndex - 1);
		var charB = controller.GetCharacterAtIndex(charIndex + 1);

		if (Not)
		{
			return (charA == null || charA.character != otherType) &&
				(charB == null || charB.character != otherType);
		}
		else
		{
			return (charA != null && charA.character == otherType) ||
				(charB != null && charB.character == otherType);
		}
	}
}


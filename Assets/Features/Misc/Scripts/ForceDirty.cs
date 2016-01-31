using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ForceDirty : MonoBehaviour 
{
	// Update is called once per frame
	void Update () {
		foreach (var comp in this.GetComponents<Graphic>())
		{
			comp.SetAllDirty();
			comp.Rebuild(CanvasUpdate.PreRender);
		}

	}
}

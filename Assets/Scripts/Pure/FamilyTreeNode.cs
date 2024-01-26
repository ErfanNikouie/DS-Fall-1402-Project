using TMPro;
using UnityEngine;

public class FamilyTreeNode : MonoBehaviour
{
	private int value = -1;
	private bool selected = false;
	private bool result = false;

	[SerializeField] private TMP_Text text = null;
	[SerializeField] private GameObject normalSprite = null;
	[SerializeField] private GameObject selectedSprite = null;
	[SerializeField] private GameObject resultSprite = null;

	private void Start() => Refresh();
	
	public void Initialize(int value, bool selected = false, bool result = false)
	{
		this.value = value;
		this.selected = selected;
		this.result = result;

		Refresh();
	}

	public void SetState(bool selected = false, bool result = false)
	{
		this.selected = selected;
		this.result = result;

		RefreshState();
	}
	
	private void RefreshText() => text.text = value.ToString();

	private void RefreshState()
	{
		normalSprite.SetActive(false);
		selectedSprite.SetActive(false);
		resultSprite.SetActive(false);

		if (result)
			resultSprite.SetActive(true);
		else if (selected)
			selectedSprite.SetActive(true);
		else
			normalSprite.SetActive(true);
	}
	
	public void Refresh()
	{
		RefreshText();
		RefreshState();
	}
}
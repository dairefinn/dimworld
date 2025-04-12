namespace Dimworld.Core.UI;

using Godot;
using Godot.Collections;


/// <summary>
/// Used to easily toggle parts of the UI on and off. This is the root of the UI tree and should be used to manage all UI elements.
/// </summary>
public partial class UIRootPanel : Control
{

	[Export] public Dictionary<string, Control> AvailablePanels { get; set; } = [];


    private Panel _screenPanel { get; set; }


	public override void _Ready() {
		base._Ready();

        _screenPanel = new Panel();

        foreach (Node node in GetChildren()) {
            if (node is Control uiElement ) {
				if (AvailablePanels.ContainsKey(uiElement.GetType().Name.ToString())) {
					continue;
				}

                AvailablePanels.Add(uiElement.GetType().Name.ToString(), uiElement);
            }
        }
	}

	public bool SetActivePanel<T>() where T : Control {
		bool wasActivated = false;
		if (_screenPanel == null) return wasActivated;

		foreach (System.Collections.Generic.KeyValuePair<string, Control> entry in AvailablePanels) {
			if (entry.Value is T panel) {
				entry.Value.Show();
				wasActivated = true;
			} else {
				entry.Value.Hide();
			}
		}

		return wasActivated;
	}

	public T EnablePanel<T>() where T : Control {
		T panel = GetPanel<T>();
		if (panel == null) return null;
		panel.Show();
		return panel;
	}

	public T DisablePanel<T>() where T : Control {
		T panel = GetPanel<T>();
		if (panel == null) return null;
		panel.Show();
		return panel;
	}

	public T GetPanel<T>() where T : Control {
		foreach (System.Collections.Generic.KeyValuePair<string, Control> entry in AvailablePanels) {
			if (entry.Value is T panel) {
				return panel;
			}
		}

		return null;
	}

	public void ToggleUI() {
		if (_screenPanel == null) return;
		if (_screenPanel.Visible) {
			DisableUI();
		} else {
			EnableUI();
		}
	}

	public void EnableUI() {
		if (_screenPanel == null) return;
		_screenPanel.Show();
	}

	public void DisableUI() {
		if (_screenPanel == null) return;
		_screenPanel.Hide();
	}

}
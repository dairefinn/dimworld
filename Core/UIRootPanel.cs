namespace Dimworld.UI;

using Godot;
using Godot.Collections;


/// <summary>
/// Used to easily toggle parts of the UI on and off. This is the root of the UI tree and should be used to manage all UI elements.
/// </summary>
public partial class UIRootPanel : Control
{

	public static UIRootPanel Instance { get; private set; }


	[Export] public Dictionary<string, Control> AvailablePanels { get; set; } = [];


    private Panel _screenPanel { get; set; }


	public UIRootPanel() {
		Instance = this;
	}


	public override void _Ready() {
		base._Ready();

        _screenPanel = new Panel();

        foreach (Node node in GetChildren()) {
            // Skip the UIRootPanel so we don't add it to itself.
            if (node is UIRootPanel) {
                continue;
            }

            if (node is Control panelComponent ) {
				if (AvailablePanels.ContainsKey(panelComponent.GetType().Name.ToString())) {
					continue;
				}

                AvailablePanels.Add(panelComponent.GetType().Name.ToString(), panelComponent);
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
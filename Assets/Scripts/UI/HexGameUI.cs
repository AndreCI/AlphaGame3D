using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;


	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		if (toggle) {
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		}
		else {
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		}
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
            if (UpdateCurrentCell()) { 
                currentCell.OnMouseOver();
            }
			if (Input.GetMouseButtonDown(0)) {
                if (currentCell != null)
                {
                    currentCell.OnMouseDown();
                }
			}/*
			else if (selectedUnit) {
				if (Input.GetMouseButtonDown(1)) {
					DoMove();
				}
				else {
					DoPathfinding();
				}
			}*/

        }
	}


	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell && cell!=null) {
            if (currentCell != null)
            {
                currentCell.OnMouseExit();
            }
			currentCell = cell;
			return true;
		}
		return false;
	}
}
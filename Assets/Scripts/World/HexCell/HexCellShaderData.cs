using System.Collections.Generic;
using UnityEngine;

public class HexCellShaderData : MonoBehaviour {

	const float transitionSpeed = 255f;

	Texture2D cellTexture;
	Color32[] cellTextureData;

	Dictionary<HexCell, Player> transitioningCells = new Dictionary<HexCell, Player>();

	bool needsVisibilityReset;

	public HexGrid Grid { get; set; }

	public bool ImmediateMode { get; set; }

	public void Initialize (int x, int z) {
		if (cellTexture) {
			cellTexture.Resize(x, z);
		}
		else {
			cellTexture = new Texture2D(
				x, z, TextureFormat.RGBA32, false, true
			);
			cellTexture.filterMode = FilterMode.Point;
			cellTexture.wrapModeU = TextureWrapMode.Repeat;
			cellTexture.wrapModeV = TextureWrapMode.Clamp;
			Shader.SetGlobalTexture("_HexCellData", cellTexture);
		}
		Shader.SetGlobalVector(
			"_HexCellData_TexelSize",
			new Vector4(1f / x, 1f / z, x, z)
		);

		if (cellTextureData == null || cellTextureData.Length != x * z) {
			cellTextureData = new Color32[x * z];
		}
		else {
			for (int i = 0; i < cellTextureData.Length; i++) {
				cellTextureData[i] = new Color32(0, 0, 0, 0);
			}
		}

		transitioningCells.Clear();
		enabled = true;
	}

	public void RefreshTerrain (HexCell cell) {
		cellTextureData[cell.Index].a = (byte)cell.TerrainTypeIndex;
		enabled = true;
	}

	public void RefreshVisibility (HexCell cell, Player owner) {
       // owner = Player.Player1;
		int index = cell.Index;
		if (ImmediateMode) {
			cellTextureData[index].r = cell.IsVisible(owner) ? (byte)255 : (byte)0;
			cellTextureData[index].g = cell.IsExplored(owner) ? (byte)255 : (byte)0;
		}
		else if (cellTextureData[index].b != 255) {
			cellTextureData[index].b = 255;
			transitioningCells.Add(cell, owner);
		}
		enabled = true;
	}

	public void SetMapData (HexCell cell, float data) {
		cellTextureData[cell.Index].b =
			data < 0f ? (byte)0 : (data < 1f ? (byte)(data * 254f) : (byte)254);
		enabled = true;
	}

	public void ViewElevationChanged () {
		needsVisibilityReset = true;
		enabled = true;
	}

	void LateUpdate () {
		if (needsVisibilityReset) {
			needsVisibilityReset = false;
			Grid.ResetVisibility(TurnManager.Instance.currentPlayer);
		}

		int delta = (int)(Time.deltaTime * transitionSpeed);
		if (delta == 0) {
			delta = 1;
		}
        List<HexCell> keys = new List<HexCell>(transitioningCells.Keys);
        for(int i = 0; i < keys.Count; i++)
        {
            if (!UpdateCellData(keys[i], delta, transitioningCells[keys[i]]))
            {
                transitioningCells.Remove(keys[i]);
                keys[i--] =
                    keys[keys.Count - 1];
                keys.RemoveAt(keys.Count - 1);
            }
        }/*
		for (int i = 0; i < transitioningCells.Count; i++) {
			if (!UpdateCellData(transitioningCells[i], delta, TurnManager.Instance.currentPlayer)) {
				transitioningCells[i--] =
					transitioningCells[transitioningCells.Count - 1];
				transitioningCells.RemoveAt(transitioningCells.Count - 1);
			}
		}*/

		cellTexture.SetPixels32(cellTextureData);
		cellTexture.Apply();
		enabled = transitioningCells.Count > 0;
	}

	bool UpdateCellData (HexCell cell, int delta, Player player) {
       // player = Player.Player1;
		int index = cell.Index;
		Color32 data = cellTextureData[index];
		bool stillUpdating = false;

		if (cell.IsExplored(player) && data.g < 255) { //g is exploration channel
			stillUpdating = true;
			int t = data.g + delta;
			data.g = t >= 255 ? (byte)255 : (byte)t;
		}

		if (cell.IsVisible(player)) {
			if (data.r < 255) { //r is visibility channel
				stillUpdating = true;
				int t = data.r + delta;
				data.r = t >= 255 ? (byte)255 : (byte)t;
			}
		}
		else if (data.r > 0) {
			stillUpdating = true;
			int t = data.r - delta;
			data.r = t < 0 ? (byte)0 : (byte)t;
		}

		if (!stillUpdating) {
			data.b = 0; //b is transition channel
		}
		cellTextureData[index] = data;
		return stillUpdating;
	}
}
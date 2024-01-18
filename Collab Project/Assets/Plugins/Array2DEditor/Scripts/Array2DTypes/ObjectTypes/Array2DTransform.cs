using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DTransform : Array2D<Transform>
    {
        [SerializeField]
        CellRowTransform[] cells = new CellRowTransform[Consts.defaultGridSize];

        protected override CellRow<Transform> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}

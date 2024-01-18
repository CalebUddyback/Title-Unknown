using UnityEngine;
using UnityEditor;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DTransform))]
    public class Array2DTranformDrawer : Array2DObjectDrawer<Transform> { }
}

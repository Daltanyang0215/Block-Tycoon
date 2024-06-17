using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

//[CustomEditor(typeof(HexaElementDataSO))]
//public class HexaElementDataEditor : Editor
//{
//    public VisualTreeAsset TreeAsset;
//    private HexaElementDataSO _hexaData;

//    private void OnEnable()
//    {
        
//    }

//    public override VisualElement CreateInspectorGUI()
//    {
//        if (TreeAsset == null)
//            return base.CreateInspectorGUI();

//        _hexaData = (HexaElementDataSO)target;

//        VisualElement root = new VisualElement();
//        TreeAsset.CloneTree(root);

//        VisualElement inspector = root.Q("default");
//        InspectorElement.FillDefaultInspector(inspector, serializedObject, this);
//        VisualElement back = root.Q("back");
//        back.style.unityBackgroundImageTintColor = Color.white;
  
//        return root;
//    }

//}

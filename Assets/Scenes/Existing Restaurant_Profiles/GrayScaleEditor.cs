using UnityEditor.Rendering;

using UnityEngine;

using UnityEngine.Rendering.HighDefinition;

using UnityEditor;

[VolumeComponentEditor(typeof(GrayscaleAttInf))]

sealed class GrayScaleEditor : VolumeComponentEditor

{

    SerializedDataParameter m_Intensity;

    /*public override bool hasAdvancedMode => false;*/

    public override void OnEnable()

    {

        base.OnEnable();

        var o = new PropertyFetcher<GrayscaleAttInf>(serializedObject);

        m_Intensity = Unpack(o.Find(x => x.intensity));

    }

    public override void OnInspectorGUI()

    {

        PropertyField(m_Intensity);

    }

}
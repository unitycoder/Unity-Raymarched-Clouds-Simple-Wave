﻿using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class Raymarching : MonoBehaviour
{
    #region Private Attributes

    [Header("UI")]
    [SerializeField] private Slider scatteringSlider = null;

    [SerializeField] private Slider densitySlider = null;
    [SerializeField] private Slider coverageSlider = null;
    [SerializeField] private Slider sunSpeedSlider = null;
    [SerializeField] private Slider jitterSlider = null;
    [SerializeField] private Toggle taaToggle = null;

    [Header("Components")]
    [SerializeField] private Transform sun = null;

    [SerializeField] private PostProcessLayer ppLayer = null;
    [SerializeField] private Transform sphere = null;

    private Material raymarchMat = null;

    private readonly int posId = Shader.PropertyToID("_SpherePos");
    private readonly int radiusId = Shader.PropertyToID("_SphereRadius");
    private readonly int frameCountId = Shader.PropertyToID("_FrameCount");
    private readonly int absortionId = Shader.PropertyToID("_Absortion");
    private readonly int outScatteringId = Shader.PropertyToID("_OutScattering");
    private readonly int densityId = Shader.PropertyToID("_Density");
    private readonly int coverageId = Shader.PropertyToID("_Coverage");
    private readonly int jitterId = Shader.PropertyToID("_JitterEnabled");

    #endregion

    #region MonoBehaviour Methods

    private void Start()
    {
        raymarchMat = GetComponent<MeshRenderer>().sharedMaterial;
        Camera.onPreRender += MyPreRender;
    }

    private void Update()
    {
        Vector3 eulers = new Vector3(0.0f, sunSpeedSlider.value * Time.deltaTime, 0.0f);
        sun.Rotate(eulers, Space.World);
    }

    private void OnDestroy()
    {
        Camera.onPreRender -= MyPreRender;
    }

    #endregion

    #region Methods

    private void SetAAMode(bool wantTaa)
    {
        ppLayer.antialiasingMode = wantTaa ? PostProcessLayer.Antialiasing.TemporalAntialiasing : PostProcessLayer.Antialiasing.None;
    }

    private void MyPreRender(Camera cam)
    {
        raymarchMat.SetFloat(absortionId, 0.0f);
        raymarchMat.SetFloat(outScatteringId, scatteringSlider.value);
        raymarchMat.SetFloat(densityId, densitySlider.value);
        raymarchMat.SetFloat(coverageId, coverageSlider.value);

        raymarchMat.SetInt(jitterId, (int)jitterSlider.value);

        raymarchMat.SetVector(posId, sphere.position);
        raymarchMat.SetFloat(radiusId, sphere.localScale.x * 0.5f);
        raymarchMat.SetFloat(frameCountId, Time.frameCount);

        SetAAMode(taaToggle.isOn);
    }

    #endregion
}
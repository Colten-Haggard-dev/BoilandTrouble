using System;
using FlaxEngine;

namespace Game;

public class Cam2Text : Script
{
    public Camera Cam;
    public MaterialBase Material;

    [Limit(1, 2000)]
    public Float2 Resolution
    {
        get => _resolution;
        set
        {
            value = Float2.Clamp(value, new Float2(1), new Float2(2000));
            if (_resolution != value)
            {
                _resolution = value;
                if (_output)
                {
                    // Resize backbuffer
                    UpdateOutput();
                }
            }
        }
    }

    public float ViewDistance = 2000;

    private Float2 _resolution = new(640, 374);
    private GPUTexture _output;
    private SceneRenderTask _task;
    private MaterialInstance _material;

    private void UpdateOutput()
    {
        var desc = GPUTextureDescription.New2D(
            (int)_resolution.X,
            (int)_resolution.Y,
            PixelFormat.R8G8B8A8_UNorm);
        _output.Init(ref desc);
    }

    public override void OnEnable()
    {
        // Create backbuffer
        if (_output == null)
            _output = new GPUTexture();
        UpdateOutput();

        // Create rendering task
        if (_task == null)
            _task = new SceneRenderTask();
        _task.Order = -100;
        _task.Camera = Cam;
        _task.Output = _output;
        _task.ViewFlags =
            ViewFlags.Reflections |
            ViewFlags.Decals |
            ViewFlags.AO |
            ViewFlags.GI |
            ViewFlags.DirectionalLights |
            ViewFlags.PointLights |
            ViewFlags.SpotLights |
            ViewFlags.SkyLights |
            ViewFlags.Sky |
            ViewFlags.Shadows |
            ViewFlags.SpecularLight |
            ViewFlags.CustomPostProcess |
            ViewFlags.ToneMapping;
        _task.Enabled = false;

        if (Material && _material == null)
        {
            // Use dynamic material instance
            if (Material.WaitForLoaded())
                throw new Exception("Failed to load material.");
            _material = Material.CreateVirtualInstance();

            // Set render task output to draw on model
            _material.SetParameterValue("CameraText", _output);

            // Bind material to parent model
            if (Actor is StaticModel staticModel && staticModel.Model)
            {
                staticModel.Model.WaitForLoaded();
                staticModel.SetMaterial(2, _material);
            }
        }

        _task.Enabled = true;
    }

    public override void OnUpdate()
    {
        // Optimize by disabling rendering if main game view is far too far
        var mainView = MainRenderTask.Instance.View;
        _task.Enabled = Vector3.Distance(Actor.Position, mainView.Origin + mainView.Position) <= ViewDistance;
    }

    public override void OnDisable()
    {
        // Unbind temporary material
        if (Actor is StaticModel staticModel && staticModel.Model && staticModel.Model.IsLoaded)
            staticModel.SetMaterial(0, Material);

        // Ensure to cleanup resources
        Destroy(ref _task);
        Destroy(ref _output);
        Destroy(ref _material);
    }
}
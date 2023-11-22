using FlaxEngine;

namespace Game;
public class ExitOnEsc : Script
{
    public override void OnUpdate()
    {
        if (Input.GetKeyUp(KeyboardKeys.F11))
            Screen.IsFullscreen = !Screen.IsFullscreen;
        
        if (Input.GetKeyUp(KeyboardKeys.Escape))
            Engine.RequestExit();
    }
}
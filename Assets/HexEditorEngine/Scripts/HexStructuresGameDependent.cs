using System.Collections;

//This file can be used as a 2in1 Template&Example

//These are purely game-dependent classes
namespace HexEditorEngine
{
    //This is information for building system - what is this hexTile-to-be next to.
    //You should want to use it, to create custom behaviour in terms of next-level snapping
    public static class BuildingSettings
    {
        public static float positionInterpolator=0.1f;
        public static float rotationInterpolator=0.1f;
    }


}
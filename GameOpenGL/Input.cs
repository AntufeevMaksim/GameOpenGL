using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace Input
{

    public class InputData
    {


        public InputData(bool[] keys, Vector2 deltaMousePos, Vector2 deltaMouseScroll)
        {
            Keys = keys;
            DeltaMousePosition = deltaMousePos;
            DeltaMouseScroll = deltaMouseScroll;
        }

        public bool[] Keys { get; private set; }
        public Vector2 DeltaMousePosition { get; private set; }
        public Vector2 DeltaMouseScroll { get; private set; }
    }

    public class InputCallback
    {
        KeyboardState _keyboardInput;
        MouseState _mouseState;
        bool[] _keys = new bool[1024];
        Vector2 _deltaMousePos;
        Vector2 _deltaMouseScroll;
        public InputCallback(KeyboardState keyboardInput, MouseState mouseState)
        {
            _keyboardInput = keyboardInput;
            _mouseState = mouseState;
        }

        public InputData GetInputData()
        {
            UpdateKeysState();
            UpdateMouseState();
            return new InputData(_keys, _deltaMousePos, _deltaMouseScroll);
        }

        private void UpdateKeysState()
        {
            List<Keys> tracked_keys = new List<Keys> { Keys.W, Keys.S, Keys.D, Keys.A };

            foreach (Keys key in tracked_keys)
            {
                if (_keyboardInput.IsKeyPressed(key))
                {
                    _keys[(int)key] = true;
                }
                else if (_keyboardInput.IsKeyReleased(key))
                {
                    _keys[(int)key] = false;
                }
            }
        }

        private void UpdateMouseState()
        {
            _deltaMousePos = _mouseState.Position - _mouseState.PreviousPosition;
            _deltaMouseScroll = _mouseState.ScrollDelta;
        }
    }




}
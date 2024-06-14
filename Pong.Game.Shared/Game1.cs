using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong.Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        private const float BallSpeed = 5f;

        private readonly Random _rnd = new Random();
        private readonly PlatformSpecific _platformSpecific;

        private readonly Point _gameResolution = new Point(960, 720);

        private bool _playWithBot = false;
        private bool _botButtonDown = false;
        private const float BotHeightCheckFactor = 0.75F;

        private bool _gameStarted = false;
        private bool _showStartMessage = true;
        private bool _roundStarted = false;
        private bool _gameEnded = false;

        private const int PadXOffset = 10;
        private const float PadScale = 0.5f;
        private const float PadSpeed = 7.5f;
        private ScreenSide _winningPlayer = ScreenSide.Center;

        private const float GamepadDeadzone = 0.1f;
        private const float GamepadSensitivity = 1.1f;

        private RenderTarget2D _renderTarget;
        private Rectangle _renderTargetDest;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private Ball _ball;
        private GameObject _leftPad;
        private GameObject _rightPad;

        private Texture2D _ballTex;

        private int _leftScore,
            _rightScore = 0;

        /// <summary>
        /// Whether left/right stopped after landing a goal.
        /// </summary>
        private bool _leftStopped,
            _rightStopped = false;

        #endregion

        #region Initialization
        public Game1(PlatformSpecific platformSpecific)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _platformSpecific = platformSpecific;
        }
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            
#if DEBUG
            // Make debug window borderless to allow breakpoints to redirect to IDE.
            _graphics.HardwareModeSwitch = false;
#endif
            
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected void Restart()
        {
            _roundStarted = false;
            _gameEnded = false;
            _showStartMessage = true;
            _winningPlayer = ScreenSide.Center;
            _leftScore = 0;
            _rightScore = 0;
            GenerateBall();
        }

        private void GenerateBall()
        {
            // Generates a random Y velocity.
            Vector2 ballVelocity = new Vector2(_rnd.Next(0, 2) == 1 ? -1 : 1, GenerateRandomBallYVelocity());

            _ball = new Ball(
                _ballTex,
                new Vector2(_gameResolution.X / 2, _gameResolution.Y / 2),
                1f,
                _spriteBatch,
                _gameResolution.X,
                _gameResolution.Y,
                ballVelocity * BallSpeed,
                _leftPad,
                _rightPad);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _leftPad = new GameObject(
                Content.Load<Texture2D>("Pad"),
                new Vector2(PadXOffset, _gameResolution.Y / 2),
                PadScale,
                _spriteBatch,
                _gameResolution.X,
                _gameResolution.Y);

            _rightPad = new GameObject(
                Content.Load<Texture2D>("Pad"),
                new Vector2(_gameResolution.X - PadXOffset, _gameResolution.Y / 2),
                PadScale,
                _spriteBatch,
                _gameResolution.X,
                _gameResolution.Y);

            _ballTex = Content.Load<Texture2D>("Ball");

            GenerateBall();

            _font = Content.Load<SpriteFont>("GameFont");

            _renderTarget = new RenderTarget2D(
                GraphicsDevice,
                _gameResolution.X,
                _gameResolution.Y);

            _renderTargetDest = GetRenderTargetDestination(
                _gameResolution,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);
        }

        protected float GenerateRandomBallYVelocity()
        {
            float output = _rnd.Next(1, 9) * 0.1f;
            return output * (_rnd.Next(0, 2) == 1 ? -1 : 1);
        }

        #endregion

        #region Scoring
        protected void Scored(ScreenSide side)
        {
            switch (side)
            {
                case ScreenSide.Left:
                    _rightScore++;
                    _roundStarted = false;
                    _leftStopped = false;
                    _rightStopped = false;

                    CheckWin();
                    if (!_gameEnded)
                        GenerateBall();
                    break;
                case ScreenSide.Right:
                    _leftScore++;
                    _roundStarted = false;
                    _leftStopped = false;
                    _rightStopped = false;

                    CheckWin();
                    if (!_gameEnded)
                        GenerateBall();
                    break;
                default:
                    return;
            }
        }
        protected void CheckWin()
        {
            if (_leftScore >= 10)
            {
                _gameEnded = true;
                _winningPlayer = ScreenSide.Left;
            }
            else if (_rightScore  >= 10)
            {
                _gameEnded = true;
                _winningPlayer = ScreenSide.Right;
            }
        }
        #endregion

        #region Update and Drawing
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState plr1GamepadState = GamePad.GetState(PlayerIndex.One);
            GamePadState plr2GamepadState = GamePad.GetState(PlayerIndex.Two);

            bool leftUsedKeyboard = false;
            bool rightUsedKeyboard = false;
            bool leftUsedStick = false;
            bool rightUsedStick = false;
            bool leftUsedDPad = false;
            bool rightUsedDPad = false;

            #region Exit, Restart and Bot controls

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboard.IsKeyDown(Keys.R)
                || plr1GamepadState.IsButtonDown(Buttons.Start)
                || plr2GamepadState.IsButtonDown(Buttons.Start))
                Restart();

            if (!_botButtonDown && keyboard.IsKeyDown(Keys.B)
                || plr1GamepadState.IsButtonDown(Buttons.Y)
                || plr2GamepadState.IsButtonDown(Buttons.Y))
            {
                _playWithBot = !_playWithBot;
                _botButtonDown = true;
            }

            if (keyboard.IsKeyUp(Keys.B)
                && plr1GamepadState.IsButtonUp(Buttons.Y)
                && plr2GamepadState.IsButtonUp(Buttons.Y))
                _botButtonDown = false;
            
            #endregion
            
            #region Keyboard controls
            
            // Left Up
            if (keyboard.IsKeyDown(Keys.W))
            {
                _leftPad.MoveNoOOS(0, -PadSpeed);
                leftUsedKeyboard = true;
            }

            // Left Down
            if (keyboard.IsKeyDown(Keys.S))
            {
                _leftPad.MoveNoOOS(0, PadSpeed);
                leftUsedKeyboard = true;
            }

            // Right Up
            if (keyboard.IsKeyDown(Keys.Up) && !_playWithBot)
            {
                _rightPad.MoveNoOOS(0, -PadSpeed);
                rightUsedKeyboard = true;
            }

            // Right Down
            if (keyboard.IsKeyDown(Keys.Down) && !_playWithBot)
            {
                _rightPad.MoveNoOOS(0, PadSpeed);
                rightUsedKeyboard = true;
            }

            #endregion

            #region Gamepad controls
            
            // Left Thumbstick
            if ((plr1GamepadState.ThumbSticks.Left.Y > GamepadDeadzone 
                 || plr1GamepadState.ThumbSticks.Left.Y < -GamepadDeadzone)
                && !leftUsedKeyboard)
            {
                _leftPad.MoveNoOOS(0, -plr1GamepadState.ThumbSticks.Left.Y * PadSpeed * GamepadSensitivity);
                leftUsedStick = true;
            }
            
            // Right Thumbstick
            if ((plr2GamepadState.ThumbSticks.Left.Y > GamepadDeadzone 
                 || plr2GamepadState.ThumbSticks.Left.Y < -GamepadDeadzone) 
                && !rightUsedKeyboard && !_playWithBot)
            {
                _rightPad.MoveNoOOS(0, -plr2GamepadState.ThumbSticks.Left.Y * PadSpeed * GamepadSensitivity);
                rightUsedStick = true;
            }
            
            // Left DPad Up
            if (plr1GamepadState.DPad.Up == ButtonState.Pressed
                && !leftUsedKeyboard && !leftUsedStick)
            {
                _leftPad.MoveNoOOS(0, -PadSpeed);
                leftUsedDPad = true;
            }

            // Left DPad Down
            if (plr1GamepadState.DPad.Down == ButtonState.Pressed
                && !leftUsedKeyboard && !leftUsedStick)
            {
                _leftPad.MoveNoOOS(0, PadSpeed);
                leftUsedDPad = true;
            }

            // Right DPad Up
            if (plr2GamepadState.DPad.Up == ButtonState.Pressed
                && !rightUsedKeyboard && !rightUsedStick
                && !_playWithBot)
            {
                _rightPad.MoveNoOOS(0, -PadSpeed);
                rightUsedDPad = true;
            }

            // Right DPad down
            if (plr2GamepadState.DPad.Down == ButtonState.Pressed
                && !rightUsedKeyboard && !rightUsedStick
                && !_playWithBot)
            {
                _rightPad.MoveNoOOS(0, PadSpeed);
                rightUsedDPad = true;
            }
            
            #endregion

            #region Bot Movement

            if (_playWithBot && _roundStarted)
            {
                if (_ball.Y > _rightPad.Y + _rightPad.Height / 2F * BotHeightCheckFactor
                    || _ball.Y < _rightPad.Y - _rightPad.Height / 2F * BotHeightCheckFactor)
                    _rightPad.MoveNoOOS(0, _rightPad.Y < _ball.Y ? PadSpeed : -PadSpeed);
            }

            #endregion

            bool moved = leftUsedKeyboard || rightUsedKeyboard
                || leftUsedStick || rightUsedStick
                || leftUsedDPad || rightUsedDPad;

            bool leftMoved = leftUsedKeyboard
                || leftUsedStick
                || leftUsedDPad;

            bool rightMoved = rightUsedKeyboard
                || rightUsedStick
                || rightUsedDPad;

            if ((leftMoved && _leftStopped) || (rightMoved && _rightStopped))
            {
                _roundStarted = true;
                _showStartMessage = false;
                _gameStarted = true;
            }

            if (!leftMoved)
                _leftStopped = true;

            if (!rightMoved)
                _rightStopped = true;

            if (_roundStarted)
                _ball.MoveByVelocity();

            if (_roundStarted)
            {
                ScreenSide scored = _ball.CheckScored();
                if (scored != ScreenSide.Center)
                    Scored(scored);
            }
            
            base.Update(gameTime);
            
            _platformSpecific?.UpdateFinished(_leftScore, _rightScore, _gameStarted);
        }

        private void WriteStatusText(string text)
        {
            Vector2 textMiddlePoint = _font.MeasureString(text) / 2;
            // Places text in center of the screen
            Vector2 position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 10);
            _spriteBatch.DrawString(_font, text, position, Color.White, 0, textMiddlePoint, 1.0f, SpriteEffects.None, 0.5f);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget);

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            _ball.Draw();
            _leftPad.Draw();
            _rightPad.Draw();

            _spriteBatch.End();

            base.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(new Color(30, 30, 30));

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            _spriteBatch.Draw(_renderTarget, _renderTargetDest, Color.White);

            #region Text
            if (!_gameEnded)
                WriteStatusText(_showStartMessage ?
                    "Left: W-S/Gamepad 1 stick/Gamepad 1 Dpad Up-Down"
                        + "\n" + (_playWithBot ? "Right: Played by bot" : "Right: Up-Down Arrow/Gamepad 2 stick/Gamepad 2 Dpad Up-Down")
                        + "\nMove to start"
                        + "\nPress B/Gamepad Y button to play with " + (_playWithBot ? "another person" : "bot")
                    : $"{_leftScore} - {_rightScore}");
            else
                WriteStatusText((_winningPlayer == ScreenSide.Left ?
                    _playWithBot ? "You won!" : "Left player won!"
                    : _playWithBot ? "You lose..." : "Right player won!")
                        + "\nPress Esc to quit. (on PC only)"
                        + "\nPress R on keyboard or Menu button on gamepad to restart.");
            #endregion

            _spriteBatch.End();
        }

        Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
        {
            float resolutionRatio = (float)resolution.X / resolution.Y;
            float screenRatio;
            Point bounds = new Point(preferredBackBufferWidth, preferredBackBufferHeight);
            screenRatio = (float)bounds.X / bounds.Y;
            float scale;
            Rectangle rectangle = new Rectangle();

            if (resolutionRatio < screenRatio)
                scale = (float)bounds.Y / resolution.Y;
            else if (resolutionRatio > screenRatio)
                scale = (float)bounds.X / resolution.X;
            else
            {
                // Resolution and window/screen share aspect ratio
                rectangle.Size = bounds;
                return rectangle;
            }
            rectangle.Width = (int)(resolution.X * scale);
            rectangle.Height = (int)(resolution.Y * scale);
            return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
        }

        static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
        {
            Point delta = outerRectangle.Center - innerRectangle.Center;
            innerRectangle.Offset(delta);
            return innerRectangle;
        }

        #endregion
    }
}
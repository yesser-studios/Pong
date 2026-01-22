using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Gum.Forms.Controls;
using Gum.Wireframe;
using MonoGameGum;
using Pong.Desktop;

namespace Pong.Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private static GumService Gum => GumService.Default;
        
        #region Variables
        private const float BallSpeed = 5f;

        private readonly Random _rnd = new Random();
        private readonly PlatformSpecific _platformSpecific;

        private readonly Point _gameResolution = new Point(960, 720);

        private bool _playWithBot = false;
        private bool _botButtonDown = false;
        private float _botTargetY;
        private const float BotDeadzone = 0.25f;

        private bool _gameStarted = false;
        private bool _showStartMessage = true;
        private bool _roundStarted = false;
        private bool _gameEnded = false;
        private bool _botModeSwitched = false;

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

        private StackPanel _mainMenuPanel;

        #endregion

        #region Initialization
        public Game1(PlatformSpecific platformSpecific)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _platformSpecific = platformSpecific;
            
            _botTargetY = _gameResolution.Y / 2f;
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
            
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _graphics.HardwareModeSwitch = false; // Force borderless window on Mac & Linux
            
            _graphics.ApplyChanges();

            Gum.Initialize(this);
            
            MainMenu();

            base.Initialize();
        }

        private void Restart()
        {
            _roundStarted = false;
            _gameEnded = false;
            _showStartMessage = true;
            _winningPlayer = ScreenSide.Center;
            _leftScore = 0;
            _rightScore = 0;
            GenerateBall();
            _leftPad.Position = new Vector2(PadXOffset, _gameResolution.Y / 2f);
            _rightPad.Position = new Vector2(_gameResolution.X - PadXOffset, _gameResolution.Y / 2f);
        }

        private void GenerateBall()
        {
            // Generates a random Y velocity.
            var ballVelocity = new Vector2(_rnd.Next(0, 2) == 1 ? -1 : 1, GenerateRandomBallYVelocity());

            _ball = new Ball(
                _ballTex,
                new Vector2(_gameResolution.X / 2f, _gameResolution.Y / 2f),
                1f,
                _spriteBatch,
                _gameResolution.X,
                _gameResolution.Y,
                ballVelocity * BallSpeed,
                _leftPad,
                _rightPad);

            _ball.OnBallBouncedEvent += OnBallBounced;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _leftPad = new GameObject(
                Content.Load<Texture2D>("Pad"),
                new Vector2(PadXOffset, _gameResolution.Y / 2f),
                PadScale,
                _spriteBatch,
                _gameResolution.X,
                _gameResolution.Y);

            _rightPad = new GameObject(
                Content.Load<Texture2D>("Pad"),
                new Vector2(_gameResolution.X - PadXOffset, _gameResolution.Y / 2f),
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

        private float GenerateRandomBallYVelocity()
        {
            var output = _rnd.Next(1, 9) * 0.1f;
            return output * (_rnd.Next(0, 2) == 1 ? -1 : 1);
        }

        private void HideUi()
        {
            Gum.Root.Children.Clear();
            IsMouseVisible = false;
        }
        
        private void MainMenu()
        {
            IsMouseVisible = true;
            
            _mainMenuPanel = new StackPanel();
            _mainMenuPanel.AddToRoot();
            _mainMenuPanel.Spacing = 3;
            _mainMenuPanel.Anchor(Anchor.Center);

            var header = new Label
            {
                Text = "Pong by Yesser Studios",
            };
            _mainMenuPanel.AddChild(header);

            var playMultiplayerButton = new Button
            {
                Text = "Play Multiplayer",
            };
            playMultiplayerButton.Click += (_, _) =>
            {
                HideUi();
                _playWithBot = false;
            };
            _mainMenuPanel.AddChild(playMultiplayerButton);
            playMultiplayerButton.Dock(Dock.FillHorizontally);
            
            var playBotButton = new Button
            {
                Text = "Play with Bot",
            };
            playBotButton.Click += (_, _) =>
            {
                HideUi();
                _playWithBot = true;
            };
            _mainMenuPanel.AddChild(playBotButton);
            playBotButton.Dock(Dock.FillHorizontally);

            var quitButton = new Button
            {
                Text = "Quit",
            };
            quitButton.Click += (_, _) =>
            {
                Exit();
            };
            _mainMenuPanel.AddChild(quitButton);
            quitButton.Dock(Dock.FillHorizontally);
        }

        #endregion

        #region Scoring

        private void Scored(ScreenSide side)
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

        private void CheckWin()
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

        #region Ball Movement

        private void OnBallBounced(ScreenSide side)
        {
            if (side == ScreenSide.Left)
                SimulateBall();
        }
        
        /// <summary>
        /// Simulates ball movement from the left side to the right side and returns the position at which it hit the right pad level. <para />
        /// The position is then stored in <see cref="_botTargetY"/>.
        /// </summary>
        private void SimulateBall()
        {
            if (!_playWithBot)
                return;

            var dummyBall = new GameObject(_ball.Texture, _ball.Position, _ball.Scale, null,
                _gameResolution.X, _gameResolution.Y)
            {
                Velocity = _ball.Velocity
            };

            var lastTouched = ScreenSide.Center;
            
            while (dummyBall.Position.X + (_ball.Width / 2) < _rightPad.Position.X - (_rightPad.Width / 2))
            {
                dummyBall.Position += dummyBall.Velocity;
                var touchSide = dummyBall.CheckOos();

                var newVelocity = dummyBall.Velocity;

                switch (touchSide)
                {
                    case ScreenSide.Bottom:
                        newVelocity.Y = -MathF.Abs(dummyBall.Velocity.Y)
                                        - (lastTouched != ScreenSide.Bottom
                                            ? Ball.BounceSpeedUp
                                            : 0);
                        lastTouched = ScreenSide.Bottom;
                        break;
                    case ScreenSide.Top:
                        newVelocity.Y = MathF.Abs(newVelocity.Y)
                                        + (lastTouched != ScreenSide.Top
                                            ? Ball.BounceSpeedUp
                                            : 0);
                        lastTouched = ScreenSide.Top;
                        break;
                }

                if (dummyBall.Position.X - (dummyBall.Width / 2) <=
                    _leftPad.Position.X + (_leftPad.Width / 2))
                {
                    newVelocity.X = MathF.Abs(newVelocity.X) +
                                    (lastTouched != ScreenSide.Left ? Ball.BounceSpeedUp : 0);
                    lastTouched = ScreenSide.Left;
                }

                dummyBall.Velocity = newVelocity;
            }

            _botTargetY = dummyBall.Position.Y;
        }
        #endregion

        #region Update and Drawing
        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var plr1GamepadState = GamePad.GetState(PlayerIndex.One);
            var plr2GamepadState = GamePad.GetState(PlayerIndex.Two);

            var leftUsedKeyboard = false;
            var rightUsedKeyboard = false;
            var leftUsedStick = false;
            var rightUsedStick = false;
            var leftUsedDPad = false;
            var rightUsedDPad = false;

            #region Exit, Restart and Bot controls

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboard.IsKeyDown(Keys.R)
                || plr1GamepadState.IsButtonDown(Buttons.Start)
                || plr2GamepadState.IsButtonDown(Buttons.Start))
                Restart();

            if (!_botButtonDown && !_roundStarted
                && (keyboard.IsKeyDown(Keys.B)
                    || plr1GamepadState.IsButtonDown(Buttons.Y)
                    || plr2GamepadState.IsButtonDown(Buttons.Y)))
            {
                _playWithBot = !_playWithBot;
                _botButtonDown = true;
                _botModeSwitched = true;
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
                _leftPad.MoveNoOos(0, -PadSpeed);
                leftUsedKeyboard = true;
            }

            // Left Down
            if (keyboard.IsKeyDown(Keys.S))
            {
                _leftPad.MoveNoOos(0, PadSpeed);
                leftUsedKeyboard = true;
            }

            // Right Up
            if (keyboard.IsKeyDown(Keys.Up) && !_playWithBot)
            {
                _rightPad.MoveNoOos(0, -PadSpeed);
                rightUsedKeyboard = true;
            }

            // Right Down
            if (keyboard.IsKeyDown(Keys.Down) && !_playWithBot)
            {
                _rightPad.MoveNoOos(0, PadSpeed);
                rightUsedKeyboard = true;
            }

            #endregion

            #region Gamepad controls
            
            // Left Thumbstick
            if ((plr1GamepadState.ThumbSticks.Left.Y > GamepadDeadzone 
                 || plr1GamepadState.ThumbSticks.Left.Y < -GamepadDeadzone)
                && !leftUsedKeyboard)
            {
                _leftPad.MoveNoOos(0, -plr1GamepadState.ThumbSticks.Left.Y * PadSpeed * GamepadSensitivity);
                leftUsedStick = true;
            }
            
            // Right Thumbstick
            if ((plr2GamepadState.ThumbSticks.Left.Y > GamepadDeadzone 
                 || plr2GamepadState.ThumbSticks.Left.Y < -GamepadDeadzone) 
                && !rightUsedKeyboard && !_playWithBot)
            {
                _rightPad.MoveNoOos(0, -plr2GamepadState.ThumbSticks.Left.Y * PadSpeed * GamepadSensitivity);
                rightUsedStick = true;
            }
            
            // Left DPad Up
            if (plr1GamepadState.DPad.Up == ButtonState.Pressed
                && !leftUsedKeyboard && !leftUsedStick)
            {
                _leftPad.MoveNoOos(0, -PadSpeed);
                leftUsedDPad = true;
            }

            // Left DPad Down
            if (plr1GamepadState.DPad.Down == ButtonState.Pressed
                && !leftUsedKeyboard && !leftUsedStick)
            {
                _leftPad.MoveNoOos(0, PadSpeed);
                leftUsedDPad = true;
            }

            // Right DPad Up
            if (plr2GamepadState.DPad.Up == ButtonState.Pressed
                && !rightUsedKeyboard && !rightUsedStick
                && !_playWithBot)
            {
                _rightPad.MoveNoOos(0, -PadSpeed);
                rightUsedDPad = true;
            }

            // Right DPad down
            if (plr2GamepadState.DPad.Down == ButtonState.Pressed
                && !rightUsedKeyboard && !rightUsedStick
                && !_playWithBot)
            {
                _rightPad.MoveNoOos(0, PadSpeed);
                rightUsedDPad = true;
            }
            
            #endregion

            #region Bot Movement

            if (_playWithBot && _roundStarted && _ball.Velocity.X > 0)
            {
                if (_botTargetY > _rightPad.Y + (_rightPad.Height * BotDeadzone)
                    || _botTargetY < _rightPad.Y - (_rightPad.Height * BotDeadzone))
                    _rightPad.MoveNoOos(0, _rightPad.Y < _botTargetY ? PadSpeed : -PadSpeed);
            }

            #endregion

            var leftMoved = leftUsedKeyboard
                            || leftUsedStick
                            || leftUsedDPad;

            var rightMoved = rightUsedKeyboard
                             || rightUsedStick
                             || rightUsedDPad;

            if ((leftMoved && _leftStopped) || (rightMoved && _rightStopped))
            {
                if (!_roundStarted && _ball.Velocity.X > 0)
                    SimulateBall();
                
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
                var scored = _ball.CheckScored();
                if (scored != ScreenSide.Center)
                    Scored(scored);
                if (_botModeSwitched)
                    _botModeSwitched = false;
            }
            
            Gum.Update(gameTime);
            
            base.Update(gameTime);
            
            _platformSpecific?.UpdateFinished(_leftScore, _rightScore, _gameStarted);
        }

        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        private void WriteStatusText(string text)
        {
            var textMiddlePoint = _font.MeasureString(text) / 2;
            // Places text in center of the screen
            var position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 10);
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
                    : _botModeSwitched ? 
                        $"   {_leftScore} - {_rightScore}" // Spaces on beginning to keep score centered 
                            + $"\nBot {(_playWithBot ? "Enabled" : "Disabled")}" 
                        : $"{_leftScore} - {_rightScore}");
            else
                WriteStatusText((_winningPlayer == ScreenSide.Left ?
                    _playWithBot ? "You won!" : "Left player won!"
                    : _playWithBot ? "You lose..." : "Right player won!")
                        + "\nPress Esc to quit. (on PC only)"
                        + "\nPress R on keyboard or Menu button on gamepad to restart.");
            #endregion

            _spriteBatch.End();
            
            Gum.Draw();
            base.Draw(gameTime);
        }

        private Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
        {
            var resolutionRatio = (float)resolution.X / resolution.Y;
            var bounds = new Point(preferredBackBufferWidth, preferredBackBufferHeight);
            var screenRatio = (float)bounds.X / bounds.Y;
            float scale;
            var rectangle = new Rectangle();

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

        private static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
        {
            var delta = outerRectangle.Center - innerRectangle.Center;
            innerRectangle.Offset(delta);
            return innerRectangle;
        }

        #endregion
    }
}
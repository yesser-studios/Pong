using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong.Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private const float BALL_SPEED = 5f;

        private Random rnd = new Random();

        private Point gameResolution = new Point(960, 720);

        private bool gameStarted = false;
        private bool showStartMessage = true;
        private bool gameEnded = false;

        private int padXOffset = 10;
        private float padScale = 0.5f;
        private float padSpeed = 7.5f;
        private ScreenSide winningPlayer = ScreenSide.Center;

        private float gamepadDeadzone = 0.1f;
        private float gamepadSensitivity = 1.1f;

        private RenderTarget2D _renderTarget;
        private Rectangle _renderTargetDest;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private Ball ball;
        private GameObject leftPad;
        private GameObject rightPad;

        private Texture2D ballTex;

        private int leftScore,
            rightScore = 0;

        /// <summary>
        /// Whether left/right stopped after landing a goal.
        /// </summary>
        private bool leftStopped,
            rightStopped = false;

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
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected float GenerateRandomBallYVelocity()
        {
            float output = rnd.Next(1, 9) * 0.1f;
            return output * (rnd.Next(0, 2) == 1 ? -1 : 1);
        }

        protected void Scored(ScreenSide side)
        {
            switch (side)
            {
                case ScreenSide.Left:
                    rightScore++;
                    gameStarted = false;
                    leftStopped = false;
                    rightStopped = false;

                    CheckWin();
                    if (!gameEnded)
                        GenerateBall();
                    break;
                case ScreenSide.Right:
                    leftScore++;
                    gameStarted = false;
                    leftStopped = false;
                    rightStopped = false;

                    CheckWin();
                    if (!gameEnded)
                        GenerateBall();
                    break;
                default:
                    return;
            }
        }

        protected void CheckWin()
        {
            if (leftScore >= 10)
            {
                gameEnded = true;
                winningPlayer = ScreenSide.Left;
            }
            else if (rightScore  >= 10)
            {
                gameEnded = true;
                winningPlayer = ScreenSide.Right;
            }
        }

        private void GenerateBall()
        {
            // Generates a random Y velocity.
            Vector2 ballVelocity = new Vector2(rnd.Next(0, 2) == 1 ? -1 : 1, GenerateRandomBallYVelocity());

            ball = new Ball(
                ballTex,
                new Vector2(gameResolution.X / 2, gameResolution.Y / 2),
                1f,
                _spriteBatch,
                gameResolution.X,
                gameResolution.Y,
                ballVelocity * BALL_SPEED,
                leftPad,
                rightPad);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            leftPad = new GameObject(
                Content.Load<Texture2D>("Pad"),
                new Vector2(padXOffset, gameResolution.Y / 2),
                padScale,
                _spriteBatch,
                gameResolution.X,
                gameResolution.Y);

            rightPad = new GameObject(
                Content.Load<Texture2D>("Pad"),
                new Vector2(gameResolution.X - padXOffset, gameResolution.Y / 2),
                padScale,
                _spriteBatch,
                gameResolution.X,
                gameResolution.Y);

            ballTex = Content.Load<Texture2D>("Ball");

            GenerateBall();

            _font = Content.Load<SpriteFont>("GameFont");

            _renderTarget = new RenderTarget2D(
                GraphicsDevice,
                gameResolution.X,
                gameResolution.Y);

            _renderTargetDest = GetRenderTargetDestination(
                gameResolution,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);
        }

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

            if (plr1GamepadState.Buttons.Start == ButtonState.Pressed
                || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            #region Keyboard controls
            if (keyboard.IsKeyDown(Keys.W))
            {
                leftPad.MoveNoOOS(0, -padSpeed);
                leftUsedKeyboard = true;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                leftPad.MoveNoOOS(0, padSpeed);
                leftUsedKeyboard = true;
            }

            if (keyboard.IsKeyDown(Keys.Up))
            {
                rightPad.MoveNoOOS(0, -padSpeed);
                rightUsedKeyboard = true;
            }


            if (keyboard.IsKeyDown(Keys.Down))
            {
                rightPad.MoveNoOOS(0, padSpeed);
                rightUsedKeyboard = true;
            }

            #endregion

            #region Gamepad controls
            if ((plr1GamepadState.ThumbSticks.Left.Y > gamepadDeadzone || plr1GamepadState.ThumbSticks.Left.Y < -gamepadDeadzone) && !leftUsedKeyboard)
            {
                leftPad.MoveNoOOS(0, -plr1GamepadState.ThumbSticks.Left.Y * padSpeed * gamepadSensitivity);
                leftUsedStick = true;
            }

            if ((plr2GamepadState.ThumbSticks.Left.Y > gamepadDeadzone || plr2GamepadState.ThumbSticks.Left.Y < -gamepadDeadzone) && !rightUsedKeyboard)
            {
                rightPad.MoveNoOOS(0, -plr2GamepadState.ThumbSticks.Left.Y * padSpeed * gamepadSensitivity);
                rightUsedStick = true;
            }

            if (plr1GamepadState.DPad.Up == ButtonState.Pressed && !leftUsedKeyboard && !leftUsedStick)
            {
                leftPad.MoveNoOOS(0, -padSpeed);
                leftUsedDPad = true;
            }

            if (plr1GamepadState.DPad.Down == ButtonState.Pressed && !leftUsedKeyboard && !leftUsedStick)
            {
                leftPad.MoveNoOOS(0, padSpeed);
                leftUsedDPad = true;
            }

            if (plr2GamepadState.DPad.Up == ButtonState.Pressed && !rightUsedKeyboard && !rightUsedStick)
            {
                rightPad.MoveNoOOS(0, -padSpeed);
                rightUsedDPad = true;
            }

            if (plr2GamepadState.DPad.Down == ButtonState.Pressed && !rightUsedKeyboard && !rightUsedStick)
            {
                rightPad.MoveNoOOS(0, padSpeed);
                rightUsedDPad = true;
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

            if ((leftMoved && leftStopped) || (rightMoved && rightStopped))
            {
                gameStarted = true;
                showStartMessage = false;
            }

            if (!leftMoved)
                leftStopped = true;

            if (!rightMoved)
                rightStopped = true;

            if (gameStarted)
                ball.MoveByVelocity();

            if (gameStarted)
            {
                ScreenSide scored = ball.CheckScored();
                if (scored != ScreenSide.Center)
                    Scored(scored);
            }
            
            base.Update(gameTime);
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
            ball.Draw();
            leftPad.Draw();
            rightPad.Draw();

            _spriteBatch.End();

            base.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(new Color(30, 30, 30));

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            _spriteBatch.Draw(_renderTarget, _renderTargetDest, Color.White);

            #region Text
            if (!gameEnded)
                WriteStatusText(showStartMessage ?
                    "Left: W-S/Gamepad 1 stick/Gamepad 1 Dpad Up-Down\n"
                        + "Right: Up-Down Arrow/Gamepad 2 stick/Gamepad 2 Dpad Up-Down\n"
                        + "Move to start"
                    : $"{leftScore} - {rightScore}");
            else
                WriteStatusText((winningPlayer == ScreenSide.Left ?
                    "Left player won!"
                    : "Right player won!")
                        + "\nPress Esc (or Menu button on gamepad) to quit.");
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
    }
}
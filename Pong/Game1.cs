using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong
{
    public class Game1 : Game
    {
        private Point gameResolution = new(960, 720);

        private int padXOffset = 10;
        private float padScale = 0.5f;
        private float padSpeed = 7.5f;

        private RenderTarget2D _renderTarget;
        private Rectangle _renderTargetDest;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameObject ball;
        private GameObject leftPad;
        private GameObject rightPad;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ball = new(
                Content.Load<Texture2D>("Ball"),
                new(gameResolution.X / 2, gameResolution.Y / 2),
                1f,
                _spriteBatch);

            leftPad = new(
                Content.Load<Texture2D>("Pad"),
                new(padXOffset, gameResolution.Y / 2),
                padScale,
                _spriteBatch);

            rightPad = new(
                Content.Load<Texture2D>("Pad"),
                new(gameResolution.X - padXOffset, gameResolution.Y / 2),
                padScale,
                _spriteBatch);

            _renderTarget = new(GraphicsDevice, gameResolution.X, gameResolution.Y);
            _renderTargetDest = GetRenderTargetDestination(gameResolution, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState plr1GamepadState = GamePad.GetState(PlayerIndex.One);
            GamePadState plr2GamepadState = GamePad.GetState(PlayerIndex.Two);

            if (plr1GamepadState.Buttons.Start == ButtonState.Pressed
                || plr2GamepadState.Buttons.Start == ButtonState.Pressed
                || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            #region Keyboard controls
            if (keyboard.IsKeyDown(Keys.W))
                leftPad.MoveNoOOS(0, -padSpeed, gameResolution.X, gameResolution.Y);

            if (keyboard.IsKeyDown(Keys.S))
                leftPad.MoveNoOOS(0, padSpeed, gameResolution.X, gameResolution.Y);

            if (keyboard.IsKeyDown(Keys.Up))
                rightPad.MoveNoOOS(0, -padSpeed, gameResolution.X, gameResolution.Y);

            if (keyboard.IsKeyDown(Keys.Down))
                rightPad.MoveNoOOS(0, padSpeed, gameResolution.X, gameResolution.Y);
            #endregion

            base.Update(gameTime);
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

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            _spriteBatch.Draw(_renderTarget, _renderTargetDest, Color.White);
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
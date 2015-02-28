using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pong {
    public class Ball : Sprite {
        private Vector2 originalPosition;
        private Random rand = new Random(); 

        public Ball(Texture2D texture2D, Vector2 position, Rectangle screenBounds) : 
            base(texture2D, position, screenBounds)
        {
            this.originalPosition = position;

            SetDefaultSpeed();
            SetRandomDirection();
        }        
        void Reset() 
        {
            position = new Vector2(originalPosition.X, originalPosition.Y);
            SetDefaultSpeed();
            SetRandomDirection();
        }
        private void SetDefaultSpeed()
        {
            Velocity = new Vector2(0.35f, 0.35f);
        }
        private void SetRandomDirection()
        {
            // Get a random angle pointing right from 55 to 125 degrees
            direction = Calc2D.GetRightPointingAngledPoint(rand.Next(55, 125));
            if (rand.Next(2) == 1)
                direction = -direction;
        }
        void ReverseYDirection() 
        {
            direction.Y = -direction.Y;
        }
        void SetPosition(float x, float y) 
        {
            this.position.X = x;
            this.position.Y = y;
        }

        public override void Update(GameTime gameTime, GameObjects gameObjects) 
        {
            position += direction * Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            CheckBounds(gameObjects);

            CheckBallCollision(gameObjects.PlayerPaddle, gameObjects.SoundManager );
            CheckBallCollision(gameObjects.ComputerPaddle, gameObjects.SoundManager);
        }
        
        protected override void CheckBounds(GameObjects gameObjects)
        {
            var player = gameObjects.PlayerPaddle;
            var computer = gameObjects.ComputerPaddle;
            var soundManager = gameObjects.SoundManager;

            if (Position.X > screenBounds.Width)
            {
                soundManager.Play("bell");
                computer.Score++;
                Reset();
            }
            else if (Position.X < 0)
            {
                soundManager.Play("bell");
                player.Score++;
                Reset();
            }
            else if (Position.Y > screenBounds.Height - Height)
            {
                soundManager.Play("ping");
                SetPosition(Position.X, screenBounds.Height - Height);
                ReverseYDirection();
            }
            else if (Position.Y < 0)
            {
                soundManager.Play("ping");
                SetPosition(Position.X, 0);
                ReverseYDirection();
            }
        }
        private void CheckBallCollision(Sprite paddle, SoundManager soundManager)
        {
            if (BoundingBox.Intersects(paddle.BoundingBox))
            {
                soundManager.Play("ping");
                Bounce(paddle);
            }
        }
        void Bounce(Sprite paddle)
        {
            Velocity *= 1.04f;

            // Calculate a new direction depending on where on the paddle the ball bounces
            float differenceToTargetCenter = paddle.BoundingBox.Center.Y - BoundingBox.Center.Y;
            direction = Calc2D.GetRightPointingAngledPoint((int)(90 + (differenceToTargetCenter * 1.3f)));

            // Set a new position to make sure we're outside the paddle
            if (paddle.BoundingBox.Center.X > BoundingBox.Center.X)
            {
                direction.X = -direction.X;
                position.X = paddle.BoundingBox.Left - texture.Width;
            }
            else
            {
                position.X = paddle.BoundingBox.Right;
            }
        }
    }
}

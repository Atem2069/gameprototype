using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace XNA_Lookat
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            SoundEffect.MasterVolume = 0.1f;
            base.Initialize();

        }
        enum EnemyState //We don't need this yet, but this will be used soon 
        {
            Alive,
            Dead
        }
        EnemyState enemyState;
        public Vector2 spritePosition,enemyPosition;
        public Rectangle playerRectangle,enemyRectangle,screen;
        Texture2D rocket,enemy,background;
        Vector2 origin;
        float spriteRotation,enemyRotation;
        float enemyHealth;
        //Sound effects
        SoundEffect bulletFired,bulletPassing,explosion,alert;
        //Bullets list
        List<Bullet> bullets = new List<Bullet>();
        //Text Items
        SpriteFont font;
        string enemyStr,stateStr;

        Camera camera;
        Stopwatch timer;
        protected override void LoadContent()
        {
            timer = new Stopwatch();
            enemyState = EnemyState.Alive;
            enemyHealth = 100;
            spriteRotation = 0;
            enemyRotation = 180;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            rocket = Content.Load<Texture2D>("rocket");
            enemy = Content.Load<Texture2D>("newenemy");
            background = Content.Load<Texture2D>("space");
            screen = new Rectangle(0, 0, 9999, 9999);
            origin = new Vector2(rocket.Width / 2, rocket.Height / 2);
            spritePosition = new Vector2(1000, 1200);
            enemyPosition = new Vector2(spritePosition.X + 500, spritePosition.Y + 500);
            playerRectangle = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, rocket.Width / 2, rocket.Height / 2);
            enemyRectangle = new Rectangle((int)enemyPosition.X, (int)enemyPosition.Y, enemy.Width / 2, enemy.Height / 2);
            Random rnd = new Random();
            //Sound effects
            bulletFired = Content.Load<SoundEffect>("Sounds\\battle032");
            bulletPassing = Content.Load<SoundEffect>("Sounds\\battle001");
            explosion = Content.Load<SoundEffect>("Sounds\\battle003");
            alert = Content.Load<SoundEffect>("Sounds\\alert");
            //Used to be here: OLD DISTANCE CALCULATION


            font = Content.Load<SpriteFont>("GameFont");
            enemyStr = "Enemy Health: 0";
            stateStr = "Enemy State: N/A";
            
            camera = new Camera(GraphicsDevice.Viewport);
        }

        bool enemyCam,vKeyDown = true;
        protected override void Update(GameTime gameTime)
        {
            //Infinite background
            KeyboardState keyState = Keyboard.GetState();
            timer.Start();
            Console.WriteLine(timer.ElapsedMilliseconds);
            enemyStr = "Enemy Health: " + enemyHealth;
            stateStr = "Enemy State: " + enemyState;
            const float velocity = 15;
            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();
            if (keyState.IsKeyDown(Keys.Left))
                spriteRotation -= 0.1f;
            if (keyState.IsKeyDown(Keys.Right))
                spriteRotation += 0.1f;

            if (keyState.IsKeyDown(Keys.D))
            {
                spritePosition.X += velocity;
            }
            if (keyState.IsKeyDown(Keys.A))
                spritePosition.X -= velocity;
            if (keyState.IsKeyDown(Keys.S))
                spritePosition.Y += velocity;
            if (keyState.IsKeyDown(Keys.W))
                spritePosition.Y -= velocity;

            if (keyState.IsKeyDown(Keys.Space) && timer.ElapsedMilliseconds >= 50)
            {
                Shoot();
                timer.Reset();
            }
            if (keyState.IsKeyDown(Keys.R))
            {
                bullets.Clear();
                Initialize();
            }

            Vector2 targetPos = spritePosition - enemyPosition;
            enemyRotation = (float)Math.Atan2(targetPos.Y,targetPos.X) + MathHelper.TwoPi + 45;
            Vector2 heading = new Vector2((float)Math.Cos(enemyRotation), (float)Math.Sin(enemyRotation));
            if (Vector2.Distance(enemyPosition, spritePosition) <= 300)
                enemyPosition += heading * 30;
            else
                enemyPosition += heading * 15;


            //Used to be here: OLD AI CODE
            enemyRectangle.X = (int)enemyPosition.X;
            enemyRectangle.Y = (int)enemyPosition.Y;
            foreach (Bullet bullet in bullets)
            {
                Rectangle rct = new Rectangle((int)bullet.Position.X, (int)bullet.Position.Y, bullet.Texture.Width, bullet.Texture.Height);
                if (rct.Intersects(enemyRectangle) && enemyState == EnemyState.Alive)
                {
                    enemyHealth -= 20;
                    Console.WriteLine(enemyHealth);
                }
                    
            }

            if (enemyRectangle.Intersects(playerRectangle) && enemyState == EnemyState.Alive)
            {
                Console.WriteLine("You were killed by an enemy");
                explosion.Play();
                bullets.Clear();
                LoadContent();
            }
            if (enemyRectangle.Intersects(playerRectangle) && enemyState == EnemyState.Dead)
            {
                enemyPosition.X = spritePosition.X + 500;
                enemyPosition.Y = spritePosition.Y + 500;
                Console.WriteLine("Watch out! The enemy is alive again!");
                alert.Play();
                bullets.Clear();
                enemyHealth = 100;
                enemyState = EnemyState.Alive;
            }
            UpdateBullets();
            if (keyState.IsKeyDown(Keys.V) && vKeyDown)
            {
                vKeyDown = false;
            }
            if (keyState.IsKeyUp(Keys.V) && !vKeyDown)
            {
                enemyCam = !enemyCam;
                vKeyDown = true;
            }

            if (enemyCam == true)
                camera.Update(enemyPosition, enemyRectangle);
            else
                camera.Update(spritePosition,playerRectangle);

            if (enemyHealth <= 0)
                enemyState = EnemyState.Dead;

            playerRectangle.X = (int)spritePosition.X; playerRectangle.Y = (int)spritePosition.Y;
            base.Update(gameTime);
        }


        public void UpdateBullets()
        {
            foreach (Bullet bullet in bullets)
            {
                bullet.Position += bullet.Velocity;
                if (Vector2.Distance(bullet.Position, spritePosition) > 10000)
                    bullet.isVisible = false;
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].isVisible == false)
                    bullets.RemoveAt(i);
                
            }
        }
        float bulletRotation;
        public void Shoot()
        {
            Bullet newBullet = new Bullet(Content.Load<Texture2D>("bullet"));
            Vector2 velocity = new Vector2((float)Math.Cos(spriteRotation - 45) , (float)Math.Sin(spriteRotation - 45));
            bulletRotation = 375;
            velocity.Normalize();
            newBullet.Velocity += velocity * 5;
            newBullet.Position = spritePosition + newBullet.Velocity * 5;
            newBullet.isVisible = true;
            if (bullets.Count < 9999)
            {

                bullets.Add(newBullet);
                if (enemyCam == true)
                    bulletPassing.Play();
                else
                    bulletFired.Play();
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,null,null,null,null,camera.transform);

            spriteBatch.Draw(background, screen, Color.White);
            spriteBatch.Draw(rocket, spritePosition, null,Color.White,spriteRotation,origin,1f,SpriteEffects.None,0);
            if (enemyState == EnemyState.Alive)
                spriteBatch.Draw(rocket, enemyPosition, null, Color.Red, enemyRotation, origin, 1f, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(rocket, enemyPosition, null, Color.White, enemyRotation, origin, 1f, SpriteEffects.None, 0);
            foreach (Bullet bullet in bullets)
                bullet.Draw(spriteBatch,bulletRotation);
            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.DrawString(font, enemyStr, new Vector2(10, 10), Color.Red);
            spriteBatch.DrawString(font, stateStr, new Vector2(10, 50), Color.Green);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

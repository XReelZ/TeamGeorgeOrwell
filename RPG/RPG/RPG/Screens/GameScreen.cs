﻿namespace Rpg.Screens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Interfaces;
    using Objects;

    internal class GameScreen
    {
        private readonly IList<MenuItems> levelUpItems = new List<MenuItems>();
        private readonly Cursor cursor = new Cursor(new Position(0, 0));
        private readonly Cursor cursorMenu = new Cursor(new Position(0, 0));
        private readonly IList<Obstacles> obstacles = new List<Obstacles>();
        private readonly IList<Bullet> bullets = new List<Bullet>();
        private readonly IList<Bullet> enemyBullets = new List<Bullet>();
        private readonly Random rand = new Random();
        private readonly IList<Units> units = new List<Units>();
        private readonly IList<Bonuses> bonuses = new List<Bonuses>();
        private readonly Rectangle exitSpot = new Rectangle(786, 157, 160, 80);

        private bool bulletShooted = false;
        private bool paused = false;
        private bool levelUp = false;
        private int stage = 1;
        private bool loaded = false;
        private int skillTimer = 0;
        private int points = 5;
        private Rectangle room;
        private Texture2D gameWindowTexture;
        private Vector2 gameWindowTexturePos;
        private Texture2D enemyHealthBars;

        private Texture2D heroBarHolder;
        private Texture2D heroBarHolderReversed;
        private Texture2D heroHealthBar;
        private Texture2D heroManaBar;
        private Texture2D heroExpBar;

        private Hero hero;
        private float currentMaxHp;
        private float currentMaxMp;
        private float currentSpeed;
        private float currentRange;
        private float currentAttack;
        private float currentDefence;

        private Texture2D bonusHPTexture;
        private Texture2D bonusMPTexture;

        private Texture2D levelUpTexture;
        private Rectangle levelUpRect;
        private Texture2D leftButton;
        private Texture2D rightButton;
        private Texture2D okButton;

        private KeyboardState keyboard;
        private KeyboardState previousKeyboard;
        private MouseState mouse;
        private MouseState previousMouse;
        private SoundEffect walk;
        private SoundEffectInstance walkInstance;
        private SoundEffect walk2;
        private SoundEffectInstance walkInstance2;
        private SoundEffect gunShot;
        private SoundEffectInstance gunShotInstance;
        private SoundEffect pain1;
        private SoundEffectInstance pain1Instance;
        private SoundEffect pain2;
        private SoundEffectInstance pain2Instance;
        private SoundEffect gameSong;
        private SoundEffectInstance gameSongInstance;

        public void Load(ContentManager content)
        {
            this.LoadMusic(content);
            this.LoadLevel(content);
            this.LoadHero(content);
            this.LoadHeroStats(content);
            this.LoadUnits(content);
            this.LoadCursor(content);
            this.LoadBullets(content);
            this.LoadObstacles(content);
            this.LoadBonusses(content);
            this.LoadLevelUp(content);
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager content)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null);

            this.DrawBackGround(spriteBatch);
            this.DrawBonusses(spriteBatch);
            this.DrawUnits(spriteBatch, content);
            this.DrawHeroStats(spriteBatch, content);
            this.DrawBullets(spriteBatch);
            this.DrawObstacles(spriteBatch);

            if (this.paused)
            {
                this.DrawPaused(spriteBatch, content);
            }

            if (this.levelUp)
            {
                this.DrawLevelUp(spriteBatch, content);
            }

            this.DrawCursor(spriteBatch);

            spriteBatch.End();
        }

        public void Update(ContentManager content)
        {
            this.keyboard = Keyboard.GetState();
            this.mouse = Mouse.GetState();
            this.UpdateCursor();

            if (!this.paused && !this.levelUp)
            {
                this.UpdateUnits();
                this.UpdateBullets();
                this.UpdateHero(content);
                this.UpdateBonusses();
            }
            else if (this.levelUp)
            {
                this.UpdateLevelUp();
            }

            if (this.keyboard.IsKeyDown(Keys.Tab))
            {
                this.paused = true;
                MainMenuScreen.PMainMenuItems[0].ItemText = "RESUME GAME";
                this.gameSongInstance.Stop();
                Rpg.ActiveWindowSet(EnumActiveWindow.MainMenu);
            }

            if (this.keyboard.IsKeyUp(Keys.P) && this.previousKeyboard.IsKeyDown(Keys.P))
            {
                if (this.paused)
                {
                    this.paused = false;
                }
                else
                {
                    this.paused = true;
                }
            }

            this.previousMouse = this.mouse;
            this.previousKeyboard = this.keyboard;
        }

        private void LoadBullets(ContentManager content)
        {

            Texture2D heroBulletTexture = content.Load<Texture2D>(@"Textures\Objects\bullet" + ChooseHeroScreen.HeroName);
            for (int i = 0; i < 10; i++)
            {
                Bullet o = new Bullet(new Position(0, 0), heroBulletTexture);
                o.Area = new Rectangle(0, 0, heroBulletTexture.Width, heroBulletTexture.Height);
                this.bullets.Add(o);
            }

            Texture2D bulletTexture = content.Load<Texture2D>(@"Textures\Objects\bullet");

            for (int i = 0; i < 10; i++)
            {
                Bullet o = new Bullet(new Position(0, 0), bulletTexture);
                o.Area = new Rectangle(0, 0, bulletTexture.Width, bulletTexture.Height);
                this.enemyBullets.Add(o);
            }
        }

        private void LoadObstacles(ContentManager content)
        {
            Texture2D invisTexture = content.Load<Texture2D>(@"Textures\Objects\invisible");

            for (int i = 0; i < 275; i += 25)
            {
                Obstacles invisble = new Obstacles(new Position(i, 350), invisTexture, false);
                invisble.Area = new Rectangle((int)invisble.Position.X, (int)invisble.Position.Y, invisTexture.Width, invisTexture.Height);
                this.obstacles.Add(invisble);
            }

            for (int i = 150; i < 275; i += 25)
            {
                Obstacles invisble = new Obstacles(new Position(i, 475), invisTexture, false);
                invisble.Area = new Rectangle((int)invisble.Position.X, (int)invisble.Position.Y, invisTexture.Width, invisTexture.Height);
                this.obstacles.Add(invisble);
            }
            for (int i = 275; i < 550; i += 25)
            {
                Obstacles invisble = new Obstacles(new Position(i, 340), invisTexture, false);
                invisble.Area = new Rectangle((int)invisble.Position.X, (int)invisble.Position.Y, invisTexture.Width, invisTexture.Height);
                this.obstacles.Add(invisble);
            }

        }

        private void LoadCursor(ContentManager content)
        {
            this.cursor.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", "crosshair"));
            this.cursorMenu.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", "cursor"));
        }

        private void LoadHeroStats(ContentManager content)
        {
            this.heroBarHolder = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", "bar_holder"));
            this.heroHealthBar = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", "life_bar"));
            this.heroManaBar = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", "energy_bar"));
            this.heroExpBar = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", "exp_bar"));
            this.heroBarHolderReversed = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", "bar_holder_reversed"));

        }

        private void LoadUnits(ContentManager content)
        {
            this.enemyHealthBars = content.Load<Texture2D>(@"Textures\Objects\EnemyHPBar");
            if (this.stage == 1)
            {
                // Mele
                this.AddMeleUnit(content, 360, 580, "mele");
                this.AddMeleUnit(content, 470, 545, "mele");

                this.AddMeleUnit(content, 760, 620, "mele");
                this.AddMeleUnit(content, 910, 620, "mele");

                this.AddMeleUnit(content, 710, 310, "mele");
                this.AddMeleUnit(content, 590, 335, "mele");

                // Range
                this.AddRangeUnit(content, 200, 660, "range");
                this.AddRangeUnit(content, 965, 670, "range");
                this.AddRangeUnit(content, 570, 170, "range");
            }

            if (this.stage == 2)
            {
                // Mele
                this.AddMeleUnit(content, 360, 580, "mele");
                this.AddMeleUnit(content, 470, 545, "mele");

                this.AddMeleUnit(content, 760, 620, "mele");
                this.AddMeleUnit(content, 910, 620, "mele");

                // Range
                this.AddRangeUnit(content, 200, 660, "range");
                this.AddRangeUnit(content, 965, 670, "range");

                // Boss
                this.AddBoss(content, 830, 180, "boss");
            }
        }

        private void LoadHero(ContentManager content)
        {
            switch (ChooseHeroScreen.HeroName)
            {
                case "ODIN":
                    {
                        // Singleton                                         
                        this.hero = Hero.Instance(new Position(this.room.Width / 2, this.room.Height / 2), 2, 900, 110, 70, 200, 800, SkillType.Defence, 5, 100);
                        break;
                    }

                case "THOR":
                    {
                        // Singleton
                        this.hero = Hero.Instance(new Position(this.room.Width / 2, this.room.Height / 2), 1.5f, 1100, 130, 90, 90, 600, SkillType.Rage, 3, 100);
                        break;
                    }

                case "EIR":
                    {
                        // Singleton
                        this.hero = Hero.Instance(new Position(this.room.Width / 2, this.room.Height / 2), 3, 750, 90, 60, 150, 1000, SkillType.Heal, 50, 100);
                        break;
                    }

                default:
                    break;
            }

            this.currentMaxHp = this.hero.MaxHP;
            this.currentMaxMp = this.hero.MaxMP;
            this.currentSpeed = this.hero.Speed;
            this.currentRange = this.hero.Range;
            this.currentAttack = this.hero.Attack;
            this.currentDefence = this.hero.Defence;

            this.hero.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}FrontRight", ChooseHeroScreen.HeroName));
            this.hero.Area = new Rectangle(0, 0, this.hero.SpriteIndex.Width, this.hero.SpriteIndex.Height);
            this.hero.Position = new Position(50, 400);
            this.hero.Alive = true;
            this.units.Add(this.hero);
        }

        private void LoadLevel(ContentManager content)
        {
            this.gameWindowTexture = content.Load<Texture2D>(@"Textures\GameScreens\Level1");

            this.room = new Rectangle(0, 0, this.gameWindowTexture.Width, this.gameWindowTexture.Height);
        }

        private void LoadMusic(ContentManager content)
        {
            this.gunShot = content.Load<SoundEffect>(string.Format(@"Textures\Sounds\{0}Shot", ChooseHeroScreen.HeroName));
            this.walk = content.Load<SoundEffect>(@"Textures\Sounds\pl_dirt1");
            this.walk2 = content.Load<SoundEffect>(@"Textures\Sounds\pl_dirt2");
            this.pain1 = content.Load<SoundEffect>(@"Textures\Sounds\pain1");
            this.pain2 = content.Load<SoundEffect>(@"Textures\Sounds\pain2");
            this.gameSong = content.Load<SoundEffect>(@"Textures\Sounds\gameSong");
            this.walkInstance = this.walk.CreateInstance();
            this.walkInstance.IsLooped = false;
            this.walkInstance.Volume = 0.1f;
            this.walkInstance2 = this.walk2.CreateInstance();
            this.walkInstance2.IsLooped = false;
            this.walkInstance2.Volume = 0.1f;
            this.gunShotInstance = this.gunShot.CreateInstance();
            this.gunShotInstance.IsLooped = false;
            this.gunShotInstance.Volume = 0.1f;
            this.pain1Instance = this.pain1.CreateInstance();
            this.pain1Instance.IsLooped = false;
            this.pain1Instance.Volume = 0.2f;
            this.pain2Instance = this.pain2.CreateInstance();
            this.pain2Instance.IsLooped = false;
            this.pain2Instance.Volume = 0.2f;
            this.gameSongInstance = this.gameSong.CreateInstance();
            this.gameSongInstance.IsLooped = false;
            this.gameSongInstance.Volume = 0.2f;
        }

        private void LoadBonusses(ContentManager content)
        {
            this.bonusHPTexture = content.Load<Texture2D>(@"Textures\Objects\HealthPotion");
            this.bonusMPTexture = content.Load<Texture2D>(@"Textures\Objects\ManaPotion");
        }

        private void LoadLevelUp(ContentManager content)
        {
            this.levelUpTexture = content.Load<Texture2D>(@"Textures\GameScreens\LevelUp");
            this.levelUpRect = new Rectangle(0, 0, this.levelUpTexture.Width, this.levelUpTexture.Height);
            this.leftButton = content.Load<Texture2D>(@"Textures\GameScreens\LevelUpLeft");
            this.rightButton = content.Load<Texture2D>(@"Textures\GameScreens\LevelUpRight");
            this.okButton = content.Load<Texture2D>(@"Textures\GameScreens\Button");
        }

        private void DrawCursor(SpriteBatch spriteBatch)
        {
            if (this.levelUp || this.paused)
            {
                Vector2 cursPos = new Vector2(this.cursorMenu.Position.X, this.cursorMenu.Position.Y);
                spriteBatch.Draw(this.cursorMenu.SpriteIndex, cursPos, Color.White);
            }
            else
            {
                Vector2 cursPos = new Vector2(this.cursor.Position.X, this.cursor.Position.Y);
                spriteBatch.Draw(this.cursor.SpriteIndex, cursPos, Color.White);
            }
        }

        private void DrawBonusses(SpriteBatch spriteBatch)
        {
            foreach (var bonus in this.bonuses)
            {
                if (bonus.Alive)
                {
                    Vector2 bonusPos = new Vector2(bonus.Position.X, bonus.Position.Y);
                    spriteBatch.Draw(bonus.SpriteIndex, bonusPos, Color.White);
                }
            }
        }

        private void DrawBullets(SpriteBatch spriteBatch)
        {
            this.bulletShooted = true;
            foreach (var bullet in this.bullets)
            {
                if (bullet.Alive)
                {
                    bullet.Rotation = this.PointDirecions(this.hero.Position.X, this.hero.Position.Y, this.mouse.X, this.mouse.Y);

                    //this.ObjectDraw(bullet.SpriteIndex, bullet.Position, null, Color.White, MathHelper.ToRadians(bullet.Rotation), center, scale, SpriteEffects.None, 0));
                    this.ObjectDraw(spriteBatch, bullet.SpriteIndex, new Vector2(bullet.Position.X, bullet.Position.Y), bullet.Rotation);
                }
            }

            foreach (var bullet in this.enemyBullets)
            {
                if (bullet.Alive && this.hero.Alive)
                {
                    this.ObjectDraw(spriteBatch, bullet.SpriteIndex, new Vector2(bullet.Position.X, bullet.Position.Y), bullet.Rotation);
                }
            }
        }

        private void DrawObstacles(SpriteBatch spriteBatch)
        {
            foreach (var obstacles in this.obstacles)
            {
                this.ObjectDraw(spriteBatch, obstacles.SpriteIndex, new Vector2(obstacles.Position.X, obstacles.Position.Y), obstacles.Rotation);
            }
        }

        private void DrawHeroStats(SpriteBatch spriteBatch, ContentManager content)
        {

            // Health
            float visibleWidth = ((float)this.heroHealthBar.Width * this.hero.Health) / this.hero.MaxHP;

            Rectangle healthRectangle = new Rectangle(12, 10,
                (int)visibleWidth,
                this.heroHealthBar.Height);

            spriteBatch.Draw(this.heroHealthBar, healthRectangle, Color.White);

            SpriteFont font = content.Load<SpriteFont>(@"Fonts/Text");
            Vector2 position = new Vector2(170, 27);
            spriteBatch.DrawString(font, string.Format("{0}", (int)this.hero.Health), position, Color.White);

            // Mana
            visibleWidth = ((float)this.heroManaBar.Width * this.hero.Mana) / this.hero.MaxMP;

            Rectangle manaRectangle = new Rectangle(12, 10,
                (int)visibleWidth,
                this.heroManaBar.Height);

            spriteBatch.Draw(this.heroManaBar, manaRectangle, Color.White);

            position = new Vector2(130, 62);
            spriteBatch.DrawString(font, string.Format("{0}", (int)this.hero.Mana), position, Color.White);

            position = new Vector2(10, 10);
            spriteBatch.Draw(this.heroBarHolder, position, Color.White);

            // Experience
            visibleWidth = ((float)this.heroExpBar.Width * this.hero.CurrentExp) / (this.hero.Level * 500);

            Rectangle expRectangle = new Rectangle(994 - (int)visibleWidth, 23,
                (int)visibleWidth,
                this.heroExpBar.Height);

            spriteBatch.Draw(this.heroExpBar, expRectangle, Color.White);

            position = new Vector2(840, 27);
            spriteBatch.DrawString(font, string.Format("{0}", (int)this.hero.CurrentExp), position, Color.White);

            // Level
            position = new Vector2(840, 62);
            spriteBatch.DrawString(font, string.Format("Level {0}", this.hero.Level), position, Color.White);

            position = new Vector2(640, 10);
            spriteBatch.Draw(this.heroBarHolderReversed, position, Color.White);

            position = new Vector2(900, 600);
            spriteBatch.DrawString(font, this.mouse.X + " " + this.mouse.Y, position, Color.White);

            if (skillTimer != 0)
            {
                string skillname = this.hero.Skill.Type.ToString() + " On!!!";
                font = font = content.Load<SpriteFont>(@"Fonts/Title");
                Vector2 textSize = font.MeasureString(skillname);

                position = new Vector2(510 - ((float)Math.Floor(textSize.X) / 2), 20);

                spriteBatch.DrawString(font, skillname, position, Color.DeepSkyBlue);
            }
        }

        private void DrawUnits(SpriteBatch spriteBatch, ContentManager content)
        {
            foreach (var unit in this.units)
            {
                if (unit.Alive)
                {
                    if (!(unit is Hero))
                    {
                        float healthPercentage = unit.Health / 250;
                        float visibleWidth = (float)this.enemyHealthBars.Width * healthPercentage;

                        Rectangle healthRectangle = new Rectangle(
                            (int)unit.Position.X - ((int)visibleWidth / 2),
                            (int)unit.Position.Y - (unit.SpriteIndex.Height / 2),
                            (int)visibleWidth,
                            this.enemyHealthBars.Height);

                        spriteBatch.Draw(this.enemyHealthBars, healthRectangle, Color.Red);

                        this.ObjectDraw(spriteBatch, unit.SpriteIndex, new Vector2(unit.Position.X, unit.Position.Y), unit.Rotation);
                    }
                    else
                    {
                        // Hero Animations
                        if (0 <= hero.Rotation && hero.Rotation < 90)
                        {
                            this.hero.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}FrontRight", ChooseHeroScreen.HeroName));
                        }
                        else if (90 <= hero.Rotation && hero.Rotation < 180)
                        {
                            this.hero.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}FrontLeft", ChooseHeroScreen.HeroName));
                        }
                        else if (270 <= hero.Rotation && hero.Rotation < 360)
                        {
                            this.hero.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}BackRight", ChooseHeroScreen.HeroName));
                        }
                        else if (180 <= hero.Rotation && hero.Rotation < 270)
                        {
                            this.hero.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}BackLeft", ChooseHeroScreen.HeroName));
                        }

                        this.ObjectDraw(spriteBatch, this.hero.SpriteIndex, new Vector2(this.hero.Position.X, this.hero.Position.Y), this.hero.Rotation);
                    }
                }
            }
        }

        private void DrawBackGround(SpriteBatch spriteBatch)
        {
            this.gameWindowTexturePos = new Vector2(0, 0);
            spriteBatch.Draw(this.gameWindowTexture, this.gameWindowTexturePos, Color.White);
        }

        private void DrawPaused(SpriteBatch spriteBatch, ContentManager content)
        {
            SpriteFont font = content.Load<SpriteFont>(@"Fonts/Title");
            string itemText = "Pause";

            Vector2 textSize = font.MeasureString(itemText);
            Vector2 position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 300);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            font = content.Load<SpriteFont>(@"Fonts/Text");

            itemText = "Hero Attack : " + this.hero.Attack;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 350);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            itemText = "Hero Defence : " + this.hero.Defence;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 400);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            itemText = "Hero Speed : " + this.hero.Speed;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 450);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            itemText = "Hero Range : " + this.hero.Range;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 500);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);
        }

        private void DrawLevelUp(SpriteBatch spriteBatch, ContentManager content)
        {
            spriteBatch.Draw(this.levelUpTexture, this.levelUpRect, Color.White);

            SpriteFont font = content.Load<SpriteFont>(@"Fonts/Title");

            string itemText = "Level Up!!!";
            Vector2 textSize = font.MeasureString(itemText);
            Vector2 position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 30);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            font = content.Load<SpriteFont>(@"Fonts/Text");

            itemText = this.points + " points to spent";
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 100);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            // Health
            position = new Vector2(360, 150);
            spriteBatch.Draw(this.leftButton, position, Color.White);
            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.leftButton, position, "Health-", font, false));
            }

            position = new Vector2(605, 150);
            spriteBatch.Draw(this.rightButton, position, Color.White);
            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.rightButton, position, "Health+", font, false));
            }

            itemText = "Health: " + this.hero.MaxHP;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 170);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            // Mana
            position = new Vector2(360, 210);
            spriteBatch.Draw(this.leftButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.leftButton, position, "Mana-", font, false));
            }

            position = new Vector2(605, 210);
            spriteBatch.Draw(this.rightButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.rightButton, position, "Mana+", font, false));
            }

            itemText = "Mana: " + this.hero.MaxMP;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 230);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            // Attack
            position = new Vector2(360, 270);
            spriteBatch.Draw(this.leftButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.leftButton, position, "Attack-", font, false));
            }

            position = new Vector2(605, 270);
            spriteBatch.Draw(this.rightButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.rightButton, position, "Attack+", font, false));
            }

            itemText = "Attack: " + this.hero.Attack;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 290);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            // Defence
            position = new Vector2(360, 330);
            spriteBatch.Draw(this.leftButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.leftButton, position, "Defence-", font, false));
            }

            position = new Vector2(605, 330);
            spriteBatch.Draw(this.rightButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.rightButton, position, "Defence+", font, false));
            }

            itemText = "Defence: " + this.hero.Defence;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 350);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            // Speed
            position = new Vector2(360, 390);
            spriteBatch.Draw(this.leftButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.leftButton, position, "Speed-", font, false));
            }

            position = new Vector2(605, 390);
            spriteBatch.Draw(this.rightButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.rightButton, position, "Speed+", font, false));
            }

            itemText = "Speed: " + this.hero.Speed;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 410);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            // Range
            position = new Vector2(360, 450);
            spriteBatch.Draw(this.leftButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.leftButton, position, "Range-", font, false));
            }

            position = new Vector2(605, 450);
            spriteBatch.Draw(this.rightButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.rightButton, position, "Range+", font, false));
            }

            itemText = "Range: " + this.hero.Range;
            textSize = font.MeasureString(itemText);
            position = new Vector2((float)Math.Floor((1020 - textSize.X) / 2), 470);
            spriteBatch.DrawString(font, itemText, position, Color.DeepSkyBlue);

            // OK button
            position = new Vector2(434, 530);
            spriteBatch.Draw(this.okButton, position, Color.White);

            if (this.levelUpItems.Count < 13)
            {
                this.levelUpItems.Add(new MenuItems(this.okButton, position, "Confirm", font, false));
            }

            Color color = new Color(248, 218, 127);

            itemText = "Confirm";
            textSize = font.MeasureString(itemText);
            Vector2 textPosition = position + new Vector2(
                (float)Math.Floor((this.okButton.Width - textSize.X) / 2),
                (float)Math.Floor((this.okButton.Height - textSize.Y) / 2));
            spriteBatch.DrawString(font, itemText, textPosition, color);
        }

        private void UpdateLevelUp()
        {
            this.mouse = Mouse.GetState();
            this.keyboard = Keyboard.GetState();

            if (this.previousMouse.LeftButton == ButtonState.Released && this.mouse.LeftButton == ButtonState.Pressed)
            {
                foreach (var item in this.levelUpItems)
                {
                    if (this.mouse.X > item.ItemPosition.X && this.mouse.X < item.ItemPosition.X + item.ItemTexture.Bounds.Width &&
                        this.mouse.Y > item.ItemPosition.Y && this.mouse.Y < item.ItemPosition.Y + item.ItemTexture.Bounds.Height)
                    {
                        if (item.ItemText == "Health+")
                        {
                            if (this.points > 0)
                            {
                                this.hero.MaxHP += 100;
                                this.points--;
                            }

                            this.hero.Health = this.hero.MaxHP;
                        }

                        if (item.ItemText == "Health-")
                        {
                            if ((this.hero.MaxHP - 100) >= this.currentMaxHp)
                            {
                                this.hero.MaxHP -= 100;
                                this.points++;
                            }

                            this.hero.Health = this.hero.MaxHP;
                        }

                        if (item.ItemText == "Mana+")
                        {
                            if (this.points > 0)
                            {
                                this.hero.MaxMP += 100;
                                this.points--;
                            }

                            this.hero.Mana = this.hero.MaxMP;
                        }

                        if (item.ItemText == "Mana-")
                        {
                            if ((this.hero.MaxMP - 100) >= this.currentMaxMp)
                            {
                                this.hero.MaxMP -= 100;
                                this.points++;
                            }

                            this.hero.Mana = this.hero.MaxMP;
                        }

                        if (item.ItemText == "Attack+")
                        {
                            if (this.points > 0)
                            {
                                this.hero.Attack += 10;
                                this.points--;
                            }
                        }

                        if (item.ItemText == "Attack-")
                        {
                            if ((this.hero.Attack - 10) >= this.currentAttack)
                            {
                                this.hero.Attack -= 10;
                                this.points++;
                            }
                        }

                        if (item.ItemText == "Defence+")
                        {
                            if (this.points > 0)
                            {
                                this.hero.Defence += 10;
                                this.points--;
                            }
                        }

                        if (item.ItemText == "Defence-")
                        {
                            if ((this.hero.Defence - 10) >= this.currentDefence)
                            {
                                this.hero.Defence -= 10;
                                this.points++;
                            }
                        }

                        if (item.ItemText == "Range+")
                        {
                            if (this.points > 0)
                            {
                                this.hero.Range += 10;
                                this.points--;
                            }
                        }

                        if (item.ItemText == "Range-")
                        {
                            if ((this.hero.Range - 10) >= this.currentRange)
                            {
                                this.hero.Range -= 10;
                                this.points++;
                            }
                        }

                        if (item.ItemText == "Speed+")
                        {
                            if (this.points > 0)
                            {
                                this.hero.Speed += 0.5f;
                                this.points--;
                            }
                        }

                        if (item.ItemText == "Speed-")
                        {
                            if ((this.hero.Speed - 0.5f) >= this.currentSpeed)
                            {
                                this.hero.Speed -= 0.5f;
                                this.points++;
                            }
                        }

                        if (item.ItemText == "Confirm" && this.points == 0)
                        {
                            this.currentMaxHp = this.hero.MaxHP;
                            this.currentMaxMp = this.hero.MaxMP;
                            this.currentSpeed = this.hero.Speed;
                            this.currentRange = this.hero.Range;
                            this.currentAttack = this.hero.Attack;
                            this.currentDefence = this.hero.Defence;
                            this.levelUp = false;
                        }
                    }
                }
            }

            this.previousKeyboard = this.keyboard;
            this.previousMouse = this.mouse;
        }

        private void UpdateHero(ContentManager content)
        {
            Vector2 oldPos = new Vector2(this.hero.Position.X, this.hero.Position.Y);
            int x = (int)this.hero.Position.X;
            int y = (int)this.hero.Position.Y;
            this.hero.Area = new Rectangle(x, y, this.hero.Area.Width, this.hero.Area.Height);
            this.gameSongInstance.Play();
            if (this.keyboard.IsKeyDown(Keys.W) && !this.Collision(new Vector2(0, -this.hero.Speed), this.hero))
            {
                if (oldPos.Y > this.room.Y + 20)
                {
                    this.hero.Position = new Position(oldPos.X, oldPos.Y - this.hero.Speed);
                    this.walkInstance.Play();
                    this.walkInstance2.Play();
                }
            }

            if (this.keyboard.IsKeyDown(Keys.A) && !this.Collision(new Vector2(-this.hero.Speed, 0), this.hero))
            {
                if (oldPos.X > this.room.X + 20)
                {
                    this.hero.Position = new Position(oldPos.X - this.hero.Speed, oldPos.Y);
                    this.walkInstance.Play();
                    this.walkInstance2.Play();
                }
            }

            if (this.keyboard.IsKeyDown(Keys.S) && !this.Collision(new Vector2(0, this.hero.Speed), this.hero))
            {
                if (oldPos.Y < this.room.Height - 90)
                {
                    this.hero.Position = new Position(oldPos.X, oldPos.Y + this.hero.Speed);
                    this.walkInstance.Play();
                    this.walkInstance2.Play();
                }
            }

            if (this.keyboard.IsKeyDown(Keys.D) && !this.Collision(new Vector2(this.hero.Speed, 0), this.hero))
            {
                if (oldPos.X < this.room.Width - 50)
                {
                    this.hero.Position = new Position(oldPos.X + this.hero.Speed, oldPos.Y);
                    this.walkInstance.Play();
                    this.walkInstance2.Play();
                }
            }

            if (this.keyboard.IsKeyUp(Keys.Space)
                && this.previousKeyboard.IsKeyDown(Keys.Space)
                && this.hero.Mana >= this.hero.Skill.Cost
                && skillTimer == 0)
            {
                switch (this.hero.Skill.Type)
                {
                    case SkillType.Heal:
                        {
                            if (this.hero.Health + this.hero.Skill.Power <= this.hero.MaxHP)
                            {
                                this.hero.Health += this.hero.Skill.Power;
                            }
                            else
                            {
                                this.hero.Health = this.hero.MaxHP;
                            }
                            this.hero.Mana -= this.hero.Skill.Cost;
                            break;
                        }
                    case SkillType.Rage:
                        {
                            this.hero.Attack *= this.hero.Skill.Power;
                            this.hero.Range *= this.hero.Skill.Power;
                            this.hero.Speed *= this.hero.Skill.Power;
                            this.hero.Defence /= this.hero.Skill.Power;
                            this.hero.Mana -= this.hero.Skill.Cost;
                            break;
                        }
                    case SkillType.Defence:
                        {
                            this.hero.Defence *= this.hero.Skill.Power;
                            this.hero.Mana -= this.hero.Skill.Cost;
                            break;
                        }
                    default:
                        break;
                }
                skillTimer = 1;
            }

            if (skillTimer > 0)
            {
                skillTimer++;
            }

            if (skillTimer > 300)
            {
                switch (this.hero.Skill.Type)
                {
                    case SkillType.Rage:
                        {
                            this.hero.Attack /= this.hero.Skill.Power;
                            this.hero.Range /= this.hero.Skill.Power;
                            this.hero.Speed /= this.hero.Skill.Power;
                            this.hero.Defence *= this.hero.Skill.Power;
                            break;
                        }
                    case SkillType.Defence:
                        {
                            this.hero.Defence /= this.hero.Skill.Power;
                            break;
                        }
                    default:
                        break;
                }
                skillTimer = 0;
            }
            oldPos = new Vector2(this.hero.Position.X, this.hero.Position.Y);

            this.hero.Rotation = this.PointDirecions(this.hero.Position.X, this.hero.Position.Y, this.mouse.X, this.mouse.Y);

            bool allDead = true;

            foreach (var unit in this.units)
            {
                if (unit.GetType() != typeof(Hero))
                {
                    if (unit.Alive)
                    {
                        allDead = false;
                    }
                }
            }

            if (allDead)
            {
                if (this.hero.Area.Intersects(this.exitSpot))
                {
                    if (this.stage == 1)
                    {
                        this.hero.Position = new Position(30, 480);
                        this.stage++;
                        this.LoadUnits(content);
                    }
                    else
                    {
                        Rpg.PActiveWindow = EnumActiveWindow.Win;
                        this.gameSongInstance.Stop();
                    }
                }
            }
        }

        private void UpdateBullets()
        {
            foreach (var bullet in this.bullets)
            {
                if (bullet.Alive && Math.Abs(this.hero.Position.X - bullet.Position.X) < this.hero.Range &&
                    Math.Abs(this.hero.Position.Y - bullet.Position.Y) < this.hero.Range)
                {
                    bullet.Area = new Rectangle((int)bullet.Position.X, (int)bullet.Position.Y, bullet.SpriteIndex.Width, bullet.SpriteIndex.Height);
                    Vector2 bulletPos = new Vector2(bullet.Position.X, bullet.Position.Y);
                    bulletPos += this.PushTo(bullet.Speed, bullet.Rotation, bullet);
                    bullet.Position = new Position(bulletPos.X, bulletPos.Y);
                }
                else
                {
                    bullet.Alive = false;
                }
            }

            foreach (var bullet in this.enemyBullets)
            {
                if (bullet.Alive && this.hero.Alive)
                {
                    bullet.Area = new Rectangle((int)bullet.Position.X, (int)bullet.Position.Y, bullet.SpriteIndex.Width, bullet.SpriteIndex.Height);
                    Vector2 bulletPos = new Vector2(bullet.Position.X, bullet.Position.Y);
                    bulletPos += this.PushTo(bullet.Speed, bullet.Rotation, bullet);
                    bullet.Position = new Position(bulletPos.X, bulletPos.Y);
                }
            }
        }

        private void UpdateUnits()
        {
            int n = this.rand.Next(0, 101);
            foreach (var unit in this.units)
            {
                if (unit.Alive)
                {
                    int x = (int)unit.Position.X;
                    int y = (int)unit.Position.Y;
                    unit.Area = new Rectangle(x, y, unit.Area.Width, unit.Area.Height);

                    unit.HitTimer++;
                    if (unit is IPlayer)
                    {
                        if (this.Collision(new Vector2(0, 0), unit))
                        {
                            try
                            {
                                this.hero.Health = this.hero.Health - (((int)RangedUnit.RangeAtk / this.hero.Defence) * 20) +
                                                   this.rand.Next((int)RangedUnit.RangeAtk / 10);
                                if (n < 50)
                                {
                                    this.pain1Instance.Play();
                                }
                                else
                                {
                                    this.pain2Instance.Play();
                                }
                            }
                            catch (NegativeDataException)
                            {
                                this.hero.Health = 0;
                                this.hero.Alive = false;
                                Rpg.PActiveWindow = EnumActiveWindow.GameOver;
                                break;
                            }
                        }

                        foreach (var mob in this.units.Where(creep => creep is MeleUnit))
                        {
                            Rectangle newArea = new Rectangle(mob.Area.X, mob.Area.Y, mob.Area.Width, mob.Area.Height);

                            if ((mob.HitTimer > mob.HitRate) && (((newArea.X + newArea.Width / 2) > this.hero.Area.X) &&
                                                                 (newArea.X < (this.hero.Area.X + this.hero.Area.Width)) && ((newArea.Y + newArea.Height / 2) > this.hero.Area.Y) &&
                                                                 (newArea.Y < (this.hero.Area.Y + this.hero.Area.Height))))
                            {
                                try
                                {
                                    this.hero.Health = this.hero.Health - ((mob.Attack / this.hero.Defence) * 20) +
                                                       this.rand.Next((int)mob.Attack / 10);
                                    if (n < 50)
                                    {
                                        this.pain1Instance.Play();
                                    }
                                    else
                                    {
                                        this.pain2Instance.Play();
                                    }

                                    mob.HitTimer = 0;
                                }
                                catch (NegativeDataException)
                                {
                                    this.hero.Health = 0;
                                    this.hero.Alive = false;
                                    this.gameSongInstance.Stop();
                                    Rpg.PActiveWindow = EnumActiveWindow.GameOver;
                                    break;
                                }
                            }
                        }

                        (unit as IShootable).FiringTimer++;
                        if (this.mouse.LeftButton == ButtonState.Released && this.previousMouse.LeftButton == ButtonState.Pressed)
                        {
                            (unit as IShootable).CheckShooting(this.bullets);
                            if (this.loaded)
                            {
                                if (!this.paused && !this.levelUp)
                                {
                                    this.gunShot.Play();
                                    this.hero.Mana -= 1;
                                }
                            }

                            this.loaded = true;
                        }
                    }
                    else if (unit is IMonster)
                    {
                        if (Math.Abs(this.hero.Position.X - unit.Position.X) < unit.Range &&
                            Math.Abs(this.hero.Position.Y - unit.Position.Y) < unit.Range)
                        {
                            (unit as IMonster).Active = true;
                        }
                        else
                        {
                            if (unit is RangedUnit)
                            {
                                (unit as IMonster).Active = false;
                            }
                        }

                        if (this.Collision(new Vector2(0, 0), unit))
                        {
                            try
                            {
                                (unit as IMonster).Active = true;
                                unit.Health = unit.Health - ((this.hero.Attack / unit.Defence) * 20) + this.rand.Next((int)this.hero.Attack / 10);
                            }
                            catch (NegativeDataException)
                            {
                                unit.Alive = false;

                                int bonusType = this.rand.Next(0, 3);

                                switch (bonusType % 2)
                                {
                                    case 0:
                                        {
                                            this.bonuses.Add(new Bonuses(unit.Position, this.bonusHPTexture, "hp", unit.Area));
                                            break;
                                        }

                                    case 1:
                                        {
                                            this.bonuses.Add(new Bonuses(unit.Position, this.bonusMPTexture, "mp", unit.Area));
                                            break;
                                        }
                                }

                                unit.Area = new Rectangle(-10, -10, 0, 0);

                                this.hero.CurrentExp = this.hero.CurrentExp + (unit as IMonster).ExpGiven;
                                if (this.hero.CurrentExp - (this.hero.Level * 500) > 0)
                                {
                                    this.hero.CurrentExp = this.hero.CurrentExp - (this.hero.Level * 500);
                                    this.hero.Level++;
                                    this.hero.Health = this.hero.MaxHP;
                                    this.hero.Mana = this.hero.MaxMP;
                                    this.points = 5;
                                    this.levelUp = true;
                                }
                            }
                        }

                        if (unit is IShootable)
                        {
                            (unit as IShootable).FiringTimer++;
                        }

                        if ((unit as IMonster).Active)
                        {
                            unit.Rotation = this.PointDirecions(unit.Position.X, unit.Position.Y, this.hero.Position.X, this.hero.Position.Y);

                            Vector2 unitPos = new Vector2(unit.Position.X, unit.Position.Y);
                            unitPos += this.PushTo(unit.Speed, unit.Rotation, unit);
                            unit.Position = new Position(unitPos.X, unitPos.Y);
                            if (unit is IShootable)
                            {
                                (unit as IShootable).CheckShooting(this.enemyBullets);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateCursor()
        {
            this.cursorMenu.Position = new Position(this.mouse.X, this.mouse.Y);
            this.cursor.Position = new Position(this.mouse.X - this.cursor.SpriteIndex.Width / 2, this.mouse.Y - this.cursor.SpriteIndex.Height / 2);
        }

        private void UpdateBonusses()
        {
            // .ElapsedGameTime.TotalSeconds;
            foreach (var bonus in this.bonuses)
            {
                bonus.SpawnTime--;
                if (bonus.SpawnTime == 0)
                {
                    bonus.Alive = false;
                }
            }
        }

        private void AddMeleUnit(ContentManager content, int x, int y, string textureName)
        {
            MeleUnit meleUnit = new MeleUnit(new Position(x, y), 1.8f, false, 150, 40, 260, 230, true, 150);
            meleUnit.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", textureName));
            meleUnit.Area = new Rectangle(0, 0, meleUnit.SpriteIndex.Width, meleUnit.SpriteIndex.Height);
            this.units.Add(meleUnit);
        }

        private void AddRangeUnit(ContentManager content, float x, float y, string textureName)
        {
            RangedUnit rangedUnit = new RangedUnit(new Position(x, y), 0, false, 400, 30, 210, 180, true, 200);
            rangedUnit.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", textureName));
            rangedUnit.Area = new Rectangle(0, 0, rangedUnit.SpriteIndex.Width, rangedUnit.SpriteIndex.Height);
            this.units.Add(rangedUnit);
        }

        private void AddBoss(ContentManager content, float x, float y, string textureName)
        {
            MeleUnit meleUnit = new MeleUnit(new Position(x, y), 1.3f, false, 200, 100, 1000, 2300, true, 100);
            meleUnit.SpriteIndex = content.Load<Texture2D>(string.Format(@"Textures\Objects\{0}", textureName));
            meleUnit.Area = new Rectangle(0, 0, meleUnit.SpriteIndex.Width, meleUnit.SpriteIndex.Height);
            this.units.Add(meleUnit);
        }

        private void ObjectDraw(SpriteBatch spriteBatch, Texture2D sprite, Vector2 position, float rotation)
        {
            Vector2 center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            float scale = 0.7f;

            if (this.bulletShooted)
            {
                spriteBatch.Draw(sprite, position, null, Color.White, MathHelper.ToRadians(rotation), center, scale, SpriteEffects.None, 0);
                this.bulletShooted = false;
            }
            else
            {
                spriteBatch.Draw(sprite, position, null, Color.White, MathHelper.ToRadians(0), center, scale, SpriteEffects.None, 0);
            }
        }

        private float PointDirecions(float x, float y, float x2, float y2)
        {
            float divX = x - x2;
            float divY = y - y2;
            float adj = divX;
            float opp = divY;
            float res = MathHelper.ToDegrees((float)Math.Atan2(opp, adj));
            res = (res - 180) % 360;
            if (res < 0)
            {
                res += 360;
            }

            return res;
        }

        private bool Collision(Vector2 pos, Obj obj)
        {
            Rectangle newArea = new Rectangle(obj.Area.X, obj.Area.Y, obj.Area.Width, obj.Area.Height);
            if (pos.X < 1)
            {
                newArea.X += (int)(pos.X * 3);
            }
            else
            {
                newArea.X += (int)pos.X;
            }

            if (pos.Y < 1)
            {
                newArea.Y += (int)(pos.Y * 3);
            }
            else
            {
                newArea.Y += (int)pos.Y;
            }

            foreach (var o in this.bonuses)
            {
                if (obj.GetType() == typeof(Hero))
                {
                    if ((newArea.X + pos.X + newArea.Width / 2) > o.Area.X && newArea.X < (o.Area.X + o.Area.Width) &&
                        (newArea.Y + pos.Y + newArea.Height / 2) > o.Area.Y && newArea.Y < (o.Area.Y + o.Area.Height))
                    {
                        if (o.Alive && o.Type == "hp")
                        {
                            if (((obj as Hero).Health + 50) < (obj as Hero).MaxHP)
                            {
                                (obj as Hero).Health += 50;
                            }
                            else
                            {
                                (obj as Hero).Health = (obj as Hero).MaxHP;
                            }

                            o.Alive = false;
                        }
                        else if (o.Alive && o.Type == "mp")
                        {
                            if (((obj as Hero).Mana + 50) < (obj as Hero).MaxMP)
                            {
                                (obj as Hero).Mana += 50;
                            }
                            else
                            {
                                (obj as Hero).Mana = (obj as Hero).MaxMP;
                            }

                            o.Alive = false;
                        }
                    }
                }
            }

            foreach (var o in this.obstacles)
            {
                if (obj.GetType() == typeof(Bullet))
                {
                    if (o.Visible)
                    {
                        if ((newArea.X + pos.X + newArea.Width / 2) > o.Area.X && newArea.X < (o.Area.X + o.Area.Width) &&
                            (newArea.Y + pos.Y + newArea.Height / 2) > o.Area.Y && newArea.Y < (o.Area.Y + o.Area.Height))
                        {
                            obj.Alive = false;
                            return true;
                        }
                    }
                }
                else
                {
                    if ((newArea.X + pos.X + newArea.Width / 2) > o.Area.X && newArea.X < (o.Area.X + o.Area.Width) &&
                        (newArea.Y + pos.Y + newArea.Height / 2) > o.Area.Y && newArea.Y < (o.Area.Y + o.Area.Height))
                    {
                        return true;
                    }
                }
            }

            if (obj.GetType() == typeof(MeleUnit) || obj.GetType() == typeof(RangedUnit))
            {
                foreach (var o in this.bullets)
                {
                    if (o.Alive)
                    {
                        if ((newArea.X + pos.X + newArea.Width / 2) > o.Area.X && newArea.X < (o.Area.X + o.Area.Width) &&
                            (newArea.Y + pos.Y + newArea.Height / 2) > o.Area.Y && newArea.Y < (o.Area.Y + o.Area.Height))
                        {
                            o.Alive = false;
                            return true;
                        }
                    }
                }
            }

            if (obj.GetType() == typeof(Hero))
            {
                foreach (var o in this.enemyBullets)
                {
                    if (o.Alive)
                    {
                        if ((newArea.X + pos.X + newArea.Width / 2) > o.Area.X && newArea.X < (o.Area.X + o.Area.Width) &&
                            (newArea.Y + pos.Y + newArea.Height / 2) > o.Area.Y && newArea.Y < (o.Area.Y + o.Area.Height))
                        {
                            o.Alive = false;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private Vector2 PushTo(float pix, float dir, Obj unit)
        {
            float newX = (float)Math.Cos(MathHelper.ToRadians(dir));
            float newY = (float)Math.Sin(MathHelper.ToRadians(dir));

            if (!this.Collision(new Vector2(newX, newY), unit))
            {
                return new Vector2(pix * newX, pix * newY);
            }

            return new Vector2(0, 0);
        }
    }
}
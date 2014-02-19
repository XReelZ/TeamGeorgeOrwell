﻿namespace Rpg.Objects
{
    using Interfaces;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    public class RangedUnit : Units, IMonster, IShootable
    {
        private int firingTimer = 0;
        private float fireRate = 80;
        private float expGiven;

        public RangedUnit(Vector2 pos, float speed, bool act, float att, float def, float hp, float exp, bool alive, float range) : base(pos, speed, act, range)
        {
            this.Attack = att;
            RangeAtk = this.Attack;
            this.Defence = def;
            this.Health = hp;
            this.ExpGiven = exp;
            this.Alive = alive;
        }

        public float ExpGiven
        {
            get
            {
                return this.expGiven;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new NegativeDataException("Enemies' experience given cannot be a negative number!",(int)value);
                }
                this.expGiven = value;
            }
        }

        public static float RangeAtk { get; private set; }

        public bool Active { get; set; }

        public float FireRate
        {
            get
            {
                return this.fireRate;
            }

            protected set
            {
                if (value < 0)
                {
                    throw new NegativeDataException("The fire rate of unit cannot be a negative number!", (int)value);
                }
                this.fireRate = value;
            }
        }

        public int FiringTimer
        {
            get
            {
                return this.firingTimer;
            }

            set
            {
                if (value < 0)
                {
                    throw new NegativeDataException("The firing timer of unit cannot be a negative number!", value);
                }
                this.firingTimer = value;
            }
        }

        public void CheckShooting(IList<Bullet> bullets)
        {
            if (this.FiringTimer > this.FireRate)
            {
                this.FiringTimer = 0;
                this.Shoot(bullets);
            }
        }

        private void Shoot(IList<Bullet> bullets)
        {
            foreach (var bullet in bullets)
            {
                if (!bullet.Alive)
                {
                    bullet.Alive = true;
                    bullet.Position = this.Position;
                    bullet.Rotation = this.Rotation;
                    bullet.Speed = 5;
                    break;
                }
            }
        }
    }
}
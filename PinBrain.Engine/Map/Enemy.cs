using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Engine.Constants;
using PinBrain.Library.Map;

namespace PinBrain.Engine.Map
{
    public class Enemy : IEnemy
    {
        public EnemyTypes EnemyType { get; private set; }
        public int HP { get; private set; }
        public int Points { get; private set; }

        public Enemy(EnemyTypes type, int hitPoints, int points)
        {
            EnemyType = type;
            HP = hitPoints;
            Points = points;
        }

        public bool Hit(int points, out int remainder)
        {
            remainder = points - HP;
            if (remainder < 0)
            {
                remainder = 0; 
                return false; //not dead
            }
            else
            {
                return true; //dead
            }
        }
    }
}

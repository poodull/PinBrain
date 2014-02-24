using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Engine.Constants;
using PinBrain.Library.Map;

namespace PinBrain.Engine.Map
{
    public class Room
    {
        public Levels Level { get; private set; }
        public int Id { get; private set; }
        public int RoomUpIndex { get; private set; }
        public int RoomStraightIndex { get; private set; }
        public int RoomDownIndex { get; private set; }
        public RoomTypes RoomType { get; private set; }
        public NavigationPaths Paths
        {
            get
            {
                NavigationPaths ret = NavigationPaths.None;
                if (_enemies.Count > 0)
                    return NavigationPaths.Blocked;
                if (RoomUpIndex != -1)
                    ret |= NavigationPaths.Up;
                if (RoomDownIndex != -1)
                    ret |= NavigationPaths.Down;
                if (RoomStraightIndex != -1)
                    ret |= NavigationPaths.Straight;
                return ret;
            }
        }

        private Items _item = Items.candle;

        public Items Item
        {
            get { return _item; }
            private set { _item = value; }
        }

        private bool _isItemCollected = false;
        public bool IsItemCollected
        {
            get { return _isItemCollected; }
            set
            {
                if (value == true)
                    Item = Items.candle; //also set the item to candle as a placeholder
            }
        }

        private List<IEnemy> _enemies;

        public Room(Levels level, int id, RoomTypes type, Items item, int upRoomIndex, int straightRoomIndex, int downRoomIndex)
        {
            Level = level;
            Id = id;
            RoomType = type;
            Item = item;
            IsItemCollected = false;
            RoomUpIndex = upRoomIndex;
            RoomStraightIndex = straightRoomIndex;
            RoomDownIndex = downRoomIndex;

            _enemies = new List<IEnemy>();
        }

        public Room Clone()
        {
            Room clone = new Room(Level, Id, RoomType, Item, RoomUpIndex, RoomStraightIndex, RoomDownIndex);
            foreach (IEnemy e in _enemies)
            {
                clone.AddEnemy(e.EnemyType, e.HP, e.Points, 1);
            }
            return clone;
        }

        public void AddEnemy(EnemyTypes type, int hitPoints, int points, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _enemies.Add(new Enemy(type, hitPoints, points));
            }
        }

        internal void RandomizeEnemyOrder()
        {
            List<IEnemy> rlist = new List<IEnemy>();
            Random rnd = new Random();
            while (_enemies.Count > 0)
            {
                int index = rnd.Next(0, _enemies.Count);
                rlist.Add(_enemies[index]);
                _enemies.RemoveAt(index);
            }
            _enemies = rlist;
        }

        /// <summary>
        /// Hits the enemies in this room for x damage.  Returns list of killed enemies.
        /// </summary>
        /// <param name="hitPoints"></param>
        /// <returns></returns>
        public List<IEnemy> DamageEnemy(int damage, bool isSplash = false)
        {
            List<IEnemy> dead = new List<IEnemy>();
            int splashDamage = 0;
            do
            {
                if (_enemies.Count == 0)
                    continue;
                IEnemy target = _enemies.OrderBy(val => val.HP).First(); //weakest character first (boss?)
                if (target == null)
                    break;
                if (target.Hit(damage, out splashDamage))
                {
                    dead.Add(target); //add the dead to our tally.
                    _enemies.Remove(target); //remove the dead enemy
                    damage = splashDamage; //carry over the extra carnage
                }
            } while (isSplash && splashDamage > 0);
            return dead;
        }

        internal List<IEnemy> GetEnemies()
        {
            return _enemies; //should copy this out instead
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Engine.Map;
using PinBrain.Library.Map;

namespace PinBrain.Engine.Managers
{
    public class RoomFactory
    {
        private static RoomFactory _instance = new RoomFactory();

        readonly Dictionary<int, Dictionary<int, Room>> _map = new Dictionary<int, Dictionary<int, Room>>();

        private RoomFactory()
        {
            initializeMaze();
        }

        private void initializeMaze()
        {
#if !DEBUG
            getBossDebugMaze();
            return;
#else
            //TODO: load maze from file
            for (int levelIndex = 0; levelIndex < 6; levelIndex++)
            {
                _map.Add(levelIndex, new Dictionary<int, Room>());

                //Level 1 Rooms
                _map[levelIndex].Add(0, new Room(Levels.Ballroom, 0, RoomTypes.Start, Items.weaponup, -1, 1, -1));
                _map[levelIndex].Add(1, new Room(Levels.Ballroom, 1, RoomTypes.Normal, Items.magicup, 2, -1, -1));
                _map[levelIndex].Add(2, new Room(Levels.Ballroom, 2, RoomTypes.Normal, Items.weaponup, -1, 3, -1));
                _map[levelIndex].Add(3, new Room(Levels.Ballroom, 3, RoomTypes.Normal, Items.bonusx, 4, 5, -1));
                _map[levelIndex].Add(4, new Room(Levels.Ballroom, 4, RoomTypes.Normal, Items.balllock, -1, 5, -1));
                _map[levelIndex].Add(5, new Room(Levels.Ballroom, 5, RoomTypes.Normal, Items.magicup, -1, 6, -1));
                _map[levelIndex].Add(6, new Room(Levels.Ballroom, 6, RoomTypes.Normal, Items.heart, -1, -1, 7));
                _map[levelIndex].Add(7, new Room(Levels.Ballroom, 7, RoomTypes.Normal, Items.candle, -1, 10, 8));
                _map[levelIndex].Add(8, new Room(Levels.Ballroom, 8, RoomTypes.Normal, Items.balllock, -1, 9, -1));
                _map[levelIndex].Add(9, new Room(Levels.Ballroom, 9, RoomTypes.Normal, Items.bonusx, 10, -1, -1));
                _map[levelIndex].Add(10, new Room(Levels.Ballroom, 10, RoomTypes.Normal, Items.weaponup, -1, 11, -1));
                _map[levelIndex].Add(11, new Room(Levels.Ballroom, 11, RoomTypes.Boss, Items.heart, -1, 12, -1));
                _map[levelIndex].Add(12, new Room(Levels.Ballroom, 12, RoomTypes.Trophy, Items.candle, -1, -1, -1));
                //Level 1 Enemies
                _map[levelIndex][0].AddEnemy(EnemyTypes.zombie, 10, 500, 3);
                //_map[levelIndex][0].AddEnemy(EnemyTypes.zombieB, 10, 555, 1); //DEBUG
                _map[levelIndex][0].AddEnemy(EnemyTypes.fishmen, 10, 5000, 2);//DEBUG
                //_map[levelIndex][0].AddEnemy(EnemyTypes.fishmenB, 10, 1000, 1);//DEBUG
                _map[levelIndex][0].AddEnemy(EnemyTypes.medusahead, 10, 100000, 2);//DEBUG
                //_map[levelIndex][0].AddEnemy(EnemyTypes.medusaheadB, 10, 50000, 1);//DEBUG
                //randomize enemies
                _map[levelIndex][0].RandomizeEnemyOrder();

                _map[levelIndex][1].AddEnemy(EnemyTypes.zombie, 10, 500, 3);
                _map[levelIndex][2].AddEnemy(EnemyTypes.zombie, 10, 500, 3);
                _map[levelIndex][3].AddEnemy(EnemyTypes.zombie, 10, 500, 3);
                _map[levelIndex][4].AddEnemy(EnemyTypes.zombie, 10, 500, 3);
                _map[levelIndex][5].AddEnemy(EnemyTypes.zombie, 10, 500, 3);
                _map[levelIndex][6].AddEnemy(EnemyTypes.zombie, 10, 500, 3);
                _map[levelIndex][7].AddEnemy(EnemyTypes.zombie, 10, 500, 3);
                _map[levelIndex][8].AddEnemy(EnemyTypes.zombie, 20, 1000, 6);
                _map[levelIndex][9].AddEnemy(EnemyTypes.zombie, 20, 1000, 6);
                _map[levelIndex][10].AddEnemy(EnemyTypes.zombie, 20, 1000, 6);
                _map[levelIndex][11].AddEnemy(EnemyTypes.zombieB, 200, 1000, 50);
            }
#endif
        }

        internal static Room GetRoom(Levels level, int index)
        {
            return _instance._map[(int)level][index].Clone();
        }

        internal static Room Restart(Room room)
        {
            //do we restart the room, or level?
            //if we restart the room, do we restart with all the enemies or keep the dead?
            return GetRoom(room.Level, room.Id);
        }

        private void getBossDebugMaze()
        {
            //TODO: load maze from file
            for (int levelIndex = 0; levelIndex < 6; levelIndex++)
            {
                _map.Add(levelIndex, new Dictionary<int, Room>());

                //Level 1 Rooms
                _map[levelIndex].Add(0, new Room(Levels.Ballroom, 0, RoomTypes.Start, Items.weaponup, -1, 11, -1)); 
                //zoom to room 11 (boss room)
                _map[levelIndex].Add(11, new Room(Levels.Ballroom, 11, RoomTypes.Boss, Items.heart, -1, 12, -1));
                _map[levelIndex].Add(12, new Room(Levels.Ballroom, 12, RoomTypes.Trophy, Items.candle, -1, -1, -1));
                //Level 1 Enemies
                _map[levelIndex][0].AddEnemy(EnemyTypes.zombie, 10, 500, 1);
                _map[levelIndex][11].AddEnemy(EnemyTypes.zombieB, 200, 1000, 1);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Map
{
    public interface IEnemy
    {
        EnemyTypes EnemyType { get; }
        int HP { get; }
        int Points { get; }
        bool Hit(int points, out int remainder);
    }
}

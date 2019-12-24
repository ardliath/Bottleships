using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.Communication
{
    public interface ICommander
    {
        string GetName();
        IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes);
        IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots);
        void RespondToShots(IEnumerable<ShotResult> results);
        void StartGame(GameStartNotification gameStartNotification);
    }
}

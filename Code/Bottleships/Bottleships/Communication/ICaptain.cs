using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.Communication
{
    public interface ICaptain
    {
        string GetName();
        IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes);
        IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots);
        void RespondToShots(IEnumerable<ShotResult> results);
        void StartGameNotification(GameStartNotification gameStartNotification);
        void EndGameNotification(GameEndNotification gameEndNotification);
        void NotifyOfBeingHit(IEnumerable<HitNotification> hits);
        void EndRoundNotification(RoundEndNotification roundEndNotification);
    }
}

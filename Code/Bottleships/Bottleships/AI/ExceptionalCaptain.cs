using Bottleships.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.AI
{
    /// <summary>
    /// This captain throws an error at everything - used for testing
    /// </summary>
    public class ExceptionalCaptain : ICaptain
    {
        public void EndGameNotification(GameEndNotification gameEndNotification)
        {
            throw new NotImplementedException();
        }

        public void EndRoundNotification(RoundEndNotification roundEndNotification)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots)
        {
            throw new NotImplementedException();
        }

        public void NotifyOfBeingHit(IEnumerable<HitNotification> hits)
        {
            throw new NotImplementedException();
        }

        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            throw new NotImplementedException();
        }

        public void StartGameNotification(GameStartNotification gameStartNotification)
        {
            throw new NotImplementedException();
        }
    }
}

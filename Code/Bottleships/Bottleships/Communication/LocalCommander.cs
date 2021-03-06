﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bottleships.Logic;

namespace Bottleships.Communication
{
    public class LocalCommander : ICommander
    {
        public LocalCommander(ICaptain captain)
        {
            Captain = captain;
        }

        public ICaptain Captain { get; private set; }

        public void EndGame(GameEndNotification gameEndNotification)
        {
            this.Captain.EndGameNotification(gameEndNotification);
        }

        public void EndRound(RoundEndNotification roundEndNotification)
        {
            this.Captain.EndRoundNotification(roundEndNotification);
        }

        public string GetName()
        {
            return Captain.GetName();
        }

        public IEnumerable<Placement> GetPlacements(IEnumerable<Clazz> classes)
        {
            return Captain.GetPlacements(classes);
        }

        public IEnumerable<Shot> GetShots(IEnumerable<EnemyFleetInfo> enemyFleetInfo, int numberOfShots)
        {            
            return Captain.GetShots(enemyFleetInfo, numberOfShots);
        }

        public void NotifyOfBeingHit(IEnumerable<HitNotification> hits)
        {
            this.Captain.NotifyOfBeingHit(hits);
        }

        public void RespondToShots(IEnumerable<ShotResult> results)
        {
            Captain.RespondToShots(results);
        }

        public void StartGame(GameStartNotification gameStartNotification)
        {
            Captain.StartGameNotification(gameStartNotification);
        }
    }
}

using Bottleships.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bottleships
{
    public class GameViewInformation
    {
        protected int? FleetDisplayIndex { get; set; }

        protected int? FleetShootingDisplayIndex { get; set; }

        public ViewPhase ViewPhase { get; set; }

        protected Queue<int> FleetsToShowShotsAt { get; set; }
        public Game Game { get; private set; }

        public int ShotImpactScreensToShow
        {
            get
            {
                return FleetsToShowShotsAt == null
                    ? 0
                    : FleetsToShowShotsAt.Count;
            }
        }

        public GameViewInformation(Game game)
        {
            this.Game = game;
            this.ViewPhase = ViewPhase.Aiming;
        }

        public void ScrollLeft()
        {
            if (FleetDisplayIndex > 0)
            {
                FleetDisplayIndex--;
            }            
        }

        public void ScrollRight()
        {
            if (FleetDisplayIndex + 2 <= Game.Fleets.Count())
            {
                FleetDisplayIndex++;
            }
        }
        

        public Fleet GetFleetToView()
        {
            var fleetIndex = FleetShootingDisplayIndex ?? FleetDisplayIndex ?? 0;
            var fleet = this.Game.Fleets.ElementAt(fleetIndex);

            return fleet;
        }

        public void StartShowingExplosions(IEnumerable<int> fleetsBeingShotAt)
        {
            this.ViewPhase = ViewPhase.Exploding;
            this.FleetsToShowShotsAt = new Queue<int>(fleetsBeingShotAt);
        }

        public bool MoveOntoNextExplosion()
        {
            if (this.FleetsToShowShotsAt == null || this.FleetsToShowShotsAt.Count == 0)
            {
                this.ViewPhase = ViewPhase.Aiming;
                return false;
            }
            else
            {
                FleetShootingDisplayIndex = this.FleetsToShowShotsAt.Dequeue();
                return true;
            }
        }
    }
}
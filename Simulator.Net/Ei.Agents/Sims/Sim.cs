using Ei.Agents.Core;
using Ei.Agents.Core.Behaviours;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ei.Agents.Sims
{
    public class Sim : EiBehaviour, IUpdates
    {

        [Category("Information")]
        public float Happiness { get; private set; }

        HungerModifier hunger = new HungerModifier();
        EnergyModifier energy = new EnergyModifier();
        ThirstModifier thirst = new ThirstModifier();
        ComfortModifier comfort = new ComfortModifier();
        FunModifier fun = new FunModifier();
        HygieneModifier hygiene = new HygieneModifier();
        SocialModifier social = new SocialModifier();
        RoomModifier room = new RoomModifier();

        [Category("Modifiers")]
        public float Hunger { get { return this.hunger.XValue; } }
        [Category("Modifiers")]
        public float Energy { get { return this.energy.XValue; } }
        [Category("Modifiers")]
        public float Thirst { get { return this.thirst.XValue; } }
        [Category("Modifiers")]
        public float Comfort { get { return this.comfort.XValue; } }
        [Category("Modifiers")]
        public float Fun { get { return this.fun.XValue; } }
        [Category("Modifiers")]
        public float Hygiene { get { return this.hygiene.XValue; } }
        [Category("Modifiers")]
        public float Social { get { return this.social.XValue; } }
        [Category("Modifiers")]
        public float Room { get { return this.room.XValue; } }

        public bool IsDead { get; set; }

        public Modifier[] modifiers { get; private set; }

        public Sim() {
            modifiers = new Modifier[] {
                hunger, energy, thirst, comfort, fun, hygiene, social, room
            };
        }

        public void Update() {
            // Debug.WriteLine("Updating: " + this.gameObject.name);
            var previous = this.hygiene.XValue;

            // update all modifiers
            this.hunger.Update(Time.deltaTime);
            this.energy.Update(Time.deltaTime);
            this.thirst.Update(Time.deltaTime);
            this.comfort.Update(Time.deltaTime);
            this.fun.Update(Time.deltaTime);
            this.hygiene.Update(Time.deltaTime);
            this.social.Update(Time.deltaTime);
            this.room.Update(Time.deltaTime);

            // Debug.WriteLine("Hygiene: " + this.hygiene.XValue + " : " + (previous - this.hygiene.XValue));
            // Debug.WriteLine(this.energy.discomfort);

            this.OnPropertyChanged("Hunger");
            this.OnPropertyChanged("Energy");
            this.OnPropertyChanged("Thirst");
            this.OnPropertyChanged("Comfort");
            this.OnPropertyChanged("Fun");
            this.OnPropertyChanged("Hygiene");
            this.OnPropertyChanged("Social");
            this.OnPropertyChanged("Room");

            this.Happiness = CalculateHappiness(this.hunger.discomfort, this.energy.discomfort, this.thirst.discomfort, this.comfort.discomfort, this.fun.discomfort, this.hygiene.discomfort, this.social.discomfort, this.room.discomfort);

            this.OnPropertyChanged("Happiness");
        }

        public void UpdateModifiers(SimAction action) {
            foreach (var modifier in action.Modifiers) {
                switch (modifier.Type) {
                    case ModifierType.Bladder:
                        this.thirst.XValue += modifier.Delta;
                        break;
                    case ModifierType.Comfort:
                        this.comfort.XValue += modifier.Delta;
                        break;
                    case ModifierType.Energy:
                        this.energy.XValue += modifier.Delta;
                        break;
                    case ModifierType.Fun:
                        this.fun.XValue += modifier.Delta;
                        break;
                    case ModifierType.Hunger:
                        this.hunger.XValue += modifier.Delta;
                        break;
                    case ModifierType.Hygiene:
                        this.hygiene.XValue += modifier.Delta;
                        break;
                    case ModifierType.Room:
                        this.room.XValue += modifier.Delta;
                        break;
                    case ModifierType.Social:
                        this.social.XValue += modifier.Delta;
                        break;
                }
            }
        }

        private float CalculateHappiness(float hunger, float energy, float thirst, float comfort, float fun, float hygiene, float social, float room) {
            var happiness = hunger + energy + thirst + comfort + fun + hygiene + social + room;
            happiness = 800 - happiness;
            return (happiness / 800) * 100;
        }

        private float UpdateLevel(Modifier modifier, float delta) {
            return modifier.CalculateY(Mathf.Clamp(modifier.XValue + delta, -100, 100));
        }

        public float CheckDelta(SimAction action) {
            var thirstValue = this.thirst.discomfort;
            var comfortValue = this.comfort.discomfort;
            var energyValue = this.energy.discomfort;
            var funValue = this.fun.discomfort;
            var hungerValue = this.hunger.discomfort;
            var hygieneValue = this.hygiene.discomfort;
            var roomValue = this.room.discomfort;
            var socialValue = this.social.discomfort;

            foreach (var modifier in action.Modifiers) {
                switch (modifier.Type) {
                    case ModifierType.Bladder:
                        thirstValue = this.UpdateLevel(this.thirst, modifier.Delta);
                        break;
                    case ModifierType.Comfort:
                        comfortValue = this.UpdateLevel(this.comfort, modifier.Delta);
                        break;
                    case ModifierType.Energy:
                        energyValue = this.UpdateLevel(this.energy, modifier.Delta);
                        break;
                    case ModifierType.Fun:
                        funValue = this.UpdateLevel(this.fun, modifier.Delta);
                        break;
                    case ModifierType.Hunger:
                        hungerValue = this.UpdateLevel(this.hunger, modifier.Delta);
                        break;
                    case ModifierType.Hygiene:
                        hygieneValue = this.UpdateLevel(this.hygiene, modifier.Delta);
                        break;
                    case ModifierType.Room:
                        roomValue = this.UpdateLevel(this.room, modifier.Delta);
                        break;
                    case ModifierType.Social:
                        socialValue = this.UpdateLevel(this.social, modifier.Delta);
                        break;
                }
            }

            return this.CalculateHappiness(hungerValue, energyValue, thirstValue, comfortValue, funValue, hygieneValue, socialValue, roomValue);
        }


        //public void Update() {
        //    foreach (var modifier in this.modifiers) {
        //        modifier.Update(Time.deltaTime);
        //    }

        //    this.Happiness = hunger + energy + thirst + comfort + fun + hygiene + social + room;
        //    this.Happiness = 800 - this.Happiness;
        //    this.Happiness = (this.Happiness / 800) * 100;
        //}
    }
}

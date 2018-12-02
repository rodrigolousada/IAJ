using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public interface WorldModel
    {
        List<Action> Actions { get; set; }
        Action[] GetExecutableActions();
        Action GetNextAction();
        object GetProperty(string propertyName);
        void SetProperty(string propertyName, object value);
        float GetGoalValue(string goalName);
        void SetGoalValue(string goalName, float value);
        WorldModel GenerateChildWorldModel();
        void CalculateNextPlayer();
        int GetNextPlayer();
        float GetScore();
        float CalculateDiscontentment(List<Goal> goals);
        void Initialize();
        bool IsTerminal();
    }

    public class DictionaryWorldModel : WorldModel
    {
        private Dictionary<string, object> Properties { get; set; }
        public List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; } 

        private Dictionary<string, float> GoalValues { get; set; } 

        protected WorldModel Parent { get; set; }

        public DictionaryWorldModel(List<Action> actions)
        {
            this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = actions;
            this.ActionEnumerator = actions.GetEnumerator();
        }

        public DictionaryWorldModel(WorldModel parent)
        {
            this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = parent.Actions;
            this.Parent = parent;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public void Initialize()
        {
            this.ActionEnumerator.Reset();
        }

        //public static Type Foo<T>(T param)
        //{
        //    return typeof(T);
        //}

        ////Useful to Debug Dictionarys by using reflection to find the generic object real type
        //public static Type CallFoo(object param)
        //{
        //    return (Type)typeof(WorldModel).GetMethod("Foo").MakeGenericMethod(new[] { param.GetType() }).Invoke(null, new[] { param });
        //}

        public virtual object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModel
            if (this.Properties.ContainsKey(propertyName))
            {
                return this.Properties[propertyName];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetProperty(propertyName);
            }
            else
            {
                return null;
            }
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            this.Properties[propertyName] = value;
        }

        public virtual float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            if (this.GoalValues.ContainsKey(goalName))
            {
                return this.GoalValues[goalName];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetGoalValue(goalName);
            }
            else
            {
                return 0;
            }
        }

        public virtual void SetGoalValue(string goalName, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }

            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            this.GoalValues[goalName] = limitedValue;
        }

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new DictionaryWorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public virtual Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;    
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }

        public virtual Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute(this)).ToArray();
        }

        public virtual bool IsTerminal()
        {
            return true;
        }
        

        public virtual float GetScore()
        {
            return 0.0f;
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }
    }


    public class ArrayWorldModel : WorldModel
    {
        private object[] Properties { get; set; }
        public List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; }

        private float[] GoalValues { get; set; }

        protected ArrayWorldModel Parent { get; set; }

        public ArrayWorldModel(List<Action> actions) {
            this.Properties = new object[13];
            this.GoalValues = new float[4];
            this.Actions = actions;
            this.ActionEnumerator = actions.GetEnumerator();
        }

        public ArrayWorldModel(ArrayWorldModel parent) {
                string[] properties = {
                GameManager.Properties.HP,
                GameManager.Properties.SHIELDHP,
                GameManager.Properties.LEVEL,
                GameManager.Properties.MANA,
                GameManager.Properties.MAXHP,
                GameManager.Properties.MONEY,
                GameManager.Properties.POSITION,
                GameManager.Properties.TIME,
                GameManager.Properties.XP,
                "Skeleton1",
                "Skeleton2",
                "Orc1",
                "Orc2",
                "Dragon",
                "Chest1",
                "Chest2",
                "Chest3",
                "Chest4",
                "Chest5",
                "ManaPotion1",
                "ManaPotion2",
                "HealthPotion1",
                "HealthPotion2"
            };

            string[] goals = {
                AutonomousCharacter.BE_QUICK_GOAL,
                AutonomousCharacter.GAIN_XP_GOAL,
                AutonomousCharacter.SURVIVE_GOAL,
                AutonomousCharacter.GET_RICH_GOAL
            };

            this.Properties = new object[properties.Length];
            this.GoalValues = new float[goals.Length];

            foreach (string property in properties) {
                this.SetProperty(property, parent.GetProperty(property));
            }
            foreach (string goal in goals) {
                this.SetGoalValue(goal, parent.GetGoalValue(goal));
            }

            this.Actions = parent.Actions;
            this.Parent = parent;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public void Initialize()
        {
            this.ActionEnumerator.Reset();
        }

        //public static Type Foo<T>(T param)
        //{
        //    return typeof(T);
        //}

        ////Useful to Debug Dictionarys by using reflection to find the generic object real type
        //public static Type CallFoo(object param)
        //{
        //    return (Type)typeof(WorldModel).GetMethod("Foo").MakeGenericMethod(new[] { param.GetType() }).Invoke(null, new[] { param });
        //}

        //Compiled to constant time resolution
        private int getPropertyIndex(string propertyName)
        {
            switch (propertyName) {
                case GameManager.Properties.HP: return 0;
                case GameManager.Properties.SHIELDHP: return 1;
                case GameManager.Properties.LEVEL: return 2;
                case GameManager.Properties.MANA: return 3;
                case GameManager.Properties.MAXHP: return 4;
                case GameManager.Properties.MONEY: return 5;
                case GameManager.Properties.POSITION: return 6;
                case GameManager.Properties.TIME: return 7;
                case GameManager.Properties.XP: return 8;
                case "Skeleton1": return 9;
                case "Skeleton2": return 10;
                case "Orc1": return 11;
                case "Orc2": return 12;
                case "Dragon": return 13;
                case "Chest1": return 14;
                case "Chest2": return 15;
                case "Chest3": return 16;
                case "Chest4": return 17;
                case "Chest5": return 18;
                case "ManaPotion1": return 19;
                case "ManaPotion2": return 20;
                case "HealthPotion1": return 21;
                case "HealthPotion2": return 22;
                default: return -1;
            }
        }

        //Compiled to constant time resolution
        private int getGoalIndex(string goalName) {
            switch (goalName) {
                case AutonomousCharacter.BE_QUICK_GOAL: return 0;
                case AutonomousCharacter.GAIN_XP_GOAL: return 1;
                case AutonomousCharacter.SURVIVE_GOAL: return 2;
                case AutonomousCharacter.GET_RICH_GOAL: return 3;
                default: return -1;
            }
        }

        public virtual object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModel
            var index = getPropertyIndex(propertyName);
            if (index >= 0) {
                return this.Properties[index];
            }
            return null;
        }

        public virtual void SetProperty(string propertyName, object value)
        {
            var index = getPropertyIndex(propertyName);
            if (index >= 0) {
                this.Properties[index] = value;
            }
        }

        public virtual float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            var index = getGoalIndex(goalName);
            if (index >= 0) {
                return this.GoalValues[index];
            }
            return 0;
        }

        public virtual void SetGoalValue(string goalName, float value)
        {

            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }

            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            var index = getGoalIndex(goalName);
            if (index >= 0) {
                this.GoalValues[index] = limitedValue;
            }
        }

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new ArrayWorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public virtual Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }

        public virtual Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute(this)).ToArray();
        }

        public virtual bool IsTerminal()
        {
            return true;
        }


        public virtual float GetScore()
        {
            return 0.0f;
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }
    }
}

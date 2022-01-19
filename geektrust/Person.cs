using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace geektrust
{
    public class Person : IPerson, IPersonBasicOperation
    {
        public string Name { get; }
        public Gender Gender { get; }
        public Person Father { get; private set; }
        public Person Spouse { get; private set; }
        public List<Person> Children { get; private set; }
        public bool HasFather { get => Father != null; }
        public bool HasSpouse { get => Spouse != null; }
        public bool HasChildren { get => Children.Count > 0; }

        internal Person(string name, Gender gender)
        {
            Name = name;
            Gender = gender;
            Father = null; //initially the king is alone, no father; And       | also when a person (apart from king) - no father assign
            Spouse = null; //no queen (spouse) when the king is born;          | - no spouse assigned
            Children = new List<Person>(); //also empty list of childrens..    | - also offcourse no children, later as per logs they are updated through below methods
        }

        private void SetFather(Person father)
        {
            if (father.Gender == Gender.Male) //father can only be Male in our case (as per geektrust's tree)
            {
                Father = father;
            }
        }

        public Person GetFather()
        {
            return Father;
        }

        private bool SetSpouse(Person spouse)
        {
            if (spouse.Gender != Gender && Spouse == null && spouse.Spouse == null) //gender should be opposite to marry (as per geektrust's tree)
            {
                spouse.Spouse = this; //spouse's spouse is also spouse
                Spouse = spouse; //married!
                Spouse.Children = Children; //children belong to both father and mother (children list is shared)
                return true;
            }
            return false;
        }

        public Person GetSpouse()
        {
            return Spouse;
        }

        private bool SetChild(Person child)
        {
            if (HasSpouse) //adding a validation here, so that only married couples i.e. having spouse can have kids
            {
                child.SetFather(Gender == Gender.Male ? this : Spouse); //also setting father for a child, easy to backtrack
                Children.Add(child); //lastly adding them to same list of children of both mother and father (list is shared)
                return true;
            }
            return false;
        }
        /// <summary>
        /// Find first child with the provided name
        /// </summary>
        /// <param name="name">name of the child to find</param>
        /// <returns>Person</returns>
        public Person GetChild(string name)
        {
            return Children.Find(x => x.Name == name);
        }
        /// <summary>
        /// recursively finds a person starting from root node.
        /// </summary>
        /// <param name="personName">name of person to find in the family tree</param>
        /// <returns>Person found</returns>
        public Person FindPerson(string personName)
        {
            if (Name == personName)
            {
                return this;
            }
            if (HasSpouse && Spouse.Name == personName)
            {
                return Spouse;
            }

            var temp = Children.Find(child => child.Name == personName);
            if (temp != null) return temp;

            foreach (var child in Children)
            {
                if (child.Name == personName) return child;
                var value = child.FindPerson(personName);
                if (value != null) return value;
            }
            return null;
        }
        /// <summary>
        /// to conduct marriage between two person
        /// </summary>
        /// <param name="newLifePartner">new life partner name</param>
        /// <returns>True if marriage is successful else false</returns>
        public bool Marry(Person newLifePartner)
        {
            return SetSpouse(newLifePartner);
        }
        /// <summary>
        /// to have a children for a married couple
        /// </summary>
        /// <param name="child"></param>
        /// <returns>true if new child is added successfully else false</returns>
        public bool HaveKid(Person child)
        {
            return SetChild(child);
        }
    }
}

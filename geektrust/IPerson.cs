using System.Collections.Generic;

namespace geektrust
{
    public enum Gender { Male, Female, Others }
    public enum Relationship { PATERNAL_UNCLE, MATERNAL_UNCLE, PATERNAL_AUNT, MATERNAL_AUNT, SISTER_IN_LAW, BROTHER_IN_LAW, SON, DAUGHTER, SIBLINGS, NONE }
    public enum OutputOperation { CHILD_ADDITION_SUCCEEDED, CHILD_ADDITION_FAILED, PERSON_NOT_FOUND, NONE }
    internal interface IPerson
    {
        public string Name { get; } //Unique idenity of a person in our family tree
        public Gender Gender { get; } //Gender of person is needed for marriage/finding sister/daugher/in-laws or brother/in-laws/son
        Person Father { get; } //Having basic identification of a person to have Father. We can get mother's detail from Father->Spouse
        Person Spouse { get; } //To have kids we need spouse, have added validation and also useful to get Mother details (using Father)
        List<Person> Children { get; } //a Person can have multiple kids but only 1 spouse and 1 father, and 1 mother.
        public Person GetChild(string name);
        public Person GetFather();
        public Person GetSpouse();

    }
    internal interface IPersonBasicOperation //to make code readable having this properties and operations
    {
        public bool HasFather { get; }
        public bool HasSpouse { get; }
        public bool HasChildren { get; }
        public bool Marry(Person newLifePartner);
        public bool HaveKid(Person child);
        public Person FindPerson(string personName);
    }
}

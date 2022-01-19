using System.IO;
using System.Linq;
using System.Reflection;

namespace geektrust
{
    public static class PersonExtensions
    {
        static Person root;
        public static Person Instance => root;
        static PersonExtensions()
        {
            LoadFamilyTreeFromExternalSource(); //to load family tree from a text file (in this case we have an embedded resource file)
            //LoadFamilyTreeSample(); //hard coded family tree as provided by geektrust question
        }
        public static Gender ParseGender(string gender)
        {
            return gender.ToLower() switch
            {
                "male" => Gender.Male,
                "female" => Gender.Female,
                _ => Gender.Others,
            };
        }
        public static Relationship ParseRelation(string relation)
        {
            relation = relation.ToLower();
            return relation switch
            {
                "paternal-uncle" => Relationship.PATERNAL_UNCLE,
                "maternal-uncle" => Relationship.MATERNAL_UNCLE,
                "paternal-aunt" => Relationship.PATERNAL_AUNT,
                "maternal-aunt" => Relationship.MATERNAL_AUNT,
                "sister-in-law" => Relationship.SISTER_IN_LAW,
                "brother-in-law" => Relationship.BROTHER_IN_LAW,
                "son" => Relationship.SON,
                "daughter" => Relationship.DAUGHTER,
                "siblings" => Relationship.SIBLINGS,
                _ => Relationship.NONE,
            };
        }
        /// <summary>
        /// To find an existing person based on relation that the given person posses with him/her
        /// </summary>
        /// <param name="name">name of person to find relations</param>
        /// <param name="relation">relationship type</param>
        /// <returns>(string) names of relations found from family tree with the given person</returns>
        public static string FindRelationShip(string name, Relationship relation)
        {
            var person = root.FindPerson(name);
            if (person == null)
            {
                return OutputOperation.PERSON_NOT_FOUND.ToString();
            }

            var personsToFind = relation switch
            {
                //own children (male)
                Relationship.SON => person.Children.FindAll(p => p.Gender == Gender.Male),
                //own children (female)
                Relationship.DAUGHTER => person.Children.FindAll(p => p.Gender == Gender.Female),
                //own father's children except you are siblings
                Relationship.SIBLINGS => person.HasFather ? person.Father.Children.FindAll(p => p.Name != name) : null,
                //sister in law can be your own siblings' spouse or your spouse's siblings (female)
                Relationship.SISTER_IN_LAW => person.HasSpouse && person.Spouse.HasFather ? person.Spouse.Father.Children.FindAll(p => p.Gender == Gender.Female && p.Name != person.Spouse.Name)
                                        : person.HasFather ? person.Father.Children.FindAll(sibling => sibling.HasSpouse && sibling.Spouse.Gender == Gender.Female).Select(sibling => sibling.Spouse).ToList() : null,
                //similarly, brother in law can be your own siblings' spouse or your spouse's siblings (male)
                Relationship.BROTHER_IN_LAW => person.HasSpouse && person.Spouse.HasFather ? person.Spouse.Father.Children.FindAll(p => p.Gender == Gender.Male && p.Name != person.Spouse.Name)
                                        : person.HasFather ? person.Father.Children.FindAll(sibling => sibling.HasSpouse && sibling.Spouse.Gender == Gender.Male).Select(sibling => sibling.Spouse).ToList() : null,
                //father's brothers
                Relationship.PATERNAL_UNCLE => person.HasFather && person.Father.HasFather ? person.Father.Father.Children.FindAll(p => p.Gender == Gender.Male && p.Name != person.Father.Name) : null,
                //mom's brothers (father's spouse = mother)
                Relationship.MATERNAL_UNCLE => person.HasFather && person.Father.Spouse.HasFather ? person.Father.Spouse.Father.Children.FindAll(p => p.Gender == Gender.Male && p.Name != person.Father.Spouse.Name) : null,
                //father's sisters
                Relationship.PATERNAL_AUNT => person.HasFather && person.Father.HasFather ? person.Father.Father.Children.FindAll(p => p.Gender == Gender.Female && p.Name != person.Father.Name) : null,
                //mom's sisters (father's spouse = mother)
                Relationship.MATERNAL_AUNT => person.HasFather && person.Father.Spouse.HasFather ? person.Father.Spouse.Father.Children.FindAll(p => p.Gender == Gender.Female && p.Name != person.Father.Spouse.Name) : null,
                _ => null,
            };
            if (personsToFind != null && personsToFind.Count > 0)
            {
                return string.Join(' ', personsToFind.Select(p => p.Name)); //avoiding last character ' ' blank space
            }
            else
            {
                return OutputOperation.NONE.ToString();
            }
        }
        /// <summary>
        /// as per our requirement we can add a child to a mother, this method also performs a validation to verify if child is added to correct mother
        /// </summary>
        /// <param name="motherName">provide a mother name</param>
        /// <param name="childName">provide a child name</param>
        /// <param name="gender">provide a gender to child</param>
        /// <returns>(string) if successful, will return CHILD_ADDITION_SUCCEEDED</returns>
        public static OutputOperation AddChild(string motherName, string childName, Gender gender)
        {
            var mother = root.FindPerson(motherName);
            if (mother == null)
            {
                return OutputOperation.PERSON_NOT_FOUND;
            }
            if (mother.Gender != Gender.Female)
            {
                return OutputOperation.CHILD_ADDITION_FAILED;
            }
            mother.HaveKid(new Person(childName, gender));

            //verify
            var child = root.FindPerson(childName);
            if (child == null)
            {
                return OutputOperation.CHILD_ADDITION_FAILED;
            }
            else if (child.Father.Name == mother.Spouse.Name)
            {
                return OutputOperation.CHILD_ADDITION_SUCCEEDED;
            }
            return OutputOperation.CHILD_ADDITION_FAILED;
        }
        /// <summary>
        /// having another method to add child to any parent
        /// </summary>
        /// <param name="parentName">parent name who is going to have a kid</param>
        /// <param name="childName">kid's name</param>
        /// <param name="gender">gender of kid</param>
        internal static void AddChildToAny(string parentName, string childName, Gender gender)
        {
            var parent = root.FindPerson(parentName);
            if (parent != null)
            {
                parent.HaveKid(new Person(childName, gender));
            }
        }
        /// <summary>
        /// create marriage between bride and groom -> if anyone doesnt exists, create one with Father = null; and auto assign gender
        /// </summary>
        /// <param name="person1">person who exists already</param>
        /// <param name="person2">New person, which do not exists (and no father assigned)</param>
        internal static void Marriage(string person1, string person2)
        {
            var brideGroom = root.FindPerson(person1);
            if (brideGroom != null)
            {
                var gender = brideGroom.Gender == Gender.Male ? Gender.Female : Gender.Male;
                brideGroom.Marry(new Person(person2, gender));
            }
        }
        /// <summary>
        /// Have created a log of Family tree in a text file that is tab delimited, read from log (.txt) and create family tree.
        /// </summary>
        /// <param name="resourceName">embedded resource file, e.g. geektrust.LengaburuFamilyLog.txt</param>
        private static void LoadFamilyTreeFromExternalSource(string resourceName = "geektrust.LengaburuFamilyLog.txt")
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);
            string result = string.Empty;
            while ((result = reader.ReadLine()) != null)
            {
                var words = result.Split('\t');
                if (words.Length > 0)
                {
                    switch (words[0])
                    {
                        case "HAVE_KID":
                            if (root != null)
                            {
                                AddChildToAny(words[1], words[2], ParseGender(words[3]));
                            }
                            else
                            {
                                root = new Person(words[2], ParseGender(words[3]));
                            }
                            break;
                        case "MARRY":
                            Marriage(words[1], words[2]);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// incase you want to create family tree programatically use this method
        /// </summary>
        private static void LoadFamilyTreeSample()
        {
            root = new Person("King Shan", Gender.Male);
            root.Marry(new Person("Queen Anga", Gender.Female));
            root.HaveKid(new Person("Chit", Gender.Male));
            root.HaveKid(new Person("Ish", Gender.Male));
            root.HaveKid(new Person("Vich", Gender.Male));
            root.HaveKid(new Person("Aras", Gender.Male));
            root.HaveKid(new Person("Satya", Gender.Female));

            root.GetChild("Chit").Marry(new Person("Amba", Gender.Female));
            root.GetChild("Vich").Marry(new Person("Lika", Gender.Female));
            root.GetChild("Aras").Marry(new Person("Chitra", Gender.Female));
            root.GetChild("Satya").Marry(new Person("Vyan", Gender.Male));

            root.GetChild("Chit").HaveKid(new Person("Dritha", Gender.Female));
            root.GetChild("Chit").HaveKid(new Person("Tritha", Gender.Female));
            root.GetChild("Chit").HaveKid(new Person("Vritha", Gender.Male));

            root.GetChild("Chit").GetChild("Dritha").Marry(new Person("Jaya", Gender.Male));
            root.GetChild("Chit").GetChild("Dritha").HaveKid(new Person("Yodhan", Gender.Male));

            root.GetChild("Vich").HaveKid(new Person("Vila", Gender.Female));
            root.GetChild("Vich").HaveKid(new Person("Chika", Gender.Female));

            root.GetChild("Aras").HaveKid(new Person("Jnki", Gender.Female));
            root.GetChild("Aras").HaveKid(new Person("Ahit", Gender.Male));
            root.GetChild("Aras").GetChild("Jnki").Marry(new Person("Arit", Gender.Male));
            root.GetChild("Aras").GetChild("Jnki").HaveKid(new Person("Laki", Gender.Male));
            root.GetChild("Aras").GetChild("Jnki").HaveKid(new Person("Lavnya", Gender.Female));

            root.GetChild("Satya").HaveKid(new Person("Asva", Gender.Male));
            root.GetChild("Satya").HaveKid(new Person("Vyas", Gender.Male));
            root.GetChild("Satya").HaveKid(new Person("Atya", Gender.Female));
            root.GetChild("Satya").GetChild("Asva").Marry(new Person("Satvy", Gender.Female));
            root.GetChild("Satya").GetChild("Asva").HaveKid(new Person("Vasa", Gender.Male));
            root.GetChild("Satya").GetChild("Vyas").Marry(new Person("Krpi", Gender.Female));
            root.GetChild("Satya").GetChild("Vyas").HaveKid(new Person("Kriya", Gender.Male));
            root.GetChild("Satya").GetChild("Vyas").HaveKid(new Person("Krithi", Gender.Female));
        }
        internal static bool ValidateInput(string[] words)
        {
            if (words.Length == 0) return false;
            switch (words[0])
            {
                case "ADD_CHILD":
                    if (words.Length == 4) return true;
                    return false;
                case "GET_RELATIONSHIP":
                    if (words.Length == 3) return true;
                    return false;
                default:
                    return false;
            }
        }
        internal static void DebugSearch(string searchName)
        {
            var childFound = root.FindPerson(searchName);
            if (childFound != null)
            {
                var type = childFound.Gender == Gender.Male ? "Son" : "Daughter";
                System.Diagnostics.Debug.WriteLine($"Found {childFound.Name}: {type} of {childFound.Father.Name}");
                if (childFound.HasSpouse)
                {
                    System.Diagnostics.Debug.WriteLine($"spouse of {childFound.GetSpouse().Name}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"{searchName} not found");
            }
        }
    }
}

# Meet the Family

Meet the Family is the Problem #1 at the GeekTrust.

## Project Specs

- framework: dotnet core 3.1
- language: c#

**Build and Execute in command line (from the project location which has .csproj file)**
```bash
dotnet build -o geektrust
dotnet geektrust/geektrust.dll <absolute_path_to_input_file>
```

## Template of a Person in the Tree Structure

```csharp
Person
{
    string Name;
    Gender Gender;
    Person Father;
    Person Spouse;
    List<Person> Children;
}
```
In the family tree each person is connected with each other, so in order to connect with each other; I have tried to implement a connected acyclic graph like structure or a tree structure. A child node can access it's ancestor by using Father and a root node can access it's descendants using Children. Spouse is not contributing currently to graph or tree, but is useful in finding out if a person is eligible to have kids (children). I have added validations if a person has spouse can have kids. Few more validations like a father can be Male and spouses can be of opposite gender only.

Enums are created as they are type safe, easy to use in switch case operation, and we have predefined values
- Gender
  - Male
  - Female
  - Others
- Relationship
  - PATERNAL_UNCLE
  - MATERNAL_UNCLE
  - PATERNAL_AUNT
  - MATERNAL_AUNT
  - SISTER_IN_LAW
  - BROTHER_IN_LAW
  - SON
  - DAUGHTER
  - SIBLINGS
  - NONE 
- OutputOperation
  - CHILD_ADDITION_SUCCEEDED
  - CHILD_ADDITION_FAILED
  - PERSON_NOT_FOUND
  - NONE

## Test Cases
14 test cases were tested with the Application
#### Input
```
ADD_CHILD Satya Ketu Male
GET_RELATIONSHIP Kriya Paternal-Uncle
GET_RELATIONSHIP Satvy Brother-In-Law
GET_RELATIONSHIP Satvy Sister-In-Law
GET_RELATIONSHIP Ish Son
GET_RELATIONSHIP Misha Daughter
ADD_CHILD Chitra Aria Female
GET_RELATIONSHIP Lavnya Maternal-Aunt
GET_RELATIONSHIP Aria Siblings
ADD_CHILD Pjali Srutak Male
GET_RELATIONSHIP Pjali Son
ADD_CHILD Asva Vani Female
GET_RELATIONSHIP Vasa Siblings
GET_RELATIONSHIP Atya Sister-In-Law
```

#### Result: Passed 
Reason: Matched with Expected results
```
CHILD_ADDITION_SUCCEEDED
Asva Ketu
Vyas Ketu
Atya
NONE
PERSON_NOT_FOUND
CHILD_ADDITION_SUCCEEDED
Aria
Jnki Ahit
PERSON_NOT_FOUND
PERSON_NOT_FOUND
CHILD_ADDITION_FAILED
NONE
Satvy Krpi
```
## Methods Used to form a Tree
### Method 1: Load programmatically
##### Pros:
- Readable
- Good way to start
- Easy to understand how Person has ancestors/descendants

##### Cons:
- Time consuming if family increases
- Long lines of code
- error prone when manually done
```csharp
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

```
### Method 2: Load from Text file
##### Pros:
- Write few basic commands in the text file and run it
- Commands can be generated programmatically (or verified)
- Readable
- Can be tab delimited or csv (Currently it is tab delimited file)
##### Cons:
- Error prone if manually done
- Need to embed the resource file along with project (but can be updated)
- More commands, means more lines of code to update (currently supports `HAVE_KID` and `MARRY` commands)
```
HAVE_KID		King Shan	MALE
HAVE_KID		Queen Anga	FEMALE
MARRY	King Shan	Queen	Anga
HAVE_KID	King Shan	Chit	Male
HAVE_KID	King Shan	Ish	Male
HAVE_KID	King Shan	Vich	Male
HAVE_KID	King Shan	Aras	Male
HAVE_KID	King Shan	Satya	Female
MARRY	Chit	Amba
MARRY	Vich	Lika
MARRY	Aras	Chitra
MARRY	Satya	Vyan
HAVE_KID	Chit	Dritha	Female
HAVE_KID	Chit	Tritha	Female
HAVE_KID	Chit	Vritha	Male
MARRY	Dritha	Jaya
HAVE_KID	Dritha	Yodhan	Male
HAVE_KID	Vich	Vila	Female
HAVE_KID	Vich	Chika	Female
HAVE_KID	Aras	Jnki	Female
HAVE_KID	Aras	Ahit	Male
MARRY	Jnki	Arit
HAVE_KID	Jnki	Laki	Male
HAVE_KID	Jnki	Lavnya	Female
HAVE_KID	Satya	Asva	Male
HAVE_KID	Satya	Vyas	Male
HAVE_KID	Satya	Atya	Female
MARRY	Asva	Satvy
HAVE_KID	Asva	Vasa	Male
MARRY	Vyas	Krpi
HAVE_KID	Vyas	Kriya	Male
HAVE_KID	Vyas	Krithi	Female
```
***Please do refer to comments present in the code.***

## Future works
- More Test cases (+ unit test project)
- Optimizing code for finding a Person
- Exception Handling
- Having non zero Exit Code of the program if error occurs
- Taking two input files as command line argument: Family tree log file and input file to add child or find relation
- Figure out: If having Siblings of a Person and optimize the Program
- Figure out: How can we implement Design Patterns
- More useful comments on code and update readme.md file
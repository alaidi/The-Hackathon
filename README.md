# Hackathon Baghdad 2017 

## Data Description 
 
The data [data.csv] is small sample of CDR ﬁles (60 MB) generated at call center for a telecom company. It consist of the following columns:

* Date and time
* Caller id
* Employee id 
* Duration of call since ringing
* Talk time 
* Status 

## Problems 

1. Peak minute of incoming phonecalls. (including not answered phonecall)
2. Peak time(minute) of simultaneous phonecalls. (call time) 
3. Find if there is any relationship between a client and an employee. 
4. Most productive employee (employee with most answered phonecalls). 
5. Least productive employee (employee with least answered phonecalls).
6. Client with longest talk time.
7. Client with most frequent calls.

## Constraints 

1. Don't use a database.
2. Don't use MS Excel or any tools out of your code.
3. Build a GUI or CLI interface for the solution.

Run the code
-------------

To run this code, you'll need to clone the repository to your computer. 
Then make sure to install dotnet core and follow this instruction:

1. Install dotnet.  See https://www.microsoft.com/net/core

2. Build the solution.

        $ cd The-Hackathon
        $ dotnet restore
        $ dotnet build
        $ dotnet run 



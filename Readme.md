# Mysterious Family
The project was about creating a family tree with hashed nodes for a course of Data Structures and Algorithms in IUST.

## Logic and Structure

As a challenge, I wanted to create the most performant, most scalable project.
To the point that there is almost no pointers used, and data from logic is perfectly separated.

### Every Internal Search is O(1):
We have 2 lists and a dictionary, one list for storing person information,
another for storing family information (father, mother, children),
and the dictionary to link a person to two families (the one they formed, and the one they were born in).

We use an indexing system, such that the family information is stored as ints which correlate to the indices in our first list.
This makes internal searching for each node much faster, since it creates a map of some kind.

### No Pointers:
Another pro of the said approach is that families do not use pointers to access people. Considering every pointer is 8 bytes
and each integer is 4 bytes, we save half the memory for other operations. This data-oriented design enables us to save time accessing the pointer values in the memory, making our iterations much faster.

### Model-View-Controller Architecture:
The data is encapsulated in the model, and the link between the view (which generates the graph) and model is the controller, which also handle the logic of all the operations.

### Unity Side:
We use object pooling for nodes of the graph, which saves GC time, since the graph needs to be recreated every time it changes.
This approach to caching nodes and re-initializing them whenever needed, makes it so that we would not experience frame drops.

> This part will be completed as soon as possible.

## Commands
#### These are some usable commands in the in-game console to work with the data structure and graph:
```in-game console
add-person 'name' //Adds a person by name. Name will be hashed and stored in the object.
add-family 'fatherID' 'motherID' //Adds a family by its father and mother IDs.
add-child 'childID' 'familyID' //Adds a child (by its ID) to a family (by its ID).

check-parent 'parentID' 'childID' //Checks if the 'parentID' is the parent of 'childID'.
check-sibling 'firstID' 'secondID' //Checks if 'firstID' and 'secondID' are siblings (i.e. born in the same family as children).
check-bfs-distant 'firstID' secondID' //Checks if 'firstID' and 'secondID' have a family in common through a breadth-first search.
check-fast-distant 'firstID' secondID' //Checks if 'firstID' and 'secondID' have a family in common through a custom-made and more optimized breadth-first search.

get-farthest-born 'parentID' //Gets the farthest born of 'parentID'.
get-farthest-relation 'rootID' //Gets the farthest relation available between the nodes.

search-person 'name' //Finds the person with 'name', and displays their ID and hash."

select 'nodeID' //Selects the node by its 'id'.
deselect 'nodeID' //Deselects the node by its 'id'.
clear-selection //Clears the current selection.
clear-results //Clears the current result.

load-mock //Loads a pre-determined mock graph.
reload //Reloads the scene.

bfs-common-families 'firstID' 'secondID' //Gets all common families between 'firstID' and 'secondID' with a breadth-first search.
fast-common-family 'firstID' 'secondID' //Gets one common family between 'firstID' and 'secondID' through a custom-made and more optimized breadth-first search."
```

## Functions

### Check Parent Relation
```C#
CheckParentRelation(PersonID parent, PersonID child);
```
This function checks if parent and child are, well, parent and child. 
It splits into two other function calls recursively.

> So, it should perform with a time complexity of O(2^F),
with F being the family height.
Which is much faster than O(2^n) with n being the height of nodes.

### Check Sibling Relation
```C#
CheckSiblingRelation(PersonID first, PersonID second);
```
This one checks if 'first' and 'second' are siblings.

> It should be obvious that due to the structure, in which these nodes are stored,
this function performs with a time complexity of O(1).

### Get Common Families BFS
```C#
List<FamilyID> GetCommonFamiliesBFS(PersonID first, PersonID second);
```
This returns all the families 'first' and 'second' have in common.
First, it performs a Breadth-First Search on 'first', caching all the families it traces.
The cache is stored in a Hashset which makes search operations almost close to O(1).
Then, another Breadth-First Search is performed on the 'second',
and each time it checks to see if the current family that it is visiting is in the cache or not.
If so, it adds the FamilyID to the list to return.

> This function performs with a time complexity of O(2^(F + 1)) at worst.
With F being the families above the nodes. It is running for two nodes, hence, F + 1.

> Note that using the families to iterate on, is much faster than using raw nodes.

### Get Common Family Fast
```C#
FamilyID GetCommonFamilyFast(PersonID first, PersonID second);
```
Custom-made and more optimized for the average family graph. 
It operates almost the same as above, but this time, 
it runs the Breadth-First Search on both families in parallel, until it gets to a root.
Then, it iterates on the other one, one by one, and checks each time if it has reached a common family.
If so, it immediately returns the FamilyID it found.

> Time complexity is the same as `GetCommonFamiliesBFS()`. 
> Though, on average, it should be faster, 
> since checking the families is not required until we get to a root.

### Check Distant Relation BFS
```C#
bool CheckDistantRelationBFS(PersonID first, PersonID second);
```
Returns true if 'first' and 'second' are distant relatives.
> By itself, the time complexity of this function is O(1),
> but it runs `GetCommonFamiliesBFS()` under the hood and checks if it returns anything.

### Check Distant Relation Fast
```C#
bool CheckDistantRelationFast(PersonID first, PersonID second);
```
Returns true if 'first' and 'second' are distant relatives.
> This one also runs `GetCommonFamilyFast()` under the hood. So, it should be the same.

### Get Farthest Born
```C#
PersonID GetFarthestBorn(PersonID person, out int distance);
```
This function runs a Depth-First Search on every person recursively,
It fills an array of each distance for every node. Then compares,
and returns the maximum value and its PersonID.

> Since every node is visited once, time complexity should be close to O(n),
> with n being the number of nodes.

### Get Farthest Relation
```C#
void GetFarthestRelation(PersonID root, out PersonID first, out PersonID second, out int distance);
```
This one operates on the function above, `GetFarthestBorn()`.
It runs that function on the 'root', and sets 'first' to the value found.
Then, runs a Breadth-First Search recursively on 'first', to find other nodes.
The distance of each node is calculated and stored in a Dictionary (provides O(1) searching).
Each visited node is added to a Hashset (provides O(1) searching).
At the end, it return the maximum distance and its PersonID.

> Since every node is visited only once, with a searching complexity of O(1),
> the final time complexity is very much close to O(n).

### Honorable Mention: Custom Graphing Algorithm
As stated in the Logic and Structure section, we are not storing data in a graph-like manner, per se.
So, creating nodes and links from this separately-stored data, requires a custom-made algorithm.

The algorithm we came up with, treats the graph like a puzzle, and puts nodes in their distinguished place like pieces.
It starts with a root node and places its spouse. Then places their children.

It performs recursively, so it is entirely possible that a non-root node would not know anything about its father/mother, children, or even spouse.
In order to combat this problem, we make sure that each node knows at least one of the above, so it should know its position accordingly.

After node placement, we place family links using those positions we acquired during the last step.
Then, using a LineRenderer, we render the lines in a special manner. Then, voila.

> Time complexity of this function is almost O(n), with n being the number of nodes, since we only visit each node one time.

## Final Thoughts?
At some point, I wanted to implement multi-threading.
I decided against that, since the graph is sparse, and in this scale it would not make sense.

Also, I wanted to add a file system with a ScriptableObject structure to handle saving and loading multiple, but separately-stored, graphs.
Due to the tight deadline, sadly, this was not possible.

Lastly, implementing and engineering this application was as exciting as it was fun.
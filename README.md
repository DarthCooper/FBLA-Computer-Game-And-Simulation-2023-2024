# FBLA-Computer-Game-And-Simulation-2023-2024
This is the repository for all of our game assets, files, programs, etc. 

This game was made using the Unity game engine, more specifically Unity 2023.3 or at least around there.

It uses a steam ID of 480 (or something like that).

### Software Used:
	We used Unity as the game engine, along with visual studio code 2022 as the compilier.
	We used photoshop, procreate, and gimp to create the sprites and UI elements.
	We used Github desktop to share this project.

### Libraries Used:
	Steamworks - Incorporate multiplayer to work with Steam
	FizzySteamworks - Allows Mirror to communicate with Steamworks, and thus Steam's servers.
	Cinemachine - Fancy Cameras.
	A* Pathfinding - pathfinding for enemies and NPCs.

## Set Up Unity Project with Github desktop
	Start off by downloading github desktop.https://desktop.github.com/
	Next download Unity. https://unity.com/download
 	Next sync github desktop with you github account.
	Then create a tutorial repository and select FBLA-Computer-Game-And-Simulation-2023-2024, and click Clone
	Choose where you want the folder to go, and viola you know have all of the files.
 	In Unity Hub create a new project and choose the newly created file of this repository. 
	Now wait, for a very long time, for unity to compile the project.
	Congragulations, you now have this project set up in Unity.

## Set up Unity Project with Git
	Start off by downloading Git. https://git-scm.com/downloads
 	Next download Unity. https://unity.com/download
	Open command prompt and type: git config --global user.name "Your Github Username" to connect Git with your Github account.
 	In command prompt use the command: cd /where you want the repo.
 	In command prompt type: git clone https://github.com/DarthCooper/FBLA-Computer-Game-And-Simulation-2023-2024.git
  	You know have all of the files for this Unity project.
   	In Unity Hub create a new project and choose the newly created file of this repository.
    Now wait for unity to compile the project.
    Congragulations, you now have this project set up in Unity.

## How to upload new files
	Add whatever files you want to add to the repo you created in your files. 
 	Github desktop automatically recognizes a new file was added, so all you have to do is "fetch" the current data. Give a commit title and description and click commit. Wait a second and click push.
  	Git will be a little more complicated. You will have to do "git add (the path to the new file)", then "git commit "Commit title"", and finally "git push" or "git push origin "branch name"".

## How to alter files
	Make whatever changes you want to.
 	Github desktop automatically recognizes the modifications, so all you have to do is "fetch" the current data. Give a commit title and description and click commit. Wait a second and click pus.
  	Git requires commands. Do "git stage -p" and type "y" to all of the changes you want to make. Next, type "git commit "Commit title"", and finally "git push" or "git push origin "branch name"".
    
### Basic rules with commiting:
	Never commit to main, and never look at where Logan commits(It may be in direct violation of this).
 	Either make the commits helpful so we can see what is being added, or make them funny. Funny is preferable, but bad practice.
  	If your current branch doesn't match up with what you are commiting, create a new branch.
